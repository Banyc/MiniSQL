using System;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Models;
using MiniSQL.Library.Exceptions;
using MiniSQL.Library.Models;

namespace MiniSQL.IndexManager.Controllers
{
    public partial class BTreeController
    {
        public BTreeNode DeleteCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations)
        {
            List<BTreeCell> nodesTobeDeleted = FindCells(root, expression, keyName, attributeDeclarations);
            foreach (var cell in nodesTobeDeleted)
            {
                root = Delete(cell, root);
            }
            return root;
        }

        public BTreeNode Delete(DBRecord key, BTreeNode Root)
        {
            BTreeNode NodeTobeDeleted = FindNode(key, Root);
            if (NodeTobeDeleted == null)
            {
                throw new KeyNotExistsException("Cannot find the key!");
            }
            return Delete_entry(NodeTobeDeleted, key, Root);
        }

        public BTreeNode Delete(BTreeCell keyCell, BTreeNode Root)
        {
            DBRecord key = keyCell.Key;
            return Delete(key, Root);
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
                    leftNode.RightPage = (uint)brotherNode.RawPage.PageNumber;
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

                    InternalTableCell newCell = new InternalTableCell(newKey, (uint)NodetobeDeleted.RawPage.PageNumber);
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
                    parentNode.RightPage = (uint)brotherNode.RawPage.PageNumber;

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

                    InternalTableCell newCell = new InternalTableCell(newKey, (uint)NodetobeDeleted.RawPage.PageNumber);
                    parentNode.DeleteBTreeCell(parent_Cell);
                    parentNode.InsertBTreeCell(newCell);

                }

            }


            NodetobeDeleted = parentNode;
            //deal with parent Nodes
            return Delete_internal_redistribute(NodetobeDeleted, key, Root);
        }

        private BTreeNode Delete_internal_redistribute(BTreeNode NodetobeDeleted, DBRecord key, BTreeNode Root)
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
                                tmpNode.ParentPage = (uint)brotherNode.RawPage.PageNumber;
                            }
                            //move the cell in parentNode to brotherNode
                            InternalTableCell insertCell = new InternalTableCell(cell.Key, (uint)NodetobeDeleted.RightPage);
                            brotherNode.InsertBTreeCell(insertCell);
                            tmpPage = _pager.ReadPage((int)insertCell.ChildPage);
                            tmpNode = new BTreeNode(tmpPage);
                            tmpNode.ParentPage = (uint)brotherNode.RawPage.PageNumber;

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
                            tmpNode.ParentPage = (uint)NodetobeDeleted.RawPage.PageNumber;

                            InternalTableCell insertParentCell = new InternalTableCell(upperKey, (uint)NodetobeDeleted.RawPage.PageNumber);
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
                                tmpNode.ParentPage = (uint)NodetobeDeleted.RawPage.PageNumber;
                            }
                            //move the cell in parentNode to brotherNode
                            InternalTableCell insertCell = new InternalTableCell(brotherCell.Key, (uint)brotherNode.RightPage);
                            NodetobeDeleted.InsertBTreeCell(insertCell);
                            tmpPage = _pager.ReadPage((int)insertCell.ChildPage);
                            tmpNode = new BTreeNode(tmpPage);
                            tmpNode.ParentPage = (uint)NodetobeDeleted.RawPage.PageNumber;

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
                            InternalTableCell insertParentCell = new InternalTableCell(upperKey, (uint)brotherNode.RawPage.PageNumber);
                            parentNode.InsertBTreeCell(insertParentCell);
                            brotherNode.DeleteBTreeCell(movedCell);

                            tmpPage = _pager.ReadPage((int)brotherNode.RightPage);
                            tmpNode = new BTreeNode(tmpPage);
                            tmpNode.ParentPage = (uint)brotherNode.RawPage.PageNumber;
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
    }
}
