using System;
using System.IO;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using System.Linq;
using System.Threading;

namespace MiniSQL.BufferManager.Controllers
{
    // map file bytes to pages of bytes and do page swapping when the number of pages hits the limit
    public class Pager
    {
        // file handler
        public FileStream Stream { get; set; }
        // number of blocks in file (not in memory)
        public long PageCount { get; set; }
        // size of each page
        public UInt16 PageSize { get; set; } = 4 * 1024;
        // all the pages read from the file
        public List<MemoryPage> Pages { get; set; } = new List<MemoryPage>();
        public int InMemoryPageCountLimit { get; set; } = 4;
        // how big is the file header in bytes
        public ushort FileHeaderSize { get; private set; } = 100;

        // constructor
        public Pager(string dbPath, UInt16 pageSize = 4 * 1024, int pageCountLimit = 4)
        {
            this.InMemoryPageCountLimit = pageCountLimit;
            Open(dbPath, pageSize);
        }

        // from file
        public void Open(string dbPath, UInt16 pageSize = 4 * 1024)
        {
            // the statement order is fixed
            this.PageSize = pageSize;
            this.Stream = File.Open(dbPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            this.PageCount = this.Stream.Length / this.PageSize;
        }

        // from file
        public byte[] ReadHeader()
        {
            byte[] header = new byte[100];
            this.Stream.Read(header, 0, 100);
            this.Stream.Close();
            return header;
        }

        private void ReadPageSizeFromFile()
        {
            byte[] header = ReadHeader();
            int pageSize = BitConverter.ToInt16(header, 0x10);
            this.PageCount = this.Stream.Length / this.PageSize;
        }

        // write file header to the file
        private void WritePageSizeToFirstPage()
        {
            lock (this)
            {
                MemoryPage firstPage = this.Pages[0];
                UInt16 pageSize = this.PageSize;
                byte[] pageSizeBytes = BitConverter.GetBytes(pageSize);
                Array.Copy(pageSizeBytes, 0, firstPage.Data, 0x10, 2);
            }
        }

        // extends the limits of the number of pages by one
        public void NewPage()
        {
            this.PageCount++;
        }

        // free a page from main memory
        public void RemovePage(MemoryPage page)
        {
            lock (this)
            {
                this.Pages.Remove(page);
                if (page.IsDirty)
                    WritePage(page);
                page.Free();
            }
        }

        // from file
        public MemoryPage ReadPage(long pageNumber)
        {
            if (pageNumber > this.PageCount || pageNumber <= 0)
                throw new System.InvalidOperationException($"Page #{pageNumber} does not exists");
            MemoryPage newPage = new MemoryPage(this);
            newPage.PageNumber = pageNumber;

            ReadPage(newPage);

            return newPage;
        }

        // from file
        // this function is mainly for page reloading after the page being swapped out
        public void ReadPage(MemoryPage page)
        {
            if (page.PageNumber > this.PageCount || page.PageNumber <= 0)
                throw new System.InvalidOperationException($"Page #{page.PageNumber} does not exists");

            lock (this)
            {
                if (this.Pages.Count >= this.InMemoryPageCountLimit)
                    RemoveLRUPage();

                page.PageSize = this.PageSize;
                page.Data = new byte[this.PageSize];

                this.Stream.Seek((page.PageNumber - 1) * this.PageSize, SeekOrigin.Begin);
                this.Stream.Read(page.Data, 0, page.PageSize);

                this.Pages.Add(page);
            }
        }

        // write page to secondary memory (the disk)
        // the changes will not immediately affect the file in secondary memory (the disk)
        public void WritePage(MemoryPage page)
        {
            if (page.PageNumber > this.PageCount || page.PageNumber <= 0)
                throw new System.InvalidOperationException($"Page #{page.PageNumber} does not exists");

            this.Stream.Seek((page.PageNumber - 1) * this.PageSize, SeekOrigin.Begin);
            this.Stream.Write(page.Data, 0, page.PageSize);
            this.Stream.Flush(true);
            page.IsDirty = false;
        }

        // write back dirty pages and close connection to the file system
        // the changes will not immediately affect the file in secondary memory (the disk)
        public void Close()
        {
            lock (this)
            {
                while (this.Pages.Count > 0)
                    RemovePage(this.Pages.First());
                this.Stream.Close();
            }
        }

        // mark a page as active and prevent it from being swapped out
        public void SetPageAsMostRecentlyUsed(MemoryPage page)
        {
            lock (this)
            {
                if (this.Pages.Count > 0 && this.Pages.Last() != page)
                {
                    int index = this.Pages.IndexOf(page);
                    // to prevent unexpected `Add` if page has not been in this.Pages
                    if (index >= 0)
                    {
                        this.Pages.RemoveAt(index);
                        this.Pages.Add(page);
                    }
                }
            }
        }

        // swap out the lest recently used page
        private void RemoveLRUPage()
        {
            lock (this)
            {
                if (this.Pages.Count == 0)
                    return;
                int victimIndex = 0;
                while (this.Pages[victimIndex].IsPinned == true)
                    victimIndex++;
                MemoryPage victim = this.Pages[victimIndex];
                RemovePage(victim);
            }
        }
    }
}
