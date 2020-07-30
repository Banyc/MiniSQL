using System;
using System.Collections.Generic;
using MiniSQL.CatalogManager.Controllers;
using MiniSQL.Library.Exceptions;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.CatalogManager.Controllers
{
    //define all the functions 
    public class Catalog : ICatalogManager
    {
        private readonly string _databaseName;

        public Catalog(string databaseName)
        {
            _databaseName = databaseName;
        }

        // try to save the create statement into file as a few schema records
        // if succeeded, return true. Vice versa
        public bool TryCreateStatement(CreateStatement createStatement, int rootPage)
        {
            try
            {
                CreateStatement(createStatement, rootPage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void CreateStatement(CreateStatement createStatement, int rootPage)
        {
            //create table
            if (createStatement.CreateType == CreateType.Table)
            {
                Catalog_table a = new Catalog_table(_databaseName);//load the tables
                a.CreateStatementForTable(createStatement, rootPage);
            }
            //create index
            else
            {
                Catalog_index b = new Catalog_index(_databaseName);//load the index
                b.CreateStatementForIndex(createStatement, rootPage);
            }
        }
        // try to remove some schema records from file
        // if succeeded, return true. Vice versa
        public bool TryDropStatement(DropStatement dropStatement)
        {
            try
            {
                DropStatement(dropStatement);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void DropStatement(DropStatement dropStatement)
        {
            if (dropStatement.TargetType == DropTarget.Table)
            {
                Catalog_table a = new Catalog_table(_databaseName);//load the table
                a.DropStatementForTable(dropStatement);
            }
            else
            {
                Catalog_index b = new Catalog_index(_databaseName);//load the index
                b.DropStatementForIndex(dropStatement);
            }
        }

        // update the root page of a table or an index
        // if succeeded, return true. Vice versa
        public bool TryUpdateSchemaRecord(string name, int rootPage)
        {
            //load both table and index
            Catalog_table a = new Catalog_table(_databaseName);
            Catalog_index b = new Catalog_index(_databaseName);

            //if it's the name of a table
            if (a.If_in(name))
            {
                return a.Update(name, rootPage);
            }

            //if it's the name of an index
            else if (b.If_in(name))
            {
                return b.Update(name, rootPage);
            }

            //if it's neither index nor table
            else
            {
                return false;
            }

        }
        // according to the table name required, return corresponding schema record
        public SchemaRecord GetTableSchemaRecord(string tableName)
        {
            Catalog_table a = new Catalog_table(_databaseName);
            return a.GetTableSchemaRecord(tableName);
        }
        // according to the table name required, return the all index schema records that is associated to the table
        public List<SchemaRecord> GetIndicesSchemaRecord(string tableName)
        {
            Catalog_index b = new Catalog_index(_databaseName);
            return b.GetIndicesSchemaRecord(tableName);
        }
        // according to the index name required, return corresponding schema record
        public SchemaRecord GetIndexSchemaRecord(string indexName)
        {
            Catalog_index b = new Catalog_index(_databaseName);
            return b.GetIndexSchemaRecord(indexName);
        }

        // check validation of the statement
        public bool IsValid(IStatement statement)
        {
            try
            {
                CheckValidation(statement);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        // check validation of the statement
        public void CheckValidation(IStatement statement)
        {
            //check validation of createStatement
            if (statement.Type == StatementType.CreateStatement)
            {
                CreateStatement x = (CreateStatement)statement;
                //to create a table
                if (x.CreateType == CreateType.Table)
                {
                    // primary key not exists
                    if (x.PrimaryKey == null || x.PrimaryKey == "")
                        throw new KeyNotExistsException($"Table \"{x.TableName}\" does not have a primary key");
                    //to check whether the table has been created before
                    Catalog_table a = new Catalog_table(_databaseName);
                    a.AssertNotExist(x.TableName);
                }
                //to create an index
                else
                {
                    // index key not exists
                    if (x.AttributeName == null || x.AttributeName == "")
                        throw new KeyNotExistsException($"Index \"{x.IndexName}\" does not have a index key");
                    Catalog_table a = new Catalog_table(_databaseName);
                    Catalog_index b = new Catalog_index(_databaseName);
                    //to check whether the table exists
                    bool condition1 = a.If_in(x.TableName);
                    //to check whether the index has been created before
                    bool condition2 = !b.If_in(x.IndexName);
                    //to check whether the attribute is in the attribute list of the table
                    bool condition3 = a.return_table(x.TableName).Has_attribute(x.AttributeName);
                    if (!condition1)
                        throw new TableOrIndexNotExistsException($"Table \"\"{x.TableName}\"\" not exists");
                    if (!condition2)
                        throw new TableOrIndexAlreadyExistsException($"Index \"\"{x.IndexName}\"\" not exists");
                    if (!condition3)
                        throw new AttributeNotExistsException($"Attribute \"\"{x.AttributeName}\"\" not exists in table \"\"{x.TableName}\"\"");
                }
            }
            //check validation of a drop statement
            else if (statement.Type == StatementType.DropStatement)
            {
                DropStatement x = (DropStatement)statement;
                Catalog_table a = new Catalog_table(_databaseName);
                Catalog_index b = new Catalog_index(_databaseName);
                //to drop a table,we need to check whether the table exists
                if (x.TargetType == DropTarget.Table)
                {
                    a.AssertExist(x.TableName);
                }
                //to drop a index,we need to check whether the index exists
                else
                {
                    if (x.TableName != "")
                    {
                        b.AssertExist(x.IndexName);
                        
                        if (b.Of_table(x.IndexName) != x.TableName)
                            throw new AttributeNotExistsException($"Index \"{x.IndexName}\" is not associated with table \"{x.TableName}\"");
                    }
                    else
                        b.AssertExist(x.IndexName);
                }
            }

            //check validation of a select statement
            else if (statement.Type == StatementType.SelectStatement)
            {
                SelectStatement x = (SelectStatement)statement;
                //check whether the table is in the tables catalog
                Catalog_table a = new Catalog_table(_databaseName);
                a.AssertExist(x.FromTable);
                if (x.Condition == null)
                {
                    return;
                }
                else if (x.Condition.AttributeName == "")
                {

                    if (x.Condition.SimpleMinterms.Count == 0)
                    {
                        //if the ands is empty and attribute name is emply, the statement means select * from a table
                        return;
                    }
                    else
                    {
                        //for each attribute in the egression list(named 'ands')
                        //check whether the attribute is in the attribute list of this table
                        foreach (KeyValuePair<string, Expression> expression_piece in x.Condition.SimpleMinterms)
                        {
                            if (!a.return_table(x.FromTable).Has_attribute(expression_piece.Key))
                            {
                                throw new AttributeNotExistsException($"Attribute \"{expression_piece.Key}\" not exists in table \"{x.FromTable}\"");
                            }
                        }
                    }

                }
                else
                {
                    //check whether the only attribute is one of the table's attributes
                    if (!a.return_table(x.FromTable).Has_attribute(x.Condition.AttributeName))
                    {
                        throw new AttributeNotExistsException($"Attribute \"{x.Condition.AttributeName}\" not exists in table \"{x.FromTable}\"");
                    }

                }
                return;
            }
            //check validation of a delete statement
            else if (statement.Type == StatementType.DeleteStatement)
            {

                DeleteStatement x = (DeleteStatement)statement;
                //check whether the table is in the tables catalog
                Catalog_table a = new Catalog_table(_databaseName);
                a.AssertExist(x.TableName);
                if (x.Condition == null)
                {
                    return;
                }
                else if (x.Condition.AttributeName == "")
                {

                    if (x.Condition.SimpleMinterms.Count == 0)
                    {
                        //if the ands is empty and attribute name is emply, the statement means select * from a table
                        return;
                    }
                    else
                    {
                        //for each attribute in the epression list(named 'ands')
                        //check whether the attribute is in the attribute list of this table
                        foreach (KeyValuePair<string, Expression> expression_piece in x.Condition.SimpleMinterms)
                        {
                            if (!a.return_table(x.TableName).Has_attribute(expression_piece.Key))
                            {
                                throw new AttributeNotExistsException($"Attribute \"{expression_piece.Key}\" not exists in table \"{x.TableName}\"");
                            }
                        }
                    }

                }
                else
                {
                    //check whether the only attribute is one of the table's attributes
                    if (!a.return_table(x.TableName).Has_attribute(x.Condition.AttributeName))
                    {
                        throw new AttributeNotExistsException($"Attribute \"{x.Condition.AttributeName}\" not exists in table \"{x.TableName}\"");
                    }

                }
                return;
            }
            //check validation of an insert statement
            else if (statement.Type == StatementType.InsertStatement)
            {
                InsertStatement x = (InsertStatement)statement;

                //check whether the table is in the table list
                Catalog_table a = new Catalog_table(_databaseName);
                a.AssertExist(x.TableName);
                //check if the number of the attributes perfectly match the number of the values
                if (x.Values.Count != a.return_table(x.TableName).attribute_list.Count)
                {
                    throw new NumberOfAttributesNotMatchsException($"Number of attributes not matchs. Expected: \"{a.return_table(x.TableName).attribute_list.Count}\"; actual: \"{x.Values.Count}\"");
                }
                //check whether the type of the inserted data well suits the data definition of each attribute 
                for (int i = 0; i < x.Values.Count; i++)
                {
                    if (a.return_table(x.TableName).attribute_list[i].type != x.Values[i].Type)
                    {
                        throw new TypeOfAttributeNotMatchsException($"Type for attribute \"{a.return_table(x.TableName).attribute_list[i].attribute_name}\" not matches. Expected: \"{a.return_table(x.TableName).attribute_list[i].type}\"; actual: \"{x.Values[i].Type}\"");
                    }
                }
                //if all data type suit, return true
                return;
            }
            //check validation of a quit statement
            else if (statement.Type == StatementType.QuitStatement)
            {
                return;
            }
            //check validation of an exec file statement
            else
            {
                return;
            }
        }

        public List<SchemaRecord> GetTablesSchemaRecord()
        {
            Catalog_table a = new Catalog_table(_databaseName);
            return a.GetTablesSchemaRecord();
        }
    }
}
