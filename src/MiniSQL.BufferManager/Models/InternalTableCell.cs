using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniSQL.BufferManager.Models
{
    // <childPage (4 bytes)> <key>
    // <childPage (4 bytes)> <DBRecord>
    public class InternalTableCell : BTreeCell
    {
        public InternalTableCell(byte[] data, int startIndex)
        {
            Unpack(data, startIndex);
        }

        public InternalTableCell(DBRecord key, UInt32 childPage)
        {
            this.Key = key;
            this.ChildPage = childPage;
        }

        // ChildPage “pointer”
        // this “pointer” is the number of the page where the referenced node can be found
        // ChildPage is the number of the page containing the entries with keys less than or equal to Key.
        public UInt32 ChildPage { get; set; }

        public override byte[] Pack()
        {
            List<byte> pack = new List<byte>();
            pack.AddRange(BitConverter.GetBytes(this.ChildPage));
            pack.AddRange(this.Key.Pack());
            return pack.ToArray();
        }

        public override void Unpack(byte[] data, int startIndex)
        {
            this.ChildPage = BitConverter.ToUInt32(data, startIndex);
            int startOffsetOfKey = 4;
            this.Key = new DBRecord(data, startIndex + startOffsetOfKey);
            int keySize = this.Key.RecordSize;
        }
    }
}
