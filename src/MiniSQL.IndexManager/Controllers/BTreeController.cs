using System;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.IndexManager.Models;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Interfaces;

namespace MiniSQL.IndexManager.Controllers
{
    public partial class BTreeController : IIndexManager
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
                throw new Exception($"maxCell expected at least 4, actually {maxCell}");
            this.MaxCell = maxCell;
        }

        // recycle page to free list
        private void DeleteNode(BTreeNode node)
        {
            MemoryPage page = node.RawPage;
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
    }
}
