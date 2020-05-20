using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    // TODO: review
    public interface ICatalogManager
    {
        // try to save the create statement into file as a few schema records
        // if succeeded, return true. Vice versa
        bool TryCreateStatement(CreateStatement createStatement, int rootPage);
        // try to remove some schema records from file
        // if succeeded, return true. Vice versa
        bool TryDropStatement(DropStatement dropStatement);
        // update the root page of a table or an index
        // if succeeded, return true. Vice versa
        bool TryUpdateSchemaRecord(string name, int rootPage);
        // according to the table name required, return corresponding schema record
        SchemaRecord GetTableSchemaRecord(string tableName);
        // according to the table name required, return the all index schema records that is associated to the table
        List<SchemaRecord> GetIndicesSchemaRecord(string tableName);
        // according to the index name required, return corresponding schema record
        SchemaRecord GetIndexSchemaRecord(string indexName);
        // check validation of the statement
        bool IsValid(IStatement statement);
    }
}
