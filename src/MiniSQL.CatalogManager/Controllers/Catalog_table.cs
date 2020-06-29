using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MiniSQL.CatalogManager.Models;
using MiniSQL.Library.Exceptions;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.CatalogManager.Controllers
{
    //save all the tables in the table catalog
    class Catalog_table
    {
        private readonly string _databaseName;
        public List<Models.Table> tables;

        //return the table name 'tableName'
        //warning:need to check whether the tableName is in the table catalog first
        public Models.Table return_table(string tableName)
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

        //return the schema record of the table named table name
        public List<SchemaRecord> GetTablesSchemaRecord()
        {
            List<SchemaRecord> list = new List<SchemaRecord>();
            for (int i = 0; i < tables.Count; i++)
            {
                SchemaRecord target_table = GetTableSchemaRecord(tables[i].table_name);
                list.Add(target_table);
            }
            return list;
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

        public void AssertExist(string name)
        {
            if (!If_in(name))
                throw new TableOrIndexNotExistsException($"Table \"{name}\" not exists");
        }

        public void AssertNotExist(string name)
        {
            if (If_in(name))
                throw new TableOrIndexAlreadyExistsException($"Table \"{name}\" already exists");
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
            Models.Table a = new Models.Table(createStatement, rootPage);
            tables.Add(a);
            Save_table(this.tables);
            return true;
        }
        public void CreateStatementForTable(CreateStatement createStatement, int rootPage)
        {
            if (this.If_in(createStatement.TableName))
            {
                throw new TableOrIndexAlreadyExistsException("Table Already Exists");
            }
            Models.Table a = new Models.Table(createStatement, rootPage);
            tables.Add(a);
            Save_table(this.tables);
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
            Catalog_index b = new Catalog_index(_databaseName);
            return if_find && b.DeleteIndicesOfTable(dropStatement.TableName);
        }
        public void DropStatementForTable(DropStatement dropStatement)
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
            Catalog_index b = new Catalog_index(_databaseName);
            if (if_find)
                b.DeleteIndicesOfTable(dropStatement.TableName);
            else
                throw new TableOrIndexNotExistsException($"Table {dropStatement.TableName} Not Exists");
        }

        //load the table from file and store the data into tables
        public void Load_table()
        {
            if (System.IO.File.Exists($"{_databaseName}.tables.dbcatalog"))
            {
                using (FileStream fs = new FileStream($"{_databaseName}.tables.dbcatalog", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.tables = bf.Deserialize(fs) as List<Models.Table>;
                    fs.Close();
                }
            }
            else
            {
                this.tables = new List<Models.Table>();
            }
            return;
        }

        //save data of tables into the file
        public void Save_table(List<Models.Table> tables)
        {
            using (FileStream fs = new FileStream($"{_databaseName}.tables.dbcatalog", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tables);
                fs.Close();
            }
        }

        //load the tables from the txt file 
        public Catalog_table(string databaseName)
        {
            _databaseName = databaseName;
            Load_table();
        }
    }

}
