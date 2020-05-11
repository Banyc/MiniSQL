// using System;
// using System.Linq;

// namespace MiniSQL.BufferManager.Models
// {
//     // <childPage> <key>
//     // <childPage> <DBRecord>
//     public class InternalTableCell
//     {
//         public byte[] Data;

//         public InternalTableCell(byte[] data, int startIndex)
//         {
//             // TODO
//         }

//         public InternalTableCell(DBRecord keyObject, UInt32 childPage)
//         {
//             int size = keyObject.Data.Length + 4;
//             this.Data = new byte[size];
//             this.ChildPage = childPage;
//             this.KeyObject = keyObject;
//         }

//         // ChildPage “pointer”
//         // this “pointer” is the number of the page where the referenced node can be found
//         // ChildPage is the number of the page containing the entries with keys less than or equal to Key.
//         public UInt32 ChildPage
//         {
//             get { return BitConverter.ToUInt32(this.Data, 0); }
//             set { Array.Copy(BitConverter.GetBytes(value), 0, this.Data, 0, 4); }
//         }
//         // Key is primary key.
//         // public UInt32 Key
//         // {
//         //     get { return BitConverter.ToUInt32(this.Data, 4); }
//         //     set { Array.Copy(BitConverter.GetBytes(value), 0, this.Data, 4, 4); }
//         // }
//         public byte[] Key
//         {
//             get { return this.Data.Skip(4).ToArray(); }
//             set { Array.Copy(value, 0, this.Data, 4, value.Length); }
//         }
//         public DBRecord KeyObject
//         {
//             get { return new DBRecord(this.Key, 0); }
//             set { this.Key = value.Data; }
//         }
//     }
// }
