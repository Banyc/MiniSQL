using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BufferManager Test Begin");

            TestDBRecord();

            TestLeafTableCell();

            TestInternalTableCell();

            TestInternalIndexCell();

            TestLeafIndexCell();

            TestPager();

            TestPagerSwapping();

            Console.WriteLine("BufferManager Test End");
        }

        static void TestPagerSwapping()
        {
            string dbPath = "./dbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath);

            pager.NewPage();
            pager.NewPage();
            pager.NewPage();
            pager.NewPage();
            pager.NewPage();
            pager.NewPage();
            pager.NewPage();

            MemoryPage page1 = pager.ReadPage(1);
            page1.IsPinned = true;
            MemoryPage page2 = pager.ReadPage(2);
            page2.Data[0] = 2;
            MemoryPage page3 = pager.ReadPage(3);
            MemoryPage page4 = pager.ReadPage(4);
            MemoryPage page5 = pager.ReadPage(5);
            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.IsSwappedOut == true);
            Debug.Assert(page3.IsSwappedOut == false);
            Debug.Assert(page4.IsSwappedOut == false);
            Debug.Assert(page5.IsSwappedOut == false);
            MemoryPage page6 = pager.ReadPage(6);
            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.IsSwappedOut == true);
            Debug.Assert(page3.IsSwappedOut == true);
            Debug.Assert(page4.IsSwappedOut == false);
            Debug.Assert(page5.IsSwappedOut == false);
            Debug.Assert(page6.IsSwappedOut == false);
            page4.Data[0] = 4;
            MemoryPage page7 = pager.ReadPage(7);
            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.IsSwappedOut == true);
            Debug.Assert(page3.IsSwappedOut == true);
            Debug.Assert(page4.IsSwappedOut == false);
            Debug.Assert(page5.IsSwappedOut == true);
            Debug.Assert(page6.IsSwappedOut == false);
            Debug.Assert(page7.IsSwappedOut == false);

            // not enough pages
            bool error = false;
            try
            {
                MemoryPage page8 = pager.ReadPage(8);
            }
            catch(Exception)
            {
                error = true;
            }
            Debug.Assert(error == true);

            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.Data[0] == 2);
            // page2.ReleaseDataMutex();

            pager.Close();
        }

        static void TestPager()
        {
            string dbPath = "./dbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath);

            Debug.Assert(pager.PageCount == 0);
            
            pager.NewPage();
            Debug.Assert(pager.PageCount == 1);

            MemoryPage page = pager.ReadPage(1);
            page.Data[2] = 114;
            page.Data[3] = 5;
            page.Data[4] = 14;

            pager.WritePage(page);
            MemoryPage page1 = pager.ReadPage(1);

            Debug.Assert(page.Data[2] == page1.Data[2]);
            Debug.Assert(page.Data[3] == page1.Data[3]);
            Debug.Assert(page.Data[4] == page1.Data[4]);

            pager.Close();

            pager.Open(dbPath);
            page1 = pager.ReadPage(1);

            Debug.Assert(page.Data[2] == page1.Data[2]);
            Debug.Assert(page.Data[3] == page1.Data[3]);
            Debug.Assert(page.Data[4] == page1.Data[4]);

            pager.Close();
        }

        static void TestLeafIndexCell()
        {
            // init record
            DBRecord record = GetTestBRecord();

            // init key
            List<AtomValue> keyValues = new List<AtomValue>();
            AtomValue key = new AtomValue() {Type = AttributeTypes.Char, CharLimit = 8, StringValue = "114514"};
            keyValues.Add(key);
            DBRecord keyRecord = new DBRecord(keyValues);

            // make raw bytes
            List<byte> rawNode = new List<byte>();
            rawNode.AddRange(new byte[30]);

            // build cell
            LeafIndexCell leafIndexCell = new LeafIndexCell(record, keyRecord);
            byte[] raw = leafIndexCell.Pack();
            rawNode.AddRange(raw);

            // clone
            LeafIndexCell clone = new LeafIndexCell(rawNode.ToArray(), 30);

            // assert
            AssertCell(leafIndexCell, clone);
            AssertDBRecords(leafIndexCell.PrimaryKey, clone.PrimaryKey);
        }

        static void TestInternalIndexCell()
        {
            // init record
            DBRecord record = GetTestBRecord();

            // init key
            List<AtomValue> keyValues = new List<AtomValue>();
            AtomValue key = new AtomValue() {Type = AttributeTypes.Char, CharLimit = 8, StringValue = "114514"};
            keyValues.Add(key);
            DBRecord keyRecord = new DBRecord(keyValues);

            // make raw bytes
            List<byte> rawNode = new List<byte>();
            rawNode.AddRange(new byte[30]);

            // build cell
            InternalIndexCell internalIndexCell = new InternalIndexCell(record, 114514, keyRecord);
            byte[] raw = internalIndexCell.Pack();
            rawNode.AddRange(raw);

            // clone
            InternalIndexCell clone = new InternalIndexCell(rawNode.ToArray(), 30);

            // assert
            AssertCell(internalIndexCell, clone);
            Debug.Assert(internalIndexCell.ChildPage == clone.ChildPage);
            AssertDBRecords(internalIndexCell.PrimaryKey, clone.PrimaryKey);
        }


        static void TestInternalTableCell()
        {
            // init record
            DBRecord record = GetTestBRecord();

            // make raw bytes
            List<byte> rawNode = new List<byte>();
            rawNode.AddRange(new byte[30]);

            // build cell
            InternalTableCell internalTableCell = new InternalTableCell(record, 114514);
            byte[] raw = internalTableCell.Pack();
            rawNode.AddRange(raw);

            // clone
            InternalTableCell clone = new InternalTableCell(rawNode.ToArray(), 30);

            // assert
            AssertCell(internalTableCell, clone);
            Debug.Assert(internalTableCell.ChildPage == clone.ChildPage);
        }

        static void TestLeafTableCell()
        {
            // init record
            DBRecord record = GetTestBRecord();

            // init key
            List<AtomValue> keyValues = new List<AtomValue>();
            AtomValue key = new AtomValue() {Type = AttributeTypes.Char, CharLimit = 8, StringValue = "114514"};
            keyValues.Add(key);
            DBRecord keyRecord = new DBRecord(keyValues);

            // make raw bytes
            List<byte> rawNode = new List<byte>();
            rawNode.AddRange(new byte[30]);

            // build cell
            LeafTableCell leafTableCell = new LeafTableCell(keyRecord, record);
            byte[] raw = leafTableCell.Pack();
            rawNode.AddRange(raw);

            // clone
            LeafTableCell leafTableCellClone = new LeafTableCell(rawNode.ToArray(), 30);

            // assert
            AssertDBRecords(leafTableCell.DBRecord, leafTableCellClone.DBRecord);
            AssertCell(leafTableCell, leafTableCellClone);
        }

        static void TestDBRecord()
        {
            // init record
            List<AtomValue> values = new List<AtomValue>();
            AtomValue value1 = new AtomValue() {Type = AttributeTypes.Int, IntegerValue = 222};
            AtomValue value2 = new AtomValue() {Type = AttributeTypes.Null};
            AtomValue value3 = new AtomValue() {Type = AttributeTypes.Char, CharLimit = 5, StringValue = "222"};
            values.Add(value1);
            values.Add(value2);
            values.Add(value3);
            DBRecord record = new DBRecord(values);

            // make raw bytes
            List<byte> rawNode = new List<byte>();
            rawNode.AddRange(new byte[30]);

            // clone
            List<AtomValue> valuesOut1 = record.GetValues();

            // clone
            byte[] raw = record.Pack();
            rawNode.AddRange(raw);
            record.Unpack(rawNode.ToArray(), 30);
            List<AtomValue> valuesOut2 = record.GetValues();

            int i;
            for (i = 0; i < valuesOut2.Count; i++)
            {
                AssertAtomValue(values[i], valuesOut2[i]);
                AssertAtomValue(valuesOut1[i], valuesOut2[i]);
            }
        }

        static DBRecord GetTestBRecord()
        {
            // init record
            List<AtomValue> values = new List<AtomValue>();
            AtomValue value1 = new AtomValue() {Type = AttributeTypes.Int, IntegerValue = 222};
            AtomValue value2 = new AtomValue() {Type = AttributeTypes.Null};
            AtomValue value3 = new AtomValue() {Type = AttributeTypes.Char, CharLimit = 5, StringValue = "222"};
            values.Add(value1);
            values.Add(value2);
            values.Add(value3);
            DBRecord record = new DBRecord(values);
            return record;
        }

        static void AssertCell(BTreeCell c1, BTreeCell c2)
        {
            Debug.Assert(c1.Types == c2.Types);
            AssertDBRecords(c1.Key, c2.Key);
        }

        static void AssertDBRecords(DBRecord r1, DBRecord r2)
        {
            List<AtomValue> valuesOut1 = r1.GetValues();
            List<AtomValue> valuesOut2 = r2.GetValues();

            Debug.Assert(valuesOut1.Count == valuesOut2.Count);

            int i;
            for (i = 0; i < valuesOut2.Count; i++)
            {
                AssertAtomValue(valuesOut1[i], valuesOut2[i]);
            }
        }

        static void AssertAtomValue(AtomValue v1, AtomValue v2)
        {
            Debug.Assert(v1.Type == v2.Type);
            Debug.Assert(v1.CharLimit == v2.CharLimit);
            Debug.Assert(v1.FloatValue == v2.FloatValue);
            Debug.Assert(v1.IntegerValue == v2.IntegerValue);
            Debug.Assert(v1.StringValue == v2.StringValue);
        }
    }
}
