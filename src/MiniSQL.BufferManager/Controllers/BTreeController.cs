using System;
using MiniSQL.BufferManager.Models;

namespace MiniSQL.BufferManager.Controllers
{
    public class BTreeController
    {
        private readonly Pager _pager;
        private readonly FreeList _freeList;

        // constructor
        public BTreeController(Pager pager, FreeList freeList)
        {
            this._pager = pager;
            this._freeList = freeList;
        }

        public void InsertCell(BTreeNode root, BTreeCell cell)
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
    }
}
