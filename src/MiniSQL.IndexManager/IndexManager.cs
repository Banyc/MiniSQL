using System.Collections.Generic;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.IndexManager
{
    public class IndexManager : IIndexManager
    {
        public int CreateIndex(CreateStatement createStatement)
        {
            throw new System.NotImplementedException();
        }

        public (int rootPage, List<AtomValue> primaryKeys) DeleteIndexRecords(DeleteStatement deleteStatement, int indexOfAttribute, int rootPage)
        {
            throw new System.NotImplementedException();
        }

        public void DropIndex(int rootPage)
        {
            throw new System.NotImplementedException();
        }

        public int InsertIndex(InsertStatement insertStatement, int rootPage)
        {
            throw new System.NotImplementedException();
        }
    }
}
