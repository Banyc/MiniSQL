using System;
using System.Threading;
using MiniSQL.BufferManager.Controllers;

namespace MiniSQL.BufferManager.Models
{
    public class MemoryPage
    {
        private Pager pager = null;
        // start from 1
        public long PageNumber { get; set; } = 0;
        // this page has been swapped out if this.data is null
        private byte[] data = null;
        // NOTICE: don't watch this property when debugging!
        public byte[] Data
        {
            get
            {
                if (data == null)
                {
                    pager.ReadPage(this);
                }
                this.pager.SetPageAsMostRecentlyUsed(this);
                return data;
            }
            set
            {
                lock (this)
                {
                    this.pager.SetPageAsMostRecentlyUsed(this);
                    this.IsDirty = true;
                    data = value;
                }
            }
        }
        public bool IsDirty { get; set; } = false;
        public UInt16 PageSize { get; set; }

        // prevent page swapping manually
        public bool IsPinned { get; set; } = false;
        public bool IsSwappedOut
        {
            get { return data == null; } 
        }

        public MemoryPage(Pager pager)
        {
            this.pager = pager;
        }

        public void CommitChanges()
        {
            this.pager.WritePage(this);
        }

        public void Free()
        {
            this.Data = null;
        }
    }
}
