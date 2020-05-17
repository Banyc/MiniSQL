using System;
using System.Threading;
using MiniSQL.BufferManager.Controllers;

namespace MiniSQL.BufferManager.Models
{
    // mapping a block to memory page and manage the page
    public class MemoryPage
    {
        // this core is shared by all MemoryPage with the same page number
        public class MemoryPageCore
        {
            // this page has been swapped out if this.data is null
            internal byte[] data = null;
            public UInt16 PageSize { get => (UInt16)data.Length; }
            public bool IsSwappedOut
            {
                get { return data == null; }
            }
            // prevent page swapping manually
            public bool IsPinned { get; set; } = false;
        }

        public MemoryPageCore Core { get; set; }
        private readonly Pager _pager = null;
        // start from 1
        public int PageNumber { get; private set; } = 0;
        // NOTICE: don't watch this property when debugging!
        public byte[] Data
        {
            get
            {
                if (Core.data == null)
                {
                    _pager.ReadPage(this);
                }
                this._pager.SetPageAsMostRecentlyUsed(this.PageNumber);
                return Core.data;
            }
            set
            {
                lock (this)
                {
                    this._pager.SetPageAsMostRecentlyUsed(this.PageNumber);
                    this.IsDirty = true;
                    Core.data = value;
                }
            }
        }
        // If the page has not saved after being modified
        public bool IsDirty { get; set; } = false;
        public UInt16 PageSize { get => this.Core.PageSize; }

        // prevent page swapping manually
        public bool IsPinned { get => this.Core.IsPinned; set => this.Core.IsPinned = value; }
        public bool IsSwappedOut { get => this.Core.IsSwappedOut; }

        // after which address the page is available.
        // in order not to corrupt the file header
        public ushort AvaliableOffset
        {
            get
            {
                if (PageNumber == 1)
                    return _pager.FileHeaderSize;
                return 0;
            }
        }
        // constructor
        public MemoryPage(Pager pager, int pageNumber)
        {
            this._pager = pager;
            this.PageNumber = pageNumber;
        }
        // write page back to file
        public void CommitChanges()
        {
            this._pager.WritePage(this);
        }
        // free up spaces
        public void Free()
        {
            this.Data = null;
        }
    }
}
