using System.Collections.Generic;
using MiniSQL.Library.Models;
using MiniSQL.Library.Interfaces;

namespace MiniSQL.Api
{
    public class Api : IApi
    {
        private readonly IInterpreter _interpreter;
        private readonly ICatalogManager _catalogManager;
        private readonly IIndexManager _indexManager;
        private readonly IRecordManager _recordManager;

        public Api(IInterpreter interpreter, ICatalogManager catalogManager, IIndexManager indexManager, IRecordManager recordManager)
        {
            _catalogManager = catalogManager;
            _indexManager = indexManager;
            _interpreter = interpreter;
            _recordManager = recordManager;
        }

        // TODO
        public List<AtomValue> Query(string sql)
        {
            Query query = Parse(sql);
            foreach (IStatement statement in query.StatementList)
            {

            }

            throw new System.NotImplementedException();
        }

        private Query Parse(string sql)
        {
            return _interpreter.GetQuery(sql);
        }
    }
}
