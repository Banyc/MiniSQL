namespace MiniSQL.Library.Models
{
    public enum SchemaTypes
    {
        Table,
        Index,
    }

    public class SchemaRecord
    {
        public SchemaTypes Type { get; set; }
        public string Name { get; set; }
        public string AssociatedTable { get; set; }
        public int RootPage { get; set; }
        public CreateStatement SQL { get; set; }
    }
}
