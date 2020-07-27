using System;
using MiniSQL.BufferManager.Models;

namespace MiniSQL.BufferManager.Controllers
{
    // a linked list recording all free pages
    public class FreeList
    {
        private readonly Pager _pager;

        private int FreeListHeadPageNumber { get => GetFreeListHead(); set => SetFreeListHead(value); }

        // constructor
        public FreeList(Pager pager)
        {
            _pager = pager;
        }

        // reset the free list to empty
        public void InitializeEmptyFreeList()
        {
            SetFreeListHead(0);
        }

        // move the page to the head of the free list
        public void RecyclePage(MemoryPage page)
        {
            FreeListTruck truck = new FreeListTruck(page);
            truck.PageNumberOfNextFreeTruck = GetFreeListHead();
            SetFreeListHead(page.PageNumber);
        }

        // return null if no free page is recorded
        public MemoryPage AllocatePage()
        {
            int headPageNumber = GetFreeListHead();
            if (headPageNumber == 0)
                return null;
            MemoryPage headPage = _pager.ReadPage(headPageNumber);
            FreeListTruck headTruck = new FreeListTruck(headPage);
            SetFreeListHead(headTruck.PageNumberOfNextFreeTruck);
            return headPage;
        }

        // from file header
        private int GetFreeListHead()
        {
            MemoryPage firstPage = _pager.ReadPage(1);
            int head = BitConverter.ToInt32(firstPage.Data, 32);
            return head;
        }

        // to file header
        private void SetFreeListHead(int newHead)
        {
            byte[] raw = BitConverter.GetBytes(newHead);
            MemoryPage firstPage = _pager.ReadPage(1);
            Array.Copy(raw, 0, firstPage.Data, 32, raw.Length);
        }
    }
}
