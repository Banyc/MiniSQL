using System;
using System.IO;
using MiniSQL.BufferManager.Controllers;

namespace MiniSQL.RecordManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

        }

        private static void TestInsertRecord()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);

            // RecordManager manager = new RecordManager(controller);

            // TODO

        }
    }
}
