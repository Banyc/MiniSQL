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
        // NOTICE: remember to release the mutex when done using this.Data after getting it
        // private Mutex dataMutex { get; set; } = new Mutex();
        // NOTICE: remember to release the mutex when done using this.Data after getting it
        // NOTICE: don't watch this property when debugging!
        public byte[] Data
        {
            get  // NOTICE: remember to release mutex manually
            {
                // this.dataMutex.WaitOne();
                if (data == null)
                {
                    pager.ReadPage(this);
                }
                this.pager.SetPageAsMostRecentlyUsed(this);
                return data;
            }
            set
            {
                // this.dataMutex.WaitOne();
                lock (this)
                {
                    this.pager.SetPageAsMostRecentlyUsed(this);
                    this.IsDirty = true;
                    data = value;
                }
                // this.dataMutex.ReleaseMutex();
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

        // public void ReleaseDataMutex()
        // {
        //     dataMutex.ReleaseMutex();
        // }

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
