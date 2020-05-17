using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager.Controllers
{
    public class BTreeController
    {
        private readonly Pager _pager;
        private readonly ushort MaxCell = 100;
        // constructor
        public BTreeController(Pager pager)
        {
            this._pager = pager;
        }

        /*public void InsertCell(BTreeNode root, BTreeCell cell)
        {
            throw new NotImplementedException();
        }

        public void DeleteCell(BTreeNode root, BTreeCell cell)
        {
            throw new NotImplementedException();
        }

        public BTreeCell FindCell(DBRecord key)
        {
            throw new NotImplementedException();
        }

        private void SplitNode(BTreeNode node)
        {
            throw new NotImplementedException();
        }

        // This functionality might no required
        private void MergeNode(BTreeNode leftNode, BTreeNode rightNode)
        {
            throw new NotImplementedException();
        }*/


        public (BTreeNode result, bool isFind) Recur_FindNode(DBRecord keys, BTreeNode T)
        {
            BTreeNode result = T;
            MemoryPage Nextpage = null;
            if (this == null)
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
                        Nextpage = _pager.ReadPage(T.RightPage);
                        result = new BTreeNode(Nextpage);
                    }
                    else             //child page in cell
                    {
                        if (cell.Types == CellTypes.InternalIndexCell)
                        {
                            InternalIndexCell internalIndexCell = (InternalIndexCell)cell;
                            Nextpage = _pager.ReadPage(internalIndexCell.ChildPage);
                            result = new BTreeNode(Nextpage);
                        }
                        else
                        {
                            InternalTableCell internalTableCell = (InternalTableCell)cell;
                            Nextpage = _pager.ReadPage(internalTableCell.ChildPage);
                            result = new BTreeNode(Nextpage);
                        }
                    }
                    return Recur_FindNode(keys,result);
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

        public void InsertNode(BTreeCell cell)
        {

        }
        public void InsertWithoutSplit(BTreeCell cell,BTreeNode T)
        {
            BTreeNode targer = null;
            bool flag;
            (targer, flag) = Recur_FindNode(cell.Key, T);
            if(flag==true)
                throw new Exception("Cannot insert duplicate key!");
            else
            {
                if (T.NumCells >= MaxCell)
                {
                    throw new Exception("The cells in the page overflow!");
                }
                else
                    T.InsertBTreeCell(cell);
            }
        }

        public void InsertWithSplit(BTreeCell cell)
        {

        }


        public void DeleteNode(BTreeCell cell)
        {

        }

        public void DeleteWithoutMerge(BTreeCell cell,BTreeNode T)
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

        public void DeleteWithMerge(BTreeCell cell)
        {

        }




    }
}
