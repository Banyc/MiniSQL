namespace MiniSQL.Library.Models
{
    
    // used when declaration, not definition
    // it won't accept concrete value
    public class AttributeDeclaration
    {
        public string AttributeName { get; set; }
        public AttributeType Type { get; set; }
        public int CharLimit { get; set; } = 1;
        public bool IsUnique { get; set; } = false;
    }
}
