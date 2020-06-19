using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Controllers;
using MiniSQL.IndexManager.Utilities;
using MiniSQL.Library.Models;

namespace MiniSQL.IndexManager.Models
{
    public static class BTreeTestCases
    {
        public static void TestAll()
        {
            TestLeafTableNode();
            TestMaxHeightBTree();
        }

        static void TestMaxHeightBTree()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);
            BTreeNode root = null;
            root = controller.OccupyNewTableNode();

            // init record
            DBRecord keyRecord = null;
            DBRecord record = null;

            // insert
            keyRecord = GetTestBRecord(1);
            record = GetTestBRecord(175.1, 1, "Person1", "000001", 18);
            // insert to tree
            root = controller.InsertCell(root, keyRecord, record);
            // visualize
            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            keyRecord = GetTestBRecord(2);
            record = GetTestBRecord(165.1, 2, "Person2", "000002", 19);
            // insert to tree
            root = controller.InsertCell(root, keyRecord, record);
            // visualize
            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            keyRecord = GetTestBRecord(3);
            record = GetTestBRecord(165.3, 3, "Person3", "000003", 20);
            // insert to tree
            root = controller.InsertCell(root, keyRecord, record);
            // visualize
            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            keyRecord = GetTestBRecord(4);
            record = GetTestBRecord(175.9, 4, "Person4", "000004", 21);
            // insert to tree
            root = controller.InsertCell(root, keyRecord, record);
            // visualize
            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            keyRecord = GetTestBRecord(5);
            record = GetTestBRecord(175.0, 5, "Person5", "000005", 22);
            // insert to tree
            root = controller.InsertCell(root, keyRecord, record);
            // visualize
            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            keyRecord = GetTestBRecord(6);
            record = GetTestBRecord(172.1, 6, "Person6", "000006", 23);
            // insert to tree
            root = controller.InsertCell(root, keyRecord, record);
            // visualize
            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            Console.WriteLine();

            pager.Close();
        }

        public static void TestLeafTableNode()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            MemoryPage page = pager.GetNewPage();

            BTreeNode node = new BTreeNode(page, PageTypes.LeafTablePage);

            // init record
            DBRecord keyRecord = null;
            DBRecord record = null;
            LeafTableCell leafTableCell = null;

            keyRecord = GetTestBRecord(1);
            record = GetTestBRecord(175.1, 1, "Person1", "000001", 18);
            // build cell + insert to node
            leafTableCell = new LeafTableCell(keyRecord, record);
            node.InsertBTreeCell(leafTableCell);

            keyRecord = GetTestBRecord(2);
            record = GetTestBRecord(165.1, 2, "Person2", "000002", 19);
            // build cell + insert to node
            leafTableCell = new LeafTableCell(keyRecord, record);
            node.InsertBTreeCell(leafTableCell);

            keyRecord = GetTestBRecord(3);
            record = GetTestBRecord(165.3, 3, "Person3", "000003", 20);
            // build cell + insert to node
            leafTableCell = new LeafTableCell(keyRecord, record);
            node.InsertBTreeCell(leafTableCell);

            keyRecord = GetTestBRecord(4);
            record = GetTestBRecord(175.9, 4, "Person4", "000004", 21);
            // build cell + insert to node
            leafTableCell = new LeafTableCell(keyRecord, record);
            node.InsertBTreeCell(leafTableCell);

            keyRecord = GetTestBRecord(5);
            record = GetTestBRecord(175.0, 5, "Person5", "000005", 22);
            // build cell + insert to node
            leafTableCell = new LeafTableCell(keyRecord, record);
            node.InsertBTreeCell(leafTableCell);
            // visualize
            BTreeNodeHelper.VisualizeIntegerTree(pager, node);

            keyRecord = GetTestBRecord(6);
            record = GetTestBRecord(172.1, 6, "Person6", "000006", 23);
            // build cell + insert to node
            leafTableCell = new LeafTableCell(keyRecord, record);
            node.InsertBTreeCell(leafTableCell);
            // visualize
            BTreeNodeHelper.VisualizeIntegerTree(pager, node);

            pager.Close();
        }

        static DBRecord GetTestBRecord(int key = 222)
        {
            // init record
            List<AtomValue> values = new List<AtomValue>();
            AtomValue value1 = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = key };
            // AtomValue value2 = new AtomValue() { Type = AttributeTypes.Null };
            // AtomValue value3 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 5, StringValue = "222" };
            values.Add(value1);
            // values.Add(value2);
            // values.Add(value3);
            DBRecord record = new DBRecord(values);
            DBRecord cloneRecord = new DBRecord(record.Pack(), 0);
            BTreeNodeHelper.AssertDBRecords(record, cloneRecord);
            return record;
        }

        static DBRecord GetTestBRecord(double height, int pid, string name, string identity, int age)
        {
            // init record
            List<AtomValue> values = new List<AtomValue>();
            AtomValue value1 = new AtomValue() { Type = AttributeTypes.Float, FloatValue = height };
            AtomValue value2 = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = pid };
            AtomValue value3 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 32, StringValue = name };
            AtomValue value4 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 128, StringValue = identity };
            AtomValue value5 = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = age };
            values.Add(value1);
            values.Add(value2);
            values.Add(value3);
            values.Add(value4);
            values.Add(value5);
            DBRecord record = new DBRecord(values);
            DBRecord cloneRecord = new DBRecord(record.Pack(), 0);
            BTreeNodeHelper.AssertDBRecords(record, cloneRecord);
            return record;
        }
    }
}
