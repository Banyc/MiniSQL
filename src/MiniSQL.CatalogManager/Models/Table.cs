using System;
using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.CatalogManager.Models
{
    
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
}
