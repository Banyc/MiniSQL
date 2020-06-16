using System;
using MiniSQL.Library.Models;

namespace MiniSQL.CatalogManager.Models
{
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
}
