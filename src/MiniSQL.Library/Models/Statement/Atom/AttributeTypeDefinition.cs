namespace MiniSQL.Library.Models
{
    public class AttributeTypeDefinition
    {
        public AttributeTypes Type { get; set; } = AttributeTypes.Null;
        // char length is fixed. The remaining is padded with blanks.
        public int CharLimit { get; set; } = 1;
    }
}
