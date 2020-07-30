using System;
using System.Collections.Generic;
using System.Diagnostics;
using MiniSQL.Library.Models;

namespace MiniSQL.Library
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Library] Start test!");

            TestExpression();

            TestAtomValueOperators();

            Console.WriteLine("[Library] End test!");
        }

        private static void TestAtomValueOperators()
        {
            AtomValue left = new AtomValue();
            AtomValue right = new AtomValue();
            left.StringValue = "abedeb";
            left.IntegerValue = 114;
            left.FloatValue = 14.14;
            right.StringValue = "abeceb";
            right.IntegerValue = 514;
            right.FloatValue = 14.14;

            left.Type = AttributeTypes.Char;
            right.Type = AttributeTypes.Char;
            Debug.Assert((left < right).BooleanValue == false);
            Debug.Assert((left > right).BooleanValue == true);
            Debug.Assert((left == right).BooleanValue == false);
            Debug.Assert((left != right).BooleanValue == true);
            Debug.Assert((left <= right).BooleanValue == false);
            Debug.Assert((left >= right).BooleanValue == true);
            Debug.Assert((left + right).StringValue == "abedebabeceb");

            left.Type = AttributeTypes.Int;
            right.Type = AttributeTypes.Int;
            Debug.Assert((left < right).BooleanValue == true);
            Debug.Assert((left > right).BooleanValue == false);
            Debug.Assert((left == right).BooleanValue == false);
            Debug.Assert((left != right).BooleanValue == true);
            Debug.Assert((left <= right).BooleanValue == true);
            Debug.Assert((left >= right).BooleanValue == false);
            Debug.Assert((left + right).IntegerValue == 628);
            Debug.Assert((left - right).IntegerValue == -400);
            Debug.Assert((left * right).IntegerValue == 114 * 514);
            Debug.Assert((left / right).IntegerValue == 114 / 514);

            left.Type = AttributeTypes.Float;
            right.Type = AttributeTypes.Float;
            Debug.Assert((left < right).BooleanValue == false);
            Debug.Assert((left > right).BooleanValue == false);
            Debug.Assert((left == right).BooleanValue == true);
            Debug.Assert((left != right).BooleanValue == false);
            Debug.Assert((left <= right).BooleanValue == true);
            Debug.Assert((left >= right).BooleanValue == true);
            Debug.Assert((left + right).FloatValue == 28.28);
            Debug.Assert((left - right).FloatValue == 0.0);
            Debug.Assert((left * right).FloatValue == 14.14 * 14.14);
            Debug.Assert((left / right).FloatValue == 1);
        }

        private static void TestExpression()
        {
            TestAndList(); 
            TestCalculateSingleValue();
            TestFullCalculate();
        }

        private static void TestFullCalculate()
        {
            Expression exp = GetAndsExpression();
            
            List<AttributeValue> nameValuePairs = new List<AttributeValue>();
            nameValuePairs.Add(new AttributeValue("a", new AtomValue {Type = AttributeTypes.Int, IntegerValue = 33}));
            nameValuePairs.Add(new AttributeValue("b", new AtomValue {Type = AttributeTypes.Char, CharLimit = 5 , StringValue = "stc"}));
            nameValuePairs.Add(new AttributeValue("c", new AtomValue {Type = AttributeTypes.Float, FloatValue = 6.0}));

            AtomValue result = exp.Calculate(nameValuePairs);
            Debug.Assert(result.BooleanValue == true);

            nameValuePairs[1].Value.StringValue = "stz";

            result = exp.Calculate(nameValuePairs);
            Debug.Assert(result.BooleanValue == false);
        }

        private static void TestCalculateSingleValue()
        {
            Expression andsExpression = GetAndsExpression();
            string attributeName = "b";
            AtomValue attributeValue = new AtomValue();
            attributeValue.Type = AttributeTypes.Char;
            attributeValue.CharLimit = 10;
            attributeValue.StringValue = "aaa";
            Debug.Assert(andsExpression.CheckKey(attributeName, attributeValue) == true);
        }

        private static void TestAndList()
        {
            Expression andsExpression = GetAndsExpression();
            var andList = andsExpression.SimpleMinterms;
            // "not equal" operator is ignored
            Debug.Assert(andList.ContainsKey("a") == false);
            Debug.Assert(andList["b"].Operator == Operator.LessThanOrEqualTo);
            Debug.Assert(andList["b"].RightOperand.ConcreteValue.StringValue == "str");
            Debug.Assert(andList["c"].Operator == Operator.LessThan);
            Debug.Assert(andList["c"].RightOperand.ConcreteValue.FloatValue == 6.6);
        }

        private static Expression GetAndsExpression()
        {
            // __expression__
            // ((32 != a) and (b <= "str")) and (6.6 > c)
            // __tree structure__
            //                       and 1
            //            and 2,                    > 3
            //     != 4,        <= 5,           6.6 6, c 7
            // 32 8, a 9,    b 10, "str" 11

            // 32
            Expression node8 = new Expression();
            node8.Operator = Operator.AtomConcreteValue;
            node8.ConcreteValue = new AtomValue();
            node8.ConcreteValue.Type = AttributeTypes.Int;
            node8.ConcreteValue.IntegerValue = 32;
            // a
            Expression node9 = new Expression();
            node9.Operator = Operator.AtomVariable;
            node9.AttributeName = "a";
            // b
            Expression node10 = new Expression();
            node10.Operator = Operator.AtomVariable;
            node10.AttributeName = "b";
            // "str"
            Expression node11 = new Expression();
            node11.Operator = Operator.AtomConcreteValue;
            node11.ConcreteValue = new AtomValue();
            node11.ConcreteValue.Type = AttributeTypes.Char;
            node11.ConcreteValue.CharLimit = 5;
            node11.ConcreteValue.StringValue = "str";
            // !=
            Expression node4 = new Expression();
            node4.Operator = Operator.NotEqual;
            node4.LeftOperand = node8;
            node4.RightOperand = node9;
            // <=
            Expression node5 = new Expression();
            node5.Operator = Operator.LessThanOrEqualTo;
            node5.LeftOperand = node10;
            node5.RightOperand = node11;
            // 6.6
            Expression node6 = new Expression();
            node6.Operator = Operator.AtomConcreteValue;
            node6.ConcreteValue = new AtomValue();
            node6.ConcreteValue.Type = AttributeTypes.Float;
            node6.ConcreteValue.FloatValue = 6.6;
            // c
            Expression node7 = new Expression();
            node7.Operator = Operator.AtomVariable;
            node7.AttributeName = "c";
            // and
            Expression node2 = new Expression();
            node2.Operator = Operator.And;
            node2.LeftOperand = node4;
            node2.RightOperand = node5;
            // >
            Expression node3 = new Expression();
            node3.Operator = Operator.MoreThan;
            node3.LeftOperand = node6;
            node3.RightOperand = node7;
            // and
            Expression node1 = new Expression();
            node1.Operator = Operator.And;
            node1.LeftOperand = node2;
            node1.RightOperand = node3;

            return node1;
        }
    }
}
