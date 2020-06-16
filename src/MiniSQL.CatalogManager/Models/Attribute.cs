using System;
using MiniSQL.Library.Models;

namespace MiniSQL.CatalogManager.Models
{
    
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
}
