namespace MiniSQL.Library.Models
{
    // used when definition, not declaration
    // it accepts concrete value
    public class AttributeValue
    {
        public string AttributeName { get; set; }
        public AttributeType Type { get; set; }
        public int IntegerValue { get; set; } = 0;
        public string StringValue { get; set; } = "";
        public double FloatValue { get; set; } = 0.0;
    }
}
