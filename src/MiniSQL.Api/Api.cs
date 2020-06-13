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
        // private readonly IIndexManager _indexManager;
        private readonly IRecordManager _recordManager;

        // public Api(IInterpreter interpreter, ICatalogManager catalogManager, IIndexManager indexManager, IRecordManager recordManager)
        public Api(IInterpreter interpreter, ICatalogManager catalogManager, IRecordManager recordManager)
        {
            _catalogManager = catalogManager;
            // _indexManager = indexManager;
            _interpreter = interpreter;
            _recordManager = recordManager;
        }

        public List<SelectResult> Query(string sql)
        {
            List<SelectResult> selectResults = new List<SelectResult>();
            Query query = Parse(sql);
            foreach (IStatement statement in query.StatementList)
            {
                SelectResult selectResult = HandleStatement(statement);
                if (selectResult != null)
                {
                    selectResults.Add(selectResult);
                }
            }
            return selectResults;
        }

        private SelectResult HandleStatement(IStatement statement)
        {
            SelectResult selectResult = null;
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
                    selectResult = HandleSelectStatement((SelectStatement)statement);
                    break;
                case StatementType.ExecFileStatement:
                    throw new Exception("Impossible reach");
            }
            return selectResult;
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
                    _catalogManager.TryCreateStatement(statement, newTableRoot);
                    break;
                case CreateType.Index:
                    // int newIndexRoot = _indexManager.CreateIndex(statement);
                    // _catalogManager.TryCreateStatement(statement.IndexName, newIndexRoot);
                    break;
            }
        }

        // TODO: review
        // drop statement
        private void HandleStatement(DropStatement statement)
        {
            // WORK AROUND: not considering drop index
            if (statement.TargetType == DropTarget.Index)
                return;

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
                    // if (_catalogManager.TryDropStatement(statement))
                    //     _indexManager.DropIndex(schema.RootPage);
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

            // // delete index records from index trees
            // foreach (SchemaRecord indexSchema in indexSchemas)
            // {
            //     int indexOfAttribute =
            //         tableSchema.SQL.AttributeDeclarations
            //             .FindIndex(x => x.AttributeName == indexSchema.SQL.AttributeName);
            //     (int newIndexRootPage, List<AtomValue> primaryKeys) = _indexManager
            //         .DeleteIndexRecords(statement, indexOfAttribute, indexSchema.RootPage);
            // }

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

        private SelectResult HandleSelectStatement(SelectStatement statement)
        {
            // get table and indices
            if (!_catalogManager.IsValid(statement))
                throw new InvalidOperationException("invalid select statement");
            SchemaRecord tableSchema = _catalogManager.GetTableSchemaRecord(statement.FromTable);
            List<SchemaRecord> indexSchemas = _catalogManager.GetIndicesSchemaRecord(statement.FromTable);

            // TODO
            // select from index tree if possible

            // select record from table tree
            List<List<AtomValue>> rows = _recordManager.SelectRecords(statement, tableSchema.SQL.PrimaryKey, tableSchema.SQL.AttributeDeclarations, tableSchema.RootPage);
            SelectResult result = new SelectResult();
            result.Rows = rows;
            result.ColumnDeclarations = tableSchema.SQL.AttributeDeclarations;
            return result;
        }

        // TODO
        private void HandleStatement(InsertStatement statement)
        {
            if (!_catalogManager.IsValid(statement))
                throw new InvalidOperationException("invalid insert statement");
            // insert into index trees
            // TODO
            // insert into table tree
            SchemaRecord schema = _catalogManager.GetTableSchemaRecord(statement.TableName);
            // adjust inlined type in insert statement
            if (schema.SQL.AttributeDeclarations.Count != statement.Values.Count)
            {
                throw new Exception("number of columns between \"create table\" and \"insert statement\' do not match");
            }
            int i;
            for (i = 0; i < statement.Values.Count; i++)
            {
                statement.Values[i].CharLimit = schema.SQL.AttributeDeclarations[i].CharLimit;
            }
            // find out primary key from insert values
            AtomValue primaryKey = 
                statement.Values[schema.SQL.AttributeDeclarations.FindIndex(x => 
                    x.AttributeName == schema.SQL.PrimaryKey)];
            // insert
            int newRoot = _recordManager.InsertRecord(statement, primaryKey, schema.RootPage);
            _catalogManager.TryUpdateSchemaRecord(statement.TableName, newRoot);
        }

        private Query Parse(string sql)
        {
            return _interpreter.GetQuery(sql);
        }
    }
}
