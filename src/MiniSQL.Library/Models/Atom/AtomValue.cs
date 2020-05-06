namespace MiniSQL.Library.Models
{
    public class AtomValue
    {
        public AttributeType Type { get; set; }
        public int IntegerValue { get; set; } = 0;
        public string StringValue { get; set; } = "";
        public double FloatValue { get; set; } = 0.0;
    }
}