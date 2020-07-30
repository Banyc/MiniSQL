using System.Collections.Generic;
using System.Linq;
using MiniSQL.Library.Utilities;

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
        Xor,
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
        public Expression LeftOperand { get; set; } = null;
        // if nary, this.RightOperand will be null
        public Expression RightOperand { get; set; } = null;
        // use if this Expression is only the attribute
        // this could be a number or a string
        public AtomValue ConcreteValue { get; set; } = null;
        // use if this Expression is only an attribute (variable)
        public string AttributeName { get; set; } = "";
        private Dictionary<string, Expression> simpleMinterms = null;
        // to help B-Tree's single key searching
        // NOTICE: never try to modify it by yourself
        // only contains simple expressions, with left child being variable, right child being concrete value, and root being non-and operator.
        // might include false negative
        // tolerate false negative
        public Dictionary<string, Expression> SimpleMinterms
        {
            get
            {
                if (this.simpleMinterms == null)
                {
                    this.simpleMinterms = new Dictionary<string, Expression>();
                    this.BuildSimpleMinterms();
                    return this.simpleMinterms;
                }
                else
                    return this.simpleMinterms;
            }
        }

        private void BuildSimpleMinterms()
        {
            if (this.Operator == Operator.AtomConcreteValue || this.Operator == Operator.AtomVariable)
            {
                return;
            }
            this.LeftOperand.BuildSimpleMinterms();
            this.RightOperand.BuildSimpleMinterms();

            if (this.Operator == Operator.Equal
                    || this.Operator == Operator.LessThan
                    || this.Operator == Operator.LessThanOrEqualTo
                    || this.Operator == Operator.MoreThan
                    || this.Operator == Operator.MoreThanOrEqualTo
                    // excluded not equal
                    // || this.Operator == Operator.NotEqual
                    )
            {
                if (this.LeftOperand.Operator == Operator.AtomConcreteValue
                    && this.RightOperand.Operator == Operator.AtomVariable)
                {
                    // swap variable to the left
                    Expression tmp = this.LeftOperand;
                    this.LeftOperand = this.RightOperand;
                    this.RightOperand = tmp;
                    // change operator
                    switch (this.Operator)
                    {
                        case Operator.LessThan:
                            this.Operator = Operator.MoreThan;
                            break;
                        case Operator.LessThanOrEqualTo:
                            this.Operator = Operator.MoreThanOrEqualTo;
                            break;
                        case Operator.MoreThan:
                            this.Operator = Operator.LessThan;
                            break;
                        case Operator.MoreThanOrEqualTo:
                            this.Operator = Operator.LessThanOrEqualTo;
                            break;
                    }
                    // add to the "and" list
                    this.SimpleMinterms[this.LeftOperand.AttributeName] = this;
                }
                else if (this.LeftOperand.Operator == Operator.AtomVariable
                    && this.RightOperand.Operator == Operator.AtomConcreteValue)
                    this.SimpleMinterms[this.LeftOperand.AttributeName] = this;
            }
            // only operator `and` could take `SimpleMinterms` in its children
            else if (this.Operator == Operator.And)
            {
                foreach (var andExpresion in this.LeftOperand.SimpleMinterms.ToList())
                {
                    this.SimpleMinterms[andExpresion.Key] = andExpresion.Value;
                }
                // duplicate key will be overwritten
                foreach (var andExpresion in this.RightOperand.SimpleMinterms.ToList())
                {
                    this.SimpleMinterms[andExpresion.Key] = andExpresion.Value;
                }
            }
        }

        // check if a value could satisfy part of the expression
        public bool CheckKey(string attributeName, AtomValue valueToCheck)
        {
            if (this.SimpleMinterms.ContainsKey(attributeName))
            {
                AtomValue result = this.SimpleMinterms[attributeName].Calculate(new List<AttributeValue>
                {
                    new AttributeValue(attributeName, new AtomValue()
                    {
                        Type = valueToCheck.Type,
                        CharLimit = valueToCheck.CharLimit,
                        FloatValue = valueToCheck.FloatValue,
                        IntegerValue = valueToCheck.IntegerValue,
                    })
                });
                return result.BooleanValue;
            }
            else
            {
                return true;
            }
        }

        // get the value of this Expression
        public AtomValue Calculate(List<AttributeDeclaration> declarations, List<AtomValue> values)
        {
            return Calculate(AttributeHelper.GetAttributeValues(declarations, values));
        }

        // get the value of this Expression
        public AtomValue Calculate(List<AttributeValue> row)
        {
            // fetch value from symbol table (from `row`)
            if (this.Operator == Operator.AtomVariable)
            {
                AttributeValue column = row.Find(x => x.AttributeName == this.AttributeName);
                if (object.ReferenceEquals(column, null))
                    throw new System.Exception($"Attribute \"{this.AttributeName}\" does not exist in this table");
                return column.Value;
            }

            // directly return the concrete value
            if (this.Operator == Operator.AtomConcreteValue)
                return this.ConcreteValue;

            // fetch the values of children
            AtomValue leftValue = this.LeftOperand?.Calculate(row);
            AtomValue rightValue = this.RightOperand?.Calculate(row);
            AtomValue result = null;

            // // make sure the types of children are the same
            // if (leftValue?.Type != rightValue?.Type)
            // {
            //     throw new System.Exception("OpersimpleMinterms type not matched!");
            // }

            // calculate the two children into a value
            switch (this.Operator)
            {
                case Operator.AtomVariable:
                    throw new System.Exception("AtomVariable is not supposed to reach this step");
                case Operator.AtomConcreteValue:
                    throw new System.Exception("AtomConcreteValue is not supposed to reach this step");
                case Operator.Plus:
                    result = leftValue + rightValue;
                    break;
                case Operator.Minus:
                    result = leftValue - rightValue;
                    break;
                case Operator.Multiply:
                    result = leftValue * rightValue;
                    break;
                case Operator.Divide:
                    result = leftValue / rightValue;
                    break;
                case Operator.Negative:
                    result = -leftValue;
                    break;
                case Operator.And:
                    result = new AtomValue();
                    result.Type = AttributeTypes.Int;
                    result.IntegerValue = (leftValue.BooleanValue && rightValue.BooleanValue)? 1 : 0;
                    break;
                case Operator.Or:
                    result = new AtomValue();
                    result.Type = AttributeTypes.Int;
                    result.IntegerValue = (leftValue.BooleanValue || rightValue.BooleanValue)? 1 : 0;
                    break;
                case Operator.Xor:
                    result = new AtomValue();
                    result.Type = AttributeTypes.Int;
                    result.IntegerValue = (leftValue.BooleanValue != rightValue.BooleanValue)? 1 : 0;
                    break;
                case Operator.Not:
                    result = !leftValue;
                    break;
                case Operator.LessThan:
                    result = leftValue < rightValue;
                    break;
                case Operator.MoreThan:
                    result = leftValue > rightValue;
                    break;
                case Operator.Equal:
                    result = leftValue == rightValue;
                    break;
                case Operator.NotEqual:
                    result = leftValue != rightValue;
                    break;
                case Operator.LessThanOrEqualTo:
                    result = leftValue <= rightValue;
                    break;
                case Operator.MoreThanOrEqualTo:
                    result = leftValue >= rightValue;
                    break;
            }
            return result;
        }
    }
}
