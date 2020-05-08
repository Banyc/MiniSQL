//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from MiniSQL.g4 by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IMiniSQLListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class MiniSQLBaseListener : IMiniSQLListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.prog"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterProg([NotNull] MiniSQLParser.ProgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.prog"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitProg([NotNull] MiniSQLParser.ProgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.simpleStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimpleStatement([NotNull] MiniSQLParser.SimpleStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.simpleStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimpleStatement([NotNull] MiniSQLParser.SimpleStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.quitStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterQuitStatement([NotNull] MiniSQLParser.QuitStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.quitStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitQuitStatement([NotNull] MiniSQLParser.QuitStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.execFileStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExecFileStatement([NotNull] MiniSQLParser.ExecFileStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.execFileStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExecFileStatement([NotNull] MiniSQLParser.ExecFileStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.createStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCreateStatement([NotNull] MiniSQLParser.CreateStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.createStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCreateStatement([NotNull] MiniSQLParser.CreateStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.createTable"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCreateTable([NotNull] MiniSQLParser.CreateTableContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.createTable"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCreateTable([NotNull] MiniSQLParser.CreateTableContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.createIndex"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCreateIndex([NotNull] MiniSQLParser.CreateIndexContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.createIndex"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCreateIndex([NotNull] MiniSQLParser.CreateIndexContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.dropStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDropStatement([NotNull] MiniSQLParser.DropStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.dropStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDropStatement([NotNull] MiniSQLParser.DropStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.deleteStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDeleteStatement([NotNull] MiniSQLParser.DeleteStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.deleteStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDeleteStatement([NotNull] MiniSQLParser.DeleteStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.insertStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInsertStatement([NotNull] MiniSQLParser.InsertStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.insertStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInsertStatement([NotNull] MiniSQLParser.InsertStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.selectStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSelectStatement([NotNull] MiniSQLParser.SelectStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.selectStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSelectStatement([NotNull] MiniSQLParser.SelectStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.queryExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterQueryExpression([NotNull] MiniSQLParser.QueryExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.queryExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitQueryExpression([NotNull] MiniSQLParser.QueryExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.fromClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFromClause([NotNull] MiniSQLParser.FromClauseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.fromClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFromClause([NotNull] MiniSQLParser.FromClauseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.queryExpressionParens"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterQueryExpressionParens([NotNull] MiniSQLParser.QueryExpressionParensContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.queryExpressionParens"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitQueryExpressionParens([NotNull] MiniSQLParser.QueryExpressionParensContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.dropIndex"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDropIndex([NotNull] MiniSQLParser.DropIndexContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.dropIndex"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDropIndex([NotNull] MiniSQLParser.DropIndexContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.dropTable"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDropTable([NotNull] MiniSQLParser.DropTableContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.dropTable"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDropTable([NotNull] MiniSQLParser.DropTableContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.insertValues"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInsertValues([NotNull] MiniSQLParser.InsertValuesContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.insertValues"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInsertValues([NotNull] MiniSQLParser.InsertValuesContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.valueList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterValueList([NotNull] MiniSQLParser.ValueListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.valueList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitValueList([NotNull] MiniSQLParser.ValueListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>values</c>
	/// labeled alternative in <see cref="MiniSQLParser.exprexprexprexprexprexprexprexprexprexpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterValues([NotNull] MiniSQLParser.ValuesContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>values</c>
	/// labeled alternative in <see cref="MiniSQLParser.exprexprexprexprexprexprexprexprexprexpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitValues([NotNull] MiniSQLParser.ValuesContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.whereClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhereClause([NotNull] MiniSQLParser.WhereClauseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.whereClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhereClause([NotNull] MiniSQLParser.WhereClauseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.tableElementList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTableElementList([NotNull] MiniSQLParser.TableElementListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.tableElementList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTableElementList([NotNull] MiniSQLParser.TableElementListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.tableElement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTableElement([NotNull] MiniSQLParser.TableElementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.tableElement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTableElement([NotNull] MiniSQLParser.TableElementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.tableConstraintDef"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTableConstraintDef([NotNull] MiniSQLParser.TableConstraintDefContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.tableConstraintDef"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTableConstraintDef([NotNull] MiniSQLParser.TableConstraintDefContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.columnDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterColumnDefinition([NotNull] MiniSQLParser.ColumnDefinitionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.columnDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitColumnDefinition([NotNull] MiniSQLParser.ColumnDefinitionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.fieldDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldDefinition([NotNull] MiniSQLParser.FieldDefinitionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.fieldDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldDefinition([NotNull] MiniSQLParser.FieldDefinitionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.dataType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDataType([NotNull] MiniSQLParser.DataTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.dataType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDataType([NotNull] MiniSQLParser.DataTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.fieldLength"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldLength([NotNull] MiniSQLParser.FieldLengthContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.fieldLength"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldLength([NotNull] MiniSQLParser.FieldLengthContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.columnAttribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterColumnAttribute([NotNull] MiniSQLParser.ColumnAttributeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.columnAttribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitColumnAttribute([NotNull] MiniSQLParser.ColumnAttributeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.createIndexTarget"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCreateIndexTarget([NotNull] MiniSQLParser.CreateIndexTargetContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.createIndexTarget"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCreateIndexTarget([NotNull] MiniSQLParser.CreateIndexTargetContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.keyListVariants"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterKeyListVariants([NotNull] MiniSQLParser.KeyListVariantsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.keyListVariants"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitKeyListVariants([NotNull] MiniSQLParser.KeyListVariantsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.tableName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTableName([NotNull] MiniSQLParser.TableNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.tableName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTableName([NotNull] MiniSQLParser.TableNameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.columnName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterColumnName([NotNull] MiniSQLParser.ColumnNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.columnName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitColumnName([NotNull] MiniSQLParser.ColumnNameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.keyPart"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterKeyPart([NotNull] MiniSQLParser.KeyPartContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.keyPart"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitKeyPart([NotNull] MiniSQLParser.KeyPartContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.indexRef"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIndexRef([NotNull] MiniSQLParser.IndexRefContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.indexRef"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIndexRef([NotNull] MiniSQLParser.IndexRefContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.tableRef"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTableRef([NotNull] MiniSQLParser.TableRefContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.tableRef"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTableRef([NotNull] MiniSQLParser.TableRefContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.fieldIdentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldIdentifier([NotNull] MiniSQLParser.FieldIdentifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.fieldIdentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldIdentifier([NotNull] MiniSQLParser.FieldIdentifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.indexName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIndexName([NotNull] MiniSQLParser.IndexNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.indexName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIndexName([NotNull] MiniSQLParser.IndexNameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.pureIdentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPureIdentifier([NotNull] MiniSQLParser.PureIdentifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.pureIdentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPureIdentifier([NotNull] MiniSQLParser.PureIdentifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.identifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIdentifier([NotNull] MiniSQLParser.IdentifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.identifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIdentifier([NotNull] MiniSQLParser.IdentifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.qualifiedIdentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterQualifiedIdentifier([NotNull] MiniSQLParser.QualifiedIdentifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.qualifiedIdentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitQualifiedIdentifier([NotNull] MiniSQLParser.QualifiedIdentifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprAtom</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprAtom([NotNull] MiniSQLParser.ExprAtomContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprAtom</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprAtom([NotNull] MiniSQLParser.ExprAtomContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprOr</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprOr([NotNull] MiniSQLParser.ExprOrContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprOr</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprOr([NotNull] MiniSQLParser.ExprOrContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprNot</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprNot([NotNull] MiniSQLParser.ExprNotContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprNot</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprNot([NotNull] MiniSQLParser.ExprNotContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprMul</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprMul([NotNull] MiniSQLParser.ExprMulContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprMul</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprMul([NotNull] MiniSQLParser.ExprMulContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprPar</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprPar([NotNull] MiniSQLParser.ExprParContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprPar</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprPar([NotNull] MiniSQLParser.ExprParContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprAdd</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprAdd([NotNull] MiniSQLParser.ExprAddContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprAdd</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprAdd([NotNull] MiniSQLParser.ExprAddContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprAnd</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprAnd([NotNull] MiniSQLParser.ExprAndContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprAnd</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprAnd([NotNull] MiniSQLParser.ExprAndContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprXor</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprXor([NotNull] MiniSQLParser.ExprXorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprXor</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprXor([NotNull] MiniSQLParser.ExprXorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprSign</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprSign([NotNull] MiniSQLParser.ExprSignContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprSign</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprSign([NotNull] MiniSQLParser.ExprSignContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>exprCompare</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprCompare([NotNull] MiniSQLParser.ExprCompareContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>exprCompare</c>
	/// labeled alternative in <see cref="MiniSQLParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprCompare([NotNull] MiniSQLParser.ExprCompareContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.compOp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCompOp([NotNull] MiniSQLParser.CompOpContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.compOp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCompOp([NotNull] MiniSQLParser.CompOpContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.atom"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAtom([NotNull] MiniSQLParser.AtomContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.atom"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAtom([NotNull] MiniSQLParser.AtomContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.scientific"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterScientific([NotNull] MiniSQLParser.ScientificContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.scientific"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitScientific([NotNull] MiniSQLParser.ScientificContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="MiniSQLParser.variable"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVariable([NotNull] MiniSQLParser.VariableContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="MiniSQLParser.variable"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVariable([NotNull] MiniSQLParser.VariableContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
