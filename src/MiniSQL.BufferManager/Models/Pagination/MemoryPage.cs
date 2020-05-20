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
            internal readonly Pager _pager;
            // start from 1
            public int PageNumber { get; internal set; } = 0;
            // this page has been swapped out if this.data is null
            // It is only permitted to be directly accessed by `Pager` to swap in
            internal byte[] data = null;
            public UInt16 PageSize { get => (UInt16)data.Length; }
            // if swapped out, the core will be out of date and needs to sync by swapping in. Replace the whole `Core` if necessary.
            public bool IsSwappedOut
            {
                get { return data == null; }
            }
            // prevent page swapping manually
            public bool IsPinned { get; set; } = false;
            // If the page has not saved after being modified
            public bool IsDirty { get; set; } = false;

            // constructor
            public MemoryPageCore(Pager pager, int pageNumber)
            {
                this._pager = pager;
                this.PageNumber = pageNumber;
            }
        }

        // if swapped out, the core will be out of date and needs to sync by swapping in. Replace the whole `Core` if necessary.
        public MemoryPageCore Core { get; set; }
        // start from 1
        public int PageNumber
        {
            get
            {
                return this.Core.PageNumber;
            }
            private set
            {
                this.Core.PageNumber = value;
            }
        }
        // NOTICE: don't watch this property when debugging!
        public byte[] Data
        {
            get
            {
                EnsureSwapIn();
                this.Core._pager.SetPageAsMostRecentlyUsed(this.PageNumber);
                // everytime this property is read, it is assumed to be modified since byte array could not be detected chances
                this.IsDirty = true;
                return this.Core.data;
            }
        }
        // If the page has not saved after being modified
        public bool IsDirty
        {
            get
            {
                EnsureSwapIn();
                return this.Core.IsDirty;
            }
            set
            {
                EnsureSwapIn();
                this.Core.IsDirty = value;
            }
        }
        public UInt16 PageSize
        {
            get
            {
                EnsureSwapIn();
                return this.Core.PageSize;
            }
        }
        // prevent page swapping manually
        public bool IsPinned
        {
            get
            {
                EnsureSwapIn();
                return this.Core.IsPinned;
            }
            set
            {
                EnsureSwapIn();
                this.Core.IsPinned = value;
            }
        }
        public bool IsSwappedOut { get => this.Core.IsSwappedOut; }

        // after which address the page is available.
        // in order not to corrupt the file header
        public ushort AvaliableOffset
        {
            get
            {
                if (PageNumber == 1)
                    return this.Core._pager.FileHeaderSize;
                return 0;
            }
        }
        // constructor
        public MemoryPage(MemoryPageCore core)
        {
            this.Core = core;
        }
        public byte this[int index]
        {
            get
            {
                EnsureSwapIn();
                this.Core._pager.SetPageAsMostRecentlyUsed(this.PageNumber);
                return this.Core.data[index];
            }
            set
            {
                EnsureSwapIn();
                this.Core._pager.SetPageAsMostRecentlyUsed(this.PageNumber);
                this.IsDirty = true;
                this.Core.data[index] = value;
            }
        }
        public void EnsureSwapIn()
        {
            if (this.IsSwappedOut)
            {
                this.Core._pager.ReadPage(this);
            }
        }
        // write page back to file
        public void CommitChanges()
        {
            this.Core._pager.WritePage(this);
        }
        // free up spaces
        public void Free()
        {
            this.Core.data = null;
        }
    }
}
