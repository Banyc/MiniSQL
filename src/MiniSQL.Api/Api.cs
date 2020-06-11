using System.Collections.Generic;
using MiniSQL.Library.Models;
using MiniSQL.Library.Interfaces;
using System;
using System.Linq;

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
                HandleStatement(statement);
            }

            throw new System.NotImplementedException();
        }

        private void HandleStatement(IStatement statement)
        {
            switch (statement.Type)
            {
                case StatementType.CreateStatement:
                    HandleStatement((CreateStatement)statement);
                    break;
                case StatementType.DropStatement:
                    HandleStatement((DropStatement)statement);
                    break;
                case StatementType.DeleteStatement:
                    HandleStatement((DeleteStatement)statement);
                    break;
                case StatementType.InsertStatement:
                    HandleStatement((InsertStatement)statement);
                    break;
                case StatementType.SelectStatement:
                    HandleStatement((SelectStatement)statement);
                    break;
                case StatementType.ExecFileStatement:
                    throw new Exception("Impossible reach");
            }
        }
        // TODO: ignore non-unique create index request
        // TODO: review code
        // create statement
        private void HandleStatement(CreateStatement statement)
        {
            if (!_catalogManager.IsValid(statement))
                throw new InvalidOperationException("invalid create statement");
            switch (statement.CreateType)
            {
                case CreateType.Table:
                    int newTableRoot = _recordManager.CreateTable(statement);
                    _catalogManager.TryUpdateSchemaRecord(statement.TableName, newTableRoot);
                    break;
                case CreateType.Index:
                    int newIndexRoot = _indexManager.CreateIndex(statement);
                    _catalogManager.TryUpdateSchemaRecord(statement.IndexName, newIndexRoot);
                    break;
            }
        }

        // TODO: review
        // drop statement
        private void HandleStatement(DropStatement statement)
        {
            if (!_catalogManager.IsValid(statement))
                throw new InvalidOperationException("invalid drop statement");
            SchemaRecord schema;
            switch (statement.TargetType)
            {
                case DropTarget.Table:
                    schema = _catalogManager.GetTableSchemaRecord(statement.TableName);
                    // drop index trees first
                    // TODO
                    // drop table
                    if (_catalogManager.TryDropStatement(statement))
                        _recordManager.DropTable(schema.RootPage);
                    break;
                case DropTarget.Index:
                    schema = _catalogManager.GetIndexSchemaRecord(statement.IndexName);
                    if (_catalogManager.TryDropStatement(statement))
                        _indexManager.DropIndex(schema.RootPage);
                    break;
            }

        }

        // TODO
        // delete statement
        private void HandleStatement(DeleteStatement statement)
        {
            // get table and indices
            if (!_catalogManager.IsValid(statement))
                throw new InvalidOperationException("invalid delete statement");
            SchemaRecord tableSchema = _catalogManager.GetTableSchemaRecord(statement.TableName);
            List<SchemaRecord> indexSchemas = _catalogManager.GetIndicesSchemaRecord(statement.TableName);

            // delete index records from index trees
            foreach (SchemaRecord indexSchema in indexSchemas)
            {
                int indexOfAttribute =
                    tableSchema.SQL.AttributeDeclarations
                        .FindIndex(x => x.AttributeName == indexSchema.SQL.AttributeName);
                (int newIndexRootPage, List<AtomValue> primaryKeys) = _indexManager
                    .DeleteIndexRecords(statement, indexOfAttribute, indexSchema.RootPage);
            }

            // TODO
            // __problem__:
            // attribute names := (priKey, a, b, c)
            // condition := b < 3 and c > 5
            // fun facts: b and c both have index trees
            // issue: to delete the records satisfying the condition above

            // delete record from table tree
            int newTableRootPage = _recordManager.DeleteRecords(statement, tableSchema.SQL.PrimaryKey, tableSchema.SQL.AttributeDeclarations, tableSchema.RootPage);
            _catalogManager.TryUpdateSchemaRecord(statement.TableName, newTableRootPage);
        }

        // TODO
        private void HandleStatement(InsertStatement statement)
        {
            if (!_catalogManager.IsValid(statement))
                throw new InvalidOperationException("invalid create statement");
            // insert into index trees
            // TODO
            // insert into table tree
            SchemaRecord schema = _catalogManager.GetTableSchemaRecord(statement.TableName);
            AtomValue primaryKey = 
                statement.Values[schema.SQL.AttributeDeclarations.FindIndex(x => 
                    x.AttributeName == schema.SQL.PrimaryKey)];
            _recordManager.InsertRecord(statement, primaryKey, schema.RootPage);
        }

        private Query Parse(string sql)
        {
            return _interpreter.GetQuery(sql);
        }
    }
}
