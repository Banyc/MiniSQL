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
        public Expression LeftOperant { get; set; } = null;
        // if nary, this.RightOperant will be null
        public Expression RightOperant { get; set; } = null;
        // use if this Expression is only the attribute
        // this could be a number or a string
        public AtomValue ConcreteValue { get; set; } = null;
        // use if this Expression is only an attribute (variable)
        public string AttributeName { get; set; } = "";

        // {attribute name: expression with only the variable/attribute}
        private Dictionary<string, Expression> ands = null;
        // to help B-Tree's single key searching
        public Dictionary<string, Expression> Ands
        {
            get
            {
                if (this.ands == null)
                {
                    this.ands = new Dictionary<string, Expression>();
                    this.BuildAndList();
                    return this.ands;
                }
                else
                    return this.ands;
            }
        }

        private void BuildAndList()
        {
            if (this.Operator == Operator.AtomConcreteValue || this.Operator == Operator.AtomVariable)
            {
                return;
            }
            this.LeftOperant.BuildAndList();
            this.RightOperant.BuildAndList();

            if (this.Operator == Operator.Equal
                    || this.Operator == Operator.LessThan
                    || this.Operator == Operator.LessThanOrEqualTo
                    || this.Operator == Operator.MoreThan
                    || this.Operator == Operator.MoreThanOrEqualTo
                    // excluded not equal
                    // || this.Operator == Operator.NotEqual
                    )
            {
                if (this.LeftOperant.Operator == Operator.AtomConcreteValue
                    && this.RightOperant.Operator == Operator.AtomVariable)
                {
                    // swap variable to the left
                    Expression tmp = this.LeftOperant;
                    this.LeftOperant = this.RightOperant;
                    this.RightOperant = tmp;
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
                    this.Ands[this.LeftOperant.AttributeName] = this;
                }
                else if (this.LeftOperant.Operator == Operator.AtomVariable
                    && this.RightOperant.Operator == Operator.AtomConcreteValue)
                    this.Ands[this.LeftOperant.AttributeName] = this;
            }
            // only operator `and` could take `Ands` in its children
            else if (this.Operator == Operator.And)
            {
                foreach (var andExpresion in this.LeftOperant.Ands.ToList())
                {
                    this.Ands[andExpresion.Key] = andExpresion.Value;
                }
                // duplicate key will be overwritten
                foreach (var andExpresion in this.RightOperant.Ands.ToList())
                {
                    this.Ands[andExpresion.Key] = andExpresion.Value;
                }
            }
        }

        public bool CheckKey(string attributeName, AtomValue valueToCheck)
        {
            if (this.Ands.ContainsKey(attributeName))
            {
                AtomValue result = this.Ands[attributeName].Calculate(new List<AttributeValue>
                {
                    new AttributeValue
                    {
                        AttributeName = attributeName,
                        Type = valueToCheck.Type,
                        CharLimit = valueToCheck.CharLimit,
                        FloatValue = valueToCheck.FloatValue,
                        IntegerValue = valueToCheck.IntegerValue,
                        StringValue = valueToCheck.StringValue}});
                return result.BooleanValue;
            }
            else
            {
                return true;
            }
        }

        // get the value of this Expression
        public AtomValue Calculate(List<AttributeValue> row)
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
            AtomValue leftValue = this.LeftOperant?.Calculate(row);
            AtomValue rightValue = this.RightOperant?.Calculate(row);
            AtomValue result = new AtomValue();

            // make sure the types of children are the same
            if (leftValue?.Type != rightValue?.Type)
            {
                throw new System.Exception("Operants type not matched!");
            }
            result.Type = leftValue.Type;

            // TODO: move those logic to class AtomValue
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
                    if (result.Type == AttributeTypes.Char)
                        throw new System.InvalidOperationException("String could not Subtract");
                    break;
                case Operator.Multiply:
                    result.IntegerValue = leftValue.IntegerValue * rightValue.IntegerValue;
                    result.FloatValue = leftValue.FloatValue * rightValue.FloatValue;
                    if (result.Type == AttributeTypes.Char)
                        throw new System.InvalidOperationException("String could not Multiply");
                    break;
                case Operator.Divide:
                    result.IntegerValue = leftValue.IntegerValue / rightValue.IntegerValue;
                    result.FloatValue = leftValue.FloatValue / rightValue.FloatValue;
                    if (result.Type == AttributeTypes.Char)
                        throw new System.InvalidOperationException("String could not Divide");
                    break;
                case Operator.Negative:
                    result.IntegerValue = -leftValue.IntegerValue;
                    result.FloatValue = -leftValue.FloatValue;
                    if (result.Type == AttributeTypes.Char)
                        throw new System.InvalidOperationException("String could not Negative");
                    break;
                case Operator.And:
                    result.IntegerValue = leftValue.IntegerValue != 0 && rightValue.IntegerValue != 0 ? 1 : 0;
                    result.FloatValue = leftValue.FloatValue != 0 && rightValue.FloatValue != 0 ? 1 : 0;
                    if (result.Type == AttributeTypes.Char)
                        throw new System.InvalidOperationException("String could not And");
                    break;
                case Operator.Or:
                    result.IntegerValue = leftValue.IntegerValue != 0 || rightValue.IntegerValue != 0 ? 1 : 0;
                    result.FloatValue = leftValue.FloatValue != 0 || rightValue.FloatValue != 0 ? 1 : 0;
                    if (result.Type == AttributeTypes.Char)
                        throw new System.InvalidOperationException("String could not Or");
                    break;
                case Operator.Xor:
                    result.IntegerValue = ((leftValue.IntegerValue != 0 || rightValue.IntegerValue != 0)
                                            && (leftValue.IntegerValue != 0 && rightValue.IntegerValue != 0)) ? 1 : 0;
                    result.FloatValue = ((leftValue.FloatValue != 0 || rightValue.FloatValue != 0)
                                            && (leftValue.FloatValue != 0 && rightValue.FloatValue != 0)) ? 1 : 0;
                    if (result.Type == AttributeTypes.Char)
                        throw new System.InvalidOperationException("String could not Or");
                    break;
                case Operator.Not:
                    result.IntegerValue = leftValue.IntegerValue == 0 ? 1 : 0;
                    result.FloatValue = leftValue.FloatValue == 0 ? 1 : 0;
                    if (result.Type == AttributeTypes.Char)
                        throw new System.InvalidOperationException("String could not Not");
                    break;
                case Operator.LessThan:
                    result.Type = AttributeTypes.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeTypes.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) < 0 ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue < rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue < rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.MoreThan:
                    result.Type = AttributeTypes.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeTypes.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) > 0 ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue > rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue > rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.Equal:
                    result.Type = AttributeTypes.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeTypes.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) == 0 ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue == rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue == rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.NotEqual:
                    result.Type = AttributeTypes.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeTypes.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) != 0 ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue != rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue != rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.LessThanOrEqualTo:
                    result.Type = AttributeTypes.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeTypes.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) <= 0 ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue <= rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue <= rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
                case Operator.MoreThanOrEqualTo:
                    result.Type = AttributeTypes.Int;
                    switch (leftValue.Type)
                    {
                        case AttributeTypes.Char:
                            result.IntegerValue = string.Compare(leftValue.StringValue, rightValue.StringValue) >= 0 ? 1 : 0;
                            break;
                        case AttributeTypes.Float:
                            result.IntegerValue = leftValue.FloatValue >= rightValue.FloatValue ? 1 : 0;
                            break;
                        case AttributeTypes.Int:
                            result.IntegerValue = leftValue.IntegerValue >= rightValue.IntegerValue ? 1 : 0;
                            break;
                    }
                    break;
            }

            return result;
        }
    }
}
