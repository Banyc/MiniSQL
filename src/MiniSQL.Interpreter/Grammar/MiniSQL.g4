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
// create unique index stunameidx on student ( sname );

// drop index stunameidx;

// select * from student;
// select * from student where sno = ‘88888888’;
// select * from student where sage > 20 and sgender = ‘F’;

// insert into student values (‘12345678’,’wy’,22,’M’);

// delete from student;
// delete from student where sno = ‘88888888’;

// quit;

// execfile 文件名 ;


// parser

prog:
    EOF
    | (simpleStatement SEMICOLON_SYMBOL)+ EOF
;

simpleStatement:
    // DDL
    | createStatement
    | dropStatement

    // DML
    | deleteStatement
    | insertStatement
    | selectStatement

    | quitStatement
    | execFileStatement
    | showTablesStatement
;

quitStatement:
    QUIT_SYMBOL
    ;

execFileStatement:
    EXECFILE_SYMBOL (SINGLE_QUOTED_TEXT | DOUBLE_QUOTED_TEXT)
;

showTablesStatement:
    SHOW_SYMBOL TABLES_SYMBOL
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
        UNIQUE_SYMBOL? type = INDEX_SYMBOL indexName createIndexTarget
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
        insertValues
    )
;

selectStatement:
    queryExpression
    | queryExpressionParens
;

queryExpression:
    SELECT_SYMBOL MULT_OPERATOR fromClause? whereClause?
;

fromClause:
    FROM_SYMBOL tableRef
;

queryExpressionParens:
    OPEN_PAR_SYMBOL (queryExpressionParens | queryExpression) CLOSE_PAR_SYMBOL
;

dropIndex:
    type = INDEX_SYMBOL indexRef (ON_SYMBOL tableRef)?
;

dropTable:
    type = (TABLE_SYMBOL | TABLES_SYMBOL) tableRef  // drop one table at a time
;

insertValues:
    (VALUES_SYMBOL | VALUE_SYMBOL) valueList
;

valueList:
    OPEN_PAR_SYMBOL values? CLOSE_PAR_SYMBOL
;

values:
    (expr) (COMMA_SYMBOL (expr))*
;

whereClause:
    WHERE_SYMBOL expr
;

tableElementList:
    tableElement (COMMA_SYMBOL tableElement)*
;

tableElement:
    columnDefinition
    | tableConstraintDef
;

// only accept the last primary key
tableConstraintDef:
    (type = PRIMARY_SYMBOL KEY_SYMBOL) keyListVariants
;

columnDefinition:
    columnName fieldDefinition
;

fieldDefinition:
    dataType columnAttribute*
;

// int char(n) float
dataType:
    type = INT_SYMBOL
    | type = FLOAT_SYMBOL
    | type = CHAR_SYMBOL fieldLength?
;

fieldLength:
    OPEN_PAR_SYMBOL (INT_NUMBER) CLOSE_PAR_SYMBOL
;

columnAttribute:
    value = UNIQUE_SYMBOL KEY_SYMBOL?
;

createIndexTarget:
    ON_SYMBOL tableRef keyListVariants
;

keyListVariants:
    OPEN_PAR_SYMBOL keyPart CLOSE_PAR_SYMBOL  // each create index/primary key could only target one column
;


//-----------------  identifiers - handled by `GetText()` ----------------- 

tableName:
    qualifiedIdentifier
;

columnName:
    fieldIdentifier
;

keyPart:
    identifier
;

indexRef: // Always internal reference. Still all qualification variations are accepted.
    fieldIdentifier
;

tableRef:
    qualifiedIdentifier
;

// A name for a field (column/index). Can be qualified with the current schema + table (although it's not a reference).
fieldIdentifier:
    qualifiedIdentifier
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
    identifier
;

//----------------- Expression support ---------------------------------------------------------------------------------

expr:
    OPEN_PAR_SYMBOL expr CLOSE_PAR_SYMBOL                                              # exprPar
    | (PLUS_OPERATOR | MINUS_OPERATOR) expr                                            # exprSign  // review: to check if it conflict with exprAdd
    | NOT_SYMBOL expr                                                                  # exprNot
    | expr op = (MULT_OPERATOR | DIV_OPERATOR)  expr                                   # exprMul
    | expr op = (PLUS_OPERATOR | MINUS_OPERATOR) expr                                  # exprAdd
    | expr op = compOp expr                                                            # exprCompare
    | expr op = (AND_SYMBOL | LOGICAL_AND_OPERATOR) expr                               # exprAnd
    | expr XOR_SYMBOL expr                                                             # exprXor
    | expr op = (OR_SYMBOL | LOGICAL_OR_OPERATOR) expr                                 # exprOr
    | atom                                                                             # exprAtom
;

compOp:
    EQUAL_OPERATOR
    | GREATER_OR_EQUAL_OPERATOR
    | GREATER_THAN_OPERATOR
    | LESS_OR_EQUAL_OPERATOR
    | LESS_THAN_OPERATOR
    | NOT_EQUAL_OPERATOR
;

atom
   : scientific
   | variable
   ;

scientific:
    INT_NUMBER
    | DECIMAL_NUMBER
    | SINGLE_QUOTED_TEXT
    | DOUBLE_QUOTED_TEXT
    ;

variable:
    identifier
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

INT_NUMBER: DIGITS;

// Float types must be handled first or the DOT_IDENTIIFER rule will make them to identifiers
// (if there is no leading digit before the dot).
DECIMAL_NUMBER: DIGITS? DOT_SYMBOL DIGITS;
FLOAT_NUMBER:   (DIGITS? DOT_SYMBOL)? DIGITS [eE] (MINUS_OPERATOR | PLUS_OPERATOR)? DIGITS;


SHOW_SYMBOL:                     S H O W;                                    // Custom
EXECFILE_SYMBOL:                 E X E C F I L E;                            // Custom
QUIT_SYMBOL:                     Q U I T;                                    // Custom
AND_SYMBOL:                      A N D;                                      // SQL-2003-R
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
NOT_SYMBOL:                      N O T;                                                                            // SQL-2003-R
ON_SYMBOL:                       O N;                                        // SQL-2003-R
ORDER_SYMBOL:                    O R D E R;                                  // SQL-2003-R
OR_SYMBOL:                       O R;                                        // SQL-2003-R
PRIMARY_SYMBOL:                  P R I M A R Y;                              // SQL-2003-R
SELECT_SYMBOL:                   S E L E C T;                                // SQL-2003-R
TABLES_SYMBOL:                   T A B L E S;
TABLE_SYMBOL:                    T A B L E;                                  // SQL-2003-R
UNIQUE_SYMBOL:                   U N I Q U E;
VALUES_SYMBOL:                   V A L U E S;                                // SQL-2003-R
VALUE_SYMBOL:                    V A L U E;                                  // SQL-2003-R
WHERE_SYMBOL:                    W H E R E;                                  // SQL-2003-R
WORK_SYMBOL:                     W O R K;                                    // SQL-2003-N
XOR_SYMBOL:                      X O R;


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
    '||'
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


// White space handling
WHITESPACE: [ \t\f\r\n] -> channel(HIDDEN); // Ignore whitespaces.

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

DOUBLE_QUOTED_TEXT: (
        DOUBLE_QUOTE (('\\' .)? .)*? DOUBLE_QUOTE
    )+
;

SINGLE_QUOTED_TEXT: (
        SINGLE_QUOTE (('\\')? .)*? SINGLE_QUOTE
    )+
;

// inVersionComment is a variable in the base lexer.
// TODO: use a lexer mode instead of a member variable.
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
