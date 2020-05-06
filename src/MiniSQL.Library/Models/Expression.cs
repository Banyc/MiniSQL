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
        Negative,
        And,
        Or,
        Not,
        LessThan,
        MoreThan,
        Equal,
        NotEqual,
        LessThanOrEqualTo,
        MoreThanOrEqualTo,
        // atom does not have an operator
        AtomVariable,
        AtomConcreteValue
    }

    public class Expression
    {
        public Operator Operator { get; set; }
        public Expression LeftOperant { get; set; } = null;
        // if nary, this.RightOperant will be null
        public Expression RightOperant { get; set; } = null;
        // use if this Expression is only the attribute
        // this could be a number or a string
        public AttributeValue ConcreteValue { get; set; } = null;
        // use if this Expression is only an attribute (variable)
        public string AttributeName { get; set; } = "";

        // get the value of this Expression
        public AttributeValue Calculate(List<AttributeValue> row)
        {
            // fetch value from symbol table (from `row`)
            if (this.Operator == Operator.AtomVariable)
            {
                AttributeValue column = row.Find(x => x.AttributeName == this.AttributeName);
                if (column == null)
                    throw new System.Exception($"Attribute \"{this.AttributeName}\" does not exist in this table");
                return column;
            }

            // directly return the concrete value
            if (this.Operator == Operator.AtomConcreteValue)
                return this.ConcreteValue;

            // fetch the values of children
            AttributeValue leftValue = this.LeftOperant?.Calculate(row);
            AttributeValue rightValue = this.RightOperant?.Calculate(row);
            AttributeValue result = new AttributeValue();

            // make sure the types of children are the same
            if (leftValue?.Type != rightValue?.Type)
            {
                throw new System.Exception("Operants type not matched!");
            }
            result.Type = leftValue.Type;

            // calculate the two children into a value
            switch (this.Operator)
            {
                case Operator.AtomVariable:
                    throw new System.Exception("AtomVariable is not supposed to reach this step");
                case Operator.AtomConcreteValue:
                    throw new System.Exception("AtomConcreteValue is not supposed to reach this step");
                case Operator.Plus:
                    result.IntegerValue = leftValue.IntegerValue + rightValue.IntegerValue;
                    result.FloatValue = leftValue.FloatValue + rightValue.FloatValue;
                    result.StringValue = leftValue.StringValue + rightValue.StringValue;
                    break;
                case Operator.Minus:
                    result.IntegerValue = leftValue.IntegerValue - rightValue.IntegerValue;
                    result.FloatValue = leftValue.FloatValue - rightValue.FloatValue;
                    if (result.Type == AttributeType.Char)
                        throw new System.Exception("String could not Subtract");
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
                case Operator.Negative:
                    result.IntegerValue = -leftValue.IntegerValue;
                    result.FloatValue = -leftValue.FloatValue;
                    if (result.Type == AttributeType.Char)
                        throw new System.Exception("String could not Negative");
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
                    result.IntegerValue = leftValue.IntegerValue == 0 ? 1 : 0;
                    result.FloatValue = leftValue.FloatValue == 0 ? 1 : 0;
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
