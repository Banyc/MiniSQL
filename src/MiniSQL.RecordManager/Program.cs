using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.IndexManager.Controllers;
using MiniSQL.IndexManager.Models;
using MiniSQL.IndexManager.Utilities;
using MiniSQL.Library.Models;
using MiniSQL.RecordManager.Controllers;

namespace MiniSQL.RecordManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[RecordManager] Test start!");

            TestInsertRandomRecord(4);

            Console.WriteLine("[RecordManager] Test end!");
        }

        private static void TestInsertRandomRecord(int maxCell)
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);

            RecordContext recordManager = new RecordContext(pager, controller);

            // create new table
            CreateStatement createStatement = GetCreateStatement();
            int newRoot = recordManager.CreateTable();

            // insert
            BTreeNode node;
            InsertStatement insertStatement;
            int key;
            int newRootAfterInsert = newRoot;
            int i;

            for (i = 0; i < maxCell; i++)
            {
                (insertStatement, key) = GetInsertStatement(1);
                AtomValue atomValue = GetAtomValue(key);
                
                newRootAfterInsert = recordManager.InsertRecord(insertStatement.Values, atomValue, newRootAfterInsert);
                Console.WriteLine(key);
                Debug.Assert(newRoot == newRootAfterInsert);
            }
            node = BTreeNodeHelper.GetBTreeNode(pager, newRootAfterInsert);
            BTreeNodeHelper.VisualizeIntegerTree(pager, node);
            Console.WriteLine();

            for (i = 0; i < maxCell; i++)
            {
                (insertStatement, key) = GetInsertStatement(1);
                AtomValue atomValue = GetAtomValue(key);
                newRootAfterInsert = recordManager.InsertRecord(insertStatement.Values, atomValue, newRootAfterInsert);
                Console.WriteLine(key);
            }
            node = BTreeNodeHelper.GetBTreeNode(pager, newRootAfterInsert);
            BTreeNodeHelper.VisualizeIntegerTree(pager, node);

            pager.Close();
        }

        private static CreateStatement GetCreateStatement()
        {
            CreateStatement createStatement = new CreateStatement();
            createStatement.CreateType = CreateType.Table;
            createStatement.TableName = "randomTable";
            createStatement.AttributeDeclarations = new List<AttributeDeclaration>()
            {
                GetAttributeDeclaration("a"),
                GetAttributeDeclaration("id"),
                GetAttributeDeclaration("b"),
            };
            createStatement.PrimaryKey = "id";
            return createStatement;
        }

        private static (InsertStatement, int) GetInsertStatement(int returnIndex)
        {
            Random rnd = new Random();
            List<int> list = new List<int>()
            {
                rnd.Next(),
                rnd.Next(),
                rnd.Next(),
            };
            InsertStatement insertStatement = new InsertStatement();
            insertStatement.TableName = "randomTable";
            insertStatement.Values = new List<AtomValue>()
            {
                GetAtomValue(list[0]),
                GetAtomValue(list[1]),
                GetAtomValue(list[2]),
            };
            return (insertStatement, list[returnIndex]);
        }

        private static AtomValue GetAtomValue(int key)
        {
            AtomValue value = new AtomValue();
            value.Type = AttributeTypes.Int;
            value.IntegerValue = key;
            return value;
        }

        private static AttributeDeclaration GetAttributeDeclaration(string name)
        {
            AttributeDeclaration attribute = new AttributeDeclaration();
            attribute.AttributeName = name;
            attribute.Type = AttributeTypes.Int;
            return attribute;
        }
    }
}
