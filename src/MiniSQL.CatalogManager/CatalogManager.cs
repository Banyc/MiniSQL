using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
                    return b.If_in(x.IndexName) && b.Of_table(x.IndexName) == x.TableName;
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

    [Serializable]
    class Attribute
    {
        public string attribute_name;
        public AttributeTypes type;//three kinds of type: float int char(with limit)
        public bool is_unique;
        public int length;//the length of string 
        public Attribute(string attribute_name, AttributeTypes type, bool is_unique, int length)
        {
            this.attribute_name = attribute_name;
            this.type = type;
            this.is_unique = is_unique;
            this.length = length;
        }
    }

    [Serializable]
    class Table
    {
        public string table_name;
        public List<Attribute> attribute_list;
        public string primary_key;
        public int root_page;
        public Table(CreateStatement createStatement, int root_page)
        {
            this.table_name = createStatement.TableName;
            this.primary_key = createStatement.PrimaryKey;
            this.root_page = root_page;
            List<Attribute> temp = new List<Attribute>();
            //to initialize every attribute of this table 
            for (int i = 0; i < createStatement.AttributeDeclarations.Count; i++)
            {
                string AttributeName = createStatement.AttributeDeclarations[i].AttributeName;
                bool IsUnique = createStatement.AttributeDeclarations[i].IsUnique;
                int Length = createStatement.AttributeDeclarations[i].CharLimit;
                AttributeTypes Type = createStatement.AttributeDeclarations[i].Type;
                Attribute a = new Attribute(AttributeName, Type, IsUnique, Length);
                temp.Add(a);
            }
            //assign the attribute list to the table
            this.attribute_list = temp;
        }
        //check whether the table has such an attribute
        public bool Has_attribute(string attribute_name)
        {
            bool flag = false;
            for (int i = 0; i < attribute_list.Count; i++)
            {
                if (attribute_list[i].attribute_name == attribute_name)
                {
                    flag = true;
                }
            }
            return flag;
        }

    }

    [Serializable]
    class Index
    {
        public string table_name { get; set; }
        public string attribute_name;
        public string index_name;
        public bool is_uniqve;
        public int root_page;
        public Index(CreateStatement createStatement, int root_page)
        {
            this.table_name = createStatement.TableName;
            this.attribute_name = createStatement.AttributeName;
            this.index_name = createStatement.IndexName;
            this.is_uniqve = createStatement.IsUnique;
            this.root_page = root_page;

        }
    }
    //save all the tables in the table catalog
    class Catalog_table
    {
        public List<Table> tables;

        //return the table name 'tableName'
        //warning:need to check whether the tableName is in the table catalog first
        public Table return_table(string tableName)
        {
            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].table_name == tableName)
                {
                    return tables[i];
                }
            }
            //useless but for syntax correctness
            return tables[tables.Count - 1];
        }

        //return the schema record of the table named table name
        public SchemaRecord GetTableSchemaRecord(string tableName)
        {
            SchemaRecord s = new SchemaRecord();
            s.Type = SchemaTypes.Table;
            s.Name = tableName;
            //since this is the table and there is no table that is associated to it
            s.AssociatedTable = " ";

            //get the target table first, then get the schema record from the table 
            Table target_table = return_table(tableName);
            s.RootPage = target_table.root_page;
            s.SQL = new CreateStatement();
            s.SQL.CreateType = CreateType.Table;
            s.SQL.TableName = tableName;

            //there is no index information for a table
            s.SQL.IndexName = " ";
            s.SQL.AttributeName = " ";

            //get every attribute declaration from the target table 

            List<AttributeDeclaration> temp = new List<AttributeDeclaration>();

            for (int i = 0; i < target_table.attribute_list.Count; i++)
            {
                AttributeDeclaration attribute = new AttributeDeclaration();
                attribute.Type = target_table.attribute_list[i].type;
                attribute.CharLimit = target_table.attribute_list[i].length;
                attribute.IsUnique = target_table.attribute_list[i].is_unique;
                attribute.AttributeName = target_table.attribute_list[i].attribute_name;
                temp.Add(attribute);
            }
            s.SQL.AttributeDeclarations = temp;

            s.SQL.PrimaryKey = target_table.primary_key;
            //Console.WriteLine("hhhh");
            return s;
        }



        //to check if there is such a table named 'name'
        //if in,then return true. Vice versa
        public bool If_in(string name)
        {
            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].table_name == name)
                {
                    return true;
                }
            }
            return false;
        }

        //update the rootPage of the table named 'name'
        //return false if not find
        public bool Update(string name, int rootPage)
        {
            bool has_find = false;
            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].table_name == name)
                {
                    tables[i].root_page = rootPage;
                    has_find = true;
                    break;
                }
            }
            Save_table(tables);
            return has_find;
        }


        //try to create one table and save the result into the file
        //if succeed, return true
        //if the table is already created, return false
        public bool TryCreateStatementForTable(CreateStatement createStatement, int rootPage)
        {
            if (this.If_in(createStatement.TableName))
            {
                return false;
            }
            Table a = new Table(createStatement, rootPage);
            tables.Add(a);
            Save_table(this.tables);
            return true;
        }


        //try to drop one table and save the result into the file
        //if succeed, return true
        //if can't find table, return false
        public bool TryDropStatementForTable(DropStatement dropStatement)
        {
            bool if_find = false;
            if (this.If_in(dropStatement.TableName))
                for (int i = 0; i < this.tables.Count;)
                {
                    if (this.tables[i].table_name == dropStatement.TableName)
                    {
                        tables.RemoveAt(i);
                        Save_table(tables);
                        if_find = true;
                    }
                    else
                        i++;
                }
            //delete all the indices that are related to the deleted table 
            Catalog_index b = new Catalog_index();
            return if_find && b.DeleteIndicesOfTable(dropStatement.TableName);


        }

        //load the table from file and store the data into tables
        public void Load_table()
        {
            if (System.IO.File.Exists("tables.txt"))
            {
                using (FileStream fs = new FileStream("tables.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.tables = bf.Deserialize(fs) as List<Table>;
                    fs.Close();
                }
            }
            else
            {
                this.tables = new List<Table>();
            }
            return;
        }

        //save data of tables into the file
        public void Save_table(List<Table> tables)
        {
            using (FileStream fs = new FileStream("tables.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tables);
                fs.Close();
            }
        }

        //load the tables from the txt file 
        public Catalog_table()
        {
            Load_table();
        }
    }


    class Catalog_index
    {
        //store all the indices
        List<Index> index;

        //return the shcema record of the index named 'index_name'
        //used after checking 'If_in',to make sure that the index does exist
        public SchemaRecord GetIndexSchemaRecord(string index_name)
        {
            SchemaRecord record = new SchemaRecord();
            for (int i = 0; i < index.Count; i++)
            {
                if (index[i].index_name == index_name)
                {
                    record.AssociatedTable = index[i].table_name;
                    record.Name = index_name;
                    record.RootPage = index[i].root_page;
                    record.Type = SchemaTypes.Index;
                    CreateStatement temp = new CreateStatement();
                    temp.CreateType = CreateType.Index;
                    temp.TableName = index[i].table_name;
                    temp.IsUnique = index[i].is_uniqve;
                    temp.IndexName = index[i].index_name;
                    temp.AttributeName = index[i].table_name;
                    record.SQL = temp;
                }
            }
            return record;
        }

        //Only used when try to drop a table 
        //Delete all the indices that related to the table named tableName from the index list
        public bool DeleteIndicesOfTable(string tableName)
        {
            for (int i = 0; i < index.Count;)
            {
                if (index[i].table_name == tableName)
                {
                    index.RemoveAt(i);


                }
                else
                    i++;
            }
            Save_index(index);
            return true;
        }

        //return all the indices of the table named 'tableName'
        public List<SchemaRecord> GetIndicesSchemaRecord(string tableName)
        {
            List<SchemaRecord> list = new List<SchemaRecord>();
            for (int i = 0; i < index.Count; i++)
            {
                if (index[i].table_name == tableName)
                {
                    SchemaRecord target_index = GetIndexSchemaRecord(index[i].index_name);
                    list.Add(target_index);
                }
            }
            return list;
        }





        //to check if there is such a index named 'name'
        //if in,then return true. Vice versa
        public bool If_in(string name)
        {
            for (int i = 0; i < index.Count; i++)
            {
                if (index[i].index_name == name)
                {
                    return true;
                }
            }
            return false;
        }



        //update the rootPage of the index named 'name'
        public bool Update(string name, int rootPage)
        {
            bool has_find = false;
            for (int i = 0; i < index.Count; i++)
            {
                if (index[i].index_name == name)
                {
                    index[i].root_page = rootPage;
                    has_find = true;
                    break;
                }
            }
            Save_index(index);
            return has_find;
        }


        public bool TryCreateStatementForIndex(CreateStatement createStatement, int rootPage)
        {
            //if the index already exists,then return false
            if (this.If_in(createStatement.IndexName))
            {
                return false;
            }
            Index b = new Index(createStatement, rootPage);
            index.Add(b);
            Save_index(this.index);
            return true;
        }



        public bool TryDropStatementForIndex(DropStatement dropStatement)
        {
            //if the index hasn't been created before, return false
            if (!this.If_in(dropStatement.IndexName))
            {
                return false;
            }
            for (int i = 0; i < index.Count;)
            {
                if (index[i].index_name == dropStatement.IndexName)
                {
                    index.RemoveAt(i);
                }
                else
                    i++;
            }
            return true;
        }

        public string Of_table(string Indexname)
        {
            for (int i = 0; i < index.Count; i++)
                if (index[i].index_name == Indexname)
                    return index[i].table_name;
            return "";
        }
        public void Load_index()
        {
            if (System.IO.File.Exists("index.txt"))
            {
                using (FileStream fs = new FileStream("index.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.index = bf.Deserialize(fs) as List<Index>;
                    fs.Close();
                }
            }
            else
            {
                this.index = new List<Index>();
            }
            return;

        }

        public void Save_index(List<Index> index)
        {
            using (FileStream fs = new FileStream("index.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, index);
                fs.Close();
            }
        }
        public Catalog_index()
        {
            Load_index();//get table list from the file
        }


    }
}