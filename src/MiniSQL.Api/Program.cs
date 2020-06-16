using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MiniSQL.Api.Controllers;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.CatalogManager;
using MiniSQL.IndexManager.Controllers;
using MiniSQL.IndexManager.Interfaces;
using MiniSQL.Interpreter;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;
using MiniSQL.RecordManager;

namespace MiniSQL.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            // init
            string dbPath = "./testdbfile.minidb";
            Pager pager = new Pager(dbPath, 1024 * 8, 400);
            FreeList freeList = new FreeList(pager);
            IIndexManager bTreeController = new BTreeController(pager, freeList, 40);
            IInterpreter interpreter = new Parsing();
            ICatalogManager catalogManager = new Catalog();
            IRecordManager recordManager = new RecordContext(pager, bTreeController);
            IApi api = new Api(interpreter, catalogManager, recordManager);

            View view = new View(
                bTreeController,
                interpreter,
                catalogManager,
                recordManager,
                api,
                pager
            );
            view.Interactive();  
        }
    }
}
