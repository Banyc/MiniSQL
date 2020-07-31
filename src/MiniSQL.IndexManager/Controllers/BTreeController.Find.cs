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
        /// <summary>
        /// Search the leaf nodes linearly.
        /// </summary>
        /// <param name="root">the root node of the B+ tree</param>
        /// <returns></returns>
        public IEnumerable<BTreeCell> LinearSearch(BTreeNode root)
        {
            BTreeNode startNode = FindMin(root);
            while (true)
            {
                foreach (var cell in startNode)
                {
                    yield return cell;
                }
                if (startNode.RightPage == 0)
                {
                    break;
                }
                startNode = BTreeNodeHelper.GetBTreeNode(_pager, (int)startNode.RightPage);
            }
        }

        public BTreeCell FindCell(DBRecord key, BTreeNode root)
        {
            BTreeNode child;
            BTreeCell cell;
            UInt16 offset;
            if (root.PageType == PageTypes.LeafTablePage)
            {
                (cell, _, _) = root.FindBTreeCell(key, false);
                return cell;
            }

            // If it's internal node
            (cell, offset, _) = root.FindBTreeCell(key);
            MemoryPage nextpage;
            if (offset == 0)      //rightpage
            {
                nextpage = _pager.ReadPage((int)root.RightPage);
                child = new BTreeNode(nextpage);
            }
            else
            {
                InternalTableCell internalTableCell = (InternalTableCell)cell;
                nextpage = _pager.ReadPage((int)internalTableCell.ChildPage);
                child = new BTreeNode(nextpage);
            }
            return FindCell(key, child);
        }

        // `keyName` := primary key in table tree; indexed value in index tree
        public List<BTreeCell> FindCells(BTreeNode root, Expression condition, string keyName, List<AttributeDeclaration> attributeDeclarations)
        {
            if (condition == null)
            {
                return LinearSearch(root, condition, attributeDeclarations);
            }
            else if (condition.SimpleMinterms.ContainsKey(keyName))
            {
                List<BTreeCell> result = new List<BTreeCell>();
                List<AtomValue> values = new List<AtomValue>();
                BTreeNode startNode;

                BTreeCell cell;
                int startIndexOfCellInStartNode;

                AtomValue bound = condition.SimpleMinterms[keyName].RightOperand.ConcreteValue;

                values.Add(bound);
                DBRecord keyFind = new DBRecord(values);
                switch (condition.SimpleMinterms[keyName].Operator)
                {
                    case Operator.NotEqual:
                        return LinearSearch(root, condition, attributeDeclarations);

                    case Operator.Equal:
                        LeafTableCell tmpCell = (LeafTableCell)FindCell(keyFind, root);
                        if (tmpCell == null)
                        {
                            break;
                        }
                        else if (condition.Calculate(attributeDeclarations, tmpCell.DBRecord.GetValues()).BooleanValue == true)
                        {
                            result.Add(tmpCell);
                        }
                        break;

                    case Operator.LessThan:
                        startNode = FindMin(root);
                        return FindCells(startNode, condition, attributeDeclarations, bound, false);
                    case Operator.LessThanOrEqualTo:
                        startNode = FindMin(root);
                        return FindCells(startNode, condition, attributeDeclarations, bound, true);
                    case Operator.MoreThan:
                        startNode = FindNode(keyFind, root, true);
                        // nothing is equal or more than the value of `keyFind` in the tree
                        if (startNode == null)
                        {
                            return result;
                        }
                        (cell, _, startIndexOfCellInStartNode) = startNode.FindBTreeCell(keyFind);
                        // 2 possible situations for `cell == null`:
                        // 1: The `keyFind` is bigger than all the key in `startNode`
                        // 2: The `keyFind` is just between the bigest one in `startNode` and the smallest one in next node
                        if (cell == null)
                        {
                            if (startNode.RightPage == 0)
                            {
                                return result;
                            }
                            else
                            {
                                startIndexOfCellInStartNode = 0;
                                MemoryPage nextpage = _pager.ReadPage((int)startNode.RightPage);
                                startNode = new BTreeNode(nextpage);
                            }
                        }
                        return FindCells(startNode, startIndexOfCellInStartNode, condition, attributeDeclarations, false);
                    case Operator.MoreThanOrEqualTo:
                        startNode = FindNode(keyFind, root, true);
                        // nothing is equal or more than the value of `keyFind` in the tree
                        if (startNode == null)
                        {
                            return result;
                        }
                        (cell, _, startIndexOfCellInStartNode) = startNode.FindBTreeCell(keyFind);
                        if (cell == null)
                        {
                            if (startNode.RightPage == 0)
                            {
                                return result;
                            }
                            else
                            {
                                startIndexOfCellInStartNode = 0;
                                MemoryPage nextpage = _pager.ReadPage((int)startNode.RightPage);
                                startNode = new BTreeNode(nextpage);
                            }
                        }
                        return FindCells(startNode, startIndexOfCellInStartNode, condition, attributeDeclarations, true);
                    default:
                        throw new Exception("The Operand is not supported!");
                }
                //This step may not need?
                return result;
            }
            else
            {
                return LinearSearch(root, condition, attributeDeclarations);
            }
        }

        // find the minimal node (leftest leaf node) from the given tree of root node `root`
        private BTreeNode FindMin(BTreeNode root)
        {
            BTreeNode child;
            InternalTableCell childCell;

            if (root.PageType == PageTypes.LeafTablePage)
            {
                return root;
            }
            // childCell = (InternalTableCell)root.GetBTreeCell(root.CellOffsetArray[0]);
            childCell = (InternalTableCell)root[0];
            MemoryPage nextpage = _pager.ReadPage((int)childCell.ChildPage);
            child = new BTreeNode(nextpage);

            return FindMin(child);
        }

        /// <summary>
        /// <para>
        /// to find cells that satisfy:
        ///  - starting from `startNode`
        ///  - ends in `upperBound`
        ///      - might be inclusive depending on `isIncludeUpperBound`
        ///  - satisfying `condition`
        /// </para>
        /// <para>`attributeDeclarations` := the names of the columns</para>
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="condition"></param>
        /// <param name="attributeDeclarations">the names of the columns</param>
        /// <param name="upperBound"></param>
        /// <param name="isIncludeUpperBound"></param>
        /// <returns></returns>
        private List<BTreeCell> FindCells(BTreeNode startNode, Expression condition, List<AttributeDeclaration> attributeDeclarations, AtomValue upperBound, bool isIncludeUpperBound)
        {
            List<BTreeCell> result = new List<BTreeCell>();
            LeafTableCell leafCell;

            while (true)
            {
                foreach (var cell in startNode)
                {
                    if (((cell.Key.GetValues()[0] > upperBound).BooleanValue && isIncludeUpperBound) ||
                        ((cell.Key.GetValues()[0] >= upperBound).BooleanValue && !isIncludeUpperBound))
                    {
                        return result;
                    }
                    leafCell = (LeafTableCell)cell;

                    if (condition.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                    {
                        result.Add(cell);
                    }
                }
                if (startNode.RightPage == 0)
                {
                    return result;
                }
                MemoryPage nextpage = _pager.ReadPage((int)startNode.RightPage);
                startNode = new BTreeNode(nextpage);
            }
        }

        /// <summary>
        /// <para>
        /// to find cells that satisfy:
        ///  - starting from `startIndexOfCell` in `startNode`
        ///      - `startIndexOfCell` might be inclusive depending on `isIncludeStartCell`
        ///  - till to the end of the leaf nodes
        ///  - satisfying `condition`
        /// </para>
        /// <para>`attributeDeclarations` := the names of the columns</para>
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="startIndexOfCell"></param>
        /// <param name="condition"></param>
        /// <param name="attributeDeclarations">the names of the columns</param>
        /// <param name="isIncludeStartCell"></param>
        /// <returns></returns>
        private List<BTreeCell> FindCells(BTreeNode startNode, int startIndexOfCell, Expression condition, List<AttributeDeclaration> attributeDeclarations, bool isIncludeStartCell)
        {
            List<BTreeCell> result = new List<BTreeCell>();
            LeafTableCell leafCell;

            if (isIncludeStartCell)
            {
                // leafCell = (LeafTableCell)startNode.GetBTreeCell(startNode.CellOffsetArray[startIndexOfCell]);
                leafCell = (LeafTableCell)startNode[startIndexOfCell];
                if (condition.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                {
                    result.Add(leafCell);
                }
            }
            for (int i = startIndexOfCell + 1; i < startNode.NumCells; i++)
            {
                // leafCell = (LeafTableCell)startNode.GetBTreeCell(startNode.CellOffsetArray[i]);
                leafCell = (LeafTableCell)startNode[i];
                if (condition.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                {
                    result.Add(leafCell);
                }
            }
            while (startNode.RightPage != 0)
            {
                MemoryPage nextpage = _pager.ReadPage((int)startNode.RightPage);
                startNode = new BTreeNode(nextpage);

                foreach (var cell in startNode)
                {
                    leafCell = (LeafTableCell)cell;
                    if (condition.Calculate(attributeDeclarations, leafCell.DBRecord.GetValues()).BooleanValue == true)
                    {
                        result.Add(cell);
                    }
                }
            }
            return result;
        }

        // if `isFuzzySearch`, this function will return the first cell that with key equal or larger than that of `cell`'s
        private BTreeNode FindNode(DBRecord key, BTreeNode root, bool isFuzzySearch = false)
        {
            BTreeNode child;
            BTreeCell cell;
            UInt16 offset;
            if (root.PageType == PageTypes.LeafTablePage)
            {
                (cell, _, _) = root.FindBTreeCell(key, isFuzzySearch);
                if (cell != null)
                    return root;
                else
                    return null;
            }

            //If it's internal node
            (cell, offset, _) = root.FindBTreeCell(key);
            MemoryPage nextpage;
            if (offset == 0)      //rightpage
            {
                nextpage = _pager.ReadPage((int)root.RightPage);
                child = new BTreeNode(nextpage);
            }
            else
            {
                InternalTableCell internalTableCell = (InternalTableCell)cell;
                nextpage = _pager.ReadPage((int)internalTableCell.ChildPage);
                child = new BTreeNode(nextpage);
            }
            return FindNode(key, child);
        }

        private List<BTreeCell> LinearSearch(BTreeNode root, Expression expression, List<AttributeDeclaration> attributeDeclarations)
        {
            BTreeNode startNode = FindMin(root);
            List<BTreeCell> result = new List<BTreeCell>();
            LeafTableCell leafCell;

            while (true)
            {
                foreach (var cell in startNode)
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
                if (startNode.RightPage == 0)
                {
                    return result;
                }
                MemoryPage nextpage = _pager.ReadPage((int)startNode.RightPage);
                startNode = new BTreeNode(nextpage);
            }
        }
    }
}
