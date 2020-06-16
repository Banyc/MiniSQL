using System.Collections.Generic;
using MiniSQL.CatalogManager.Controllers;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.CatalogManager
{
    //define all the functions 
    public class Catalog : ICatalogManager
    {
        // try to save the create statement into file as a few schema records
        // if succeeded, return true. Vice versa
        public bool TryCreateStatement(CreateStatement createStatement, int rootPage)
        {
            //create table
            if (createStatement.CreateType == CreateType.Table)
            {
                Catalog_table a = new Catalog_table();//load the tables
                return a.TryCreateStatementForTable(createStatement, rootPage);
            }
            //create index
            else
            {
                Catalog_index b = new Catalog_index();//load the index
                return b.TryCreateStatementForIndex(createStatement, rootPage);
            }
        }
        // try to remove some schema records from file
        // if succeeded, return true. Vice versa
        public bool TryDropStatement(DropStatement dropStatement)
        {
            if (dropStatement.TargetType == DropTarget.Table)
            {
                Catalog_table a = new Catalog_table();//load the table
                return a.TryDropStatementForTable(dropStatement);
            }
            else
            {
                Catalog_index b = new Catalog_index();//load the index
                return b.TryDropStatementForIndex(dropStatement);
            }
        }

        // update the root page of a table or an index
        // if succeeded, return true. Vice versa
        public bool TryUpdateSchemaRecord(string name, int rootPage)
        {
            //load both table and index
            Catalog_table a = new Catalog_table();
            Catalog_index b = new Catalog_index();

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
            Catalog_table a = new Catalog_table();
            return a.GetTableSchemaRecord(tableName);
        }
        // according to the table name required, return the all index schema records that is associated to the table
        public List<SchemaRecord> GetIndicesSchemaRecord(string tableName)
        {
            Catalog_index b = new Catalog_index();
            return b.GetIndicesSchemaRecord(tableName);
        }
        // according to the index name required, return corresponding schema record
        public SchemaRecord GetIndexSchemaRecord(string indexName)
        {
            Catalog_index b = new Catalog_index();
            return b.GetIndexSchemaRecord(indexName);
        }

        // check validation of the statement
        public bool IsValid(IStatement statement)
        {
            //check validation of createStatement
            if (statement.Type == StatementType.CreateStatement)
            {
                CreateStatement x = (CreateStatement)statement;
                //to create a table
                if (x.CreateType == CreateType.Table)
                {
                    //to check whether the table has been created before
                    Catalog_table a = new Catalog_table();
                    return !a.If_in(x.TableName);
                }
                //to create an index
                else
                {
                    Catalog_table a = new Catalog_table();
                    Catalog_index b = new Catalog_index();
                    //to check whether the table exists
                    bool condition1 = a.If_in(x.TableName);
                    //to check whether the index has been created before
                    bool condition2 = !b.If_in(x.IndexName);
                    //to check whether the attribute is in the attribute list of the table
                    bool condition3 = a.return_table(x.TableName).Has_attribute(x.AttributeName);
                    //return true if the statement meets all the conditions.Vice Versa
                    return condition1 && condition2 && condition3;
                }
            }
            //check validation of a drop statement
            else if (statement.Type == StatementType.DropStatement)
            {
                DropStatement x = (DropStatement)statement;
                Catalog_table a = new Catalog_table();
                Catalog_index b = new Catalog_index();
                //to drop a table,we need to check whether the table exists
                if (x.TargetType == DropTarget.Table)
                {
                    return a.If_in(x.TableName);
                }
                //to drop a index,we need to check whether the index exists
                else
                {
                    if (x.TableName != "")
                        return b.If_in(x.IndexName) && b.Of_table(x.IndexName) == x.TableName;
                    else
                        return b.If_in(x.IndexName);
                }
            }

            //check validation of a select statement
            else if (statement.Type == StatementType.SelectStatement)
            {
                SelectStatement x = (SelectStatement)statement;
                //check whether the table is in the tables catalog
                Catalog_table a = new Catalog_table();
                if (!a.If_in(x.FromTable))
                {
                    return false;
                }
                if (x.Condition == null)
                {
                    return true;
                }
                else if (x.Condition.AttributeName == "")
                {

                    if (x.Condition.Ands.Count == 0)
                    {
                        //if the ands is empty and attribute name is emply, the statement means select * from a table
                        return true;
                    }
                    else
                    {
                        //for each attribute in the epression list(named 'ands')
                        //check whether the attribute is in the attribute list of this table
                        foreach (KeyValuePair<string, Expression> expression_piece in x.Condition.Ands)
                        {
                            if (!a.return_table(x.FromTable).Has_attribute(expression_piece.Key))
                            {
                                return false;
                            }
                        }
                    }

                }
                else
                {
                    //check whether the only attribute is one of the table's attributes
                    if (!a.return_table(x.FromTable).Has_attribute(x.Condition.AttributeName))
                    {
                        return false;
                    }

                }


                return true;
            }
            //check validation of a delete statement
            else if (statement.Type == StatementType.DeleteStatement)
            {

                DeleteStatement x = (DeleteStatement)statement;
                //check whether the table is in the tables catalog
                Catalog_table a = new Catalog_table();
                if (!a.If_in(x.TableName))
                {
                    return false;
                }
                if (x.Condition == null)
                {
                    return true;
                }
                else if (x.Condition.AttributeName == "")
                {

                    if (x.Condition.Ands.Count == 0)
                    {
                        //if the ands is empty and attribute name is emply, the statement means select * from a table
                        return true;
                    }
                    else
                    {
                        //for each attribute in the epression list(named 'ands')
                        //check whether the attribute is in the attribute list of this table
                        foreach (KeyValuePair<string, Expression> expression_piece in x.Condition.Ands)
                        {
                            if (!a.return_table(x.TableName).Has_attribute(expression_piece.Key))
                            {
                                return false;
                            }
                        }
                    }

                }
                else
                {
                    //check whether the only attribute is one of the table's attributes
                    if (!a.return_table(x.TableName).Has_attribute(x.Condition.AttributeName))
                    {
                        return false;
                    }

                }


                return true;
            }
            //check validation of an insert statement
            else if (statement.Type == StatementType.InsertStatement)
            {
                InsertStatement x = (InsertStatement)statement;

                //check whether the table is in the table list
                Catalog_table a = new Catalog_table();
                if (!a.If_in(x.TableName))
                {
                    return false;
                }
                //check if the number of the attributes perfectly match the number of the values
                if (x.Values.Count != a.return_table(x.TableName).attribute_list.Count)
                {
                    return false;
                }
                //check whether the type of the inserted data well suits the data definition of each attribute 
                for (int i = 0; i < x.Values.Count; i++)
                {
                    if (a.return_table(x.TableName).attribute_list[i].type != x.Values[i].Type)
                    {
                        return false;
                    }
                }
                //if all data type suit, return true
                return true;

            }
            //check validation of a quit statement
            else if (statement.Type == StatementType.QuitStatement)
            {
                return true;
            }

            //check validation of an exec file statement
            else
            {
                return true;
            }
        }
    }
}
