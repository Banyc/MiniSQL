using System.Collections.Generic;
using System.Linq;

namespace MiniSQL.Library.Models
{
    public enum Operator
    {
        Plus,
        Minus,
        Multiply,
        Divide,
        And,
        Or,
        Not,
        LessThan,
        MoreThan,
        Equal,
        NotEqual,
        LessThanOrEqualTo,
        MoreThanOrEqualTo,
        Atom
    }

    public class Expression
    {
        public Operator Operator { get; set; }
        // if nary, left will be null
        public Expression LeftOperant { get; set; } = null;
        public Expression RightOperant { get; set; } = null;
        public AttributeValue Atom { get; set; } = null;

        public AttributeValue Calculate(List<AttributeValue> row)
        {
            // match from symbol table
            if (this.Operator == Operator.Atom)
            {
                AttributeValue column = row.Find(x => x.AttributeName == this.Atom.AttributeName);
                if (column == null)
                    throw new System.Exception($"{this.Atom.AttributeName} Not found");
                return column;
            }

            AttributeValue leftValue = this.LeftOperant?.Calculate(row);
            AttributeValue rightValue = this.RightOperant?.Calculate(row);
            AttributeValue result = new AttributeValue();

            if (leftValue?.Type != rightValue?.Type)
            {
                throw new System.Exception("Operants type not matched!");
            }
            result.Type = rightValue.Type;

            switch (this.Operator)
            {
                case Operator.Atom:
                    return this.Atom;
                case Operator.Plus:
                    result.IntegerValue = leftValue.IntegerValue + rightValue.IntegerValue;
                    result.FloatValue = leftValue.FloatValue + rightValue.FloatValue;
                    result.StringValue = leftValue.StringValue + rightValue.StringValue;
                    break;
                case Operator.Minus:
                    result.IntegerValue = leftValue.IntegerValue - rightValue.IntegerValue;
                    result.FloatValue = leftValue.FloatValue - rightValue.FloatValue;
                    if (result.Type == AttributeType.Char)
                        throw new System.Exception("String could not subtract");
                    break;
                case Operator.Multiply:
                    result.IntegerValue = leftValue.IntegerValue * rightValue.IntegerValue;
                    result.FloatValue = leftValue.FloatValue * rightValue.FloatValue;
                    if (result.Type == AttributeType.Char)
                        throw new System.Exception("String could not Multiply");
                    break;
                case Operator.Divide:
                    result.IntegerValue = leftValue.IntegerValue / rightValue.IntegerValue;
                    result.FloatValue = leftValue.FloatValue / rightValue.FloatValue;
                    if (result.Type == AttributeType.Char)
                        throw new System.Exception("String could not Divide");
                    break;
                case Operator.And:
                    result.IntegerValue = leftValue.IntegerValue != 0 && rightValue.IntegerValue != 0 ? 1 : 0;
                    result.FloatValue = leftValue.FloatValue != 0 && rightValue.FloatValue != 0 ? 1 : 0;
                    if (result.Type == AttributeType.Char)
                        throw new System.Exception("String could not And");
                    break;
                case Operator.Or:
                    result.IntegerValue = leftValue.IntegerValue != 0 || rightValue.IntegerValue != 0 ? 1 : 0;
                    result.FloatValue = leftValue.FloatValue != 0 || rightValue.FloatValue != 0 ? 1 : 0;
                    if (result.Type == AttributeType.Char)
                        throw new System.Exception("String could not Or");
                    break;
                case Operator.Not:
                    result.IntegerValue = rightValue.IntegerValue == 0 ? 1 : 0;
                    result.FloatValue = rightValue.FloatValue == 0 ? 1 : 0;
                    if (result.Type == AttributeType.Char)
                        throw new System.Exception("String could not Not");
                    break;
                case Operator.LessThan:
                    result.Type = AttributeType.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeType.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) < 0 ? 1 : 0;
                            break;
                        case AttributeType.Float:
                            result.IntegerValue = leftValue.FloatValue < rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeType.Int:
                            result.IntegerValue = leftValue.IntegerValue < rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.MoreThan:
                    result.Type = AttributeType.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeType.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) > 0 ? 1 : 0;
                            break;
                        case AttributeType.Float:
                            result.IntegerValue = leftValue.FloatValue > rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeType.Int:
                            result.IntegerValue = leftValue.IntegerValue > rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.Equal:
                    result.Type = AttributeType.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeType.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) == 0 ? 1 : 0;
                            break;
                        case AttributeType.Float:
                            result.IntegerValue = leftValue.FloatValue == rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeType.Int:
                            result.IntegerValue = leftValue.IntegerValue == rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.NotEqual:
                    result.Type = AttributeType.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeType.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) != 0 ? 1 : 0;
                            break;
                        case AttributeType.Float:
                            result.IntegerValue = leftValue.FloatValue != rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeType.Int:
                            result.IntegerValue = leftValue.IntegerValue != rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.LessThanOrEqualTo:
                    result.Type = AttributeType.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeType.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) <= 0 ? 1 : 0;
                            break;
                        case AttributeType.Float:
                            result.IntegerValue = leftValue.FloatValue <= rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeType.Int:
                            result.IntegerValue = leftValue.IntegerValue <= rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.MoreThanOrEqualTo:
                    result.Type = AttributeType.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeType.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) >= 0 ? 1 : 0;
                            break;
                        case AttributeType.Float:
                            result.IntegerValue = leftValue.FloatValue >= rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeType.Int:
                            result.IntegerValue = leftValue.IntegerValue >= rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
            }

            return result;
        }
    }
}
