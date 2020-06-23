using System;
using System.IO;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using System.Linq;
using System.Threading;
using static MiniSQL.BufferManager.Models.MemoryPage;

namespace MiniSQL.BufferManager.Controllers
{
    // map file bytes to pages of bytes and do page swapping when the number of pages hits the limit
    public class Pager
    {
        // file handler
        public FileStream Stream { get; set; }
        // number of blocks in file (not in memory)
        public int PageCount { get; set; }
        // size of each page
        // page size is not allowed to be modified after the file is created.
        public UInt16 PageSize { get; private set; } = 0;
        // all the pages read from the file
        public Dictionary<int, (MemoryPage, DateTime)> Pages { get; set; } = new Dictionary<int, (MemoryPage, DateTime)>();
        public int InMemoryPageCountLimit { get; set; } = 4;
        // how big is the file header in bytes
        public ushort FileHeaderSize { get; private set; } = 100;

        // constructor
        public Pager(string dbPath, UInt16 defaultPageSize = 4 * 1024, int pageCountLimit = 4)
        {
            this.InMemoryPageCountLimit = pageCountLimit;
            Open(dbPath, defaultPageSize);
        }

        // from file
        public void Open(string dbPath, UInt16 defaultPageSize = 4 * 1024)
        {
            bool isNewFile = false;
            // check if new file
            if (!File.Exists(dbPath))
                isNewFile = true;
            // open file
            this.Stream = File.Open(dbPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            // initialize file format
            if (isNewFile)
            {
                this.PageSize = defaultPageSize;
                // the first page is resevered for metadata, not available from GetNewPage()
                this.PageCount = 1;
                WritePageSizeToFirstPage();
            }
            else
            {
                ReadPageSizeFromFile();
                // get number of pages available
                this.PageCount = (int)this.Stream.Length / this.PageSize;
            }
        }

        // from file
        // NOTICE: this method could ONLY used when initialization of pager
        private byte[] ReadHeader()
        {
            lock (this)
            {
                byte[] header = new byte[100];
                this.Stream.Seek(0, SeekOrigin.Begin);
                this.Stream.Read(header, 0, 100);
                return header;
            }
        }

        private void ReadPageSizeFromFile()
        {
            byte[] header = ReadHeader();
            this.PageSize = BitConverter.ToUInt16(header, 0x10);
            this.PageCount = (int)this.Stream.Length / this.PageSize;
        }

        // write file header to the file
        private void WritePageSizeToFirstPage()
        {
            lock (this)
            {
                MemoryPage firstPage = ReadPage(1);
                UInt16 pageSize = this.PageSize;
                byte[] pageSizeBytes = BitConverter.GetBytes(pageSize);
                Array.Copy(pageSizeBytes, 0, firstPage.Data, 0x10, 2);
            }
        }

        // extends the limits of the number of pages by one
        public void ExtendNumberOfPages()
        {
            this.PageCount++;
        }

        // get a newly allocated page
        public MemoryPage GetNewPage()
        {
            ExtendNumberOfPages();
            return ReadPage(this.PageCount);
        }

        // free a page from main memory
        public void RemovePage(MemoryPage page)
        {
            lock (this)
            {
                this.Pages.Remove(page.PageNumber);
                if (page.IsDirty)
                    WritePage(page);
                page.Free();
            }
        }

        // from file
        public MemoryPage ReadPage(int pageNumber)
        {
            if (pageNumber > this.PageCount || pageNumber <= 0)
                throw new System.InvalidOperationException($"Page #{pageNumber} does not exists");

            MemoryPageCore core = new MemoryPageCore(this, pageNumber);
            MemoryPage newPage = new MemoryPage(core);

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
                // directly get the shared part and return
                if (this.Pages.ContainsKey(page.PageNumber))
                {
                    (MemoryPage existingPage, DateTime lastAccessTime) = this.Pages[page.PageNumber];
                    // renew the last access time
                    SetPageAsMostRecentlyUsed(page.PageNumber);
                    // share the same data
                    page.Core = existingPage.Core;
                    return;
                }

                // remove LRU if out of limit
                if (this.Pages.Count >= this.InMemoryPageCountLimit)
                    RemoveLRUPage();
                if (this.Pages.Count >= this.InMemoryPageCountLimit)
                    throw new Exception("Race condition!");

                page.Core = new MemoryPage.MemoryPageCore(this, page.PageNumber);
                page.Core.data = new byte[this.PageSize];

                this.Stream.Seek((page.PageNumber - 1) * this.PageSize, SeekOrigin.Begin);
                this.Stream.Read(page.Core.data, 0, page.Core.PageSize);

                this.Pages.Add(page.PageNumber, (page, DateTime.Now));
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
            // WORK AROUND: make good use of OS-level buffer XD
            // this.Stream.Flush(true);
            page.IsDirty = false;
        }

        // write back dirty pages and close connection to the file system
        // the changes will not immediately affect the file in secondary memory (the disk)
        public void Close()
        {
            lock (this)
            {
                while (this.Pages.Count > 0)
                    RemovePage(this.Pages.First().Value.Item1);
                this.Stream.Close();
            }
        }

        // write back dirty pages without closing connection to the file system
        // the changes will not immediately affect the file in secondary memory (the disk)
        public void CleanAllPagesFromMainMemory()
        {
            lock (this)
            {
                while (this.Pages.Count > 0)
                    RemovePage(this.Pages.First().Value.Item1);
                this.Stream.FlushAsync();
            }
        }

        // mark a page as active and prevent it from being swapped out
        public void SetPageAsMostRecentlyUsed(int pageNumber)
        {
            lock (this)
            {
                // if a MemoryPage is initialized, this method will also be called
                if (!this.Pages.ContainsKey(pageNumber))
                    return;
                // find page
                (MemoryPage page, DateTime lastAccessTime) = this.Pages[pageNumber];
                // renew the last access time
                this.Pages[pageNumber] = (page, DateTime.Now);
            }
        }

        // swap out the lest recently used page
        private void RemoveLRUPage()
        {
            lock (this)
            {
                if (this.Pages.Count == 0)
                    return;
                DateTime lru = DateTime.Now;
                MemoryPage victim = null;
                foreach (var keyValue in this.Pages)
                {
                    if (keyValue.Value.Item1.IsPinned)
                        continue;
                    if (keyValue.Value.Item2 <= lru)
                    {
                        lru = keyValue.Value.Item2;
                        victim = keyValue.Value.Item1;
                    }
                }

                if (victim == null)
                {
                    throw new Exception("Cannot find any valid LRU page!");
                }

                RemovePage(victim);
            }
        }
    }
}
