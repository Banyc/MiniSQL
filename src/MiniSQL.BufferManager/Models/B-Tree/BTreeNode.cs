using System;
using System.Collections;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager.Models
{
    public class BTreeNode : IEnumerable<BTreeCell>
    {
        private MemoryPage page = null;
        public PageTypes PageType
        {
            get { return (PageTypes)page.Data[0 + page.AvaliableOffset]; }
            set { page.Data[0 + page.AvaliableOffset] = (byte)value; }
        }
        // The byte offset at which the free space starts. 
        // Note that this must be updated every time the cell offset array grows.
        public UInt16 FreeOffset
        {
            get { return BitConverter.ToUInt16(page.Data, 1 + page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, page.Data, 1 + page.AvaliableOffset, 2); }
        }
        public UInt16 NumCells
        {
            get { return BitConverter.ToUInt16(page.Data, 3 + page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, page.Data, 3 + page.AvaliableOffset, 2); }
        }
        // The byte offset at which the cells start.
        // If the page contains no cells, this field contains the value PageSize. 
        // This value must be updated every time a cell is added.
        public UInt16 CellsOffset
        {
            get { return BitConverter.ToUInt16(page.Data, 5 + page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, page.Data, 5 + page.AvaliableOffset, 2); }
        }
        // internal node only
        // RightPage pointer is, essentially, the “rightmost pointer” in a B-Tree node
        public UInt32 RightPage
        {
            get { return BitConverter.ToUInt32(page.Data, 8 + page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, page.Data, 8 + page.AvaliableOffset, 4); }
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
                this.FreeOffset = (ushort)(page.AvaliableOffset + 8 + 4);
            }
            this.NumCells = 0;
        }

        public void DeleteBTreeCell(BTreeCell cell)
        {
            (BTreeCell cellFound, UInt16 offset, int indexInOffsetArray) = FindBTreeCell(cell, false);
            DeleteBTreeCell(offset);
        }

        // NOTICE: remember to re-get the `CellOffsetArray` after deletion
        public void DeleteBTreeCell(UInt16 address)
        {
            List<UInt16> offsetsSorted = this.CellOffsetArray;
            List<UInt16> offsets = this.CellOffsetArray;
            offsetsSorted.Sort();
            int physicalRankOfDeletingCell = offsetsSorted.IndexOf(address);
            if (physicalRankOfDeletingCell < 0)
                throw new Exception($"There is no B-Tree cell at offset {address}");
            // remove from header
            offsets.Remove(address);
            int sizeOfDeletingBTreeCell;
            if (physicalRankOfDeletingCell == offsetsSorted.Count - 1)
            {
                // cell being delete physically at the end of the node
                sizeOfDeletingBTreeCell = this.page.PageSize - address;
            }
            else
            {
                // cell being delete physically in the middle among the cells
                sizeOfDeletingBTreeCell = offsetsSorted[physicalRankOfDeletingCell + 1] - address;
            }

            int i;
            for (i = physicalRankOfDeletingCell - 1; i >= 0; i--)
            {
                UInt16 originalOffset = offsetsSorted[i];
                // update offset array
                int index = offsets.IndexOf(originalOffset);
                offsets[index] = (ushort)(originalOffset + sizeOfDeletingBTreeCell);
                // update cell's physical location
                byte[] raw = GetBTreeCell(originalOffset).Pack();
                Array.Copy(raw, 0, this.page.Data, originalOffset + sizeOfDeletingBTreeCell, raw.Length);
            }
            this.CellOffsetArray = offsets;
            this.CellsOffset += (ushort)sizeOfDeletingBTreeCell;
        }

        public BTreeCell GetBTreeCell(UInt32 address)
        {
            BTreeCell cell = null;
            switch (this.PageType)
            {
                case PageTypes.InternalIndexPage:
                    cell = new InternalIndexCell(page.Data, (int)address);
                    break;
                case PageTypes.InternalTablePage:
                    cell = new InternalTableCell(page.Data, (int)address);
                    break;
                case PageTypes.LeafIndexPage:
                    cell = new LeafIndexCell(page.Data, (int)address);
                    break;
                case PageTypes.LeafTablePage:
                    cell = new LeafTableCell(page.Data, (int)address);
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

            // get key values
            List<AtomValue> keyValues = cell.Key.GetValues();
            // handle the part in lower address
            List<UInt16> offsets = this.CellOffsetArray;
            // find the next peer of the cell
            int i;
            BTreeCell cellTemp;
            UInt16 offset;
            (cellTemp, offset, i) = FindBTreeCell(keyValues);
            // update header
            offsets.Insert(i, (UInt16)startAddress);
            this.CellOffsetArray = offsets;
        }

        // NOTICE: if `isFuzzySearch`, this function will return the first cell that with key equal or larger than that of `cell`'s
        public (BTreeCell cell, UInt16 offset, int indexInOffsetArray) FindBTreeCell(BTreeCell cell, bool isFuzzySearch = true)
        {
            return FindBTreeCell(cell.Key, isFuzzySearch);
        }

        public (BTreeCell cell, UInt16 offset, int indexInOffsetArray) FindBTreeCell(DBRecord key, bool isFuzzySearch = true)
        {
            List<AtomValue> keyValues = key.GetValues();
            return FindBTreeCell(keyValues, isFuzzySearch);
        }

        public (BTreeCell cell, UInt16 offset, int indexInOffsetArray) FindBTreeCell(List<AtomValue> keys, bool isFuzzySearch = true)
        {
            List<UInt16> offsets = this.CellOffsetArray;

            int i;
            bool isFound = false;
            BTreeCell peer = null;
            for (i = 0; i < offsets.Count && !isFound; i++)
            {
                peer = GetBTreeCell(offsets[i]);

                switch (keys[0].Type)
                {
                    case AttributeTypes.Int:
                        if (isFuzzySearch)
                        {
                            if (keys[0].IntegerValue <= peer.Key.GetValues()[0].IntegerValue)
                                isFound = true;
                        }
                        else
                        {
                            if (keys[0].IntegerValue == peer.Key.GetValues()[0].IntegerValue)
                                isFound = true;
                        }
                        break;
                    case AttributeTypes.Float:
                        if (isFuzzySearch)
                        {
                            if (keys[0].FloatValue <= peer.Key.GetValues()[0].FloatValue)
                                isFound = true;
                        }
                        else
                        {
                            if (keys[0].FloatValue == peer.Key.GetValues()[0].FloatValue)
                                isFound = true;
                        }
                        break;
                    case AttributeTypes.Char:
                        if (isFuzzySearch)
                        {
                            if (string.Compare(keys[0].StringValue, peer.Key.GetValues()[0].StringValue) <= 0)
                                isFound = true;
                        }
                        else
                        {
                            if (string.Compare(keys[0].StringValue, peer.Key.GetValues()[0].StringValue) == 0)
                                isFound = true;
                        }
                        break;
                    case AttributeTypes.Null:
                    default:
                        throw new Exception("Key could not be NULL");
                }
            }

            UInt16 offset = 0;
            if (isFound)
            {
                // before check on `isFound`, `i` will increment
                i--;
                offset = offsets[i];
            }
            else
            {
                offset = 0;
                peer = null;
            }

            return (peer, offset, i);
        }

        public IEnumerator<BTreeCell> GetEnumerator()
        {
            foreach (var offset in this.CellOffsetArray)
            {
                yield return this.GetBTreeCell(offset);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public BTreeCell this[int index]
        {
            get
            {
                return GetBTreeCell(this.CellOffsetArray[index]);
            }
        }
    }
}
