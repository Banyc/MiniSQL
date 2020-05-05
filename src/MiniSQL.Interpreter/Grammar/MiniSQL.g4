// credit - <https://github.com/mysql/mysql-workbench/blob/8.0/library/parsers/grammars/>

grammar MiniSQL;

// available statements:

// create table student (
// 		sno char(8),
// 		sname char(16) unique,
// 		sage int,
// 		sgender char (1),
// 		primary key ( sno )
// );

// drop table student;

// create index stunameidx on student ( sname );

// drop index stunameidx;

// select * from student;
// select * from student where sno = ‘88888888’;
// select * from student where sage > 20 and sgender = ‘F’;

// insert into student values (‘12345678’,’wy’,22,’M’);

// delete from student;
// delete from student where sno = ‘88888888’;

// quit;

// TODO:
// execfile 文件名 ;


// parser

query:
    EOF
    | (simpleStatement | beginWork) (SEMICOLON_SYMBOL EOF? | EOF)
;

simpleStatement:
    // DDL
    | createStatement
    | dropStatement

    // DML
    | deleteStatement
    | insertStatement
    | selectStatement
;

createStatement:
    CREATE_SYMBOL (
        createTable
        | createIndex
    )
;

createTable:
    TABLE_SYMBOL tableName (
        (OPEN_PAR_SYMBOL tableElementList CLOSE_PAR_SYMBOL)?
    )
;

createIndex:
    (
        UNIQUE_SYMBOL? type = INDEX_SYMBOL (
            indexName
        ) createIndexTarget
    )
;

dropStatement:
    DROP_SYMBOL (
        dropIndex
        | dropTable
    )
;

deleteStatement:
    DELETE_SYMBOL (
        FROM_SYMBOL (
            tableRef
                whereClause?
        )
    )
;

insertStatement:
    INSERT_SYMBOL INTO_SYMBOL? tableRef (
        insertFromConstructor
    )
;

selectStatement:
    queryExpression
    | queryExpressionParens
;

// BEGIN WORK is separated from transactional statements as it must not appear as part of a stored program.
beginWork:
    BEGIN_SYMBOL WORK_SYMBOL?
;


queryExpression:
     (
        queryExpressionBody orderClause?
     //    | queryExpressionParens (orderClause limitClause? | limitClause)
    )
;

orderClause:
    ORDER_SYMBOL BY_SYMBOL orderList
;

orderList:
    orderExpression (COMMA_SYMBOL orderExpression)*
;

orderExpression:
    expr direction?
;

direction:
    ASC_SYMBOL
    | DESC_SYMBOL
;


queryExpressionBody:
    querySpecification
;

querySpecification:
    SELECT_SYMBOL selectItemList fromClause? whereClause?
;

// selectItemList: (selectItem | MULT_OPERATOR) (COMMA_SYMBOL selectItem)*
// ;
selectItemList: MULT_OPERATOR
;

selectItem:
    expr selectAlias?
;

selectAlias:
    AS_SYMBOL? (identifier | textStringLiteral)
;

// TEXT_STRING_sys + TEXT_STRING_literal + TEXT_STRING_filesystem + TEXT_STRING + TEXT_STRING_password +
// TEXT_STRING_validated in sql_yacc.yy.
textStringLiteral:
    value = SINGLE_QUOTED_TEXT
;

fromClause:
    FROM_SYMBOL (tableReferenceList)
;

tableReferenceList:
    tableReference (COMMA_SYMBOL tableReference)*
;

tableReference: ( // Note: we have also a tableRef rule for identifiers that reference a table anywhere.
        tableFactor
    )
;

/**
  MySQL has a syntax extension where a comma-separated list of table
  references is allowed as a table reference in itself, for instance
    SELECT * FROM (t1, t2) JOIN t3 ON 1
  which is not allowed in standard SQL. The syntax is equivalent to
    SELECT * FROM (t1 CROSS JOIN t2) JOIN t3 ON 1
  We call this rule tableReferenceListParens.
*/
tableFactor:
    singleTable
;

singleTable:
    tableRef
;

queryExpressionParens:
    OPEN_PAR_SYMBOL (queryExpressionParens | queryExpression) CLOSE_PAR_SYMBOL
;

dropIndex:
    type = INDEX_SYMBOL indexRef ON_SYMBOL tableRef
;

dropTable:
    type = (TABLE_SYMBOL | TABLES_SYMBOL) tableRefList
;

tableRefList:
    tableRef (COMMA_SYMBOL tableRef)*
;

insertFromConstructor:
    insertValues
;

insertValues:
    (VALUES_SYMBOL | VALUE_SYMBOL) valueList
;

valueList:
    OPEN_PAR_SYMBOL values? CLOSE_PAR_SYMBOL (
        COMMA_SYMBOL OPEN_PAR_SYMBOL values? CLOSE_PAR_SYMBOL
    )*
;

values:
    (expr) (COMMA_SYMBOL (expr))*
;


whereClause:
    WHERE_SYMBOL expr
;


tableName:
    qualifiedIdentifier
    | dotIdentifier
;

tableElementList:
    tableElement (COMMA_SYMBOL tableElement)*
;

tableElement:
    columnDefinition
    | tableConstraintDef
;

tableConstraintDef:
//     constraintName? (
    (
        (type = PRIMARY_SYMBOL KEY_SYMBOL) keyListVariants
    )
;


columnDefinition:
    columnName fieldDefinition
;

columnName:
    fieldIdentifier
;

fieldDefinition:
    dataType (
        columnAttribute*
    )
;

// int char(n) float
dataType: // type in sql_yacc.yy
    type = (
        INT_SYMBOL
    )
    | type = (FLOAT_SYMBOL)
    | type = CHAR_SYMBOL fieldLength?
;

fieldLength:
    OPEN_PAR_SYMBOL (real_ulonglong_number) CLOSE_PAR_SYMBOL
;

real_ulonglong_number:
    INT_NUMBER
;

columnAttribute:
//     NOT_SYMBOL? nullLiteral
//     | value = AUTO_INCREMENT_SYMBOL
//     | PRIMARY_SYMBOL? value = KEY_SYMBOL
    value = UNIQUE_SYMBOL KEY_SYMBOL?
;

createIndexTarget:
    ON_SYMBOL tableRef keyListVariants
;

keyListVariants:
    keyList
;

keyList:
    OPEN_PAR_SYMBOL keyPart (COMMA_SYMBOL keyPart)* CLOSE_PAR_SYMBOL
;

keyPart:
    identifier fieldLength? direction?
;

indexRef: // Always internal reference. Still all qualification variations are accepted.
    fieldIdentifier
;

tableRef:
    qualifiedIdentifier
    | dotIdentifier
;

// A name for a field (column/index). Can be qualified with the current schema + table (although it's not a reference).
fieldIdentifier:
    dotIdentifier
    | qualifiedIdentifier dotIdentifier?
;

// This rule encapsulates the frequently used dot + identifier sequence, which also requires a special
// treatment in the lexer. See there in the DOT_IDENTIFIER rule.
dotIdentifier:
    DOT_SYMBOL identifier
;

indexName:
    identifier
;

// Identifiers excluding keywords (except if they are quoted). IDENT_sys in sql_yacc.yy.
pureIdentifier:
    (IDENTIFIER)
;

// Identifiers including a certain set of keywords, which are allowed also if not quoted.
// ident in sql_yacc.yy
identifier:
    pureIdentifier
;

qualifiedIdentifier:
    identifier dotIdentifier?
;

expr:
    boolPri (IS_SYMBOL notRule? type = (TRUE_SYMBOL | FALSE_SYMBOL | UNKNOWN_SYMBOL))? # exprIs
    | NOT_SYMBOL expr                                                                  # exprNot
    | expr op = (AND_SYMBOL | LOGICAL_AND_OPERATOR) expr                               # exprAnd
    | expr XOR_SYMBOL expr                                                             # exprXor
    | expr op = (OR_SYMBOL | LOGICAL_OR_OPERATOR) expr                                 # exprOr
;

// lexer

fragment A: [aA];
fragment B: [bB];
fragment C: [cC];
fragment D: [dD];
fragment E: [eE];
fragment F: [fF];
fragment G: [gG];
fragment H: [hH];
fragment I: [iI];
fragment J: [jJ];
fragment K: [kK];
fragment L: [lL];
fragment M: [mM];
fragment N: [nN];
fragment O: [oO];
fragment P: [pP];
fragment Q: [qQ];
fragment R: [rR];
fragment S: [sS];
fragment T: [tT];
fragment U: [uU];
fragment V: [vV];
fragment W: [wW];
fragment X: [xX];
fragment Y: [yY];
fragment Z: [zZ];


fragment DIGIT:    [0-9];
fragment DIGITS:   DIGIT+;
fragment HEXDIGIT: [0-9a-fA-F];

// Only lower case 'x' and 'b' count for hex + bin numbers. Otherwise it's an identifier.
HEX_NUMBER: ('0x' HEXDIGIT+) | ('x\'' HEXDIGIT+ '\'');
BIN_NUMBER: ('0b' [01]+) | ('b\'' [01]+ '\'');

INT_NUMBER: DIGITS { setType(determineNumericType(getText())); };

// Float types must be handled first or the DOT_IDENTIIFER rule will make them to identifiers
// (if there is no leading digit before the dot).
DECIMAL_NUMBER: DIGITS? DOT_SYMBOL DIGITS;
FLOAT_NUMBER:   (DIGITS? DOT_SYMBOL)? DIGITS [eE] (MINUS_OPERATOR | PLUS_OPERATOR)? DIGITS;

// Special rule that should also match all keywords if they are directly preceded by a dot.
// Hence it's defined before all keywords.
// Here we make use of the ability in our base lexer to emit multiple tokens with a single rule.
DOT_IDENTIFIER:
    DOT_SYMBOL LETTER_WHEN_UNQUOTED_NO_DIGIT LETTER_WHEN_UNQUOTED* { emitDot(); } -> type(IDENTIFIER)
;


ASC_SYMBOL:                      A S C;                                      // SQL-2003-N
AS_SYMBOL:                       A S;                                        // SQL-2003-R
BEGIN_SYMBOL:                    B E G I N;                                  // SQL-2003-R
BY_SYMBOL:                       B Y;                                        // SQL-2003-R
CHAR_SYMBOL:                     C H A R;                                    // SQL-2003-R
CREATE_SYMBOL:                   C R E A T E;                                // SQL-2003-R
DELETE_SYMBOL:                   D E L E T E;                                // SQL-2003-R
DESC_SYMBOL:                     D E S C;                                    // SQL-2003-N
DROP_SYMBOL:                     D R O P;                                    // SQL-2003-R
FLOAT_SYMBOL:                    F L O A T;                                  // SQL-2003-R
FROM_SYMBOL:                     F R O M;
INDEX_SYMBOL:                    I N D E X;
INSERT_SYMBOL:                   I N S E R T;                                // SQL-2003-R
INTO_SYMBOL:                     I N T O;                                    // SQL-2003-R
INT_SYMBOL:                      I N T;                                      // SQL-2003-R
KEY_SYMBOL:                      K E Y;                                      // SQL-2003-N
ON_SYMBOL:                       O N;                                        // SQL-2003-R
ORDER_SYMBOL:                    O R D E R;                                  // SQL-2003-R
PRIMARY_SYMBOL:                  P R I M A R Y;                              // SQL-2003-R
SELECT_SYMBOL:                   S E L E C T;                                // SQL-2003-R
TABLES_SYMBOL:                   T A B L E S;
TABLE_SYMBOL:                    T A B L E;                                  // SQL-2003-R
UNIQUE_SYMBOL:                   U N I Q U E;
VALUES_SYMBOL:                   V A L U E S;                                // SQL-2003-R
VALUE_SYMBOL:                    V A L U E;                                  // SQL-2003-R
WHERE_SYMBOL:                    W H E R E;                                  // SQL-2003-R
WORK_SYMBOL:                     W O R K;                                    // SQL-2003-N

// Operators
EQUAL_OPERATOR:            '='; // Also assign.
ASSIGN_OPERATOR:           ':=';
NULL_SAFE_EQUAL_OPERATOR:  '<=>';
GREATER_OR_EQUAL_OPERATOR: '>=';
GREATER_THAN_OPERATOR:     '>';
LESS_OR_EQUAL_OPERATOR:    '<=';
LESS_THAN_OPERATOR:        '<';
NOT_EQUAL_OPERATOR:        '!=';
NOT_EQUAL2_OPERATOR:       '<>' -> type(NOT_EQUAL_OPERATOR);

PLUS_OPERATOR:  '+';
MINUS_OPERATOR: '-';
MULT_OPERATOR:  '*';
DIV_OPERATOR:   '/';

MOD_OPERATOR: '%';

LOGICAL_NOT_OPERATOR: '!';
BITWISE_NOT_OPERATOR: '~';

SHIFT_LEFT_OPERATOR:  '<<';
SHIFT_RIGHT_OPERATOR: '>>';

LOGICAL_AND_OPERATOR: '&&';
BITWISE_AND_OPERATOR: '&';

BITWISE_XOR_OPERATOR: '^';

LOGICAL_OR_OPERATOR:
    '||' { setType(isSqlModeActive(PipesAsConcat) ? CONCAT_PIPES_SYMBOL : LOGICAL_OR_OPERATOR); }
;
BITWISE_OR_OPERATOR: '|';

DOT_SYMBOL:         '.';
COMMA_SYMBOL:       ',';
SEMICOLON_SYMBOL:   ';';
COLON_SYMBOL:       ':';
OPEN_PAR_SYMBOL:    '(';
CLOSE_PAR_SYMBOL:   ')';
OPEN_CURLY_SYMBOL:  '{';
CLOSE_CURLY_SYMBOL: '}';
UNDERLINE_SYMBOL:   '_';




// Identifiers might start with a digit, even though it is discouraged, and may not consist entirely of digits only.
// All keywords above are automatically excluded.
IDENTIFIER:
    DIGITS+ [eE] (LETTER_WHEN_UNQUOTED_NO_DIGIT LETTER_WHEN_UNQUOTED*)? // Have to exclude float pattern, as this rule matches more.
    | DIGITS+ LETTER_WITHOUT_FLOAT_PART LETTER_WHEN_UNQUOTED*
    | LETTER_WHEN_UNQUOTED_NO_DIGIT LETTER_WHEN_UNQUOTED* // INT_NUMBER matches first if there are only digits.
;

NCHAR_TEXT: [nN] SINGLE_QUOTED_TEXT;

// MySQL supports automatic concatenation of multiple single and double quoted strings if they follow each other as separate
// tokens. This is reflected in the `textLiteral` parser rule.
// Here we handle duplication of quotation chars only (which must be replaced by a single char in the target code).

fragment BACK_TICK:    '`';
fragment SINGLE_QUOTE: '\'';
fragment DOUBLE_QUOTE: '"';

BACK_TICK_QUOTED_ID: BACK_TICK (({!isSqlModeActive(NoBackslashEscapes)}? '\\')? .)*? BACK_TICK;

DOUBLE_QUOTED_TEXT: (
        DOUBLE_QUOTE (({!isSqlModeActive(NoBackslashEscapes)}? '\\' .)? .)*? DOUBLE_QUOTE
    )+
;

SINGLE_QUOTED_TEXT: (
        SINGLE_QUOTE (({!isSqlModeActive(NoBackslashEscapes)}? '\\')? .)*? SINGLE_QUOTE
    )+
;

// There are 3 types of block comments:
// /* ... */ - The standard multi line comment.
// /*! ... */ - A comment used to mask code for other clients. In MySQL the content is handled as normal code.
// /*!12345 ... */ - Same as the previous one except code is only used when the given number is lower
//                   than the current server version (specifying so the minimum server version the code can run with).
VERSION_COMMENT_START: ('/*!' DIGITS) (
        {checkVersion(getText())}? // Will set inVersionComment if the number matches.
        | .*? '*/'
    ) -> channel(HIDDEN)
;

// inVersionComment is a variable in the base lexer.
// TODO: use a lexer mode instead of a member variable.
MYSQL_COMMENT_START: '/*!' { inVersionComment = true; }                     -> channel(HIDDEN);
VERSION_COMMENT_END: '*/' {inVersionComment}? { inVersionComment = false; } -> channel(HIDDEN);
BLOCK_COMMENT:       ( '/**/' | '/*' ~[!] .*? '*/')                         -> channel(HIDDEN);

POUND_COMMENT:    '#' ~([\n\r])*                                   -> channel(HIDDEN);
DASHDASH_COMMENT: DOUBLE_DASH ([ \t] (~[\n\r])* | LINEBREAK | EOF) -> channel(HIDDEN);

fragment DOUBLE_DASH: '--';
fragment LINEBREAK:   [\n\r];

fragment SIMPLE_IDENTIFIER: (DIGIT | [a-zA-Z_$] | DOT_SYMBOL)+;

fragment ML_COMMENT_HEAD: '/*';
fragment ML_COMMENT_END:  '*/';

// As defined in https://dev.mysql.com/doc/refman/8.0/en/identifiers.html.
fragment LETTER_WHEN_UNQUOTED: DIGIT | LETTER_WHEN_UNQUOTED_NO_DIGIT;

fragment LETTER_WHEN_UNQUOTED_NO_DIGIT: [a-zA-Z_$\u0080-\uffff];

// Any letter but without e/E and digits (which are used to match a decimal number).
fragment LETTER_WITHOUT_FLOAT_PART: [a-df-zA-DF-Z_$\u0080-\uffff];
