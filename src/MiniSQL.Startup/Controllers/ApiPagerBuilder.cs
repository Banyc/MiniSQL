
using MiniSQL.Api.Controllers;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.CatalogManager.Controllers;
using MiniSQL.IndexManager.Controllers;
using MiniSQL.IndexManager.Interfaces;
using MiniSQL.Interpreter;
using MiniSQL.Library.Interfaces;
using MiniSQL.RecordManager;

namespace MiniSQL.Startup.Controllers
{
    public class ApiPagerBuilder
    {
        public (IApi, Pager) UseDatabase(string databaseName)
        {
            // init
            string dbPath = $"./{databaseName}.minidb";
            Pager pager = new Pager(dbPath, 1024 * 8, 400);
            FreeList freeList = new FreeList(pager);
            IIndexManager bTreeController = new BTreeController(pager, freeList, 40);
            IInterpreter interpreter = new Parsing();
            ICatalogManager catalogManager = new Catalog(databaseName);
            IRecordManager recordManager = new RecordContext(pager, bTreeController);
            IApi api = new ApiController(interpreter, catalogManager, recordManager);
            
            return (api, pager);
        }
    }
}
