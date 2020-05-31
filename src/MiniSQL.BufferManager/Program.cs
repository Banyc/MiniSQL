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

            TestBTreeInsert();

            /*TestDBRecord();

            TestLeafTableCell();

            TestInternalTableCell();

            TestInternalIndexCell();

            TestLeafIndexCell();

            TestPager();

            TestPagerSwapping();

            TestInsertIntoAndDeletionInsideBTreeNode();

            TestFreeList();*/

            Console.WriteLine("BufferManager Test End");
        }

        static void TestBTreeInsert()
        {
            // init record
            DBRecord record_0 = GetTestBRecord(100);
            DBRecord record_1 = GetTestBRecord(101);
            DBRecord record_2 = GetTestBRecord(102);
            DBRecord record_3 = GetTestBRecord(103);
            DBRecord record_4 = GetTestBRecord(104);
            DBRecord record_5 = GetTestBRecord(105);
            DBRecord record_6 = GetTestBRecord(106);
            DBRecord record_7 = GetTestBRecord(107);

            // init key
            List<AtomValue> keyValues_0 = new List<AtomValue>();
            AtomValue key_0 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 3, StringValue = "1" };
            keyValues_0.Add(key_0);
            DBRecord keyRecord_0 = new DBRecord(keyValues_0);


            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);

            BTreeNode root = null;
            root = controller.Insert(keyRecord_0, record_0, root);

            //1
            List<AtomValue> keyValues_1 = new List<AtomValue>();
            AtomValue key_1 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 3, StringValue = "2" };
            keyValues_1.Add(key_1);
            DBRecord keyRecord_1 = new DBRecord(keyValues_1);
            root = controller.Insert(keyRecord_1, record_1, root);
            //2
            List<AtomValue> keyValues_2 = new List<AtomValue>();
            AtomValue key_2 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 3, StringValue = "3" };
            keyValues_2.Add(key_2);
            DBRecord keyRecord_2 = new DBRecord(keyValues_2);
            root = controller.Insert(keyRecord_2, record_2, root);
            //3
            List<AtomValue> keyValues_3 = new List<AtomValue>();
            AtomValue key_3 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 3, StringValue = "4" };
            keyValues_3.Add(key_1);
            DBRecord keyRecord_3 = new DBRecord(keyValues_3);
            root = controller.Insert(keyRecord_3, record_3, root);
            //4
            List<AtomValue> keyValues_4 = new List<AtomValue>();
            AtomValue key_4 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 3, StringValue = "5" };
            keyValues_4.Add(key_4);
            DBRecord keyRecord_4 = new DBRecord(keyValues_4);
            root = controller.Insert(keyRecord_4, record_4, root);
            //5
            List<AtomValue> keyValues_5 = new List<AtomValue>();
            AtomValue key_5 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 3, StringValue = "6" };
            keyValues_5.Add(key_5);
            DBRecord keyRecord_5 = new DBRecord(keyValues_5);
            root = controller.Insert(keyRecord_5, record_5, root);
            //6
            List<AtomValue> keyValues_6 = new List<AtomValue>();
            AtomValue key_6 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 3, StringValue = "7" };
            keyValues_6.Add(key_6);
            DBRecord keyRecord_6 = new DBRecord(keyValues_6);
            root = controller.Insert(keyRecord_6, record_6, root);


            //Find
            LeafTableCell result = null;
            result = (LeafTableCell)controller.Find(keyRecord_0, root);
            //Debug.Assert(result.DBRecord==record_0);
            result = (LeafTableCell)controller.Find(keyRecord_1, root);
            //Debug.Assert(result.DBRecord==record_1);
            result = (LeafTableCell)controller.Find(keyRecord_2, root);
            //Debug.Assert(result.DBRecord==record_2);
            result = (LeafTableCell)controller.Find(keyRecord_3, root);
            //Debug.Assert(result.DBRecord==record_3);
            result = (LeafTableCell)controller.Find(keyRecord_4, root);
            //Debug.Assert(result.DBRecord==record_4);
            result = (LeafTableCell)controller.Find(keyRecord_5, root);
            //Debug.Assert(result.DBRecord==record_5);
            result = (LeafTableCell)controller.Find(keyRecord_6, root);
            //Debug.Assert(result.DBRecord==record_6);



            pager.Close();
        }

        static void TestFreeList()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath, 4096, 100);

            List<MemoryPage> pageList = new List<MemoryPage>();
            pageList.Add(pager.GetNewPage());  // page #2
            pageList.Add(pager.GetNewPage());  // page #3
            pageList.Add(pager.GetNewPage());  // page #4
            pageList.Add(pager.GetNewPage());  // page #5

            FreeList freeList = new FreeList(pager);

            // test initialization
            MemoryPage newPage = null;
            newPage = freeList.AllocatePage();  // freeList is now empty
            Debug.Assert(newPage == null);

            // recycle pages
            // MemoryPage tempPage = pager.ReadPage(3);
            freeList.RecyclePage(pageList[2]);  // freeList->4
            freeList.RecyclePage(pageList[1]);  // freeList->3->4
            freeList.RecyclePage(pageList[3]);  // freeList->5->3->4

            // fetch page from free list
            newPage = freeList.AllocatePage();  // freeList->3->4
            Debug.Assert(newPage.PageNumber == 5);
            newPage = freeList.AllocatePage();  // freeList->4
            Debug.Assert(newPage.PageNumber == 3);

            // recycle a page
            freeList.RecyclePage(pageList[3]);  // freeList->5->4

            // fetch remaining pages
            newPage = freeList.AllocatePage();  // freeList->4
            Debug.Assert(newPage.PageNumber == 5);
            newPage = freeList.AllocatePage();  // freeList->null
            Debug.Assert(newPage.PageNumber == 4);
            newPage = freeList.AllocatePage();  // freeList->null
            Debug.Assert(newPage == null);

            pager.Close();
        }

        static void TestInsertIntoAndDeletionInsideBTreeNode()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath);
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();

            MemoryPage page = pager.ReadPage(1);

            BTreeNode node = new BTreeNode(page);
            node.InitializeEmptyFormat(PageTypes.InternalIndexPage);



            List<DBRecord> keys = new List<DBRecord>();
            keys.Add(GetTestBRecord(1));
            keys.Add(GetTestBRecord(6));
            keys.Add(GetTestBRecord(2));
            keys.Add(GetTestBRecord(5));
            keys.Add(GetTestBRecord(3));
            keys.Add(GetTestBRecord(4));

            List<InternalIndexCell> cells = new List<InternalIndexCell>();
            cells.Add(new InternalIndexCell(keys[0], 114, GetTestBRecord(0)));
            cells.Add(new InternalIndexCell(keys[1], 114, GetTestBRecord(1)));
            cells.Add(new InternalIndexCell(keys[2], 114, GetTestBRecord(2)));
            cells.Add(new InternalIndexCell(keys[3], 114, GetTestBRecord(3)));
            cells.Add(new InternalIndexCell(keys[4], 114, GetTestBRecord(4)));
            cells.Add(new InternalIndexCell(keys[5], 114, GetTestBRecord(5)));

            byte[] raw = cells[0].Pack();
            InternalIndexCell cellClone = new InternalIndexCell(raw, 0);

            node.InsertBTreeCell(cells[0]);
            node.InsertBTreeCell(cells[1]);
            node.InsertBTreeCell(cells[2]);
            node.InsertBTreeCell(cells[3]);
            node.InsertBTreeCell(cells[4]);
            node.InsertBTreeCell(cells[5]);

            List<ushort> offsets = node.CellOffsetArray;
            List<InternalIndexCell> clonecells = new List<InternalIndexCell>();
            clonecells.Add((InternalIndexCell)node.GetBTreeCell(offsets[0]));
            clonecells.Add((InternalIndexCell)node.GetBTreeCell(offsets[1]));
            clonecells.Add((InternalIndexCell)node.GetBTreeCell(offsets[2]));
            clonecells.Add((InternalIndexCell)node.GetBTreeCell(offsets[3]));
            clonecells.Add((InternalIndexCell)node.GetBTreeCell(offsets[4]));
            clonecells.Add((InternalIndexCell)node.GetBTreeCell(offsets[5]));

            List<AtomValue> cloneCellKey = clonecells[0].Key.GetValues();

            Debug.Assert(node.GetBTreeCell(offsets[0]).Key.GetValues()[0].IntegerValue == 1);
            Debug.Assert(node.GetBTreeCell(offsets[1]).Key.GetValues()[0].IntegerValue == 2);
            Debug.Assert(node.GetBTreeCell(offsets[2]).Key.GetValues()[0].IntegerValue == 3);
            Debug.Assert(node.GetBTreeCell(offsets[3]).Key.GetValues()[0].IntegerValue == 4);
            Debug.Assert(node.GetBTreeCell(offsets[4]).Key.GetValues()[0].IntegerValue == 5);
            Debug.Assert(node.GetBTreeCell(offsets[5]).Key.GetValues()[0].IntegerValue == 6);

            List<AtomValue> tmpAtomList = null;

            // iterate
            int i = 1;
            foreach (var iteratedCell in node)
            {
                tmpAtomList = iteratedCell.Key.GetValues();
                Debug.Assert(tmpAtomList[0].IntegerValue == i);
                i++;
            }

            // node indexing
            AssertCell(node[0], cells[0]);
            AssertCell(node[1], cells[2]);

            // find
            (BTreeCell cell, ushort offset, int indexInOffsetArray) = node.FindBTreeCell(keys[2]);
            tmpAtomList = cell.Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 2);

            // delete
            node.DeleteBTreeCell(offset);

            // check deletion
            offsets = node.CellOffsetArray;
            tmpAtomList = node.GetBTreeCell(offsets[0]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 1);
            tmpAtomList = node.GetBTreeCell(offsets[1]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 3);
            tmpAtomList = node.GetBTreeCell(offsets[2]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 4);
            tmpAtomList = node.GetBTreeCell(offsets[3]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 5);
            tmpAtomList = node.GetBTreeCell(offsets[4]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 6);

            // delete
            node.DeleteBTreeCell(offsets[2]);

            // check deletion
            offsets = node.CellOffsetArray;
            tmpAtomList = node.GetBTreeCell(offsets[0]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 1);
            tmpAtomList = node.GetBTreeCell(offsets[1]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 3);
            tmpAtomList = node.GetBTreeCell(offsets[2]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 5);
            tmpAtomList = node.GetBTreeCell(offsets[3]).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 6);

            // delete by index
            node.DeleteBTreeCell(node[0]);
            tmpAtomList = node[0].Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 3);
            tmpAtomList = node[1].Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 5);
            tmpAtomList = node[2].Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 6);

            // delete all
            node.DeleteBTreeCell(offsets[0]);
            offsets = node.CellOffsetArray;
            node.DeleteBTreeCell(offsets[0]);
            offsets = node.CellOffsetArray;
            node.DeleteBTreeCell(offsets[0]);
            offsets = node.CellOffsetArray;

            pager.Close();
        }

        static void TestPagerSwapping()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath, 4096, 4);

            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            // pager.ExtendNumberOfPages();

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
            page4[1] = 44;
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
            catch (Exception)
            {
                error = true;
            }
            Debug.Assert(error == true);

            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.Data[0] == 2);
            Debug.Assert(page4.Data[0] == 4);
            Debug.Assert(page4[1] == 44);

            pager.Close();
        }

        static void TestPager()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath, 4096, 4);

            Debug.Assert(pager.PageCount == 1);

            pager.ExtendNumberOfPages();
            Debug.Assert(pager.PageCount == 2);

            MemoryPage page = pager.ReadPage(1);
            page.Data[2] = 114;
            page.Data[3] = 5;
            page.Data[4] = 14;

            pager.WritePage(page);
            MemoryPage page1 = pager.ReadPage(1);

            Debug.Assert(page.Data[2] == page1.Data[2]);
            Debug.Assert(page.Data[3] == page1.Data[3]);
            Debug.Assert(page.Data[4] == page1.Data[4]);

            // it will save all dirty pages and remove all pages from recording
            // after that, any MemoryPage returned from this `pager` will no longer legal to use
            pager.Close();

            pager.Open(dbPath);
            // another page also with page number 1
            page1 = pager.ReadPage(1);

            // test if preventing buffer duplication
            Debug.Assert(page.Data[2] == page1.Data[2]);
            Debug.Assert(page.Data[3] == page1.Data[3]);
            Debug.Assert(page.Data[4] == page1.Data[4]);

            // test if the two page pointing to the same buffer
            Debug.Assert(page1.Data[100] == 0);
            page.Data[100] = 1;
            Debug.Assert(page1.Data[100] == 1);

            pager.Close();
        }

        static void TestLeafIndexCell()
        {
            // init record
            DBRecord record = GetTestBRecord();

            // init key
            List<AtomValue> keyValues = new List<AtomValue>();
            AtomValue key = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 8, StringValue = "114514" };
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
            AtomValue key = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 8, StringValue = "114514" };
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
            AtomValue key = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 8, StringValue = "114514" };
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
            AtomValue value1 = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = 222 };
            AtomValue value2 = new AtomValue() { Type = AttributeTypes.Null };
            AtomValue value3 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 5, StringValue = "222" };
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

        static DBRecord GetTestBRecord(int key = 222)
        {
            // init record
            List<AtomValue> values = new List<AtomValue>();
            AtomValue value1 = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = key };
            AtomValue value2 = new AtomValue() { Type = AttributeTypes.Null };
            AtomValue value3 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 5, StringValue = "222" };
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
