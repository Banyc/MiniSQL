namespace MiniSQL.BufferManager.Models
{
    public class MemoryPage
    {
        // start from 1
        public long PageNumber { get; set; }
        public byte[] Data { get; set; }
        public int PageSize { get; set; } 
        public bool IsPinned { get; set; } = false;

        public void Free()
        {
            this.Data = null;
        }
    }
}
