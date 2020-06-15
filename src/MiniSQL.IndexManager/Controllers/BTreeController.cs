using System;
using System.Collections.Generic;
using System.Runtime;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.IndexManager.Models;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Interfaces;
using MiniSQL.Library.Models;
using MiniSQL.IndexManager.Utilities;

namespace MiniSQL.IndexManager.Controllers
{
    public class BTreeController : IIndexManager
    {
        private readonly Pager _pager;
        private readonly FreeList _freeList;
        public int MaxCell { get; }  // At least 4

        // constructor
        public BTreeController(Pager pager, FreeList freeList, int maxCell = 4)
        {
            this._pager = pager;
            this._freeList = freeList;
            if (maxCell < 4)
                throw new Exception($"maxCell expected at least 4, actual {maxCell}");
            this.MaxCell = maxCell;
        }

        // TODO: test
        public BTreeNode OccupyNewTableNode()
        {
            BTreeNode newNode = GetNewNode(PageTypes.LeafTablePage);
            return newNode;
        }

        // TODO: test
        public void RemoveTree(BTreeNode root)
        {
            if (root.PageType == PageTypes.LeafIndexPage || root.PageType == PageTypes.LeafTablePage)
            {
                DeleteNode(root);
                return;
            }
            if (root.PageType == PageTypes.InternalIndexPage)
            {
                foreach (BTreeCell cell in root)
                {
                    MemoryPage page = _pager.ReadPage((int)((InternalIndexCell)cell).ChildPage);
                    BTreeNode node = new BTreeNode(page);
                    RemoveTree(node);
                }
            }
            else  // (root.PageType == PageTypes.InternalTablePage)
            {
                foreach (BTreeCell cell in root)
                {
                    MemoryPage page = _pager.ReadPage((int)((InternalTableCell)cell).ChildPage);
                    BTreeNode node = new BTreeNode(page);
                    RemoveTree(node);
                }
            }
            MemoryPage rightPage = _pager.ReadPage((int)root.RightPage);
            BTreeNode rightNode = new BTreeNode(rightPage);
            RemoveTree(rightNode);
            // post-order traversal
            DeleteNode(root);
        }

        // recycle page to free list
        private void DeleteNode(BTreeNode node)
        {
            MemoryPage page = node.GetRawPage();
            node.IsDisabled = true;
            _freeList.RecyclePage(page);
        }

        // allocate a new node from free list or from pager
        private BTreeNode GetNewNode(PageTypes nodeType)
        {
            // allocate
            MemoryPage newPage = _freeList.AllocatePage();
            if (newPage == null)
            {
                newPage = _pager.GetNewPage();
            }

            // initialize node
            BTreeNode node = new BTreeNode(newPage, nodeType);

            return node;
        }


        public BTreeNode InsertCell(BTreeNode Root, DBRecord key, DBRecord dBRecord)
        {
            //create a new root
            if (Root == null)
            {
                BTreeNode root = GetNewNode(PageTypes.LeafTablePage);
                LeafTableCell NewCell = new LeafTableCell(key, dBRecord);
                root.InsertBTreeCell(NewCell);
                return root;
            }
            //no need to split
            if (Root.NumCells < MaxCell)
            {
                InsertNonFull(Root, key, dBRecord);
                return Root;
            }

            //need to split,and return with a new root
            BTreeNode newRoot = GetNewNode(PageTypes.InternalTablePage);
            Root.ParentPage = (uint)newRoot.GetRawPage().PageNumber;
            SplitChild(Root, newRoot, key);
            InsertNonFull(newRoot, key, dBRecord);
            return newRoot;
        }

        private void SplitChild(BTreeNode nodeTobeSplit, BTreeNode parantNode, DBRecord newKey)
        {
            int i;
            BTreeNode splitNode = GetNewNode(nodeTobeSplit.PageType);
            //tmp is used for the change of parentPage
            InternalTableCell tmpCell;
            MemoryPage tmpPage;
            BTreeNode tmpNode;

            //key should be the primary key to be inserted in the parent node
            int DeleteIndex = nodeTobeSplit.NumCells / 2;
            DBRecord key = nodeTobeSplit.GetBTreeCell(nodeTobeSplit.CellOffsetArray[DeleteIndex - 1]).Key;
            if ((newKey.GetValues()[0] >= key.GetValues()[0]).BooleanValue)
            {
                DeleteIndex += 1;
                key = nodeTobeSplit.GetBTreeCell(nodeTobeSplit.CellOffsetArray[DeleteIndex - 1]).Key;
            }

            for (i = DeleteIndex; i < MaxCell; i++)
            {
                ushort DeleteOffSet = nodeTobeSplit.CellOffsetArray[DeleteIndex];
                //change the parent Page
                if (nodeTobeSplit.PageType == PageTypes.InternalTablePage)
                {
                    tmpCell = (InternalTableCell)nodeTobeSplit.GetBTreeCell(DeleteOffSet);
                    tmpPage = _pager.ReadPage((int)tmpCell.ChildPage);
                    tmpNode = new BTreeNode(tmpPage);
                    tmpNode.ParentPage = (uint)splitNode.GetRawPage().PageNumber;
                }

                splitNode.InsertBTreeCell(nodeTobeSplit.GetBTreeCell(DeleteOffSet));
                nodeTobeSplit.DeleteBTreeCell(DeleteOffSet);

            }
            //If the parentNode need to spilt,this will be wrong
            InternalTableCell newCell = new InternalTableCell(key, (uint)nodeTobeSplit.GetRawPage().PageNumber);

            //connnect two child node by rightpage
            splitNode.RightPage = nodeTobeSplit.RightPage;
            if (nodeTobeSplit.PageType == PageTypes.LeafTablePage)
            {
                nodeTobeSplit.RightPage = (uint)splitNode.GetRawPage().PageNumber;
            }
            //for a internal node,a cell in nodeTobeSplit has to be deleted
            else if (nodeTobeSplit.PageType == PageTypes.InternalTablePage)
            {
                //for internal node,the parent Page of child page need to be changed
                tmpPage = _pager.ReadPage((int)nodeTobeSplit.RightPage);
                tmpNode = new BTreeNode(tmpPage);
                tmpNode.ParentPage = (uint)splitNode.GetRawPage().PageNumber;


                InternalTableCell tmp_cell = (InternalTableCell)nodeTobeSplit.GetBTreeCell(nodeTobeSplit.CellOffsetArray[DeleteIndex - 1]);
                nodeTobeSplit.RightPage = tmp_cell.ChildPage;
                nodeTobeSplit.DeleteBTreeCell(nodeTobeSplit.CellOffsetArray[DeleteIndex - 1]);

                tmpPage = _pager.ReadPage((int)tmp_cell.ChildPage);
                tmpNode = new BTreeNode(tmpPage);
                tmpNode.ParentPage = (uint)nodeTobeSplit.GetRawPage().PageNumber;
            }

            splitNode.ParentPage = (uint)parantNode.GetRawPage().PageNumber;

            //reconnect the particular cell(or right) in the parent node because of the change of the child node
            //This is a new empty root
            if (parantNode.NumCells == 0)
            {
                parantNode.RightPage = (uint)splitNode.GetRawPage().PageNumber;
            }
            //There are some cell in the parents node
            else
            {
                BTreeCell cell;
                UInt16 offset;
                int indexInOffsetArray;
                (cell, offset, indexInOffsetArray) = parantNode.FindBTreeCell(key);
                //The case when the node to be inserted into parent node is in the rightest position
                if (cell == null)
                {
                    parantNode.RightPage = (uint)splitNode.GetRawPage().PageNumber;
                }
                //The normal case
                else
                {
                    InternalTableCell tmp_cell = new InternalTableCell(cell.Key, (uint)splitNode.GetRawPage().PageNumber);
                    parantNode.DeleteBTreeCell(cell);
                    parantNode.InsertBTreeCell(tmp_cell);
                }
            }
            //This must be done after we reconnect the treeNode
            parantNode.InsertBTreeCell(newCell);

        }

        private BTreeNode InsertNonFull(BTreeNode node, DBRecord newKey, DBRecord dBRecord)
        {
            if (node.PageType == PageTypes.LeafTablePage)
            {
                LeafTableCell newCell = new LeafTableCell(newKey, dBRecord);
                node.InsertBTreeCell(newCell);
                return node;
            }

            BTreeNode child;
            MemoryPage Nextpage = null;
            BTreeCell cell;
            UInt16 offset;
            int indexInOffsetArray;
            (cell, offset, indexInOffsetArray) = node.FindBTreeCell(newKey);
            if (offset == 0)  //rightpage
            {
                Nextpage = _pager.ReadPage((int)node.RightPage);
                child = new BTreeNode(Nextpage);
            }
            else
            {
                InternalTableCell internalTableCell = (InternalTableCell)cell;
                Nextpage = _pager.ReadPage((int)internalTableCell.ChildPage);
                child = new BTreeNode(Nextpage);
            }
            if (child.NumCells >= MaxCell)
            {
                SplitChild(child, node, newKey);
                return InsertNonFull(node, newKey, dBRecord);

            }
            return InsertNonFull(child, newKey, dBRecord);

        }

        public BTreeCell FindCell(DBRecord key, BTreeNode root)
        {
            return InternalFind(key, root);
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

        public BTreeNode Delete(DBRecord key, BTreeNode Root)
        {
            BTreeNode NodeTobeDeleted = FindNode(key, Root);
            if (NodeTobeDeleted == null)
            {
                throw new Exception("Cannot find the key!");
            }
            return Delete_entry(NodeTobeDeleted, key, Root);

        }

        public BTreeNode Delete(BTreeCell keyCell, BTreeNode Root)
        {
            DBRecord key = keyCell.Key;
            return Delete(key, Root);
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

        private BTreeNode Delete_leaf_redistri(BTreeNode NodetobeDeleted, DBRecord key, BTreeNode Root)
        {
            BTreeCell cell;
            UInt16 offset;
            int indexInOffsetArray;

            MemoryPage parentPage = _pager.ReadPage((int)NodetobeDeleted.ParentPage);
            BTreeNode parentNode = new BTreeNode(parentPage);

            (cell, offset, indexInOffsetArray) = parentNode.FindBTreeCell(key);
            //the deleted node is not on the right page of parentNode
            if (cell != null)
            {
                MemoryPage brotherPage = _pager.ReadPage((int)NodetobeDeleted.RightPage);
                BTreeNode brotherNode = new BTreeNode(brotherPage);
                //If there is a node on the left of the deleted node,it need to be connected to the brother node.
                if (indexInOffsetArray >= 1)
                {
                    InternalTableCell leftNodeCell = (InternalTableCell)parentNode.GetBTreeCell(parentNode.CellOffsetArray[indexInOffsetArray - 1]);
                    MemoryPage leftPage = _pager.ReadPage((int)leftNodeCell.ChildPage);
                    BTreeNode leftNode = new BTreeNode(leftPage);
                    leftNode.RightPage = (uint)brotherNode.GetRawPage().PageNumber;
                }

                if (brotherNode.NumCells + NodetobeDeleted.NumCells <= MaxCell)  //merge
                {
                    //After the merge,one cell in the parentNode will be deleted
                    parentNode.DeleteBTreeCell(cell);
                    //merge two node
                    for (int i = 0; i < NodetobeDeleted.NumCells; i++)
                    {
                        brotherNode.InsertBTreeCell(NodetobeDeleted.GetBTreeCell(NodetobeDeleted.CellOffsetArray[i]));
                    }
                    DeleteNode(NodetobeDeleted);

                }
                else    //redistribute
                {
                    BTreeCell movedCell = brotherNode.GetBTreeCell(brotherNode.CellOffsetArray[0]);
                    DBRecord newKey = brotherNode.GetBTreeCell(brotherNode.CellOffsetArray[1]).Key;

                    NodetobeDeleted.InsertBTreeCell(movedCell);
                    brotherNode.DeleteBTreeCell(movedCell);

                    BTreeCell parent_Cell;
                    UInt16 parent_offset;
                    int parent_indexInOffsetArray;
                    (parent_Cell, parent_offset, parent_indexInOffsetArray) = parentNode.FindBTreeCell(key);

                    InternalTableCell newCell = new InternalTableCell(newKey, (uint)NodetobeDeleted.GetRawPage().PageNumber);
                    parentNode.DeleteBTreeCell(parent_Cell);
                    parentNode.InsertBTreeCell(newCell);

                }
            }
            //The node to be deleted is on the rightpage,the brother node is on the left
            else
            {
                InternalTableCell brotherCell = (InternalTableCell)parentNode.GetBTreeCell(parentNode.CellOffsetArray[parentNode.NumCells - 1]);
                MemoryPage brotherPage = _pager.ReadPage((int)brotherCell.ChildPage);
                BTreeNode brotherNode = new BTreeNode(brotherPage);

                //The right page of brother node should be 0

                brotherNode.RightPage = NodetobeDeleted.RightPage;

                if (brotherNode.NumCells + NodetobeDeleted.NumCells <= MaxCell)  //merge
                {
                    //After the merge,one cell in the parentNode will be deleted
                    parentNode.DeleteBTreeCell(brotherCell);
                    //merge two node
                    for (int i = 0; i < NodetobeDeleted.NumCells; i++)
                    {
                        brotherNode.InsertBTreeCell(NodetobeDeleted.GetBTreeCell(NodetobeDeleted.CellOffsetArray[i]));
                    }
                    DeleteNode(NodetobeDeleted);
                    parentNode.RightPage = (uint)brotherNode.GetRawPage().PageNumber;

                }
                else    //redistribute
                {
                    BTreeCell movedCell = brotherNode.GetBTreeCell(brotherNode.CellOffsetArray[brotherNode.NumCells - 1]);
                    DBRecord newKey = movedCell.Key;

                    NodetobeDeleted.InsertBTreeCell(movedCell);
                    brotherNode.DeleteBTreeCell(movedCell);

                    BTreeCell parent_Cell;
                    UInt16 parent_offset;
                    int parent_indexInOffsetArray;
                    (parent_Cell, parent_offset, parent_indexInOffsetArray) = parentNode.FindBTreeCell(key);

                    InternalTableCell newCell = new InternalTableCell(newKey, (uint)NodetobeDeleted.GetRawPage().PageNumber);
                    parentNode.DeleteBTreeCell(parent_Cell);
                    parentNode.InsertBTreeCell(newCell);

                }

            }


            NodetobeDeleted = parentNode;
            //deal with parent Nodes
            return Delete_internal_redistri(NodetobeDeleted, key, Root);
        }

        private BTreeNode Delete_internal_redistri(BTreeNode NodetobeDeleted, DBRecord key, BTreeNode Root)
        {
            InternalTableCell tmpCell;
            MemoryPage tmpPage;
            BTreeNode tmpNode;

            BTreeCell cell;
            UInt16 offset;
            int indexInOffsetArray;

            MemoryPage parentPage = null;
            BTreeNode parentNode = null;

            while (NodetobeDeleted.NumCells < (MaxCell + 1) / 2)
            {
                //The root
                if (NodetobeDeleted.ParentPage == 0)
                {
                    if (NodetobeDeleted.NumCells == 0)
                    {
                        MemoryPage NewRootPage = _pager.ReadPage((int)NodetobeDeleted.RightPage);
                        BTreeNode newRoot = new BTreeNode(NewRootPage);
                        newRoot.ParentPage = 0;
                        return newRoot;
                    }
                    return Root;
                }
                else
                {
                    parentPage = _pager.ReadPage((int)NodetobeDeleted.ParentPage);
                    parentNode = new BTreeNode(parentPage);
                    (cell, offset, indexInOffsetArray) = parentNode.FindBTreeCell(key);
                    //The node to be deleted isn't on the rightpage
                    if (cell != null)
                    {
                        InternalTableCell brotherCell = null;
                        MemoryPage brotherPage = null;
                        BTreeNode brotherNode = null;
                        //if the right brother is on rightpage
                        if (indexInOffsetArray + 1 == parentNode.NumCells)
                        {
                            brotherPage = _pager.ReadPage((int)parentNode.RightPage);
                            brotherNode = new BTreeNode(brotherPage);

                        }
                        else
                        {
                            brotherCell = (InternalTableCell)parentNode.GetBTreeCell(parentNode.CellOffsetArray[indexInOffsetArray + 1]);
                            brotherPage = _pager.ReadPage((int)brotherCell.ChildPage);
                            brotherNode = new BTreeNode(brotherPage);
                        }
                        //merge
                        if (brotherNode.NumCells + NodetobeDeleted.NumCells < MaxCell)
                        {
                            for (int i = 0; i < NodetobeDeleted.NumCells; i++)
                            {
                                tmpCell = (InternalTableCell)NodetobeDeleted.GetBTreeCell(NodetobeDeleted.CellOffsetArray[i]);

                                brotherNode.InsertBTreeCell(tmpCell);

                                tmpPage = _pager.ReadPage((int)tmpCell.ChildPage);
                                tmpNode = new BTreeNode(tmpPage);
                                tmpNode.ParentPage = (uint)brotherNode.GetRawPage().PageNumber;
                            }
                            //move the cell in parentNode to brotherNode
                            InternalTableCell insertCell = new InternalTableCell(cell.Key, (uint)NodetobeDeleted.RightPage);
                            brotherNode.InsertBTreeCell(insertCell);
                            tmpPage = _pager.ReadPage((int)insertCell.ChildPage);
                            tmpNode = new BTreeNode(tmpPage);
                            tmpNode.ParentPage = (uint)brotherNode.GetRawPage().PageNumber;

                            DeleteNode(NodetobeDeleted);
                            parentNode.DeleteBTreeCell(cell);
                        }
                        //redistribute
                        else
                        {
                            InternalTableCell movedCell = (InternalTableCell)brotherNode.GetBTreeCell(brotherNode.CellOffsetArray[0]);
                            DBRecord upperKey = movedCell.Key;
                            DBRecord downKey = cell.Key;

                            InternalTableCell insertDeletedCell = new InternalTableCell(downKey, (uint)NodetobeDeleted.RightPage);
                            NodetobeDeleted.InsertBTreeCell(insertDeletedCell);
                            NodetobeDeleted.RightPage = movedCell.ChildPage;

                            tmpPage = _pager.ReadPage((int)movedCell.ChildPage);
                            tmpNode = new BTreeNode(tmpPage);
                            tmpNode.ParentPage = (uint)NodetobeDeleted.GetRawPage().PageNumber;

                            InternalTableCell insertParentCell = new InternalTableCell(upperKey, (uint)NodetobeDeleted.GetRawPage().PageNumber);
                            parentNode.DeleteBTreeCell(cell);
                            parentNode.InsertBTreeCell(insertParentCell);
                            brotherNode.DeleteBTreeCell(movedCell);

                        }

                    }
                    //The node to be deleted is on the rightpage
                    else
                    {
                        InternalTableCell brotherCell = null;
                        MemoryPage brotherPage = null;
                        BTreeNode brotherNode = null;

                        brotherCell = (InternalTableCell)parentNode.GetBTreeCell(parentNode.CellOffsetArray[parentNode.NumCells - 1]);
                        brotherPage = _pager.ReadPage((int)brotherCell.ChildPage);
                        brotherNode = new BTreeNode(brotherPage);
                        //merge
                        if (brotherNode.NumCells + NodetobeDeleted.NumCells < MaxCell)
                        {
                            for (int i = 0; i < brotherNode.NumCells; i++)
                            {
                                tmpCell = (InternalTableCell)brotherNode.GetBTreeCell(brotherNode.CellOffsetArray[i]);

                                NodetobeDeleted.InsertBTreeCell(tmpCell);

                                tmpPage = _pager.ReadPage((int)tmpCell.ChildPage);
                                tmpNode = new BTreeNode(tmpPage);
                                tmpNode.ParentPage = (uint)NodetobeDeleted.GetRawPage().PageNumber;
                            }
                            //move the cell in parentNode to brotherNode
                            InternalTableCell insertCell = new InternalTableCell(brotherCell.Key, (uint)brotherNode.RightPage);
                            NodetobeDeleted.InsertBTreeCell(insertCell);
                            tmpPage = _pager.ReadPage((int)insertCell.ChildPage);
                            tmpNode = new BTreeNode(tmpPage);
                            tmpNode.ParentPage = (uint)NodetobeDeleted.GetRawPage().PageNumber;

                            DeleteNode(brotherNode);
                            parentNode.DeleteBTreeCell(parentNode.GetBTreeCell(parentNode.CellOffsetArray[parentNode.NumCells - 1]));
                        }
                        //redistribute
                        else
                        {
                            DBRecord downKey = parentNode.GetBTreeCell(parentNode.CellOffsetArray[parentNode.NumCells - 1]).Key;
                            InternalTableCell insertDeletedCell = new InternalTableCell(downKey, brotherNode.RightPage);
                            NodetobeDeleted.InsertBTreeCell(insertDeletedCell);


                            InternalTableCell movedCell = (InternalTableCell)brotherNode.GetBTreeCell(brotherNode.CellOffsetArray[brotherNode.NumCells - 1]);
                            brotherNode.RightPage = movedCell.ChildPage;
                            DBRecord upperKey = movedCell.Key;
                            InternalTableCell insertParentCell = new InternalTableCell(upperKey, (uint)brotherNode.GetRawPage().PageNumber);
                            parentNode.InsertBTreeCell(insertParentCell);
                            brotherNode.DeleteBTreeCell(movedCell);

                            tmpPage = _pager.ReadPage((int)brotherNode.RightPage);
                            tmpNode = new BTreeNode(tmpPage);
                            tmpNode.ParentPage = (uint)brotherNode.GetRawPage().PageNumber;
                        }
                    }
                }
                NodetobeDeleted = parentNode;
            }
            return Root;
        }

        private BTreeNode Delete_entry(BTreeNode NodetobeDeleted, DBRecord key, BTreeNode Root)
        {
            BTreeCell cell;
            UInt16 offset;
            int indexInOffsetArray;
            (cell, offset, indexInOffsetArray) = NodetobeDeleted.FindBTreeCell(key, false);
            NodetobeDeleted.DeleteBTreeCell(cell);

            if (NodetobeDeleted.NumCells < MaxCell / 2 && NodetobeDeleted.ParentPage != 0)
            {
                return Delete_leaf_redistri(NodetobeDeleted, key, Root);
            }
            return Root;
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




        public List<BTreeCell> FindCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations)
        {
            if (expression == null)
            {
                return LinearSearch(root, expression, attributeDeclarations);
            }
            else if (expression.Ands.ContainsKey(keyName))
            {
                List<BTreeCell> result = new List<BTreeCell>();
                List<AtomValue> values = new List<AtomValue>();
                BTreeNode beginNode;

                BTreeCell cell;
                UInt16 offset;
                int begin_Index;

                AtomValue bound = expression.Ands[keyName].RightOperant.ConcreteValue;

                values.Add(bound);
                DBRecord keyFind = new DBRecord(values);
                switch (expression.Ands[keyName].Operator)
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
                        throw new Exception("The Operant is not supported!");
                }
                //This step may not need?
                return result;

            }
            else
            {
                return LinearSearch(root, expression, attributeDeclarations);
            }


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

        public BTreeNode DeleteCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations)
        {
            List<BTreeCell> nodesTobeDeleted = FindCells(root, expression, keyName, attributeDeclarations);
            foreach (var cell in nodesTobeDeleted)
            {
                root = Delete(cell, root);
            }
            return root;
        }

    }
}
