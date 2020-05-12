using System;
using System.IO;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using System.Linq;
using System.Threading;

namespace MiniSQL.BufferManager.Controllers
{
    // map file bytes to pages of bytes
    public class Pager
    {
        public FileStream Stream { get; set; }
        // number of blocks in file (not in memory)
        public long PageCount { get; set; }
        public UInt16 PageSize { get; set; } = 4 * 1024;
        public List<MemoryPage> Pages { get; set; } = new List<MemoryPage>();
        public int InMemoryPageCountLimit { get; set; } = 4;

        public Pager(string dbPath, UInt16 pageSize = 4 * 1024, int pageCountLimit = 4)
        {
            this.InMemoryPageCountLimit = pageCountLimit;
            Open(dbPath, pageSize);
        }

        public void Open(string dbPath, UInt16 pageSize = 4 * 1024)
        {
            // the statement order is fixed
            this.PageSize = pageSize;
            this.Stream = File.Open(dbPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            this.PageCount = this.Stream.Length / this.PageSize;
        }

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

        public void NewPage()
        {
            this.PageCount++;
        }

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

        public MemoryPage ReadPage(long pageNumber)
        {
            if (pageNumber > this.PageCount || pageNumber <= 0)
                throw new System.InvalidOperationException($"Page #{pageNumber} does not exists");
            MemoryPage newPage = new MemoryPage(this);
            newPage.PageNumber = pageNumber;

            ReadPage(newPage);

            return newPage;
        }

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

        public void WritePage(MemoryPage page)
        {
            if (page.PageNumber > this.PageCount || page.PageNumber <= 0)
                throw new System.InvalidOperationException($"Page #{page.PageNumber} does not exists");

            this.Stream.Seek((page.PageNumber - 1) * this.PageSize, SeekOrigin.Begin);
            this.Stream.Write(page.Data, 0, page.PageSize);
            this.Stream.Flush(true);
            page.IsDirty = false;
        }

        public void Close()
        {
            this.Stream.Close();
        }

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
