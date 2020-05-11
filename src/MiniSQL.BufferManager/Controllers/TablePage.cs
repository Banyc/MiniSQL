using System;
using MiniSQL.BufferManager.Models;

namespace MiniSQL.BufferManager.Controllers
{
    public class TablePage : MemoryPage
    {
        public PageType PageType
        {
            get { return (PageType)this.Data[0]; }
            set { this.Data[0] = (byte)value; }
        }
        // The byte offset at which the free space starts. 
        // Note that this must be updated every time the cell offset array grows.
        public UInt16 FreeOffset
        {
            get { return BitConverter.ToUInt16(this.Data, 1); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.Data, 1, 2); }
        }
        public UInt16 NumCells
        {
            get { return BitConverter.ToUInt16(this.Data, 3); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.Data, 3, 2); }
        }
        // The byte offset at which the cells start.
        // If the page contains no cells, this field contains the value PageSize. 
        // This value must be updated every time a cell is added.
        public UInt16 CellsOffset
        {
            get { return BitConverter.ToUInt16(this.Data, 5); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.Data, 5, 2); }
        }
        // internal node only
        // RightPage pointer is, essentially, the “rightmost pointer” in a B-Tree node
        public UInt32 RightPage
        {
            get { return BitConverter.ToUInt32(this.Data, 8); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.Data, 8, 4); }
        }

        public TablePage()
        {

        }

        public void InitializeEmpty(PageType pageType)
        {
            this.PageType = pageType;
            this.CellsOffset = this.PageSize;
            if (this.PageType == PageType.InternalIndexPage || this.PageType == PageType.InternalTablePage)
                this.FreeOffset = 8;
            else
            {
                this.RightPage = 0;
                this.FreeOffset = 8 + 4;
            }
            this.NumCells = 0;
        }

        // public InternalTableCell GetInternalTableCell(int address)
        // {
        //     return new InternalTableCell(this.Data, address);
        // }

        // public LeafTableCell GetLeafTableCell(int address)
        // {
        //     return new LeafTableCell(this.Data, address);
        // }
    }
}
