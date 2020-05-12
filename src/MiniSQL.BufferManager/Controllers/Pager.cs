using System;
using System.IO;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using System.Linq;

namespace MiniSQL.BufferManager.Controllers
{
    // map file bytes to pages of bytes
    public class Pager
    {
        public FileStream Stream { get; set; }
        public long PageCount { get; set; }
        public UInt16 PageSize { get; set; } = 4 * 1024;
        public List<MemoryPage> Pages { get; set; } = new List<MemoryPage>();

        public Pager(string dbPath, UInt16 pageSize = 4 * 1024)
        {
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
            MemoryPage firstPage = this.Pages[0];
            UInt16 pageSize = this.PageSize;
            byte[] pageSizeBytes = BitConverter.GetBytes(pageSize);
            Array.Copy(pageSizeBytes, 0, firstPage.Data, 0x10, 2);
        }

        public void NewPage()
        {
            this.PageCount++;
        }

        public void RemovePage(MemoryPage page)
        {
            this.Pages.Remove(page);
            page.Free();
        }

        public MemoryPage ReadPage(long pageNumber)
        {
            if (pageNumber > this.PageCount || pageNumber <= 0)
                throw new System.InvalidOperationException($"Page #{pageNumber} does not exists");
            MemoryPage newPage = new MemoryPage();
            newPage.PageNumber = pageNumber;
            newPage.PageSize = this.PageSize;
            newPage.Data = new byte[this.PageSize];

            this.Stream.Seek((pageNumber - 1) * this.PageSize, SeekOrigin.Begin);
            this.Stream.Read(newPage.Data, 0, newPage.PageSize);

            this.Pages.Add(newPage);
            return newPage;
        }

        public void WritePage(MemoryPage page)
        {
            if (page.PageNumber > this.PageCount || page.PageNumber <= 0)
                throw new System.InvalidOperationException($"Page #{page.PageNumber} does not exists");

            this.Stream.Seek((page.PageNumber - 1) * this.PageSize, SeekOrigin.Begin);
            this.Stream.Write(page.Data, 0, page.PageSize);
            this.Stream.Flush(true);
        }

        public void Close()
        {
            this.Stream.Close();
        }
    }
}
