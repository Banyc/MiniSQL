using System;

namespace MiniSQL.BufferManager.Models
{
    public class MemoryPage
    {
        // start from 1
        public long PageNumber { get; set; }
        public byte[] Data = null;
        public UInt16 PageSize { get; set; } 
        public bool IsPinned { get; set; } = false;

        public void Free()
        {
            this.Data = null;
        }
    }
}
