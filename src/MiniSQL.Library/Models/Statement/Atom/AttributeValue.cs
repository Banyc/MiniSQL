namespace MiniSQL.Library.Models
{
    // used when definition, not declaration
    // it accepts concrete value
    public class AttributeValue
    {
        public string AttributeName { get; set; }
        public AtomValue Value { get; set; }

        public AttributeValue(string attributeName, AtomValue value)
        {
            this.Value = value;
            this.AttributeName = attributeName;
        }
    }
}
