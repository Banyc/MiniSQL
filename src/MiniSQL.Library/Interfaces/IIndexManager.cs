using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    // TODO: review
    public interface IIndexManager
    {
        // return the root page number
        int CreateIndex(CreateStatement createStatement);
        void DropIndex(int rootPage);
        // insert a cell
        int InsertIndex(InsertStatement insertStatement, int rootPage);
        // delete some records -> delete some corresponding indices
        // return the root page number and the primary keys to be deleted
        // indexOfAttribute: := the index of the attribute to be deleted
        (int rootPage, List<AtomValue> primaryKeys) DeleteIndexRecords(DeleteStatement deleteStatement, int indexOfAttribute, int rootPage);
    }
}
