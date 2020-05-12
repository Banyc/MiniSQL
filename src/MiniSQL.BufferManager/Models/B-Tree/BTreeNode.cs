using System;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager.Models
{
    public class BTreeNode
    {
        private MemoryPage page = null;
        public PageTypes PageType
        {
            get { return (PageTypes)page.Data[0]; }
            set { page.Data[0] = (byte)value; }
        }
        // The byte offset at which the free space starts. 
        // Note that this must be updated every time the cell offset array grows.
        public UInt16 FreeOffset
        {
            get { return BitConverter.ToUInt16(page.Data, 1); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, page.Data, 1, 2); }
        }
        public UInt16 NumCells
        {
            get { return BitConverter.ToUInt16(page.Data, 3); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, page.Data, 3, 2); }
        }
        // The byte offset at which the cells start.
        // If the page contains no cells, this field contains the value PageSize. 
        // This value must be updated every time a cell is added.
        public UInt16 CellsOffset
        {
            get { return BitConverter.ToUInt16(page.Data, 5); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, page.Data, 5, 2); }
        }
        // internal node only
        // RightPage pointer is, essentially, the “rightmost pointer” in a B-Tree node
        public UInt32 RightPage
        {
            get { return BitConverter.ToUInt32(page.Data, 8); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, page.Data, 8, 4); }
        }

        public List<UInt16> CellOffsetArray
        {
            get
            {
                List<UInt16> offsets = new List<ushort>();
                int i;
                int startAddress = this.FreeOffset - this.NumCells * 2;
                for (i = 0; i < this.NumCells; i++)
                {
                    // of each offset
                    UInt16 offset = BitConverter.ToUInt16(this.page.Data, startAddress);
                    offsets.Add(offset);
                    startAddress += 2;
                }
                return offsets;
            }
            set
            {
                int startAddress = this.FreeOffset - this.NumCells * 2;
                foreach (UInt16 offset in value)
                {
                    Array.Copy(BitConverter.GetBytes(offset), 0, page.Data, startAddress, 2);
                    startAddress += 2;
                }
                this.FreeOffset = (UInt16)startAddress;
                this.NumCells = (UInt16)value.Count;
            }
        }

        public BTreeNode(MemoryPage page)
        {
            this.page = page;
        }

        public void InitializeEmptyFormat(PageTypes pageType)
        {
            this.PageType = pageType;
            this.CellsOffset = page.PageSize;
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
                    cell = new InternalIndexCell(page.Data, address);
                    break;
                case PageTypes.InternalTablePage:
                    cell = new InternalTableCell(page.Data, address);
                    break;
                case PageTypes.LeafIndexPage:
                    cell = new LeafIndexCell(page.Data, address);
                    break;
                case PageTypes.LeafTablePage:
                    cell = new LeafTableCell(page.Data, address);
                    break;
                default:
                    throw new Exception($"Page type {this.PageType} does not exist");
            }
            return cell;
        }

        public void InsertBTreeCell(BTreeCell cell)
        {
            byte[] raw = cell.Pack();

            // handle the part in higher address
            int startAddress = this.CellsOffset - raw.Length;
            Array.Copy(raw, 0, this.page.Data, startAddress, raw.Length);
            this.CellsOffset = (ushort)startAddress;

            // debug
            GetBTreeCell(startAddress);

            // handle the part in lower address
            List<UInt16> offsets = this.CellOffsetArray;
            // find the next peer of the cell
            int i;
            bool isExitLoop = false;
            for (i = 0; i < offsets.Count && !isExitLoop; i++)
            {
                BTreeCell peer = GetBTreeCell(offsets[i]);

                switch (cell.Key.GetValues()[0].Type)
                {
                    case AttributeTypes.Int:
                        if (cell.Key.GetValues()[0].IntegerValue <= peer.Key.GetValues()[0].IntegerValue)
                            isExitLoop = true;
                        break;
                    case AttributeTypes.Float:
                        if (cell.Key.GetValues()[0].FloatValue <= peer.Key.GetValues()[0].FloatValue)
                            isExitLoop = true;
                        break;
                    case AttributeTypes.Char:
                        if (string.Compare(cell.Key.GetValues()[0].StringValue, peer.Key.GetValues()[0].StringValue) <= 0)
                            isExitLoop = true;
                        break;
                    case AttributeTypes.Null:
                    default:
                        throw new Exception("Key could not be NULL");
                }
            }
            // it found the index of the next peer, so the new cell should be one index ahead of the peer
            if (isExitLoop)
                i--;
            offsets.Insert(i, (UInt16)startAddress);
            // update header
            this.CellOffsetArray = offsets;
        }
    }
}
