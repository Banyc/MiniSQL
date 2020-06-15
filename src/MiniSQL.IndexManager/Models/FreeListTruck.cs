using System;
using MiniSQL.BufferManager.Models;

namespace MiniSQL.IndexManager.Models
{
    public class FreeListTruck
    {
        private readonly MemoryPage _page;
        public int PageNumberOfNextFreeTruck
        {
            get 
            {
                int nextPageNumber = BitConverter.ToInt32(_page.Data, 0);
                return nextPageNumber;
            }
            set
            {
                byte[] raw = BitConverter.GetBytes(value);
                Array.Copy(raw, 0, _page.Data, 0, raw.Length);
            }
        }

        public FreeListTruck(MemoryPage page)
        {
            _page = page;
        }
    }
}
