using System;
using System.Collections.Generic;
using System.Runtime;
using MiniSQL.BufferManager.Interfaces;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager.Controllers
{
    public class BTreeController //: IBufferManager
    {
        private readonly Pager _pager;
        private readonly FreeList _freeList;
        private readonly int MaxCell = 3;


        // constructor
        public BTreeController(Pager pager, FreeList freeList)
        {
            this._pager = pager;
            this._freeList = freeList;
        }

        /*public void InsertCell(BTreeNode root, BTreeCell cell)
        {
            throw new NotImplementedException();
        }

        public BTreeCell FindCell(DBRecord key)
        {
            throw new NotImplementedException();
        }

        public List<BTreeCell> FindCells(Expression expression)
        {
            throw new NotImplementedException();
        }

        public void DeleteCells(BTreeNode root, Expression expression)
        {
            throw new NotImplementedException();
        }

        public void DeleteCell(BTreeNode root, DBRecord key)
        {
            throw new NotImplementedException();
        }

        private void SplitNode(BTreeNode node)
        {
            throw new NotImplementedException();
        }*/

        // This functionality might no required
        private void MergeNode(BTreeNode leftNode, BTreeNode rightNode)
        {
            throw new NotImplementedException();
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
            BTreeNode node = new BTreeNode(newPage);
            node.InitializeEmptyFormat(nodeType);

            return node;
        }

        public BTreeNode Insert(DBRecord key, DBRecord dBRecord, BTreeNode Root)
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
                return InsertNonFull(Root, key, dBRecord);
            }

            //need to split,and return with a new root
            BTreeNode newRoot = GetNewNode(PageTypes.InternalTablePage);
            SplitChild(Root, newRoot, key);
            InsertNonFull(newRoot, key, dBRecord);
            return newRoot;
        }



        private void SplitChild(BTreeNode nodeTobeSplit, BTreeNode parantNode, DBRecord newKey)
        {
            int i;
            BTreeNode splitNode = GetNewNode(nodeTobeSplit.PageType);
            //List<ushort> offsets = nodeTobeSplit.CellOffsetArray;

            //key should be the primary key to be inserted in the parent node
            DBRecord key = nodeTobeSplit.GetBTreeCell(nodeTobeSplit.CellOffsetArray[nodeTobeSplit.NumCells / 2]).Key;
            int DeleteIndex = nodeTobeSplit.NumCells / 2;
            for (i = DeleteIndex; i < MaxCell; i++)
            {
                splitNode.InsertBTreeCell(nodeTobeSplit.GetBTreeCell(nodeTobeSplit.CellOffsetArray[DeleteIndex]));
                nodeTobeSplit.DeleteBTreeCell(nodeTobeSplit.CellOffsetArray[DeleteIndex]);
            }
            //If the parentNode need to spilt,this will be wrong
            InternalTableCell newCell = new InternalTableCell(key, (uint)nodeTobeSplit.GetRawPage().PageNumber);

            //connnect two child node by rightpage
            splitNode.RightPage = nodeTobeSplit.RightPage;
            nodeTobeSplit.RightPage = (uint)splitNode.GetRawPage().PageNumber;
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
                    InternalTableCell tmp_cell = (InternalTableCell)cell;
                    tmp_cell.ChildPage = (uint)splitNode.GetRawPage().PageNumber;
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

            }
            return InsertNonFull(child, newKey, dBRecord);

        }

        public BTreeCell Find(DBRecord key, BTreeNode root)
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
                (cell, offset, indexInOffsetArray) = root.FindBTreeCell(key);
                return cell;
            }

            //If it's internal node
            (cell, offset, indexInOffsetArray) = root.FindBTreeCell(key);
            if (offset == 0)  //rightpage
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




        /*The following is the previous code which cannot work*/

        //return with a position where the key is supposed to be,if it exist,isFind=True
        //The function mainly used by insertion and deletetion,we use another one to implenment the real Find in SQL
        /*public (BTreeNode result, bool isFind) Recur_FindNode(DBRecord keys, BTreeNode T)
        {
            BTreeNode result = T;
            MemoryPage Nextpage = null;
            if (T == null)
                return (result, false);
            else
            {
                BTreeCell cell;
                UInt16 offset;
                int indexInOffsetArray;
                //internal leaf,go to the child
                if (T.PageType == PageTypes.InternalIndexPage || T.PageType == PageTypes.InternalTablePage)
                {
                    (cell, offset, indexInOffsetArray) = T.FindBTreeCell(keys);
                    if (offset == 0)  //rightpage
                    {
                        Nextpage = _pager.ReadPage((int)T.RightPage);
                        result = new BTreeNode(Nextpage);
                    }
                    else             //child page in cell
                    {
                        if (cell.Types == CellTypes.InternalIndexCell)
                        {
                            InternalIndexCell internalIndexCell = (InternalIndexCell)cell;
                            Nextpage = _pager.ReadPage((int)internalIndexCell.ChildPage);
                            result = new BTreeNode(Nextpage);
                        }
                        else
                        {
                            InternalTableCell internalTableCell = (InternalTableCell)cell;
                            Nextpage = _pager.ReadPage((int)internalTableCell.ChildPage);
                            result = new BTreeNode(Nextpage);
                        }
                    }
                    return Recur_FindNode(keys, result);
                }
                //leaf node
                else
                {
                    (cell, offset, indexInOffsetArray) = T.FindBTreeCell(keys);
                    if (offset == 0)
                    {
                        return (T, false);
                    }
                    else
                    {
                        if (cell.Key == keys)
                        {
                            return (T, true);
                        }
                        else
                        {
                            return (T, false);
                        }
                    }
                }
            }
        }
        //implenment by InsertWithoutSplit and InsertWithSplit,return with the new BTree Root
        public BTreeNode InsertNode(BTreeCell cell, BTreeNode T)
        {
            BTreeNode targer = null;
            bool flag;
            (targer, flag) = Recur_FindNode(cell.Key, T);
            if (flag == true)
                throw new Exception("Cannot insert duplicate key!");
            else
            {
                if (T.NumCells >= MaxCell)
                {
                    return InsertWithSplit(cell,targer,T);
                }
                else if(targer=null){                            //empty tree
                    targer=GetNewNode(PageTypes.LeafTablePage);
                    targer.InsertBTreeCell(cell);
                    T=targer;
                    return T;
                }
                else{
                    targer.InsertBTreeCell(cell);
                    return T;
                }
                    
            }
        }

        public BTreeNode InsertWithSplit(BTreeCell cell, BTreeNode T, BTreeNode root)
        {
            int i;
            BTreeNode splitNode =GetNewNode(PageTypes.LeafTablePage);
            List<ushort> offsets = BTreeNode.CellOffsetArray;

            DBRecord key=T.GetBTreeCell(offsets[T.NumCells/2]).Key;
            for(i=T.NumCells/2;i<Maxcell;i++)
            {
                splitNode.InsertBTreeCell(T.GetBTreeCell(offsets[i]));
                T.DeleteBTreeCell(offsets[i]);
            }
            //judge the position of the cell waiting to be inserted
            T.InsertBTreeCell(cell);

            //connnect two page by rightpage
            splitNode.RightPage = T.RightPage;
            T.RightPage = splitNode.GetRawPage().PageNumber;
            return InsertParentsNode(T,splitNode,key,root);    //maybe the root isn't needed because we can use parentPage
        }

        //tricky function:there remains many problem:
        //Noted that only child_left has parent Node at first
        public BTreeNode InsertParentsNode(BTreeNode child_left,BTreeNode child_right,DBRecord key,BTreeNode root)
        {
            BTreeNode parents=null;
            //No parents above, need to create a new one
            if(child_left.ParentPage==0) 
            {
                BTreeNode Head=GetNewNode(PageTypes.InternalTablePage);
                BTreeCell NewCell=new InternalTableCell(key,child_left.GetRawPage().PageNumber);
                Head.InsertBTreeCell(NewCell);
                Head.RightPage=child_right.GetRawPage().PageNumber;
                child_right.ParentPage=Head.GetRawPage().PageNumber;
                return Head;
            }
            else
            {
                parents=_pager.ReadPage(child_left.ParentPage);
                if(parents.NumCells<MaxCell)  //No need to split
                {
                    BTreeCell NewCell=new InternalTableCell(key,child_left.GetRawPage().PageNumber);
                    if(parents.RightPage==child_left.GetRawPage().PageNumber)
                    {
                        parents.RightPage=child_left.GetRawPage().PageNumber;
                    }
                    else
                    {
                        BTreeNode leftNode=_pager.ReadPage(child_left.RightPage);

                        List<ushort> offsets = leftNode.CellOffsetArray;
                        BTreeCell tmp = leftNode.GetBTreeCell(offsets[0]);

                    }
                }
            }

            */

        /* BTreeNode result = root;
         MemoryPage Nextpage = null;
         BTreeCell cell;
         UInt16 offset;
         int indexInOffsetArray;

         //sovle the problem recursively,find the node until it get to the leaves,then split and connected the node one by one
         if (root.PageType == PageTypes.InternalIndexPage || root.PageType == PageTypes.InternalTablePage)
         {
              (cell, offset, indexInOffsetArray) = root.FindBTreeCell(key);
              if (offset == 0)  //rightpage
              {
                 Nextpage = _pager.ReadPage((int)root.RightPage);
                 result = new BTreeNode(Nextpage);
              }
              else             //child page in cell
              {
                 if (cell.Types == CellTypes.InternalIndexCell)   //these code is useless because we has ignore the index tree
                 {
                     InternalIndexCell internalIndexCell = (InternalIndexCell)cell;
                     Nextpage = _pager.ReadPage((int)internalIndexCell.ChildPage);
                     result = new BTreeNode(Nextpage);
                 }
                 else
                 {
                     InternalTableCell internalTableCell = (InternalTableCell)cell;
                     Nextpage = _pager.ReadPage((int)internalTableCell.ChildPage);
                     result = new BTreeNode(Nextpage);
                  }
              }
             BTreeNode curNode = InsertParentsNode(child_left, child_right, key, result);
             //no need to split
             if(curNode.NumCells < MaxCell)   
             {
                 InternalTableCell insertCell=new InternalTableCell(key,child_left.GetRawPage().PageNumber);
                 curNode.InsertBTreeCell(insertCell);
                 return curNode;    //WARINING!Should I return the curNode or someting else?????
             }


         }
         //when comes to the leaves node,return to the last recursion
         //but return with what??????
         else
         {

         }
     }

     //Same as InsertNode
     public BTreeNode DeleteNode(BTreeCell cell)
     {

     }

     public BTreeNode DeleteWithoutMerge(BTreeCell cell, BTreeNode T)
     {
         BTreeNode targer = null;
         bool flag;
         (targer, flag) = Recur_FindNode(cell.Key, T);
         if (flag == false)
             throw new Exception("Cannot find the cell!");
         else
         {
             T.DeleteBTreeCell(cell);
         }
     }

     public BTreeNode DeleteWithMerge(BTreeCell cell)
     {

     }

     //The real Find operation
     public List<BTreeCell> FindCells(Expression expression)
     {
     }
     */



    }
}
