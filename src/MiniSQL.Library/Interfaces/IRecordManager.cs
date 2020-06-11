using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    // TODO: review
    public interface IRecordManager
    {
        // return the root page number
        int CreateTable(CreateStatement createStatement);
        void DropTable(int rootPage);
        // insert a cell
        int InsertRecord(InsertStatement insertStatement, AtomValue key, int rootPage);
        // delete some records
        // return the root page number
        int DeleteRecords(DeleteStatement deleteStatement, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage);
        // delete some records
        // return the root page number
        // each primary key corresponds to one record/row
        int DeleteRecords(List<AtomValue> primaryKeys, int rootPage);
        // select some records
        List<List<AtomValue>> SelectRecords(SelectStatement selectStatement, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage);
        // select some records
        // each primary key corresponds to one record/row
        List<List<AtomValue>> SelectRecords(List<AtomValue> primaryKeys, int rootPage);
    }
}
