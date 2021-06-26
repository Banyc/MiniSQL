using System.Collections.Generic;
using MiniSQL.Library.Models;
using MiniSQL.Library.Interfaces;
using System;
using System.Linq;
using MiniSQL.Library.Exceptions;

namespace MiniSQL.Api.Controllers
{
    public class DatabaseController : IDatabaseController
    {

        private readonly IInterpreter _interpreter;
        private readonly ICatalogManager _catalogManager;
        private readonly IRecordManager _recordManager;

        public DatabaseController(IInterpreter interpreter, ICatalogManager catalogManager, IRecordManager recordManager)
        {
            _catalogManager = catalogManager;
            _interpreter = interpreter;
            _recordManager = recordManager;
        }

        public List<SelectResult> Query(string sql)
        {
            List<SelectResult> selectResults = new List<SelectResult>();
            Query query;
            try
            {
                query = Parse(sql);
            }
            catch (Exception ex)
            {
                throw new StatementPreCheckException(ex.Message, ex.InnerException);
            }
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
                case StatementType.ShowStatement:
                    selectResult = HandleSelectStatement((ShowStatement)statement);
                    break;
                case StatementType.ExecFileStatement:
                    throw new Exception("Impossible reach");
            }
            return selectResult;
        }
        
        // create statement
        private void HandleStatement(CreateStatement statement)
        {
            _catalogManager.CheckValidation(statement);
            switch (statement.CreateType)
            {
                case CreateType.Table:
                    int newTableRoot = _recordManager.CreateTable();
                    _catalogManager.CreateStatement(statement, newTableRoot);
                    break;
                case CreateType.Index:
                    SchemaRecord tableSchema = _catalogManager.GetTableSchemaRecord(statement.TableName);
                    int newIndexRoot = _recordManager.CreateIndex(tableSchema.RootPage, statement.AttributeName, tableSchema.SQL.AttributeDeclarations);
                    _catalogManager.CreateStatement(statement, newIndexRoot);
                    break;
            }
        }

        // drop statement
        private void HandleStatement(DropStatement statement)
        {
            _catalogManager.CheckValidation(statement);
            SchemaRecord schema;
            switch (statement.TargetType)
            {
                case DropTarget.Table:
                    schema = _catalogManager.GetTableSchemaRecord(statement.TableName);
                    List<SchemaRecord> indices = _catalogManager.GetIndicesSchemaRecord(statement.TableName);
                    // drop table
                    _catalogManager.DropStatement(statement);
                    _recordManager.DropTable(schema.RootPage);
                    // drop index trees
                    foreach (SchemaRecord index in indices)
                    {
                        _recordManager.DropTable(index.RootPage);
                    }
                    break;
                case DropTarget.Index:
                    schema = _catalogManager.GetIndexSchemaRecord(statement.IndexName);
                    _catalogManager.DropStatement(statement);
                    _recordManager.DropTable(schema.RootPage);
                    break;
            }
        }

        // delete statement
        // NOTICE: relative index trees will NOT change accordingly
        private void HandleStatement(DeleteStatement statement)
        {
            // get table and indices
            _catalogManager.CheckValidation(statement);
            SchemaRecord tableSchema = _catalogManager.GetTableSchemaRecord(statement.TableName);
            List<SchemaRecord> indexSchemas = _catalogManager.GetIndicesSchemaRecord(statement.TableName);

            // TODO
            // delete index records from index trees
            // __problem__:
            // attribute names := (priKey, a, b, c)
            // condition := b < 3 and c > 5
            // fun facts: b and c both have index trees
            // issue: to delete the records satisfying the condition above

            // delete record from table tree
            int newTableRootPage = _recordManager.DeleteRecords(statement.Condition, tableSchema.SQL.PrimaryKey, tableSchema.SQL.AttributeDeclarations, tableSchema.RootPage);
            _catalogManager.TryUpdateSchemaRecord(statement.TableName, newTableRootPage);
        }

        private SelectResult HandleSelectStatement(ShowStatement statement)
        {
            List<SchemaRecord> tableSchemas = _catalogManager.GetTablesSchemaRecord();
            SelectResult result = new SelectResult();
            result.ColumnDeclarations = new List<AttributeDeclaration>() { new AttributeDeclaration() {AttributeName = "Table", Type = AttributeTypes.Char, CharLimit = 80}};
            result.Rows = new List<List<AtomValue>>();
            foreach (SchemaRecord tableSchema in tableSchemas)
            {
                List<AtomValue> row = new List<AtomValue>();
                AtomValue col = new AtomValue();
                col.Type = AttributeTypes.Char;
                col.CharLimit = 80;
                col.StringValue = tableSchema.Name;
                row.Add(col);
                result.Rows.Add(row);
            }
            return result;
        }

        private SelectResult HandleSelectStatement(SelectStatement statement)
        {
            bool isIndexTreeAvailable = false;
            // get table and indices
            _catalogManager.CheckValidation(statement);
            SchemaRecord tableSchema = _catalogManager.GetTableSchemaRecord(statement.FromTable);
            List<SchemaRecord> indexSchemas = _catalogManager.GetIndicesSchemaRecord(statement.FromTable);

            // select from index tree if possible
            AtomValue primaryKey = null;
            if (statement.Condition != null)
            {
                foreach (SchemaRecord indexSchema in indexSchemas)
                {
                    // if there has a condition `=` on indexed column
                    if (statement.Condition.SimpleMinterms.ContainsKey(indexSchema.SQL.AttributeName)
                        && statement.Condition.SimpleMinterms[indexSchema.SQL.AttributeName].Operator == Operator.Equal)
                    {
                        isIndexTreeAvailable = true;
                        // find out the primary key
                        List<AtomValue> wrappedPrimaryKey = _recordManager.SelectRecord(statement.Condition.SimpleMinterms[indexSchema.SQL.AttributeName].RightOperand.ConcreteValue, indexSchema.RootPage);
                        primaryKey = wrappedPrimaryKey?[1];
                        break;
                    }
                }
            }
            SelectResult result = new SelectResult();
            result.ColumnDeclarations = tableSchema.SQL.AttributeDeclarations;
            // index tree is not available
            if (!isIndexTreeAvailable)
            {
                // select records from table tree
                List<List<AtomValue>> rows = _recordManager.SelectRecords(statement, tableSchema.SQL.PrimaryKey, tableSchema.SQL.AttributeDeclarations, tableSchema.RootPage);
                result.Rows = rows;
            }
            // index tree is available
            else
            {
                // select one record from table tree
                List<List<AtomValue>> rows = new List<List<AtomValue>>();
                if (!object.ReferenceEquals(primaryKey, null))
                {
                    List<AtomValue> recordFromTable = _recordManager.SelectRecord(primaryKey, tableSchema.RootPage);
                    rows.Add(recordFromTable);
                    result.Rows = rows;
                }
                else  // the primary key is null.
                // if primaryKey is null, 
                // it means that from the index tree it could not find the primary key.
                // in order words, there is not a row existing in the table tree satisfying the condition.
                // thus, no need to visit the table tree
                {
                    // assign an empty list
                    result.Rows = new List<List<AtomValue>>();
                }
            }
            return result;
        }

        private void HandleStatement(InsertStatement statement)
        {
            _catalogManager.CheckValidation(statement);
            // get table schema
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
            // insert into index trees
            List<SchemaRecord> indexSchemas = _catalogManager.GetIndicesSchemaRecord(statement.TableName);
            foreach (SchemaRecord indexSchema in indexSchemas)
            {
                // find indexed value from insert values
                AtomValue indexedValue =
                    statement.Values[schema.SQL.AttributeDeclarations.FindIndex(x =>
                        x.AttributeName == indexSchema.SQL.AttributeName)];
                // wrap up indexed value and primary key
                List<AtomValue> indexPrimaryKeyPair = new List<AtomValue>() { indexedValue, primaryKey };
                // insert into index trees
                int newIndexRoot = _recordManager.InsertRecord(indexPrimaryKeyPair, indexedValue, indexSchema.RootPage);
                _catalogManager.TryUpdateSchemaRecord(indexSchema.Name, newIndexRoot);
            }
            // insert into table tree
            int newRoot = _recordManager.InsertRecord(statement.Values, primaryKey, schema.RootPage);
            _catalogManager.TryUpdateSchemaRecord(statement.TableName, newRoot);
        }

        private Query Parse(string sql)
        {
            return _interpreter.GetQuery(sql);
        }
    }
}
