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
        // according to the name required, return the full schema record
        SchemaRecord GetSchemaRecord(string name);
        // check validation of the insert statement
        bool CheckInsertStatementValidation(InsertStatement insertStatement);
        // check validation of the delete statement
        bool CheckDeleteStatementValidation(DeleteStatement deleteStatement);
    }
}
