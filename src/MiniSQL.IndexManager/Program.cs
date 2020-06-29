using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Controllers;
using MiniSQL.IndexManager.Interfaces;
using MiniSQL.IndexManager.Models;
using MiniSQL.IndexManager.Utilities;
using MiniSQL.Library.Exceptions;
using MiniSQL.Library.Models;

namespace MiniSQL.IndexManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[IndexManager] Test Begin");

            // BTreeTestCases.TestAll();

            // TestMaxHeightBTree(8, false, true, true);

            BugTest1();

            Bugtest2();

            BugTest3();

            TestExpressionFind();

            TestExpressionDelete();

            TestBTreeInsert();

            TestBTreeDelete();

            HardTestForBTree();

            TestDBRecord();

            TestLeafTableCell();

            TestInternalTableCell();

            TestInternalIndexCell();

            TestLeafIndexCell();

            TestInsertIntoAndDeletionInsideBTreeNode();

            // TestPager();

            // TestPagerSwapping();

            // TestFreeList();

            Console.WriteLine("[IndexManager] Test End");
        }

        // Fixed issue at the #6 stage
        static void BugTest3()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);
            BTreeNode root = null;
            LeafTableCell result = null;
            // 1
            // DBRecord record = GetTestBRecord(74396264);
            // DBRecord keyRecord = GetTestBRecord(74396264);
            DBRecord record = GetTestBRecord(1);
            DBRecord keyRecord = GetTestBRecord(1);
            root = controller.InsertCell(root, keyRecord, record);
            // 2
            // record = GetTestBRecord(1766307441);
            // keyRecord = GetTestBRecord(1766307441);
            record = GetTestBRecord(7);
            keyRecord = GetTestBRecord(7);
            root = controller.InsertCell(root, keyRecord, record);
            // 3
            // record = GetTestBRecord(2025306881);
            // keyRecord = GetTestBRecord(2025306881);
            record = GetTestBRecord(8);
            keyRecord = GetTestBRecord(8);
            root = controller.InsertCell(root, keyRecord, record);
            // 4
            // record = GetTestBRecord(147488698);
            // keyRecord = GetTestBRecord(147488698);
            record = GetTestBRecord(2);
            keyRecord = GetTestBRecord(2);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);
            // 5
            // record = GetTestBRecord(1109110087);
            // keyRecord = GetTestBRecord(1109110087);
            record = GetTestBRecord(4);
            keyRecord = GetTestBRecord(4);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);
            // test 5
            // keyRecord = GetTestBRecord(1109110087);
            keyRecord = GetTestBRecord(4);
            result = (LeafTableCell)controller.FindCell(keyRecord, root);
            Debug.Assert(result != null);
            // Debug.Assert(result.Key.GetValues()[0].IntegerValue == 1109110087);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 4);

            // 6
            // record = GetTestBRecord(1163206015);
            // keyRecord = GetTestBRecord(1163206015);
            record = GetTestBRecord(5);
            keyRecord = GetTestBRecord(5);
            // ISSUE HERE
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            // 7
            // record = GetTestBRecord(1485715653);
            // keyRecord = GetTestBRecord(1485715653);
            record = GetTestBRecord(6);
            keyRecord = GetTestBRecord(6);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            // 8
            // record = GetTestBRecord(1087082570);
            // keyRecord = GetTestBRecord(1087082570);
            record = GetTestBRecord(3);
            keyRecord = GetTestBRecord(3);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            pager.Close();
        }

        static void BugTest1()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);
            BTreeNode root = null;
            LeafTableCell result = null;

            DBRecord record = GetTestBRecord(76767785);
            DBRecord keyRecord = GetTestBRecord(76767785);
            root = controller.InsertCell(root, keyRecord, record);

            record = GetTestBRecord(1922063022);
            keyRecord = GetTestBRecord(1922063022);
            root = controller.InsertCell(root, keyRecord, record);

            record = GetTestBRecord(514874720);
            keyRecord = GetTestBRecord(514874720);
            root = controller.InsertCell(root, keyRecord, record);

            record = GetTestBRecord(724803552);
            keyRecord = GetTestBRecord(724803552);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            record = GetTestBRecord(1219882375);
            keyRecord = GetTestBRecord(1219882375);
            root = controller.InsertCell(root, keyRecord, record);

            keyRecord = GetTestBRecord(724803552);
            result = (LeafTableCell)controller.FindCell(keyRecord, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 724803552);


            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            record = GetTestBRecord(681446986);
            keyRecord = GetTestBRecord(681446986);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            record = GetTestBRecord(1427789753);
            keyRecord = GetTestBRecord(1427789753);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            record = GetTestBRecord(1066176166);
            keyRecord = GetTestBRecord(1066176166);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            pager.Close();
        }

        static void Bugtest2()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);
            BTreeNode root = null;
            LeafTableCell result = null;

            DBRecord record = GetTestBRecord(660132168);
            DBRecord keyRecord = GetTestBRecord(660132168);
            root = controller.InsertCell(root, keyRecord, record);

            record = GetTestBRecord(2007593075);
            keyRecord = GetTestBRecord(2007593075);
            root = controller.InsertCell(root, keyRecord, record);

            record = GetTestBRecord(356456016);
            keyRecord = GetTestBRecord(356456016);
            root = controller.InsertCell(root, keyRecord, record);

            record = GetTestBRecord(32731844);
            keyRecord = GetTestBRecord(32731844);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            record = GetTestBRecord(159431057);
            keyRecord = GetTestBRecord(159431057);
            root = controller.InsertCell(root, keyRecord, record);

            keyRecord = GetTestBRecord(660132168);
            result = (LeafTableCell)controller.FindCell(keyRecord, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 660132168);


            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            record = GetTestBRecord(991596943);
            keyRecord = GetTestBRecord(991596943);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            record = GetTestBRecord(794643883);
            keyRecord = GetTestBRecord(794643883);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            record = GetTestBRecord(1158712065);
            keyRecord = GetTestBRecord(1158712065);
            root = controller.InsertCell(root, keyRecord, record);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            pager.Close();
        }


        static void TestMaxHeightBTree(int cellNumber, bool isKeyRandom, bool isShowInsertOrder, bool isShowIntermediateTree)
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);
            BTreeNode root = null;
            //LeafTableCell result = null;
            Random rnd = new Random();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            //Construct BTree
            for (int i = 1; i < cellNumber + 1; i++)
            {
                int key = i;
                if (isKeyRandom)
                {
                    key = rnd.Next();
                }
                DBRecord record = GetTestBRecord(i + 100);
                DBRecord keyRecord = GetTestBRecord(key);
                if (isShowInsertOrder)
                {
                    Console.WriteLine(key);
                }
                root = controller.InsertCell(root, keyRecord, record);
                if (isShowIntermediateTree)
                {
                    BTreeNodeHelper.VisualizeIntegerTree(pager, root);
                }
            }
            stopwatch.Stop();

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);
            Console.WriteLine();
            Console.WriteLine($"Time cost: {stopwatch.Elapsed.TotalSeconds}s");

            pager.Close();
        }


        static void TestExpressionDelete()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);
            BTreeNode root = null;

            Expression expression = GetAndsExpression();

            for (int i = 1; i < 30; i++)
            {
                DBRecord record = GetTestRecord_expression(i, "str", (float)3.3);
                DBRecord key = GetTestKey_expression(i);
                if (i == 17 || i == 19)
                {
                    record = GetTestRecord_expression(i, "str", (float)10.5);
                }
                else if (i == 20 || i == 23)
                {
                    record = GetTestRecord_expression(i, "www", (float)1.3);
                }
                root = controller.InsertCell(root, key, record);
            }
            List<AttributeDeclaration> attributeNames = new List<AttributeDeclaration>();

            AttributeDeclaration attribute_1 = new AttributeDeclaration();
            attribute_1.AttributeName = "a";
            attribute_1.IsUnique = true;
            attributeNames.Add(attribute_1);

            AttributeDeclaration attribute_2 = new AttributeDeclaration();
            attribute_2.AttributeName = "b";
            attribute_2.IsUnique = false;
            attributeNames.Add(attribute_2);

            AttributeDeclaration attribute_3 = new AttributeDeclaration();
            attribute_3.AttributeName = "c";
            attribute_3.IsUnique = false;
            attributeNames.Add(attribute_3);

            root = controller.DeleteCells(root, expression, "a", attributeNames);

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            pager.Close();
        }

        static void TestExpressionFind()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);
            BTreeNode root = null;
            List<BTreeCell> result = null;

            Expression expression = GetAndsExpression();

            for (int i = 1; i < 30; i++)
            {
                DBRecord record = GetTestRecord_expression(i, "str", (float)3.3);
                DBRecord key = GetTestKey_expression(i);
                if (i == 17 || i == 19)
                {
                    record = GetTestRecord_expression(i, "str", (float)10.5);
                }
                else if (i == 20 || i == 23)
                {
                    record = GetTestRecord_expression(i, "www", (float)1.3);
                }
                root = controller.InsertCell(root, key, record);
            }
            List<AttributeDeclaration> attributeNames = new List<AttributeDeclaration>();

            AttributeDeclaration attribute_1 = new AttributeDeclaration();
            attribute_1.AttributeName = "a";
            attribute_1.IsUnique = true;
            attributeNames.Add(attribute_1);

            AttributeDeclaration attribute_2 = new AttributeDeclaration();
            attribute_2.AttributeName = "b";
            attribute_2.IsUnique = false;
            attributeNames.Add(attribute_2);

            AttributeDeclaration attribute_3 = new AttributeDeclaration();
            attribute_3.AttributeName = "c";
            attribute_3.IsUnique = false;
            attributeNames.Add(attribute_3);

            result = controller.FindCells(root, expression, "a", attributeNames);

            foreach (var cell in result)
            {
                LeafTableCell leafTableCell = (LeafTableCell)cell;
                Console.Write(leafTableCell.DBRecord.GetValues()[0].IntegerValue);
                Console.Write("|");
                Console.Write(leafTableCell.DBRecord.GetValues()[1].StringValue);
                Console.Write("|");
                Console.Write(leafTableCell.DBRecord.GetValues()[2].FloatValue);
                Console.WriteLine();
            }

            pager.Close();
        }

        private static DBRecord GetTestRecord_expression(int value_1, string value_2, float value_3)
        {
            List<AtomValue> values = new List<AtomValue>();
            AtomValue value1 = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = value_1 };
            AtomValue value2 = new AtomValue() { Type = AttributeTypes.Char, CharLimit = 5, StringValue = value_2 };
            AtomValue value3 = new AtomValue() { Type = AttributeTypes.Float, FloatValue = value_3 };
            values.Add(value1);
            values.Add(value2);
            values.Add(value3);
            DBRecord record = new DBRecord(values);

            return record;
        }

        private static DBRecord GetTestKey_expression(int value_1)
        {
            List<AtomValue> values = new List<AtomValue>();
            AtomValue value1 = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = value_1 };
            values.Add(value1);
            DBRecord record = new DBRecord(values);
            return record;
        }

        private static Expression GetAndsExpression()
        {
            // __tree structure__
            //             and 1
            //        and 2,     > 3
            //     <= 4, <= 5, 6.6 6, c 7
            // 15 8, a, 9, b 10, "str" 11

            // 32
            Expression node8 = new Expression();
            node8.Operator = Operator.AtomConcreteValue;
            node8.ConcreteValue = new AtomValue();
            node8.ConcreteValue.Type = AttributeTypes.Int;
            node8.ConcreteValue.IntegerValue = 15;
            // a
            Expression node9 = new Expression();
            node9.Operator = Operator.AtomVariable;
            node9.AttributeName = "a";
            // b
            Expression node10 = new Expression();
            node10.Operator = Operator.AtomVariable;
            node10.AttributeName = "b";
            // "str"
            Expression node11 = new Expression();
            node11.Operator = Operator.AtomConcreteValue;
            node11.ConcreteValue = new AtomValue();
            node11.ConcreteValue.Type = AttributeTypes.Char;
            node11.ConcreteValue.CharLimit = 5;
            node11.ConcreteValue.StringValue = "str";
            // <=
            Expression node4 = new Expression();
            node4.Operator = Operator.LessThanOrEqualTo;
            node4.LeftOperand = node8;
            node4.RightOperand = node9;
            // <=
            Expression node5 = new Expression();
            node5.Operator = Operator.LessThanOrEqualTo;
            node5.LeftOperand = node10;
            node5.RightOperand = node11;
            // 6.6
            Expression node6 = new Expression();
            node6.Operator = Operator.AtomConcreteValue;
            node6.ConcreteValue = new AtomValue();
            node6.ConcreteValue.Type = AttributeTypes.Float;
            node6.ConcreteValue.FloatValue = 6.6;
            // c
            Expression node7 = new Expression();
            node7.Operator = Operator.AtomVariable;
            node7.AttributeName = "c";
            // and
            Expression node2 = new Expression();
            node2.Operator = Operator.And;
            node2.LeftOperand = node4;
            node2.RightOperand = node5;
            // >
            Expression node3 = new Expression();
            node3.Operator = Operator.MoreThan;
            node3.LeftOperand = node6;
            node3.RightOperand = node7;
            // and
            Expression node1 = new Expression();
            node1.Operator = Operator.And;
            node1.LeftOperand = node2;
            node1.RightOperand = node3;

            return node1;
        }

        static void TestBTreeDelete()
        {

            LeafTableCell result = null;
            // init record
            DBRecord record_0 = GetTestBRecord(100);
            DBRecord record_1 = GetTestBRecord(101);
            DBRecord record_2 = GetTestBRecord(102);
            DBRecord record_3 = GetTestBRecord(103);
            DBRecord record_4 = GetTestBRecord(104);
            DBRecord record_5 = GetTestBRecord(105);
            DBRecord record_6 = GetTestBRecord(106);
            DBRecord record_7 = GetTestBRecord(107);
            DBRecord record_8 = GetTestBRecord(108);

            DBRecord keyRecord_0 = GetTestBRecord(1);
            DBRecord keyRecord_1 = GetTestBRecord(2);
            DBRecord keyRecord_2 = GetTestBRecord(3);
            DBRecord keyRecord_3 = GetTestBRecord(4);
            DBRecord keyRecord_4 = GetTestBRecord(5);
            DBRecord keyRecord_5 = GetTestBRecord(6);
            DBRecord keyRecord_6 = GetTestBRecord(7);
            DBRecord keyRecord_7 = GetTestBRecord(8);
            DBRecord keyRecord_8 = GetTestBRecord(9);


            // init key
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);
            BTreeNode root = null;
            //0
            root = controller.InsertCell(root, keyRecord_0, record_0);
            //1
            root = controller.InsertCell(root, keyRecord_1, record_1);
            //2
            root = controller.InsertCell(root, keyRecord_2, record_2);
            //3
            root = controller.InsertCell(root, keyRecord_3, record_3);
            //4
            root = controller.InsertCell(root, keyRecord_4, record_4);
            //5
            root = controller.InsertCell(root, keyRecord_5, record_5);
            //6
            root = controller.InsertCell(root, keyRecord_6, record_6);

            //Delete test;

            //6
            root = controller.Delete(keyRecord_6, root);
            result = (LeafTableCell)controller.FindCell(keyRecord_6, root);
            Debug.Assert(result == null);

            //5
            root = controller.Delete(keyRecord_5, root);
            result = (LeafTableCell)controller.FindCell(keyRecord_5, root);
            Debug.Assert(result == null);

            //4
            root = controller.Delete(keyRecord_4, root);
            result = (LeafTableCell)controller.FindCell(keyRecord_4, root);
            Debug.Assert(result == null);


            //3
            root = controller.Delete(keyRecord_3, root);
            result = (LeafTableCell)controller.FindCell(keyRecord_3, root);
            Debug.Assert(result == null);

            //2
            root = controller.Delete(keyRecord_2, root);
            result = (LeafTableCell)controller.FindCell(keyRecord_2, root);
            Debug.Assert(result == null);

            //1
            root = controller.Delete(keyRecord_1, root);
            result = (LeafTableCell)controller.FindCell(keyRecord_1, root);
            Debug.Assert(result == null);


            //0
            root = controller.Delete(keyRecord_0, root);
            result = (LeafTableCell)controller.FindCell(keyRecord_0, root);
            Debug.Assert(result == null);


            //insert after delete
            root = controller.InsertCell(root, keyRecord_0, record_0);
            result = (LeafTableCell)controller.FindCell(keyRecord_0, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 1);

            root = controller.InsertCell(root, keyRecord_7, record_7);
            result = (LeafTableCell)controller.FindCell(keyRecord_7, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 8);


            root = controller.InsertCell(root, keyRecord_6, record_6);
            result = (LeafTableCell)controller.FindCell(keyRecord_6, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 7);

            pager.Close();
        }

        static void HardTestForBTree()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList, 4);
            BTreeNode root = null;
            LeafTableCell result = null;

            //Construct BTree
            for (int i = 1; i < 20; i++)
            {
                DBRecord record = GetTestBRecord(i + 100);
                DBRecord keyRecord = GetTestBRecord(i);
                root = controller.InsertCell(root, keyRecord, record);

                result = (LeafTableCell)controller.FindCell(keyRecord, root);
                Debug.Assert(result != null);
                Debug.Assert(result.Key.GetValues()[0].IntegerValue == i);

            }
            // test inserting records with repeated primary keys
            DBRecord record_D = GetTestBRecord(103);
            DBRecord keyRecord_D = GetTestBRecord(3);
            bool isError = false;
            try
            {
                root = controller.InsertCell(root, keyRecord_D, record_D);
            }
            catch (RepeatedKeyException)
            {
                isError = true;
            }
            Debug.Assert(isError == true);

            isError = false;
            record_D = GetTestBRecord(105);
            keyRecord_D = GetTestBRecord(5);
            try
            {
                root = controller.InsertCell(root, keyRecord_D, record_D);
            }
            catch (RepeatedKeyException)
            {
                isError = true;
            }

            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            //find all
            for (int i = 1; i < 20; i++)
            {
                DBRecord keyRecord = GetTestBRecord(i);

                result = (LeafTableCell)controller.FindCell(keyRecord, root);
                Debug.Assert(result != null);
                Debug.Assert(result.Key.GetValues()[0].IntegerValue == i);

            }

            //delete
            for (int i = 10; i < 20; i++)
            {
                DBRecord keyRecord = GetTestBRecord(i);
                root = controller.Delete(keyRecord, root);

                result = (LeafTableCell)controller.FindCell(keyRecord, root);
                Debug.Assert(result == null);

                for (int m = 1; m < 10; m++)
                {
                    DBRecord keyRecord_check = GetTestBRecord(m);

                    result = (LeafTableCell)controller.FindCell(keyRecord_check, root);
                    Debug.Assert(result != null);
                    Debug.Assert(result.Key.GetValues()[0].IntegerValue == m);

                }
            }
            BTreeNodeHelper.VisualizeIntegerTree(pager, root);

            //find others
            for (int i = 1; i < 10; i++)
            {
                DBRecord keyRecord = GetTestBRecord(i);

                result = (LeafTableCell)controller.FindCell(keyRecord, root);
                Debug.Assert(result != null);
                Debug.Assert(result.Key.GetValues()[0].IntegerValue == i);

            }


            //insert after delete
            for (int i = 10; i < 20; i++)
            {
                DBRecord record = GetTestBRecord(i + 100);
                DBRecord keyRecord = GetTestBRecord(i);
                root = controller.InsertCell(root, keyRecord, record);

                result = (LeafTableCell)controller.FindCell(keyRecord, root);
                Debug.Assert(result != null);
                Debug.Assert(result.Key.GetValues()[0].IntegerValue == i);

            }

            //find all
            for (int i = 1; i < 20; i++)
            {
                DBRecord keyRecord = GetTestBRecord(i);

                result = (LeafTableCell)controller.FindCell(keyRecord, root);
                Debug.Assert(result != null);
                Debug.Assert(result.Key.GetValues()[0].IntegerValue == i);

            }

            pager.Close();
        }

        static void TestBTreeInsert()
        {
            LeafTableCell result = null;
            // init record
            DBRecord record_0 = GetTestBRecord(100);
            DBRecord record_1 = GetTestBRecord(101);
            DBRecord record_2 = GetTestBRecord(102);
            DBRecord record_3 = GetTestBRecord(103);
            DBRecord record_4 = GetTestBRecord(104);
            DBRecord record_5 = GetTestBRecord(105);
            DBRecord record_6 = GetTestBRecord(106);
            DBRecord record_7 = GetTestBRecord(107);

            DBRecord keyRecord_0 = GetTestBRecord(1);
            DBRecord keyRecord_1 = GetTestBRecord(2);
            DBRecord keyRecord_2 = GetTestBRecord(3);
            DBRecord keyRecord_3 = GetTestBRecord(4);
            DBRecord keyRecord_4 = GetTestBRecord(5);
            DBRecord keyRecord_5 = GetTestBRecord(6);
            DBRecord keyRecord_6 = GetTestBRecord(7);


            // init key
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);

            BTreeNode root = null;
            root = controller.InsertCell(root, keyRecord_0, record_0);
            result = (LeafTableCell)controller.FindCell(keyRecord_0, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 1);
            //1
            root = controller.InsertCell(root, keyRecord_1, record_1);
            result = (LeafTableCell)controller.FindCell(keyRecord_1, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 2);
            //2
            root = controller.InsertCell(root, keyRecord_2, record_2);
            result = (LeafTableCell)controller.FindCell(keyRecord_2, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 3);
            //3
            root = controller.InsertCell(root, keyRecord_3, record_3);
            result = (LeafTableCell)controller.FindCell(keyRecord_3, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 4);
            //4
            root = controller.InsertCell(root, keyRecord_4, record_4);
            result = (LeafTableCell)controller.FindCell(keyRecord_4, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 5);
            //5
            root = controller.InsertCell(root, keyRecord_5, record_5);
            result = (LeafTableCell)controller.FindCell(keyRecord_5, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 6);

            //6
            root = controller.InsertCell(root, keyRecord_6, record_6);
            result = (LeafTableCell)controller.FindCell(keyRecord_6, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 7);
            //Find

            result = (LeafTableCell)controller.FindCell(keyRecord_0, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 1);

            result = (LeafTableCell)controller.FindCell(keyRecord_1, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 2);

            result = (LeafTableCell)controller.FindCell(keyRecord_2, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 3);

            result = (LeafTableCell)controller.FindCell(keyRecord_3, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 4);

            result = (LeafTableCell)controller.FindCell(keyRecord_4, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 5);

            result = (LeafTableCell)controller.FindCell(keyRecord_5, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 6);

            result = (LeafTableCell)controller.FindCell(keyRecord_6, root);
            Debug.Assert(result != null);
            Debug.Assert(result.Key.GetValues()[0].IntegerValue == 7);

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

            BTreeNode node = new BTreeNode(page, PageTypes.InternalIndexPage);


            // `keys` := (1, 6, 2, 5, 3, 4)
            List<DBRecord> keys = new List<DBRecord>();
            keys.Add(GetTestBRecord(1));
            keys.Add(GetTestBRecord(6));
            keys.Add(GetTestBRecord(2));
            keys.Add(GetTestBRecord(5));
            keys.Add(GetTestBRecord(3));
            keys.Add(GetTestBRecord(4));

            // `cells` := ((1, 0), (6, 1), (2, 2), (5, 3), (3, 4), (4, 5))
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

            // all cells (not the list `cells`) inside `node`:
            // ((1, 0), (2, 2), (3, 4), (4, 5), (5, 3), (6, 1))
            // they are all pointing to page #114

            // check if the keys are stored in ascending order
            Debug.Assert(node.GetBTreeCell(offsets[0]).Key.GetValues()[0].IntegerValue == 1);
            Debug.Assert(node.GetBTreeCell(offsets[1]).Key.GetValues()[0].IntegerValue == 2);
            Debug.Assert(node.GetBTreeCell(offsets[2]).Key.GetValues()[0].IntegerValue == 3);
            Debug.Assert(node.GetBTreeCell(offsets[3]).Key.GetValues()[0].IntegerValue == 4);
            Debug.Assert(node.GetBTreeCell(offsets[4]).Key.GetValues()[0].IntegerValue == 5);
            Debug.Assert(node.GetBTreeCell(offsets[5]).Key.GetValues()[0].IntegerValue == 6);

            List<AtomValue> tmpAtomList = null;

            // check if `node` could be iterated by "foreach" statement
            int i = 1;
            foreach (var iteratedCell in node)
            {
                tmpAtomList = iteratedCell.Key.GetValues();
                Debug.Assert(tmpAtomList[0].IntegerValue == i);
                i++;
            }

            // check node indexing
            AssertCell(node[0], cells[0]);
            AssertCell(node[1], cells[2]);

            BTreeCell cell;
            ushort offset;
            int indexInOffsetArray;

            // find by the keys below and check if it returns currect cells
            // key 6: value 1
            (cell, offset, indexInOffsetArray) = node.FindBTreeCell(keys[1]);
            tmpAtomList = ((InternalIndexCell)cell).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 6);
            tmpAtomList = ((InternalIndexCell)cell).PrimaryKey.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 1);
            // key 5: value 3
            (cell, offset, indexInOffsetArray) = node.FindBTreeCell(keys[3]);
            tmpAtomList = ((InternalIndexCell)cell).Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 5);
            tmpAtomList = ((InternalIndexCell)cell).PrimaryKey.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 3);
            // key 2: value 2
            (cell, offset, indexInOffsetArray) = node.FindBTreeCell(keys[2]);
            tmpAtomList = cell.Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 2);
            tmpAtomList = ((InternalIndexCell)cell).PrimaryKey.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 2);

            // delete cell with key == 2
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

            // delete cell with key == 4
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

            // delete by index 0 (cell with key == 1)
            node.DeleteBTreeCell(node[0]);
            tmpAtomList = node[0].Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 3);
            tmpAtomList = node[1].Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 5);
            tmpAtomList = node[2].Key.GetValues();
            Debug.Assert(tmpAtomList[0].IntegerValue == 6);

            // delete remaining cells
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
            DBRecord cloneRecord = new DBRecord(record.Pack(), 0);
            AssertDBRecords(record, cloneRecord);
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