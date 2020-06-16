using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    public interface IRecordManager
    {
        // return the root page number of the table tree
        int CreateTable();
        // return the root page number of the index tree
        int CreateIndex(int tableRootPage, string indexedColumnName, List<AttributeDeclaration> attributeDeclarations);
        void DropTable(int rootPage);
        // insert a cell
        int InsertRecord(List<AtomValue> values, AtomValue key, int rootPage);
        // delete some records
        // return the root page number
        int DeleteRecords(Expression condition, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage);
        // delete some records
        // return the root page number
        // each primary key corresponds to one record/row
        int DeleteRecords(List<AtomValue> primaryKeys, int rootPage);
        // select some records
        List<List<AtomValue>> SelectRecords(SelectStatement selectStatement, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage);
        // select some records
        // the primary key corresponds to one record/row
        // return null if not found
        List<AtomValue> SelectRecord(AtomValue primaryKey, int rootPage);
    }
}
