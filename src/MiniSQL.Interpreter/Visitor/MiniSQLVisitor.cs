using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using MiniSQL.Interpreter.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.Interpreter
{
    public class MiniSQLVisitor : MiniSQLBaseVisitor<object>
    {
        public override object VisitAtom([NotNull] MiniSQLParser.AtomContext context)
        {
            Expression obj = new Expression();
            // TODO: review
            if (context.scientific() != null)
            {
                AtomValue atomValue = (AtomValue)Visit(context.scientific());
                obj.Operator = Operator.AtomConcreteValue;
                obj.ConcreteValue = atomValue;
            }
            else if (context.variable() != null)
            {
                string variableName = context.variable().GetText();
                obj.Operator = Operator.AtomVariable;
                obj.AttributeName = variableName;
            }
            else
                throw new System.NotImplementedException();
            return obj;
        }

        public override object VisitColumnAttribute([NotNull] MiniSQLParser.ColumnAttributeContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitColumnDefinition([NotNull] MiniSQLParser.ColumnDefinitionContext context)
        {
            AttributeDeclaration declaration = (AttributeDeclaration)Visit(context.fieldDefinition());
            declaration.AttributeName = context.columnName().GetText();
            return declaration;
        }

        public override object VisitColumnName([NotNull] MiniSQLParser.ColumnNameContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitCompOp([NotNull] MiniSQLParser.CompOpContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitCreateIndex([NotNull] MiniSQLParser.CreateIndexContext context)
        {
            CreateStatement obj = new CreateStatement();
            obj.CreateType = CreateType.Index;
            obj.IndexName = context.indexName().GetText();
            (string, string) tableAttributeNameIsUnique = ((string, string))Visit(context.createIndexTarget());
            obj.TableName = tableAttributeNameIsUnique.Item1;
            obj.AttributeName = tableAttributeNameIsUnique.Item2;
            if (context.UNIQUE_SYMBOL() != null)
                obj.IsUnique = true;
            else
                obj.IsUnique = false;
            return obj;
        }

        public override object VisitCreateIndexTarget([NotNull] MiniSQLParser.CreateIndexTargetContext context)
        {
            string tableName = context.tableRef().GetText();
            string attributeName = (string)Visit(context.keyListVariants());
            return (tableName, attributeName);
        }

        public override object VisitCreateStatement([NotNull] MiniSQLParser.CreateStatementContext context)
        {
            CreateStatement ret;
            if (context.createIndex() != null)
                ret = (CreateStatement)Visit(context.createIndex());
            else
                ret = (CreateStatement)Visit(context.createTable());
            return ret;
        }

        public override object VisitCreateTable([NotNull] MiniSQLParser.CreateTableContext context)
        {
            CreateStatement obj = new CreateStatement();
            obj.TableName = context.tableName().GetText();
            if (context.tableElementList() == null)
                obj.AttributeDeclarations = new List<AttributeDeclaration>();
            else
            {
                (List<AttributeDeclaration>, string) AttributesPrimaryPair = ((List<AttributeDeclaration>, string))Visit(context.tableElementList());
                obj.AttributeDeclarations = AttributesPrimaryPair.Item1;
                obj.PrimaryKey = AttributesPrimaryPair.Item2;
            }
            return obj;
        }

        public override object VisitDataType([NotNull] MiniSQLParser.DataTypeContext context)
        {
            AttributeDeclaration attribute = new AttributeDeclaration();
            string type = context.type.Text.ToLower();
            switch (type)
            {
                case "int":
                    attribute.Type = AttributeTypes.Int;
                    break;
                case "float":
                    attribute.Type = AttributeTypes.Float;
                    break;
                case "char":
                    attribute.Type = AttributeTypes.Char;
                    if (context.fieldLength() != null)
                        attribute.CharLimit = (int)Visit(context.fieldLength());
                    break;
            }
            return attribute;
        }

        public override object VisitDeleteStatement([NotNull] MiniSQLParser.DeleteStatementContext context)
        {
            Expression exp = (Expression)Visit(context.whereClause());
            string tableName = context.tableRef().GetText();
            DeleteStatement obj = new DeleteStatement();
            obj.TableName = tableName;
            obj.Condition = exp;
            return obj;
        }

        public override object VisitDropIndex([NotNull] MiniSQLParser.DropIndexContext context)
        {
            DropStatement obj = new DropStatement();
            obj.TargetType = DropTarget.Index;
            if (context.tableRef() != null)
                obj.TableName = context.tableRef().GetText();
            obj.IndexName = context.indexRef().GetText();
            return obj;
        }

        public override object VisitDropStatement([NotNull] MiniSQLParser.DropStatementContext context)
        {
            DropStatement ret;
            if (context.dropIndex() != null)
                ret = (DropStatement)Visit(context.dropIndex());
            else
                ret = (DropStatement)Visit(context.dropTable());
            return ret;
        }

        public override object VisitDropTable([NotNull] MiniSQLParser.DropTableContext context)
        {
            DropStatement obj = new DropStatement();
            obj.TargetType = DropTarget.Table;
            obj.TableName = context.tableRef().GetText();
            return obj;
        }

        public override object VisitExecFileStatement([NotNull] MiniSQLParser.ExecFileStatementContext context)
        {
            ExecFileStatement obj = new ExecFileStatement();
            string filePathWithQuote;
            if (context.SINGLE_QUOTED_TEXT() != null)
                filePathWithQuote = context.SINGLE_QUOTED_TEXT().GetText();
            else
                filePathWithQuote = context.DOUBLE_QUOTED_TEXT().GetText();
            string filePath = filePathWithQuote.Substring(1, filePathWithQuote.Length - 2);
            obj.FilePath = filePath;
            return obj;
        }

        public override object VisitExprAdd([NotNull] MiniSQLParser.ExprAddContext context)
        {
            Expression left = (Expression)Visit(context.expr(0));
            Expression right = (Expression)Visit(context.expr(1));
            Expression obj = new Expression();
            obj.LeftOperand = left;
            obj.RightOperand = right;
            if (context.PLUS_OPERATOR() != null)
                obj.Operator = Operator.Plus;
            else
                obj.Operator = Operator.Minus;
            // optimize expression
            if (left.Operator == Operator.AtomConcreteValue
                && right.Operator == Operator.AtomConcreteValue)
            {
                left.ConcreteValue = obj.Calculate(null);
                return left;
            }
            return obj;
        }

        public override object VisitExprAnd([NotNull] MiniSQLParser.ExprAndContext context)
        {
            Expression left = (Expression)Visit(context.expr(0));
            Expression right = (Expression)Visit(context.expr(1));
            Expression obj = new Expression();
            obj.LeftOperand = left;
            obj.RightOperand = right;
            obj.Operator = Operator.And;
            // optimize expression
            if (left.Operator == Operator.AtomConcreteValue
                && right.Operator == Operator.AtomConcreteValue)
            {
                left.ConcreteValue = obj.Calculate(null);
                return left;
            }
            return obj;
        }

        public override object VisitExprAtom([NotNull] MiniSQLParser.ExprAtomContext context)
        {
            return (Expression)Visit(context.atom());
        }

        public override object VisitExprCompare([NotNull] MiniSQLParser.ExprCompareContext context)
        {
            Expression left = (Expression)Visit(context.expr(0));
            Expression right = (Expression)Visit(context.expr(1));
            Expression obj = new Expression();
            obj.LeftOperand = left;
            obj.RightOperand = right;
            if (context.compOp().EQUAL_OPERATOR() != null)
                obj.Operator = Operator.Equal;
            else if (context.compOp().GREATER_OR_EQUAL_OPERATOR() != null)
                obj.Operator = Operator.MoreThanOrEqualTo;
            else if (context.compOp().GREATER_THAN_OPERATOR() != null)
                obj.Operator = Operator.MoreThan;
            else if (context.compOp().LESS_OR_EQUAL_OPERATOR() != null)
                obj.Operator = Operator.LessThanOrEqualTo;
            else if (context.compOp().LESS_THAN_OPERATOR() != null)
                obj.Operator = Operator.LessThan;
            else if (context.compOp().NOT_EQUAL_OPERATOR() != null)
                obj.Operator = Operator.NotEqual;
            else
                throw new System.NotImplementedException();
            // optimize expression
            if (left.Operator == Operator.AtomConcreteValue
                && right.Operator == Operator.AtomConcreteValue)
            {
                left.ConcreteValue = obj.Calculate(null);
                return left;
            }
            return obj;
        }

        public override object VisitExprMul([NotNull] MiniSQLParser.ExprMulContext context)
        {
            Expression left = (Expression)Visit(context.expr(0));
            Expression right = (Expression)Visit(context.expr(1));
            Expression obj = new Expression();
            obj.LeftOperand = left;
            obj.RightOperand = right;
            if (context.MULT_OPERATOR() != null)
                obj.Operator = Operator.Multiply;
            else
                obj.Operator = Operator.Divide;
            // optimize expression
            if (left.Operator == Operator.AtomConcreteValue
                && right.Operator == Operator.AtomConcreteValue)
            {
                left.ConcreteValue = obj.Calculate(null);
                return left;
            }
            return obj;
        }

        public override object VisitExprNot([NotNull] MiniSQLParser.ExprNotContext context)
        {
            Expression left = (Expression)Visit(context.expr());
            // if (left.Operator == Operator.AtomConcreteValue)
            // {
            //     if (left.ConcreteValue.Type == AttributeType.Char)
            //         throw new System.InvalidOperationException("Operator '!' cannot be applied to operand of type 'string'");
            //     if (left.ConcreteValue.Type == AttributeType.Float)
            //         throw new System.InvalidOperationException("Operator '!' cannot be applied to operand of type 'float'");
            //     if (left.ConcreteValue.Type == AttributeType.Int)
            //     {
            //         if (left.ConcreteValue.IntegerValue == 0)
            //             left.ConcreteValue.IntegerValue = 1;
            //         else
            //             left.ConcreteValue.IntegerValue = 0;
            //     }
            //     return left;
            // }
            Expression obj = new Expression();
            obj.LeftOperand = left;
            obj.Operator = Operator.Not;
            // optimize expression
            if (left.Operator == Operator.AtomConcreteValue)
            {
                left.ConcreteValue = obj.Calculate(null);
                return left;
            }
            return obj;
        }

        public override object VisitExprOr([NotNull] MiniSQLParser.ExprOrContext context)
        {
            Expression left = (Expression)Visit(context.expr(0));
            Expression right = (Expression)Visit(context.expr(1));
            Expression obj = new Expression();
            obj.LeftOperand = left;
            obj.RightOperand = right;
            obj.Operator = Operator.Or;
            // optimize expression
            if (left.Operator == Operator.AtomConcreteValue
                && right.Operator == Operator.AtomConcreteValue)
            {
                left.ConcreteValue = obj.Calculate(null);
                return left;
            }
            return obj;
        }

        public override object VisitExprPar([NotNull] MiniSQLParser.ExprParContext context)
        {
            return (Expression)Visit(context.expr());
        }

        public override object VisitExprSign([NotNull] MiniSQLParser.ExprSignContext context)
        {
            Expression left = (Expression)Visit(context.expr());
            // optimize expression
            if (context.PLUS_OPERATOR() != null)
            {
                if (left.ConcreteValue.Type == AttributeTypes.Char)
                    throw new System.InvalidOperationException("Operator '+' cannot be applied to operand of type 'string'");
                return left;
            }
            // if (left.Operator == Operator.AtomConcreteValue)
            // {
            //     if (left.ConcreteValue.Type == AttributeType.Char)
            //         throw new System.InvalidOperationException("Operator '-' cannot be applied to operand of type 'string'");
            //     if (left.ConcreteValue.Type == AttributeType.Int)
            //         left.ConcreteValue.IntegerValue = -left.ConcreteValue.IntegerValue;
            //     if (left.ConcreteValue.Type == AttributeType.Float)
            //         left.ConcreteValue.FloatValue = -left.ConcreteValue.FloatValue;
            //     return left;
            // }
            Expression obj = new Expression();
            obj.LeftOperand = left;
            obj.Operator = Operator.Negative;
            // optimize expression
            if (left.Operator == Operator.AtomConcreteValue)
            {
                left.ConcreteValue = obj.Calculate(null);
                return left;
            }
            return obj;
        }

        public override object VisitExprXor([NotNull] MiniSQLParser.ExprXorContext context)
        {
            Expression left = (Expression)Visit(context.expr(0));
            Expression right = (Expression)Visit(context.expr(1));
            Expression obj = new Expression();
            obj.LeftOperand = left;
            obj.RightOperand = right;
            obj.Operator = Operator.Xor;
            // optimize expression
            if (left.Operator == Operator.AtomConcreteValue
                && right.Operator == Operator.AtomConcreteValue)
            {
                left.ConcreteValue = obj.Calculate(null);
                return left;
            }
            return obj;
        }

        public override object VisitFieldDefinition([NotNull] MiniSQLParser.FieldDefinitionContext context)
        {
            AttributeDeclaration declaration = (AttributeDeclaration)Visit(context.dataType());
            // workaround
            if (context.columnAttribute(0) != null)
                declaration.IsUnique = true;
            return declaration;
        }

        public override object VisitFieldIdentifier([NotNull] MiniSQLParser.FieldIdentifierContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitFieldLength([NotNull] MiniSQLParser.FieldLengthContext context)
        {
            return int.Parse(context.INT_NUMBER().GetText());
        }

        public override object VisitFromClause([NotNull] MiniSQLParser.FromClauseContext context)
        {
            return context.tableRef().GetText();
        }

        public override object VisitIdentifier([NotNull] MiniSQLParser.IdentifierContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitIndexName([NotNull] MiniSQLParser.IndexNameContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitIndexRef([NotNull] MiniSQLParser.IndexRefContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitInsertStatement([NotNull] MiniSQLParser.InsertStatementContext context)
        {
            InsertStatement obj = new InsertStatement();
            obj.TableName = context.tableRef().GetText();
            obj.Values = (List<AtomValue>)Visit(context.insertValues());
            return obj;
        }

        public override object VisitInsertValues([NotNull] MiniSQLParser.InsertValuesContext context)
        {
            return (List<AtomValue>)Visit(context.valueList());
        }

        public override object VisitKeyListVariants([NotNull] MiniSQLParser.KeyListVariantsContext context)
        {
            return context.keyPart().GetText();
        }

        // GetText()
        public override object VisitKeyPart([NotNull] MiniSQLParser.KeyPartContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitProg([NotNull] MiniSQLParser.ProgContext context)
        {
            Query obj = new Query();
            int count = 0;
            IStatement ret;
            while (context.simpleStatement(count) != null)
            {
                ret = (IStatement)Visit(context.simpleStatement(count));
                obj.StatementList.Add(ret);
                count++;
            }
            return obj;
        }

        public override object VisitPureIdentifier([NotNull] MiniSQLParser.PureIdentifierContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitQualifiedIdentifier([NotNull] MiniSQLParser.QualifiedIdentifierContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitQueryExpression([NotNull] MiniSQLParser.QueryExpressionContext context)
        {
            SelectStatement obj = new SelectStatement();
            if (context.whereClause() != null)
                obj.Condition = (Expression)Visit(context.whereClause());
            if (context.fromClause() != null)
                obj.FromTable = (string)Visit(context.fromClause());
            return obj;
        }

        public override object VisitQueryExpressionParens([NotNull] MiniSQLParser.QueryExpressionParensContext context)
        {
            SelectStatement obj = (SelectStatement)Visit(context.queryExpression());
            if (obj == null)
                obj = (SelectStatement)Visit(context.queryExpressionParens());
            return obj;
        }

        public override object VisitQuitStatement([NotNull] MiniSQLParser.QuitStatementContext context)
        {
            return new QuitStatement();
        }

        public override object VisitScientific([NotNull] MiniSQLParser.ScientificContext context)
        {
            AtomValue value = new AtomValue();
            if (context.INT_NUMBER() != null)
            {
                value.Type = AttributeTypes.Int;
                value.IntegerValue = int.Parse(context.INT_NUMBER().GetText());
            }
            else if (context.DECIMAL_NUMBER() != null)
            {
                value.Type = AttributeTypes.Float;
                value.FloatValue = float.Parse(context.DECIMAL_NUMBER().GetText());
            }
            else if (context.SINGLE_QUOTED_TEXT() != null)
            {
                value.Type = AttributeTypes.Char;
                string withQuotes = context.SINGLE_QUOTED_TEXT().GetText();
                value.StringValue = withQuotes.Substring(1, withQuotes.Length - 2);
                value.CharLimit = Encoding.UTF8.GetByteCount(value.StringValue);
            }
            else if (context.DOUBLE_QUOTED_TEXT() != null)
            {
                value.Type = AttributeTypes.Char;
                string withQuotes = context.DOUBLE_QUOTED_TEXT().GetText();
                value.StringValue = withQuotes.Substring(1, withQuotes.Length - 2);
                value.CharLimit = Encoding.UTF8.GetByteCount(value.StringValue);
            }
            else
                throw new System.NotImplementedException();
            return value;
        }

        public override object VisitSelectStatement([NotNull] MiniSQLParser.SelectStatementContext context)
        {
            if (context.queryExpression() != null)
                return (SelectStatement)Visit(context.queryExpression());
            else
                return (SelectStatement)Visit(context.queryExpressionParens());
        }

        public override object VisitShowTablesStatement([NotNull] MiniSQLParser.ShowTablesStatementContext context)
        {
            ShowStatement statement = new ShowStatement();
            statement.ShowType = ShowType.Table;
            return statement;
        }

        public override object VisitSimpleStatement([NotNull] MiniSQLParser.SimpleStatementContext context)
        {
            if (context.createStatement() != null)
                return (IStatement)Visit(context.createStatement());
            else if (context.deleteStatement() != null)
                return (IStatement)Visit(context.deleteStatement());
            else if (context.dropStatement() != null)
                return (IStatement)Visit(context.dropStatement());
            else if (context.execFileStatement() != null)
                return (IStatement)Visit(context.execFileStatement());
            else if (context.insertStatement() != null)
                return (IStatement)Visit(context.insertStatement());
            else if (context.quitStatement() != null)
                return (IStatement)Visit(context.quitStatement());
            else if (context.selectStatement() != null)
                return (IStatement)Visit(context.selectStatement());
            else if (context.showTablesStatement() != null)
                return (IStatement)Visit(context.showTablesStatement());
            else
                throw new System.NotImplementedException(); 
        }

        public override object VisitTableConstraintDef([NotNull] MiniSQLParser.TableConstraintDefContext context)
        {
            return (string)Visit(context.keyListVariants());
        }

        public override object VisitTableElement([NotNull] MiniSQLParser.TableElementContext context)
        {
            TableElement element = new TableElement();
            if (context.columnDefinition() != null)
            {
                element.Type = TableElementType.AttributeDeclaration;
                element.AttributeDeclaration = (AttributeDeclaration)Visit(context.columnDefinition());
            }
            else
            {
                element.Type = TableElementType.PrimaryKey;
                element.PrimaryKey = (string)Visit(context.tableConstraintDef());
            }
            return element;
        }

        public override object VisitTableElementList([NotNull] MiniSQLParser.TableElementListContext context)
        {
            List<AttributeDeclaration> list = new List<AttributeDeclaration>();
            string primaryKey = "";
            int count = 0;
            while (context.tableElement(count) != null)
            {
                TableElement element = (TableElement)Visit(context.tableElement(count));
                switch (element.Type)
                {
                    case TableElementType.AttributeDeclaration:
                        list.Add(element.AttributeDeclaration);
                        break;
                    case TableElementType.PrimaryKey:
                        primaryKey = element.PrimaryKey;
                        break;
                }
                count++;
            }
            return (list, primaryKey);
        }

        public override object VisitTableName([NotNull] MiniSQLParser.TableNameContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitTableRef([NotNull] MiniSQLParser.TableRefContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitValueList([NotNull] MiniSQLParser.ValueListContext context)
        {
            if (context.values() != null)
                return (List<AtomValue>)Visit(context.values());
            else
                return new List<AtomValue>();
        }

        public override object VisitValues([NotNull] MiniSQLParser.ValuesContext context)
        {
            List<AtomValue> values = new List<AtomValue>();
            int count = 0;
            while (context.expr(count) != null)
            {
                Expression expr = (Expression)Visit(context.expr(count));
                if (expr.Operator != Operator.AtomConcreteValue)
                {
                    throw new System.Exception("Value could not contain any variable");
                }
                values.Add(expr.ConcreteValue);
                count++;
            }
            return values;
        }

        public override object VisitVariable([NotNull] MiniSQLParser.VariableContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object VisitWhereClause([NotNull] MiniSQLParser.WhereClauseContext context)
        {
            Expression exp = (Expression)Visit(context.expr());
            return exp;
        }
    }
}
