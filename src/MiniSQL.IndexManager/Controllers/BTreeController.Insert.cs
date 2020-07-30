using System;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Models;
using MiniSQL.Library.Exceptions;

namespace MiniSQL.IndexManager.Controllers
{
    public partial class BTreeController
    {
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
                BTreeCell check_repeat;
                ushort offSet;
                int indexinOffsetArray;
                (check_repeat,offSet,indexinOffsetArray)=node.FindBTreeCell(newKey,false);
                if(check_repeat!=null)
                {
                    
                    throw new RepeatedKeyException("The primary key to be inserted is repeated!");

                }
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
    }
}
