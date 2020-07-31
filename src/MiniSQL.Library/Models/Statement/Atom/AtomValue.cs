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
                        return false;
                    default:
                        return false;
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

        public static AtomValue operator !(AtomValue leftValue)
        {
            AtomValue result = new AtomValue();
            result.Type = AttributeTypes.Int;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    throw new System.InvalidOperationException("String could not Not");
                case AttributeTypes.Float:
                case AttributeTypes.Int:
                    result.IntegerValue = leftValue.BooleanValue ? 0 : 1;
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator <(AtomValue leftValue, AtomValue rightValue)
        {
            AtomValue result = new AtomValue();
            result.Type = AttributeTypes.Int;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    ThrowErrorIfNotSameType(leftValue, rightValue);
                    result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) < 0 ? 1 : 0;
                    break;
                case AttributeTypes.Float:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.FloatValue < rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue < rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Int:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue < rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.IntegerValue < rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    result.IntegerValue = leftValue.IntegerValue < rightValue.IntegerValue ? 1 : 0;
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator >(AtomValue leftValue, AtomValue rightValue)
        {
            AtomValue result = new AtomValue();
            result.Type = AttributeTypes.Int;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    ThrowErrorIfNotSameType(leftValue, rightValue);
                    result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) > 0 ? 1 : 0;
                    break;
                case AttributeTypes.Float:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.FloatValue > rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue > rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Int:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue > rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.IntegerValue > rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator <=(AtomValue leftValue, AtomValue rightValue)
        {
            AtomValue result = new AtomValue();
            result.Type = AttributeTypes.Int;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    ThrowErrorIfNotSameType(leftValue, rightValue);
                    result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) <= 0 ? 1 : 0;
                    break;
                case AttributeTypes.Float:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.FloatValue <= rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue <= rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Int:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue <= rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.IntegerValue <= rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator >=(AtomValue leftValue, AtomValue rightValue)
        {
            AtomValue result = new AtomValue();
            result.Type = AttributeTypes.Int;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    ThrowErrorIfNotSameType(leftValue, rightValue);
                    result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) >= 0 ? 1 : 0;
                    break;
                case AttributeTypes.Float:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.FloatValue >= rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue >= rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Int:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue >= rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.IntegerValue >= rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator ==(AtomValue leftValue, AtomValue rightValue)
        {
            AtomValue result = new AtomValue();
            result.Type = AttributeTypes.Int;
            if (object.ReferenceEquals(null, leftValue) && object.ReferenceEquals(null, rightValue))
            {
                result.IntegerValue = 1;
                return result;
            }
            else if (object.ReferenceEquals(null, leftValue) || object.ReferenceEquals(null, rightValue))
            {
                result.IntegerValue = 0;
                return result;
            }
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    ThrowErrorIfNotSameType(leftValue, rightValue);
                    result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) == 0 ? 1 : 0;
                    break;
                case AttributeTypes.Float:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.FloatValue == rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue == rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Int:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue == rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.IntegerValue == rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Null:
                    result.IntegerValue = 1;
                    break;
            }
            return result;
        }

        public static AtomValue operator !=(AtomValue leftValue, AtomValue rightValue)
        {
            AtomValue result = new AtomValue();
            result.Type = AttributeTypes.Int;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    ThrowErrorIfNotSameType(leftValue, rightValue);
                    result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) != 0 ? 1 : 0;
                    break;
                case AttributeTypes.Float:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.FloatValue != rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue != rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Int:
                    switch (rightValue.Type)
                    {
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue != rightValue.IntegerValue ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.IntegerValue != rightValue.FloatValue ? 1 : 0;
                            break;
                        default:
                            ThrowErrorIfNotSameType(leftValue, rightValue);
                            break;
                    }
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator +(AtomValue leftValue, AtomValue rightValue)
        {
            ThrowErrorIfNotSameType(leftValue, rightValue);
            AtomValue result = new AtomValue();
            result.Type = leftValue.Type;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    result.StringValue = leftValue.StringValue + rightValue.StringValue;
                    break;
                case AttributeTypes.Float:
                    result.FloatValue = leftValue.FloatValue + rightValue.FloatValue;
                    break;
                case AttributeTypes.Int:
                    result.IntegerValue = leftValue.IntegerValue + rightValue.IntegerValue;
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator -(AtomValue leftValue, AtomValue rightValue)
        {
            ThrowErrorIfNotSameType(leftValue, rightValue);
            AtomValue result = new AtomValue();
            result.Type = leftValue.Type;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    throw new System.InvalidOperationException("String could not Subtract");
                case AttributeTypes.Float:
                    result.FloatValue = leftValue.FloatValue - rightValue.FloatValue;
                    break;
                case AttributeTypes.Int:
                    result.IntegerValue = leftValue.IntegerValue - rightValue.IntegerValue;
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator -(AtomValue leftValue)
        {
            AtomValue result = new AtomValue();
            result.Type = leftValue.Type;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    throw new System.InvalidOperationException("String could not Subtract");
                case AttributeTypes.Float:
                    result.FloatValue = -leftValue.FloatValue;
                    break;
                case AttributeTypes.Int:
                    result.IntegerValue = -leftValue.IntegerValue;
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator *(AtomValue leftValue, AtomValue rightValue)
        {
            ThrowErrorIfNotSameType(leftValue, rightValue);
            AtomValue result = new AtomValue();
            result.Type = leftValue.Type;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    throw new System.InvalidOperationException("String could not Multiply");
                case AttributeTypes.Float:
                    result.FloatValue = leftValue.FloatValue * rightValue.FloatValue;
                    break;
                case AttributeTypes.Int:
                    result.IntegerValue = leftValue.IntegerValue * rightValue.IntegerValue;
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        public static AtomValue operator /(AtomValue leftValue, AtomValue rightValue)
        {
            ThrowErrorIfNotSameType(leftValue, rightValue);
            AtomValue result = new AtomValue();
            result.Type = leftValue.Type;
            switch (leftValue.Type)
            {
                case AttributeTypes.Char:
                    throw new System.InvalidOperationException("String could not Divide");
                case AttributeTypes.Float:
                    result.FloatValue = leftValue.FloatValue / rightValue.FloatValue;
                    break;
                case AttributeTypes.Int:
                    result.IntegerValue = leftValue.IntegerValue / rightValue.IntegerValue;
                    break;
                case AttributeTypes.Null:
                    throw new System.NullReferenceException();
            }
            return result;
        }

        private static void ThrowErrorIfNotSameType(AtomValue leftValue, AtomValue rightValue)
        {
            // make sure the types of children are the same
            if (leftValue?.Type != rightValue?.Type)
            {
                throw new System.Exception("Operands type not matched!");
            }
        }

        public override bool Equals(object obj)
        {
            return (this == (AtomValue)obj).BooleanValue;
        }

        public override int GetHashCode()
        {
            switch (this.Type)
            {
                case AttributeTypes.Char:
                    return this.StringValue.GetHashCode();
                case AttributeTypes.Float:
                    return this.FloatValue.GetHashCode();
                case AttributeTypes.Int:
                    return this.IntegerValue.GetHashCode();
                case AttributeTypes.Null:
                    return 0;
                default:
                    return 0;
            }
        }

        public override string ToString()
        {
            return this.Type switch
            {
                AttributeTypes.Null => "null",
                AttributeTypes.Int => this.IntegerValue.ToString(),
                AttributeTypes.Char => this.StringValue,
                AttributeTypes.Float => this.FloatValue.ToString(),
                _ => null,
            };
        }
    }
}
