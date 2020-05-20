using System;
using System.Diagnostics;
using MiniSQL.Library.Models;

namespace MiniSQL.Library
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start test!");

            TestExpression();

            Console.WriteLine("End test!");
        }

        private static void TestExpression()
        {
            TestAndList(); 
            TestCalculateSingleValue();
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
            var andList = andsExpression.Ands;
            // "not equal" operator is ignored
            Debug.Assert(andList.ContainsKey("a") == false);
            Debug.Assert(andList["b"].Operator == Operator.LessThanOrEqualTo);
            Debug.Assert(andList["b"].RightOperant.ConcreteValue.StringValue == "str");
            Debug.Assert(andList["c"].Operator == Operator.LessThan);
            Debug.Assert(andList["c"].RightOperant.ConcreteValue.FloatValue == 6.6);
        }

        private static Expression GetAndsExpression()
        {
            // __tree structure__
            //             and 1
            //        and 2,     > 3
            //     != 4, <= 5, 6.6 6, c 7
            // 32 8, a, 9, b 10, "str" 11

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
            node4.LeftOperant = node8;
            node4.RightOperant = node9;
            // <=
            Expression node5 = new Expression();
            node5.Operator = Operator.LessThanOrEqualTo;
            node5.LeftOperant = node10;
            node5.RightOperant = node11;
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
            node2.LeftOperant = node4;
            node2.RightOperant = node5;
            // >
            Expression node3 = new Expression();
            node3.Operator = Operator.MoreThan;
            node3.LeftOperant = node6;
            node3.RightOperant = node7;
            // and
            Expression node1 = new Expression();
            node1.Operator = Operator.And;
            node1.LeftOperant = node2;
            node1.RightOperant = node3;

            return node1;
        }
    }
}
