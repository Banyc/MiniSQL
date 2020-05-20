using System;

namespace MiniSQL.Library.Models
{
    // used when declaration, not definition
    // it won't accept concrete value
    [Serializable]
    public class AttributeDeclaration : AttributeTypeDefinition
    {
        public string AttributeName { get; set; }
        public bool IsUnique { get; set; } = false;
        // for serialization
        public AttributeDeclaration() {}
    }
}
