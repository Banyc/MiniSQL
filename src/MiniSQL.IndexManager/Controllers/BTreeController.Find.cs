using System;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Models;
using MiniSQL.IndexManager.Utilities;
using MiniSQL.Library.Models;

namespace MiniSQL.IndexManager.Controllers
{
    public partial class BTreeController
    {
        public System.Collections.Generic.IEnumerable<BTreeCell> LinearSearch(BTreeNode root)
        {
            BTreeNode beginNode = FindMin(root);
            List<BTreeCell> result = new List<BTreeCell>();

            while (true)
            {
                foreach (var cell in beginNode)
                {
                    yield return cell;
                }
                if (beginNode.RightPage == 0)
                {
                    break;
                }
                beginNode = BTreeNodeHelper.GetBTreeNode(_pager, (int)beginNode.RightPage);
            }
        }
                
        public BTreeCell FindCell(DBRecord key, BTreeNode root)
        {
            return InternalFind(key, root);
        }

        public List<BTreeCell> FindCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations)
        {
            if (expression == null)
            {
                return LinearSearch(root, expression, attributeDeclarations);
            }
            else if (expression.SimpleMinterms.ContainsKey(keyName))
            {
                List<BTreeCell> result = new List<BTreeCell>();
                List<AtomValue> values = new List<AtomValue>();
                BTreeNode beginNode;

                BTreeCell cell;
                UInt16 offset;
                int begin_Index;

                AtomValue bound = expression.SimpleMinterms[keyName].RightOperand.ConcreteValue;

                values.Add(bound);
                DBRecord keyFind = new DBRecord(values);
                switch (expression.SimpleMinterms[keyName].Operator)
                {
                    case Operator.NotEqual:
                        return LinearSearch(root, expression, attributeDeclarations);

                    case Operator.Equal:
                        LeafTableCell tmpCell = (LeafTableCell)FindCell(keyFind, root);
                        if (tmpCell == null)
                        {
                            break;
                        }
                        else if (expression.Calculate(attributeDeclarations, tmpCell.DBRecord.GetValues()).BooleanValue == true)
                        {
                            result.Add(tmpCell);
                        }
                        break;

                    case Operator.LessThan:
                        beginNode = FindMin(root);
                        return LessFind(beginNode, expression, attributeDeclarations, bound, false);
                    case Operator.LessThanOrEqualTo:
                        beginNode = FindMin(root);
                        return LessFind(beginNode, expression, attributeDeclarations, bound, true);
                    case Operator.MoreThan:
                        beginNode = FindNode(keyFind, root, true);
                        // WORK AROUND
                        if (beginNode == null)
                        {
                            // variable > <overlimited number>
                            return result;
                        }
                        (cell, offset, begin_Index) = beginNode.FindBTreeCell(keyFind);
                        //2 possible sitiutions for cell==null:
                        //1:The keyFind is bigger than all the key
                        //2:The keyFind is just between the bigest one in this node and the smallest one in next node
                        if (cell == null)
                        {
                            if (beginNode.RightPage == 0)
                            {
                                return result;
                            }
                            else
                            {
                                begin_Index = 0;
                                MemoryPage Nextpage = _pager.ReadPage((int)beginNode.RightPage);
                                beginNode = new BTreeNode(Nextpage);
                            }
                        }
                        return MoreFind(beginNode, begin_Index, expression, attributeDeclarations, false);
                    case Operator.MoreThanOrEqualTo:
                        beginNode = FindNode(keyFind, root, true);
                        // WORK AROUND
                        if (beginNode == null)
                        {
                            // variable > <overlimited number>
                            return result;
                        }
                        (cell, offset, begin_Index) = beginNode.FindBTreeCell(keyFind);
                        if (cell == null)
                        {
                            if (beginNode.RightPage == 0)
                            {
                                return result;
                            }
                            else
                            {
                                begin_Index = 0;
                                MemoryPage Nextpage = _pager.ReadPage((int)beginNode.RightPage);
                                beginNode = new BTreeNode(Nextpage);
                            }
                        }
                        return MoreFind(beginNode, begin_Index, expression, attributeDeclarations, true);
                    default:
                        throw new Exception("The Operand is not supported!");
                }
                //This step may not need?
                return result;

            }
            else
            {
                return LinearSearch(root, expression, attributeDeclarations);
            }
        }
        
        private BTreeNode FindMin(BTreeNode root)
        {
            BTreeNode child;
            MemoryPage Nextpage = null;
            InternalTableCell childCell;

            if (root.PageType == PageTypes.LeafTablePage)
            {
                return root;
            }
            childCell = (InternalTableCell)root.GetBTreeCell(root.CellOffsetArray[0]);
            Nextpage = _pager.ReadPage((int)childCell.ChildPage);
            child = new BTreeNode(Nextpage);

            return FindMin(child);
        }

        private List<BTreeCell> LessFind(BTreeNode begin, Expression expression, List<AttributeDeclaration> attributeDeclarations, AtomValue UpperBound, bool isEqual)
        {
            MemoryPage Nextpage = null;
            List<BTreeCell> result = new List<BTreeCell>();
            LeafTableCell leafCell;

            while (true)
            {
                foreach (var cell in begin)
                {
                    if (((cell.Key.GetValues()[0] > UpperBound).BooleanValue && isEqual) ||
                        ((cell.Key.GetValues()[0] >= UpperBound).BooleanValue) && !isEqual)
                    {
                        return result;
                    }
                    leafCell = (LeafTableCell)cell;

                    if (expression.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                    {
                        result.Add(cell);
                    }
                }
                if (begin.RightPage == 0)
                {
                    return result;
                }
                Nextpage = _pager.ReadPage((int)begin.RightPage);
                begin = new BTreeNode(Nextpage);
            }
        }

        private List<BTreeCell> MoreFind(BTreeNode begin_Node, int begin_Index, Expression expression, List<AttributeDeclaration> attributeDeclarations, bool isEqual)
        {
            MemoryPage Nextpage = null;
            List<BTreeCell> result = new List<BTreeCell>();
            LeafTableCell leafCell;

            if (isEqual)
            {
                leafCell = (LeafTableCell)begin_Node.GetBTreeCell(begin_Node.CellOffsetArray[begin_Index]);
                if (expression.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                {
                    result.Add(leafCell);
                }

            }
            for (int i = begin_Index + 1; i < begin_Node.NumCells; i++)
            {
                leafCell = (LeafTableCell)begin_Node.GetBTreeCell(begin_Node.CellOffsetArray[i]);
                if (expression.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                {
                    result.Add(leafCell);
                }
            }
            while (begin_Node.RightPage != 0)
            {
                Nextpage = _pager.ReadPage((int)begin_Node.RightPage);
                begin_Node = new BTreeNode(Nextpage);

                foreach (var cell in begin_Node)
                {
                    leafCell = (LeafTableCell)cell;
                    if (expression.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                    {
                        result.Add(cell);
                    }
                }

            }
            return result;
        }

        
        private BTreeNode FindNode(DBRecord key, BTreeNode root, bool isFuzzySearch = false)
        {
            BTreeNode child;
            MemoryPage Nextpage = null;
            BTreeCell cell;
            UInt16 offset;
            int indexInOffsetArray;

            if (root.PageType == PageTypes.LeafTablePage)
            {
                (cell, offset, indexInOffsetArray) = root.FindBTreeCell(key, isFuzzySearch);
                if (cell != null)
                    return root;
                else
                    return null;
            }

            //If it's internal node
            (cell, offset, indexInOffsetArray) = root.FindBTreeCell(key);
            if (offset == 0)      //rightpage
            {
                Nextpage = _pager.ReadPage((int)root.RightPage);
                child = new BTreeNode(Nextpage);
            }
            else
            {
                InternalTableCell internalTableCell = (InternalTableCell)cell;
                Nextpage = _pager.ReadPage((int)internalTableCell.ChildPage);
                child = new BTreeNode(Nextpage);
            }
            return FindNode(key, child);
        }
   
        private BTreeCell InternalFind(DBRecord key, BTreeNode root)
        {
            BTreeNode child;
            MemoryPage Nextpage = null;
            BTreeCell cell;
            UInt16 offset;
            int indexInOffsetArray;

            if (root.PageType == PageTypes.LeafTablePage)
            {
                (cell, offset, indexInOffsetArray) = root.FindBTreeCell(key, false);
                return cell;
            }

            //If it's internal node
            (cell, offset, indexInOffsetArray) = root.FindBTreeCell(key);
            if (offset == 0)      //rightpage
            {
                Nextpage = _pager.ReadPage((int)root.RightPage);
                child = new BTreeNode(Nextpage);
            }
            else
            {
                InternalTableCell internalTableCell = (InternalTableCell)cell;
                Nextpage = _pager.ReadPage((int)internalTableCell.ChildPage);
                child = new BTreeNode(Nextpage);
            }
            return InternalFind(key, child);
        }


        private List<BTreeCell> LinearSearch(BTreeNode root, Expression expression, List<AttributeDeclaration> attributeDeclarations)
        {
            BTreeNode beginNode = FindMin(root);

            MemoryPage Nextpage = null;
            List<BTreeCell> result = new List<BTreeCell>();
            LeafTableCell leafCell;

            while (true)
            {
                foreach (var cell in beginNode)
                {
                    leafCell = (LeafTableCell)cell;
                    if (expression == null)
                    {
                        result.Add(cell);
                    }
                    else if (expression.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                    {
                        result.Add(cell);
                    }
                }
                if (beginNode.RightPage == 0)
                {
                    return result;
                }
                Nextpage = _pager.ReadPage((int)beginNode.RightPage);
                beginNode = new BTreeNode(Nextpage);
            }
        }
    }
}
