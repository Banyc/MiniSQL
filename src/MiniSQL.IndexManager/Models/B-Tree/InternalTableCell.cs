using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniSQL.IndexManager.Models
{
    // original: <childPage (4 bytes)> <key (4 bytes)>
    // update: <childPage (4 bytes)> <key as DBRecord>
    public class InternalTableCell : BTreeCell
    {
        // constructor
        public InternalTableCell(byte[] data, int startIndex)
        {
            this.Types = CellTypes.InternalTableCell;
            Unpack(data, startIndex);
        }

        // constructor
        public InternalTableCell(DBRecord key, UInt32 childPage)
        {
            this.Types = CellTypes.InternalTableCell;
            this.Key = key;
            this.ChildPage = childPage;
        }

        // ChildPage “pointer”
        // this “pointer” is the number of the page where the referenced node can be found
        // ChildPage is the number of the page containing the entries with keys less than or equal to Key.
        public UInt32 ChildPage { get; set; }

        // To bytes
        public override byte[] Pack()
        {
            List<byte> pack = new List<byte>();
            pack.AddRange(BitConverter.GetBytes(this.ChildPage));
            pack.AddRange(this.Key.Pack());
            return pack.ToArray();
        }

        // From bytes
        public override void Unpack(byte[] data, int startIndex)
        {
            this.ChildPage = BitConverter.ToUInt32(data, startIndex);
            int startOffsetOfKey = 4;
            this.Key = new DBRecord(data, startIndex + startOffsetOfKey);
            int keySize = this.Key.RecordSize;
        }
    }
}
