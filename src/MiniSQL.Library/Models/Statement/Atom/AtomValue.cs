namespace MiniSQL.Library.Models
{
    public class AtomValue : AttributeTypeDefinition
    {
        public int IntegerValue { get; set; } = 0;
        public string StringValue { get; set; } = "";
        public double FloatValue { get; set; } = 0.0;
        public bool BooleanValue
        {
            get
            {
                switch (this.Type)
                {
                    case AttributeTypes.Char:
                        if (this.StringValue == "")
                            return false;
                        else
                            return true;
                    case AttributeTypes.Int:
                        if (this.IntegerValue == 0)
                            return false;
                        else
                            return true;
                    case AttributeTypes.Float:
                        if (this.FloatValue == 0.0)
                            return false;
                        else
                            return true;
                    // workaround
                    case AttributeTypes.Null:
                        return true;
                    default:
                        return true;
                }
            }
            set
            {
                switch (this.Type)
                {
                    case AttributeTypes.Char:
                        throw new System.InvalidCastException("bool could not cast to string");
                    case AttributeTypes.Int:
                        if (value)
                            this.IntegerValue = 1;
                        else
                            this.IntegerValue = 0;
                        break;
                    case AttributeTypes.Float:
                        if (value)
                            this.FloatValue = 1;
                        else
                            this.FloatValue = 0;
                        break;
                    // if null, set it to int
                    case AttributeTypes.Null:
                        this.Type = AttributeTypes.Int;
                        if (value)
                            this.IntegerValue = 1;
                        else
                            this.IntegerValue = 0;
                        break;
                    default:
                        throw new System.InvalidCastException("bool could not cast to unknown type");
                }
            }
        }
    }
}
