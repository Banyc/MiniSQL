using System;
using MiniSQL.BufferManager.Models;

namespace MiniSQL.BufferManager.Models
{
    public class BTreeNode : MemoryPage
    {
        public PageTypes PageType
        {
            get { return (PageTypes)this.Data[0]; }
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

        public BTreeNode()
        {

        }

        public void InitializeEmpty(PageTypes pageType)
        {
            this.PageType = pageType;
            this.CellsOffset = this.PageSize;
            if (this.PageType == PageTypes.InternalIndexPage || this.PageType == PageTypes.InternalTablePage)
                this.FreeOffset = 8;
            else
            {
                this.RightPage = 0;
                this.FreeOffset = 8 + 4;
            }
            this.NumCells = 0;
        }

        public void DeleteBTreeCell(int address)
        {
            // TODO
        }

        public BTreeCell GetBTreeCell(int address)
        {
            BTreeCell cell = null;
            switch (this.PageType)
            {
                case PageTypes.InternalIndexPage:
                    cell = new InternalTableCell(this.Data, address);
                    break;
                case PageTypes.InternalTablePage:
                    break;
                case PageTypes.LeafIndexPage:
                    break;
                case PageTypes.LeafTablePage:
                    cell = new LeafTableCell(this.Data, address);
                    break;
                default:
                    throw new Exception($"Page type {this.PageType} does not exist");
            }
            return cell;
        }

        public void InsertBTreeCell(BTreeCell cell, int address)
        {
            // TODO
        }
    }
}
