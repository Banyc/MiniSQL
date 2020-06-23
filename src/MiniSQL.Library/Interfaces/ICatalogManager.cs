using System;
using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    public class TableOrIndexAlreadyExistsException : Exception
    {
        public TableOrIndexAlreadyExistsException()
        { }
        public TableOrIndexAlreadyExistsException(string message)
            : base(message) { }
        public TableOrIndexAlreadyExistsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class TableOrIndexNotExistsException : Exception
    {
        public TableOrIndexNotExistsException()
        { }
        public TableOrIndexNotExistsException(string message)
            : base(message) { }
        public TableOrIndexNotExistsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class AttributeNotExistsException : Exception
    {
        public AttributeNotExistsException()
        { }
        public AttributeNotExistsException(string message)
            : base(message) { }
        public AttributeNotExistsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class NumberOfAttributesNotMatchsException : Exception
    {
        public NumberOfAttributesNotMatchsException()
        { }
        public NumberOfAttributesNotMatchsException(string message)
            : base(message) { }
        public NumberOfAttributesNotMatchsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class TypeOfAttributeNotMatchsException : Exception
    {
        public TypeOfAttributeNotMatchsException()
        { }
        public TypeOfAttributeNotMatchsException(string message)
            : base(message) { }
        public TypeOfAttributeNotMatchsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public interface ICatalogManager
    {
        // try to save the create statement into file as a few schema records
        // if succeeded, return true. Vice versa
        bool TryCreateStatement(CreateStatement createStatement, int rootPage);
        // save the create statement into file as a few schema records
        void CreateStatement(CreateStatement createStatement, int rootPage);
        // try to remove some schema records from file
        // if succeeded, return true. Vice versa
        bool TryDropStatement(DropStatement dropStatement);
        // remove some schema records from file
        void DropStatement(DropStatement dropStatement);
        // update the root page of a table or an index
        // if succeeded, return true. Vice versa
        bool TryUpdateSchemaRecord(string name, int rootPage);
        // according to the table name requested, return corresponding schema record
        SchemaRecord GetTableSchemaRecord(string tableName);
        // according to the table name requested, return the all index schema records that is associated to the table
        List<SchemaRecord> GetIndicesSchemaRecord(string tableName);
        // according to the index name requested, return corresponding schema record
        SchemaRecord GetIndexSchemaRecord(string indexName);
        // check validation of the statement
        bool IsValid(IStatement statement);
        void CheckValidation(IStatement statement);
    }
}
