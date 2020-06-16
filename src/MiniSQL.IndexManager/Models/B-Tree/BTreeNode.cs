using System;
using System.Collections;
using System.Collections.Generic;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.IndexManager.Models
{
    // The term "offset" is the same as "address"
    // each node exclusively owns one page
    // <PageType (1 byte)> <FreeOffset (2 bytes)> <NumCells (2 bytes)> <CellsOffset (2 bytes)> <0 (1 byte)> <RightPage (4 bytes)> <ParentPage (4 bytes)>
    public class BTreeNode : IEnumerable<BTreeCell>
    {
        private readonly MemoryPage _page = null;
        public PageTypes PageType
        {
            get { return (PageTypes)_page.Data[0 + _page.AvaliableOffset]; }
            private set { _page.Data[0 + _page.AvaliableOffset] = (byte)value; }
        }
        // The byte offset at which the free space starts. 
        // Note that this must be updated every time the cell offset array grows.
        public UInt16 FreeOffset
        {
            get { return BitConverter.ToUInt16(_page.Data, 1 + _page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _page.Data, 1 + _page.AvaliableOffset, 2); }
        }
        // number of cells
        public UInt16 NumCells
        {
            get { return BitConverter.ToUInt16(_page.Data, 3 + _page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _page.Data, 3 + _page.AvaliableOffset, 2); }
        }
        // The byte offset at which the cells start.
        // If the page contains no cells, this field contains the value PageSize. 
        // This value must be updated every time a cell is added.
        public UInt16 CellsOffset
        {
            get { return BitConverter.ToUInt16(_page.Data, 5 + _page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _page.Data, 5 + _page.AvaliableOffset, 2); }
        }
        // deprecated: internal node only
        // WORKAROUND: all types of node have the `RightPage` pointer
        // RightPage pointer is, essentially, the “rightmost pointer” in a B-Tree node
        public UInt32 RightPage
        {
            get { return BitConverter.ToUInt32(_page.Data, 8 + _page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _page.Data, 8 + _page.AvaliableOffset, 4); }
        }
        // customized: the page number of parent node
        public UInt32 ParentPage
        {
            get { return BitConverter.ToUInt32(_page.Data, 12 + _page.AvaliableOffset); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _page.Data, 12 + _page.AvaliableOffset, 4); }
        }

        // if tree node is freed/disabled
        public bool IsDisabled { get; set; } = false;

        // the offset array at the low address of the page
        // the array indicates the offset (address) of each cell at the high address space
        // the order of the array is carefully set in ascending order. It is based on the first value of `Key` of each cell.
        public List<UInt16> CellOffsetArray
        {
            get
            {
                // build a container for output
                List<UInt16> offsets = new List<ushort>();
                int i;
                // locates the first item in the offset array
                int startAddress = this.FreeOffset - this.NumCells * 2;
                // visits those items one-by-one and load them to the container
                for (i = 0; i < this.NumCells; i++)
                {
                    // of each cell
                    UInt16 offset = BitConverter.ToUInt16(this._page.Data, startAddress);
                    offsets.Add(offset);
                    startAddress += 2;
                }
                return offsets;
            }
            private set
            {
                // locates the first item in the offset array
                int startAddress = this.FreeOffset - this.NumCells * 2;
                // visits those items one-by-one and copy them to page
                foreach (UInt16 offset in value)
                {
                    Array.Copy(BitConverter.GetBytes(offset), 0, _page.Data, startAddress, 2);
                    startAddress += 2;
                }
                // update metadata at header
                this.FreeOffset = (UInt16)startAddress;
                this.NumCells = (UInt16)value.Count;
            }
        }

        // constructor
        public BTreeNode(MemoryPage assembledPage)
        {
            this._page = assembledPage;
        }

        public BTreeNode(MemoryPage emptyPage, PageTypes pageType)
        {
            this._page = emptyPage;
            InitializeEmptyFormat(pageType);
        }

        // use it when freeing this node
        public MemoryPage GetRawPage()
        {
            return _page;
        }

        // formatting an empty page with initialized B-Tree node (page) header
        public void InitializeEmptyFormat(PageTypes pageType)
        {
            this.PageType = pageType;
            this.CellsOffset = _page.PageSize;
            if (this.PageType == PageTypes.InternalIndexPage || this.PageType == PageTypes.InternalTablePage)
            {
                // internal pages (nodes) have `RightPage` section in header
                this.RightPage = 0;
                // customized: added field `ParentPage`
                this.ParentPage = 0;

                // deprecated:
                // this.FreeOffset = (ushort)(_page.AvaliableOffset + 8 + 4);

                // customized: added field `ParentPage`
                this.FreeOffset = (ushort)(_page.AvaliableOffset + 8 + 4 + 4);
            }
            else
            {
                // deprecated: leaf pages (nodes) do not have `RightPage` section in header
                // this.FreeOffset = (ushort)(_page.AvaliableOffset + 8);

                // WORKAROUND: leaf pages (nodes) ALSO have `RightPage` section in header
                this.RightPage = 0;
                // customized: added field `ParentPage`
                this.ParentPage = 0;

                // deprecated:
                // this.FreeOffset = (ushort)(_page.AvaliableOffset + 8 + 4);

                // customized: added field `ParentPage`
                this.FreeOffset = (ushort)(_page.AvaliableOffset + 8 + 4 + 4);
            }
            this.NumCells = 0;
        }

        // delete a tree cell
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
                sizeOfDeletingBTreeCell = this._page.PageSize - address;
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
                Array.Copy(raw, 0, this._page.Data, originalOffset + sizeOfDeletingBTreeCell, raw.Length);
            }
            this.CellOffsetArray = offsets;
            this.CellsOffset += (ushort)sizeOfDeletingBTreeCell;
        }

        // get a cell given an offset (address)
        // NOTICE: you are only getting a COPY, any modification on the cell will NOT affect the node
        public BTreeCell GetBTreeCell(UInt32 address)
        {
            BTreeCell cell = null;
            switch (this.PageType)
            {
                case PageTypes.InternalIndexPage:
                    cell = new InternalIndexCell(_page.Data, (int)address);
                    break;
                case PageTypes.InternalTablePage:
                    cell = new InternalTableCell(_page.Data, (int)address);
                    break;
                case PageTypes.LeafIndexPage:
                    cell = new LeafIndexCell(_page.Data, (int)address);
                    break;
                case PageTypes.LeafTablePage:
                    cell = new LeafTableCell(_page.Data, (int)address);
                    break;
                default:
                    throw new Exception($"Page type {this.PageType} does not exist");
            }
            return cell;
        }

        // insert a cell. It will place the new cell in ascending order
        public void InsertBTreeCell(BTreeCell cell)
        {
            byte[] raw = cell.Pack();

            // handle the part in higher address
            int startAddress = this.CellsOffset - raw.Length;
            if (startAddress <= this.FreeOffset + 4)
            {
                // BTreeNode is too full to contain the new cell
                throw new Exception("BTreeNode is full");
            }
            Array.Copy(raw, 0, this._page.Data, startAddress, raw.Length);
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
        // if no cell matches, the output `cell` field will be `null` and `offset` will be set to 0
        public (BTreeCell cell, UInt16 offset, int indexInOffsetArray) FindBTreeCell(BTreeCell cell, bool isFuzzySearch = true)
        {
            return FindBTreeCell(cell.Key, isFuzzySearch);
        }

        // NOTICE: if `isFuzzySearch`, this function will return the first cell that with key equal or larger than that of `cell`'s
        // if no cell matches, the output `cell` field will be `null` and `offset` will be set to 0
        public (BTreeCell cell, UInt16 offset, int indexInOffsetArray) FindBTreeCell(DBRecord key, bool isFuzzySearch = true)
        {
            List<AtomValue> keyValues = key.GetValues();
            return FindBTreeCell(keyValues, isFuzzySearch);
        }

        // NOTICE: if `isFuzzySearch`, this function will return the first cell that with key equal or larger than that of `cell`'s
        // if no cell matches, the output `cell` field will be `null` and `offset` will be set to 0
        // WORKAROUND: only the first key is used. The remaining keys will be ignored
        public (BTreeCell cell, UInt16 offset, int indexInOffsetArray) FindBTreeCell(List<AtomValue> keys, bool isFuzzySearch = true)
        {
            return FindBTreeCell(keys[0], isFuzzySearch);
        }

        public (BTreeCell cell, UInt16 offset, int indexInOffsetArray) FindBTreeCell(AtomValue key, bool isFuzzySearch = true)
        {
            // get the list of existing peers to visit
            List<UInt16> offsets = this.CellOffsetArray;

            // find peer
            int i;
            bool isFound = false;
            BTreeCell peer = null;
            for (i = 0; i < offsets.Count && !isFound; i++)
            {
                peer = GetBTreeCell(offsets[i]);

                switch (key.Type)
                {
                    case AttributeTypes.Int:
                        if (isFuzzySearch)
                        {
                            if (key.IntegerValue <= peer.Key.GetValues()[0].IntegerValue)
                                isFound = true;
                        }
                        else
                        {
                            if (key.IntegerValue == peer.Key.GetValues()[0].IntegerValue)
                                isFound = true;
                        }
                        break;
                    case AttributeTypes.Float:
                        if (isFuzzySearch)
                        {
                            if (key.FloatValue <= peer.Key.GetValues()[0].FloatValue)
                                isFound = true;
                        }
                        else
                        {
                            if (key.FloatValue == peer.Key.GetValues()[0].FloatValue)
                                isFound = true;
                        }
                        break;
                    case AttributeTypes.Char:
                        if (isFuzzySearch)
                        {
                            if (string.Compare(key.StringValue, peer.Key.GetValues()[0].StringValue) <= 0)
                                isFound = true;
                        }
                        else
                        {
                            if (string.Compare(key.StringValue, peer.Key.GetValues()[0].StringValue) == 0)
                                isFound = true;
                        }
                        break;
                    case AttributeTypes.Null:
                    default:
                        throw new Exception("Key could not be NULL");
                }
            }
            // handle cases when matched cell found or not found
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

        // TODO
        // modify field `ChildPage` on internal index cells or internal table cells
        public void ChangeChildPage(int indexInOffsetArray, UInt32 newChildPage)
        {
            if (this.PageType == PageTypes.LeafIndexPage
                || this.PageType == PageTypes.LeafTablePage)
            {
                throw new Exception("Leaf Cells do not have field `ChildPage`");
            }

        } 

        // make this object to be iterable
        public IEnumerator<BTreeCell> GetEnumerator()
        {
            foreach (var offset in this.CellOffsetArray)
            {
                // NOTICE: you are only getting a COPY, any modification on the cell will NOT affect the node
                yield return this.GetBTreeCell(offset);
            }
        }

        // make this object to be iterable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        // overrides "object[index]" operator
        public BTreeCell this[int index]
        {
            get
            {
                // NOTICE: you are only getting a COPY, any modification on the cell will NOT affect the node
                return GetBTreeCell(this.CellOffsetArray[index]);
            }
        }
    }
}
