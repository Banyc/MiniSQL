using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MiniSQL.CatalogManager.Models;
using MiniSQL.Library.Exceptions;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.CatalogManager.Controllers
{

    class Catalog_index
    {
        private readonly string _databaseName;
        //store all the indices
        List<Models.Index> index;

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
                    temp.AttributeName = index[i].attribute_name;
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

        public void AssertExist(string name)
        {
            if (!If_in(name))
                throw new TableOrIndexNotExistsException($"Index \"{name}\" not exists");
        }

        public void AssertNotExist(string name)
        {
            if (If_in(name))
                throw new TableOrIndexAlreadyExistsException($"Index \"{name}\" already exists");
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
            Models.Index b = new Models.Index(createStatement, rootPage);
            index.Add(b);
            Save_index(this.index);
            return true;
        }
        public void CreateStatementForIndex(CreateStatement createStatement, int rootPage)
        {
            //if the index already exists,then return false
            if (this.If_in(createStatement.IndexName))
            {
                throw new TableOrIndexAlreadyExistsException("Index Already Exists");
            }
            Models.Index b = new Models.Index(createStatement, rootPage);
            index.Add(b);
            Save_index(this.index);
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
                    break;
                }
                else
                    i++;
            }
            Save_index(this.index);
            return true;
        }
        public void DropStatementForIndex(DropStatement dropStatement)
        {
            //if the index hasn't been created before, return false
            if (!this.If_in(dropStatement.IndexName))
            {
                throw new TableOrIndexNotExistsException($"Index {dropStatement.IndexName} Not Exists");
            }
            for (int i = 0; i < index.Count;)
            {
                if (index[i].index_name == dropStatement.IndexName)
                {
                    index.RemoveAt(i);
                    break;
                }
                else
                    i++;
            }
            Save_index(this.index);
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
            if (System.IO.File.Exists($"{_databaseName}.indices.dbcatalog"))
            {
                using (FileStream fs = new FileStream($"{_databaseName}.indices.dbcatalog", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.index = bf.Deserialize(fs) as List<Models.Index>;
                    fs.Close();
                }
            }
            else
            {
                this.index = new List<Models.Index>();
            }
            return;

        }

        public void Save_index(List<Models.Index> index)
        {
            using (FileStream fs = new FileStream($"{_databaseName}.indices.dbcatalog", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, index);
                fs.Close();
            }
        }
        public Catalog_index(string databaseName)
        {
            _databaseName = databaseName;
            Load_index();//get table list from the file
        }
    }
}
