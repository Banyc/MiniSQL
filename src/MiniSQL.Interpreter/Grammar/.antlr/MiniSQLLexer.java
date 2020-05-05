// Generated from d:\MEGAsync\Coding\DotNet\ConsoleApp\MiniSQL\src\MiniSQL.Interpreter\Grammar\MiniSQL.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class MiniSQLLexer extends Lexer {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		HEX_NUMBER=1, BIN_NUMBER=2, INT_NUMBER=3, DECIMAL_NUMBER=4, FLOAT_NUMBER=5, 
		ASC_SYMBOL=6, AS_SYMBOL=7, BEGIN_SYMBOL=8, BY_SYMBOL=9, CHAR_SYMBOL=10, 
		CREATE_SYMBOL=11, DELETE_SYMBOL=12, DESC_SYMBOL=13, DROP_SYMBOL=14, FLOAT_SYMBOL=15, 
		FROM_SYMBOL=16, INDEX_SYMBOL=17, INSERT_SYMBOL=18, INTO_SYMBOL=19, INT_SYMBOL=20, 
		KEY_SYMBOL=21, ON_SYMBOL=22, ORDER_SYMBOL=23, PRIMARY_SYMBOL=24, SELECT_SYMBOL=25, 
		TABLES_SYMBOL=26, TABLE_SYMBOL=27, UNIQUE_SYMBOL=28, VALUES_SYMBOL=29, 
		VALUE_SYMBOL=30, WHERE_SYMBOL=31, WORK_SYMBOL=32, EQUAL_OPERATOR=33, ASSIGN_OPERATOR=34, 
		NULL_SAFE_EQUAL_OPERATOR=35, GREATER_OR_EQUAL_OPERATOR=36, GREATER_THAN_OPERATOR=37, 
		LESS_OR_EQUAL_OPERATOR=38, LESS_THAN_OPERATOR=39, NOT_EQUAL_OPERATOR=40, 
		PLUS_OPERATOR=41, MINUS_OPERATOR=42, MULT_OPERATOR=43, DIV_OPERATOR=44, 
		MOD_OPERATOR=45, LOGICAL_NOT_OPERATOR=46, BITWISE_NOT_OPERATOR=47, SHIFT_LEFT_OPERATOR=48, 
		SHIFT_RIGHT_OPERATOR=49, LOGICAL_AND_OPERATOR=50, BITWISE_AND_OPERATOR=51, 
		BITWISE_XOR_OPERATOR=52, LOGICAL_OR_OPERATOR=53, BITWISE_OR_OPERATOR=54, 
		DOT_SYMBOL=55, COMMA_SYMBOL=56, SEMICOLON_SYMBOL=57, COLON_SYMBOL=58, 
		OPEN_PAR_SYMBOL=59, CLOSE_PAR_SYMBOL=60, OPEN_CURLY_SYMBOL=61, CLOSE_CURLY_SYMBOL=62, 
		UNDERLINE_SYMBOL=63, IDENTIFIER=64, NCHAR_TEXT=65, BACK_TICK_QUOTED_ID=66, 
		DOUBLE_QUOTED_TEXT=67, SINGLE_QUOTED_TEXT=68, VERSION_COMMENT_START=69, 
		MYSQL_COMMENT_START=70, VERSION_COMMENT_END=71, BLOCK_COMMENT=72, POUND_COMMENT=73, 
		DASHDASH_COMMENT=74, NOT_EQUAL2_OPERATOR=75;
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	public static final String[] ruleNames = {
		"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", 
		"O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "DIGIT", "DIGITS", 
		"HEXDIGIT", "HEX_NUMBER", "BIN_NUMBER", "INT_NUMBER", "DECIMAL_NUMBER", 
		"FLOAT_NUMBER", "DOT_IDENTIFIER", "ASC_SYMBOL", "AS_SYMBOL", "BEGIN_SYMBOL", 
		"BY_SYMBOL", "CHAR_SYMBOL", "CREATE_SYMBOL", "DELETE_SYMBOL", "DESC_SYMBOL", 
		"DROP_SYMBOL", "FLOAT_SYMBOL", "FROM_SYMBOL", "INDEX_SYMBOL", "INSERT_SYMBOL", 
		"INTO_SYMBOL", "INT_SYMBOL", "KEY_SYMBOL", "ON_SYMBOL", "ORDER_SYMBOL", 
		"PRIMARY_SYMBOL", "SELECT_SYMBOL", "TABLES_SYMBOL", "TABLE_SYMBOL", "UNIQUE_SYMBOL", 
		"VALUES_SYMBOL", "VALUE_SYMBOL", "WHERE_SYMBOL", "WORK_SYMBOL", "EQUAL_OPERATOR", 
		"ASSIGN_OPERATOR", "NULL_SAFE_EQUAL_OPERATOR", "GREATER_OR_EQUAL_OPERATOR", 
		"GREATER_THAN_OPERATOR", "LESS_OR_EQUAL_OPERATOR", "LESS_THAN_OPERATOR", 
		"NOT_EQUAL_OPERATOR", "NOT_EQUAL2_OPERATOR", "PLUS_OPERATOR", "MINUS_OPERATOR", 
		"MULT_OPERATOR", "DIV_OPERATOR", "MOD_OPERATOR", "LOGICAL_NOT_OPERATOR", 
		"BITWISE_NOT_OPERATOR", "SHIFT_LEFT_OPERATOR", "SHIFT_RIGHT_OPERATOR", 
		"LOGICAL_AND_OPERATOR", "BITWISE_AND_OPERATOR", "BITWISE_XOR_OPERATOR", 
		"LOGICAL_OR_OPERATOR", "BITWISE_OR_OPERATOR", "DOT_SYMBOL", "COMMA_SYMBOL", 
		"SEMICOLON_SYMBOL", "COLON_SYMBOL", "OPEN_PAR_SYMBOL", "CLOSE_PAR_SYMBOL", 
		"OPEN_CURLY_SYMBOL", "CLOSE_CURLY_SYMBOL", "UNDERLINE_SYMBOL", "IDENTIFIER", 
		"NCHAR_TEXT", "BACK_TICK", "SINGLE_QUOTE", "DOUBLE_QUOTE", "BACK_TICK_QUOTED_ID", 
		"DOUBLE_QUOTED_TEXT", "SINGLE_QUOTED_TEXT", "VERSION_COMMENT_START", "MYSQL_COMMENT_START", 
		"VERSION_COMMENT_END", "BLOCK_COMMENT", "POUND_COMMENT", "DASHDASH_COMMENT", 
		"DOUBLE_DASH", "LINEBREAK", "SIMPLE_IDENTIFIER", "ML_COMMENT_HEAD", "ML_COMMENT_END", 
		"LETTER_WHEN_UNQUOTED", "LETTER_WHEN_UNQUOTED_NO_DIGIT", "LETTER_WITHOUT_FLOAT_PART"
	};

	private static final String[] _LITERAL_NAMES = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, "'='", "':='", "'<=>'", 
		"'>='", "'>'", "'<='", "'<'", "'!='", "'+'", "'-'", "'*'", "'/'", "'%'", 
		"'!'", "'~'", "'<<'", "'>>'", "'&&'", "'&'", "'^'", "'||'", "'|'", "'.'", 
		"','", "';'", "':'", "'('", "')'", "'{'", "'}'", "'_'", null, null, null, 
		null, null, null, null, null, null, null, null, "'<>'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "HEX_NUMBER", "BIN_NUMBER", "INT_NUMBER", "DECIMAL_NUMBER", "FLOAT_NUMBER", 
		"ASC_SYMBOL", "AS_SYMBOL", "BEGIN_SYMBOL", "BY_SYMBOL", "CHAR_SYMBOL", 
		"CREATE_SYMBOL", "DELETE_SYMBOL", "DESC_SYMBOL", "DROP_SYMBOL", "FLOAT_SYMBOL", 
		"FROM_SYMBOL", "INDEX_SYMBOL", "INSERT_SYMBOL", "INTO_SYMBOL", "INT_SYMBOL", 
		"KEY_SYMBOL", "ON_SYMBOL", "ORDER_SYMBOL", "PRIMARY_SYMBOL", "SELECT_SYMBOL", 
		"TABLES_SYMBOL", "TABLE_SYMBOL", "UNIQUE_SYMBOL", "VALUES_SYMBOL", "VALUE_SYMBOL", 
		"WHERE_SYMBOL", "WORK_SYMBOL", "EQUAL_OPERATOR", "ASSIGN_OPERATOR", "NULL_SAFE_EQUAL_OPERATOR", 
		"GREATER_OR_EQUAL_OPERATOR", "GREATER_THAN_OPERATOR", "LESS_OR_EQUAL_OPERATOR", 
		"LESS_THAN_OPERATOR", "NOT_EQUAL_OPERATOR", "PLUS_OPERATOR", "MINUS_OPERATOR", 
		"MULT_OPERATOR", "DIV_OPERATOR", "MOD_OPERATOR", "LOGICAL_NOT_OPERATOR", 
		"BITWISE_NOT_OPERATOR", "SHIFT_LEFT_OPERATOR", "SHIFT_RIGHT_OPERATOR", 
		"LOGICAL_AND_OPERATOR", "BITWISE_AND_OPERATOR", "BITWISE_XOR_OPERATOR", 
		"LOGICAL_OR_OPERATOR", "BITWISE_OR_OPERATOR", "DOT_SYMBOL", "COMMA_SYMBOL", 
		"SEMICOLON_SYMBOL", "COLON_SYMBOL", "OPEN_PAR_SYMBOL", "CLOSE_PAR_SYMBOL", 
		"OPEN_CURLY_SYMBOL", "CLOSE_CURLY_SYMBOL", "UNDERLINE_SYMBOL", "IDENTIFIER", 
		"NCHAR_TEXT", "BACK_TICK_QUOTED_ID", "DOUBLE_QUOTED_TEXT", "SINGLE_QUOTED_TEXT", 
		"VERSION_COMMENT_START", "MYSQL_COMMENT_START", "VERSION_COMMENT_END", 
		"BLOCK_COMMENT", "POUND_COMMENT", "DASHDASH_COMMENT", "NOT_EQUAL2_OPERATOR"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}


	public MiniSQLLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "MiniSQL.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	@Override
	public void action(RuleContext _localctx, int ruleIndex, int actionIndex) {
		switch (ruleIndex) {
		case 31:
			INT_NUMBER_action((RuleContext)_localctx, actionIndex);
			break;
		case 34:
			DOT_IDENTIFIER_action((RuleContext)_localctx, actionIndex);
			break;
		case 83:
			LOGICAL_OR_OPERATOR_action((RuleContext)_localctx, actionIndex);
			break;
		case 103:
			MYSQL_COMMENT_START_action((RuleContext)_localctx, actionIndex);
			break;
		case 104:
			VERSION_COMMENT_END_action((RuleContext)_localctx, actionIndex);
			break;
		}
	}
	private void INT_NUMBER_action(RuleContext _localctx, int actionIndex) {
		switch (actionIndex) {
		case 0:
			 setType(determineNumericType(getText())); 
			break;
		}
	}
	private void DOT_IDENTIFIER_action(RuleContext _localctx, int actionIndex) {
		switch (actionIndex) {
		case 1:
			 emitDot(); 
			break;
		}
	}
	private void LOGICAL_OR_OPERATOR_action(RuleContext _localctx, int actionIndex) {
		switch (actionIndex) {
		case 2:
			 setType(isSqlModeActive(PipesAsConcat) ? CONCAT_PIPES_SYMBOL : LOGICAL_OR_OPERATOR); 
			break;
		}
	}
	private void MYSQL_COMMENT_START_action(RuleContext _localctx, int actionIndex) {
		switch (actionIndex) {
		case 3:
			 inVersionComment = true; 
			break;
		}
	}
	private void VERSION_COMMENT_END_action(RuleContext _localctx, int actionIndex) {
		switch (actionIndex) {
		case 4:
			 inVersionComment = false; 
			break;
		}
	}
	@Override
	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 99:
			return BACK_TICK_QUOTED_ID_sempred((RuleContext)_localctx, predIndex);
		case 100:
			return DOUBLE_QUOTED_TEXT_sempred((RuleContext)_localctx, predIndex);
		case 101:
			return SINGLE_QUOTED_TEXT_sempred((RuleContext)_localctx, predIndex);
		case 102:
			return VERSION_COMMENT_START_sempred((RuleContext)_localctx, predIndex);
		case 104:
			return VERSION_COMMENT_END_sempred((RuleContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean BACK_TICK_QUOTED_ID_sempred(RuleContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return !isSqlModeActive(NoBackslashEscapes);
		}
		return true;
	}
	private boolean DOUBLE_QUOTED_TEXT_sempred(RuleContext _localctx, int predIndex) {
		switch (predIndex) {
		case 1:
			return !isSqlModeActive(NoBackslashEscapes);
		}
		return true;
	}
	private boolean SINGLE_QUOTED_TEXT_sempred(RuleContext _localctx, int predIndex) {
		switch (predIndex) {
		case 2:
			return !isSqlModeActive(NoBackslashEscapes);
		}
		return true;
	}
	private boolean VERSION_COMMENT_START_sempred(RuleContext _localctx, int predIndex) {
		switch (predIndex) {
		case 3:
			return checkVersion(getText());
		}
		return true;
	}
	private boolean VERSION_COMMENT_END_sempred(RuleContext _localctx, int predIndex) {
		switch (predIndex) {
		case 4:
			return inVersionComment;
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\2M\u031c\b\1\4\2\t"+
		"\2\4\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13"+
		"\t\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\4&\t&\4\'\t\'\4(\t(\4)\t)\4*\t*\4+\t+\4"+
		",\t,\4-\t-\4.\t.\4/\t/\4\60\t\60\4\61\t\61\4\62\t\62\4\63\t\63\4\64\t"+
		"\64\4\65\t\65\4\66\t\66\4\67\t\67\48\t8\49\t9\4:\t:\4;\t;\4<\t<\4=\t="+
		"\4>\t>\4?\t?\4@\t@\4A\tA\4B\tB\4C\tC\4D\tD\4E\tE\4F\tF\4G\tG\4H\tH\4I"+
		"\tI\4J\tJ\4K\tK\4L\tL\4M\tM\4N\tN\4O\tO\4P\tP\4Q\tQ\4R\tR\4S\tS\4T\tT"+
		"\4U\tU\4V\tV\4W\tW\4X\tX\4Y\tY\4Z\tZ\4[\t[\4\\\t\\\4]\t]\4^\t^\4_\t_\4"+
		"`\t`\4a\ta\4b\tb\4c\tc\4d\td\4e\te\4f\tf\4g\tg\4h\th\4i\ti\4j\tj\4k\t"+
		"k\4l\tl\4m\tm\4n\tn\4o\to\4p\tp\4q\tq\4r\tr\4s\ts\4t\tt\4u\tu\3\2\3\2"+
		"\3\3\3\3\3\4\3\4\3\5\3\5\3\6\3\6\3\7\3\7\3\b\3\b\3\t\3\t\3\n\3\n\3\13"+
		"\3\13\3\f\3\f\3\r\3\r\3\16\3\16\3\17\3\17\3\20\3\20\3\21\3\21\3\22\3\22"+
		"\3\23\3\23\3\24\3\24\3\25\3\25\3\26\3\26\3\27\3\27\3\30\3\30\3\31\3\31"+
		"\3\32\3\32\3\33\3\33\3\34\3\34\3\35\6\35\u0123\n\35\r\35\16\35\u0124\3"+
		"\36\3\36\3\37\3\37\3\37\3\37\6\37\u012d\n\37\r\37\16\37\u012e\3\37\3\37"+
		"\3\37\3\37\6\37\u0135\n\37\r\37\16\37\u0136\3\37\3\37\5\37\u013b\n\37"+
		"\3 \3 \3 \3 \6 \u0141\n \r \16 \u0142\3 \3 \3 \3 \6 \u0149\n \r \16 \u014a"+
		"\3 \5 \u014e\n \3!\3!\3!\3\"\5\"\u0154\n\"\3\"\3\"\3\"\3#\5#\u015a\n#"+
		"\3#\5#\u015d\n#\3#\3#\3#\3#\5#\u0163\n#\3#\3#\3$\3$\3$\7$\u016a\n$\f$"+
		"\16$\u016d\13$\3$\3$\3$\3$\3%\3%\3%\3%\3&\3&\3&\3\'\3\'\3\'\3\'\3\'\3"+
		"\'\3(\3(\3(\3)\3)\3)\3)\3)\3*\3*\3*\3*\3*\3*\3*\3+\3+\3+\3+\3+\3+\3+\3"+
		",\3,\3,\3,\3,\3-\3-\3-\3-\3-\3.\3.\3.\3.\3.\3.\3/\3/\3/\3/\3/\3\60\3\60"+
		"\3\60\3\60\3\60\3\60\3\61\3\61\3\61\3\61\3\61\3\61\3\61\3\62\3\62\3\62"+
		"\3\62\3\62\3\63\3\63\3\63\3\63\3\64\3\64\3\64\3\64\3\65\3\65\3\65\3\66"+
		"\3\66\3\66\3\66\3\66\3\66\3\67\3\67\3\67\3\67\3\67\3\67\3\67\3\67\38\3"+
		"8\38\38\38\38\38\39\39\39\39\39\39\39\3:\3:\3:\3:\3:\3:\3;\3;\3;\3;\3"+
		";\3;\3;\3<\3<\3<\3<\3<\3<\3<\3=\3=\3=\3=\3=\3=\3>\3>\3>\3>\3>\3>\3?\3"+
		"?\3?\3?\3?\3@\3@\3A\3A\3A\3B\3B\3B\3B\3C\3C\3C\3D\3D\3E\3E\3E\3F\3F\3"+
		"G\3G\3G\3H\3H\3H\3H\3H\3I\3I\3J\3J\3K\3K\3L\3L\3M\3M\3N\3N\3O\3O\3P\3"+
		"P\3P\3Q\3Q\3Q\3R\3R\3R\3S\3S\3T\3T\3U\3U\3U\3U\3U\3V\3V\3W\3W\3X\3X\3"+
		"Y\3Y\3Z\3Z\3[\3[\3\\\3\\\3]\3]\3^\3^\3_\3_\3`\6`\u0259\n`\r`\16`\u025a"+
		"\3`\3`\3`\7`\u0260\n`\f`\16`\u0263\13`\5`\u0265\n`\3`\6`\u0268\n`\r`\16"+
		"`\u0269\3`\3`\7`\u026e\n`\f`\16`\u0271\13`\3`\3`\7`\u0275\n`\f`\16`\u0278"+
		"\13`\5`\u027a\n`\3a\3a\3a\3b\3b\3c\3c\3d\3d\3e\3e\3e\5e\u0288\ne\3e\7"+
		"e\u028b\ne\fe\16e\u028e\13e\3e\3e\3f\3f\3f\3f\5f\u0296\nf\3f\7f\u0299"+
		"\nf\ff\16f\u029c\13f\3f\3f\6f\u02a0\nf\rf\16f\u02a1\3g\3g\3g\5g\u02a7"+
		"\ng\3g\7g\u02aa\ng\fg\16g\u02ad\13g\3g\3g\6g\u02b1\ng\rg\16g\u02b2\3h"+
		"\3h\3h\3h\3h\3h\3h\3h\7h\u02bd\nh\fh\16h\u02c0\13h\3h\3h\5h\u02c4\nh\3"+
		"h\3h\3i\3i\3i\3i\3i\3i\3i\3i\3j\3j\3j\3j\3j\3j\3j\3j\3k\3k\3k\3k\3k\3"+
		"k\3k\3k\3k\7k\u02e1\nk\fk\16k\u02e4\13k\3k\3k\5k\u02e8\nk\3k\3k\3l\3l"+
		"\7l\u02ee\nl\fl\16l\u02f1\13l\3l\3l\3m\3m\3m\7m\u02f8\nm\fm\16m\u02fb"+
		"\13m\3m\3m\5m\u02ff\nm\3m\3m\3n\3n\3n\3o\3o\3p\3p\3p\6p\u030b\np\rp\16"+
		"p\u030c\3q\3q\3q\3r\3r\3r\3s\3s\5s\u0317\ns\3t\3t\3u\3u\7\u028c\u029a"+
		"\u02ab\u02be\u02e2\2v\3\2\5\2\7\2\t\2\13\2\r\2\17\2\21\2\23\2\25\2\27"+
		"\2\31\2\33\2\35\2\37\2!\2#\2%\2\'\2)\2+\2-\2/\2\61\2\63\2\65\2\67\29\2"+
		";\2=\3?\4A\5C\6E\7G\2I\bK\tM\nO\13Q\fS\rU\16W\17Y\20[\21]\22_\23a\24c"+
		"\25e\26g\27i\30k\31m\32o\33q\34s\35u\36w\37y {!}\"\177#\u0081$\u0083%"+
		"\u0085&\u0087\'\u0089(\u008b)\u008d*\u008fM\u0091+\u0093,\u0095-\u0097"+
		".\u0099/\u009b\60\u009d\61\u009f\62\u00a1\63\u00a3\64\u00a5\65\u00a7\66"+
		"\u00a9\67\u00ab8\u00ad9\u00af:\u00b1;\u00b3<\u00b5=\u00b7>\u00b9?\u00bb"+
		"@\u00bdA\u00bfB\u00c1C\u00c3\2\u00c5\2\u00c7\2\u00c9D\u00cbE\u00cdF\u00cf"+
		"G\u00d1H\u00d3I\u00d5J\u00d7K\u00d9L\u00db\2\u00dd\2\u00df\2\u00e1\2\u00e3"+
		"\2\u00e5\2\u00e7\2\u00e9\2\3\2%\4\2CCcc\4\2DDdd\4\2EEee\4\2FFff\4\2GG"+
		"gg\4\2HHhh\4\2IIii\4\2JJjj\4\2KKkk\4\2LLll\4\2MMmm\4\2NNnn\4\2OOoo\4\2"+
		"PPpp\4\2QQqq\4\2RRrr\4\2SSss\4\2TTtt\4\2UUuu\4\2VVvv\4\2WWww\4\2XXxx\4"+
		"\2YYyy\4\2ZZzz\4\2[[{{\4\2\\\\||\3\2\62;\5\2\62;CHch\3\2\62\63\3\2##\4"+
		"\2\f\f\17\17\4\2\13\13\"\"\6\2&&C\\aac|\7\2&&C\\aac|\u0082\1\t\2&&CFH"+
		"\\aacfh|\u0082\1\2\u031c\2=\3\2\2\2\2?\3\2\2\2\2A\3\2\2\2\2C\3\2\2\2\2"+
		"E\3\2\2\2\2G\3\2\2\2\2I\3\2\2\2\2K\3\2\2\2\2M\3\2\2\2\2O\3\2\2\2\2Q\3"+
		"\2\2\2\2S\3\2\2\2\2U\3\2\2\2\2W\3\2\2\2\2Y\3\2\2\2\2[\3\2\2\2\2]\3\2\2"+
		"\2\2_\3\2\2\2\2a\3\2\2\2\2c\3\2\2\2\2e\3\2\2\2\2g\3\2\2\2\2i\3\2\2\2\2"+
		"k\3\2\2\2\2m\3\2\2\2\2o\3\2\2\2\2q\3\2\2\2\2s\3\2\2\2\2u\3\2\2\2\2w\3"+
		"\2\2\2\2y\3\2\2\2\2{\3\2\2\2\2}\3\2\2\2\2\177\3\2\2\2\2\u0081\3\2\2\2"+
		"\2\u0083\3\2\2\2\2\u0085\3\2\2\2\2\u0087\3\2\2\2\2\u0089\3\2\2\2\2\u008b"+
		"\3\2\2\2\2\u008d\3\2\2\2\2\u008f\3\2\2\2\2\u0091\3\2\2\2\2\u0093\3\2\2"+
		"\2\2\u0095\3\2\2\2\2\u0097\3\2\2\2\2\u0099\3\2\2\2\2\u009b\3\2\2\2\2\u009d"+
		"\3\2\2\2\2\u009f\3\2\2\2\2\u00a1\3\2\2\2\2\u00a3\3\2\2\2\2\u00a5\3\2\2"+
		"\2\2\u00a7\3\2\2\2\2\u00a9\3\2\2\2\2\u00ab\3\2\2\2\2\u00ad\3\2\2\2\2\u00af"+
		"\3\2\2\2\2\u00b1\3\2\2\2\2\u00b3\3\2\2\2\2\u00b5\3\2\2\2\2\u00b7\3\2\2"+
		"\2\2\u00b9\3\2\2\2\2\u00bb\3\2\2\2\2\u00bd\3\2\2\2\2\u00bf\3\2\2\2\2\u00c1"+
		"\3\2\2\2\2\u00c9\3\2\2\2\2\u00cb\3\2\2\2\2\u00cd\3\2\2\2\2\u00cf\3\2\2"+
		"\2\2\u00d1\3\2\2\2\2\u00d3\3\2\2\2\2\u00d5\3\2\2\2\2\u00d7\3\2\2\2\2\u00d9"+
		"\3\2\2\2\3\u00eb\3\2\2\2\5\u00ed\3\2\2\2\7\u00ef\3\2\2\2\t\u00f1\3\2\2"+
		"\2\13\u00f3\3\2\2\2\r\u00f5\3\2\2\2\17\u00f7\3\2\2\2\21\u00f9\3\2\2\2"+
		"\23\u00fb\3\2\2\2\25\u00fd\3\2\2\2\27\u00ff\3\2\2\2\31\u0101\3\2\2\2\33"+
		"\u0103\3\2\2\2\35\u0105\3\2\2\2\37\u0107\3\2\2\2!\u0109\3\2\2\2#\u010b"+
		"\3\2\2\2%\u010d\3\2\2\2\'\u010f\3\2\2\2)\u0111\3\2\2\2+\u0113\3\2\2\2"+
		"-\u0115\3\2\2\2/\u0117\3\2\2\2\61\u0119\3\2\2\2\63\u011b\3\2\2\2\65\u011d"+
		"\3\2\2\2\67\u011f\3\2\2\29\u0122\3\2\2\2;\u0126\3\2\2\2=\u013a\3\2\2\2"+
		"?\u014d\3\2\2\2A\u014f\3\2\2\2C\u0153\3\2\2\2E\u015c\3\2\2\2G\u0166\3"+
		"\2\2\2I\u0172\3\2\2\2K\u0176\3\2\2\2M\u0179\3\2\2\2O\u017f\3\2\2\2Q\u0182"+
		"\3\2\2\2S\u0187\3\2\2\2U\u018e\3\2\2\2W\u0195\3\2\2\2Y\u019a\3\2\2\2["+
		"\u019f\3\2\2\2]\u01a5\3\2\2\2_\u01aa\3\2\2\2a\u01b0\3\2\2\2c\u01b7\3\2"+
		"\2\2e\u01bc\3\2\2\2g\u01c0\3\2\2\2i\u01c4\3\2\2\2k\u01c7\3\2\2\2m\u01cd"+
		"\3\2\2\2o\u01d5\3\2\2\2q\u01dc\3\2\2\2s\u01e3\3\2\2\2u\u01e9\3\2\2\2w"+
		"\u01f0\3\2\2\2y\u01f7\3\2\2\2{\u01fd\3\2\2\2}\u0203\3\2\2\2\177\u0208"+
		"\3\2\2\2\u0081\u020a\3\2\2\2\u0083\u020d\3\2\2\2\u0085\u0211\3\2\2\2\u0087"+
		"\u0214\3\2\2\2\u0089\u0216\3\2\2\2\u008b\u0219\3\2\2\2\u008d\u021b\3\2"+
		"\2\2\u008f\u021e\3\2\2\2\u0091\u0223\3\2\2\2\u0093\u0225\3\2\2\2\u0095"+
		"\u0227\3\2\2\2\u0097\u0229\3\2\2\2\u0099\u022b\3\2\2\2\u009b\u022d\3\2"+
		"\2\2\u009d\u022f\3\2\2\2\u009f\u0231\3\2\2\2\u00a1\u0234\3\2\2\2\u00a3"+
		"\u0237\3\2\2\2\u00a5\u023a\3\2\2\2\u00a7\u023c\3\2\2\2\u00a9\u023e\3\2"+
		"\2\2\u00ab\u0243\3\2\2\2\u00ad\u0245\3\2\2\2\u00af\u0247\3\2\2\2\u00b1"+
		"\u0249\3\2\2\2\u00b3\u024b\3\2\2\2\u00b5\u024d\3\2\2\2\u00b7\u024f\3\2"+
		"\2\2\u00b9\u0251\3\2\2\2\u00bb\u0253\3\2\2\2\u00bd\u0255\3\2\2\2\u00bf"+
		"\u0279\3\2\2\2\u00c1\u027b\3\2\2\2\u00c3\u027e\3\2\2\2\u00c5\u0280\3\2"+
		"\2\2\u00c7\u0282\3\2\2\2\u00c9\u0284\3\2\2\2\u00cb\u029f\3\2\2\2\u00cd"+
		"\u02b0\3\2\2\2\u00cf\u02b4\3\2\2\2\u00d1\u02c7\3\2\2\2\u00d3\u02cf\3\2"+
		"\2\2\u00d5\u02e7\3\2\2\2\u00d7\u02eb\3\2\2\2\u00d9\u02f4\3\2\2\2\u00db"+
		"\u0302\3\2\2\2\u00dd\u0305\3\2\2\2\u00df\u030a\3\2\2\2\u00e1\u030e\3\2"+
		"\2\2\u00e3\u0311\3\2\2\2\u00e5\u0316\3\2\2\2\u00e7\u0318\3\2\2\2\u00e9"+
		"\u031a\3\2\2\2\u00eb\u00ec\t\2\2\2\u00ec\4\3\2\2\2\u00ed\u00ee\t\3\2\2"+
		"\u00ee\6\3\2\2\2\u00ef\u00f0\t\4\2\2\u00f0\b\3\2\2\2\u00f1\u00f2\t\5\2"+
		"\2\u00f2\n\3\2\2\2\u00f3\u00f4\t\6\2\2\u00f4\f\3\2\2\2\u00f5\u00f6\t\7"+
		"\2\2\u00f6\16\3\2\2\2\u00f7\u00f8\t\b\2\2\u00f8\20\3\2\2\2\u00f9\u00fa"+
		"\t\t\2\2\u00fa\22\3\2\2\2\u00fb\u00fc\t\n\2\2\u00fc\24\3\2\2\2\u00fd\u00fe"+
		"\t\13\2\2\u00fe\26\3\2\2\2\u00ff\u0100\t\f\2\2\u0100\30\3\2\2\2\u0101"+
		"\u0102\t\r\2\2\u0102\32\3\2\2\2\u0103\u0104\t\16\2\2\u0104\34\3\2\2\2"+
		"\u0105\u0106\t\17\2\2\u0106\36\3\2\2\2\u0107\u0108\t\20\2\2\u0108 \3\2"+
		"\2\2\u0109\u010a\t\21\2\2\u010a\"\3\2\2\2\u010b\u010c\t\22\2\2\u010c$"+
		"\3\2\2\2\u010d\u010e\t\23\2\2\u010e&\3\2\2\2\u010f\u0110\t\24\2\2\u0110"+
		"(\3\2\2\2\u0111\u0112\t\25\2\2\u0112*\3\2\2\2\u0113\u0114\t\26\2\2\u0114"+
		",\3\2\2\2\u0115\u0116\t\27\2\2\u0116.\3\2\2\2\u0117\u0118\t\30\2\2\u0118"+
		"\60\3\2\2\2\u0119\u011a\t\31\2\2\u011a\62\3\2\2\2\u011b\u011c\t\32\2\2"+
		"\u011c\64\3\2\2\2\u011d\u011e\t\33\2\2\u011e\66\3\2\2\2\u011f\u0120\t"+
		"\34\2\2\u01208\3\2\2\2\u0121\u0123\5\67\34\2\u0122\u0121\3\2\2\2\u0123"+
		"\u0124\3\2\2\2\u0124\u0122\3\2\2\2\u0124\u0125\3\2\2\2\u0125:\3\2\2\2"+
		"\u0126\u0127\t\35\2\2\u0127<\3\2\2\2\u0128\u0129\7\62\2\2\u0129\u012a"+
		"\7z\2\2\u012a\u012c\3\2\2\2\u012b\u012d\5;\36\2\u012c\u012b\3\2\2\2\u012d"+
		"\u012e\3\2\2\2\u012e\u012c\3\2\2\2\u012e\u012f\3\2\2\2\u012f\u013b\3\2"+
		"\2\2\u0130\u0131\7z\2\2\u0131\u0132\7)\2\2\u0132\u0134\3\2\2\2\u0133\u0135"+
		"\5;\36\2\u0134\u0133\3\2\2\2\u0135\u0136\3\2\2\2\u0136\u0134\3\2\2\2\u0136"+
		"\u0137\3\2\2\2\u0137\u0138\3\2\2\2\u0138\u0139\7)\2\2\u0139\u013b\3\2"+
		"\2\2\u013a\u0128\3\2\2\2\u013a\u0130\3\2\2\2\u013b>\3\2\2\2\u013c\u013d"+
		"\7\62\2\2\u013d\u013e\7d\2\2\u013e\u0140\3\2\2\2\u013f\u0141\t\36\2\2"+
		"\u0140\u013f\3\2\2\2\u0141\u0142\3\2\2\2\u0142\u0140\3\2\2\2\u0142\u0143"+
		"\3\2\2\2\u0143\u014e\3\2\2\2\u0144\u0145\7d\2\2\u0145\u0146\7)\2\2\u0146"+
		"\u0148\3\2\2\2\u0147\u0149\t\36\2\2\u0148\u0147\3\2\2\2\u0149\u014a\3"+
		"\2\2\2\u014a\u0148\3\2\2\2\u014a\u014b\3\2\2\2\u014b\u014c\3\2\2\2\u014c"+
		"\u014e\7)\2\2\u014d\u013c\3\2\2\2\u014d\u0144\3\2\2\2\u014e@\3\2\2\2\u014f"+
		"\u0150\59\35\2\u0150\u0151\b!\2\2\u0151B\3\2\2\2\u0152\u0154\59\35\2\u0153"+
		"\u0152\3\2\2\2\u0153\u0154\3\2\2\2\u0154\u0155\3\2\2\2\u0155\u0156\5\u00ad"+
		"W\2\u0156\u0157\59\35\2\u0157D\3\2\2\2\u0158\u015a\59\35\2\u0159\u0158"+
		"\3\2\2\2\u0159\u015a\3\2\2\2\u015a\u015b\3\2\2\2\u015b\u015d\5\u00adW"+
		"\2\u015c\u0159\3\2\2\2\u015c\u015d\3\2\2\2\u015d\u015e\3\2\2\2\u015e\u015f"+
		"\59\35\2\u015f\u0162\t\6\2\2\u0160\u0163\5\u0093J\2\u0161\u0163\5\u0091"+
		"I\2\u0162\u0160\3\2\2\2\u0162\u0161\3\2\2\2\u0162\u0163\3\2\2\2\u0163"+
		"\u0164\3\2\2\2\u0164\u0165\59\35\2\u0165F\3\2\2\2\u0166\u0167\5\u00ad"+
		"W\2\u0167\u016b\5\u00e7t\2\u0168\u016a\5\u00e5s\2\u0169\u0168\3\2\2\2"+
		"\u016a\u016d\3\2\2\2\u016b\u0169\3\2\2\2\u016b\u016c\3\2\2\2\u016c\u016e"+
		"\3\2\2\2\u016d\u016b\3\2\2\2\u016e\u016f\b$\3\2\u016f\u0170\3\2\2\2\u0170"+
		"\u0171\b$\4\2\u0171H\3\2\2\2\u0172\u0173\5\3\2\2\u0173\u0174\5\'\24\2"+
		"\u0174\u0175\5\7\4\2\u0175J\3\2\2\2\u0176\u0177\5\3\2\2\u0177\u0178\5"+
		"\'\24\2\u0178L\3\2\2\2\u0179\u017a\5\5\3\2\u017a\u017b\5\13\6\2\u017b"+
		"\u017c\5\17\b\2\u017c\u017d\5\23\n\2\u017d\u017e\5\35\17\2\u017eN\3\2"+
		"\2\2\u017f\u0180\5\5\3\2\u0180\u0181\5\63\32\2\u0181P\3\2\2\2\u0182\u0183"+
		"\5\7\4\2\u0183\u0184\5\21\t\2\u0184\u0185\5\3\2\2\u0185\u0186\5%\23\2"+
		"\u0186R\3\2\2\2\u0187\u0188\5\7\4\2\u0188\u0189\5%\23\2\u0189\u018a\5"+
		"\13\6\2\u018a\u018b\5\3\2\2\u018b\u018c\5)\25\2\u018c\u018d\5\13\6\2\u018d"+
		"T\3\2\2\2\u018e\u018f\5\t\5\2\u018f\u0190\5\13\6\2\u0190\u0191\5\31\r"+
		"\2\u0191\u0192\5\13\6\2\u0192\u0193\5)\25\2\u0193\u0194\5\13\6\2\u0194"+
		"V\3\2\2\2\u0195\u0196\5\t\5\2\u0196\u0197\5\13\6\2\u0197\u0198\5\'\24"+
		"\2\u0198\u0199\5\7\4\2\u0199X\3\2\2\2\u019a\u019b\5\t\5\2\u019b\u019c"+
		"\5%\23\2\u019c\u019d\5\37\20\2\u019d\u019e\5!\21\2\u019eZ\3\2\2\2\u019f"+
		"\u01a0\5\r\7\2\u01a0\u01a1\5\31\r\2\u01a1\u01a2\5\37\20\2\u01a2\u01a3"+
		"\5\3\2\2\u01a3\u01a4\5)\25\2\u01a4\\\3\2\2\2\u01a5\u01a6\5\r\7\2\u01a6"+
		"\u01a7\5%\23\2\u01a7\u01a8\5\37\20\2\u01a8\u01a9\5\33\16\2\u01a9^\3\2"+
		"\2\2\u01aa\u01ab\5\23\n\2\u01ab\u01ac\5\35\17\2\u01ac\u01ad\5\t\5\2\u01ad"+
		"\u01ae\5\13\6\2\u01ae\u01af\5\61\31\2\u01af`\3\2\2\2\u01b0\u01b1\5\23"+
		"\n\2\u01b1\u01b2\5\35\17\2\u01b2\u01b3\5\'\24\2\u01b3\u01b4\5\13\6\2\u01b4"+
		"\u01b5\5%\23\2\u01b5\u01b6\5)\25\2\u01b6b\3\2\2\2\u01b7\u01b8\5\23\n\2"+
		"\u01b8\u01b9\5\35\17\2\u01b9\u01ba\5)\25\2\u01ba\u01bb\5\37\20\2\u01bb"+
		"d\3\2\2\2\u01bc\u01bd\5\23\n\2\u01bd\u01be\5\35\17\2\u01be\u01bf\5)\25"+
		"\2\u01bff\3\2\2\2\u01c0\u01c1\5\27\f\2\u01c1\u01c2\5\13\6\2\u01c2\u01c3"+
		"\5\63\32\2\u01c3h\3\2\2\2\u01c4\u01c5\5\37\20\2\u01c5\u01c6\5\35\17\2"+
		"\u01c6j\3\2\2\2\u01c7\u01c8\5\37\20\2\u01c8\u01c9\5%\23\2\u01c9\u01ca"+
		"\5\t\5\2\u01ca\u01cb\5\13\6\2\u01cb\u01cc\5%\23\2\u01ccl\3\2\2\2\u01cd"+
		"\u01ce\5!\21\2\u01ce\u01cf\5%\23\2\u01cf\u01d0\5\23\n\2\u01d0\u01d1\5"+
		"\33\16\2\u01d1\u01d2\5\3\2\2\u01d2\u01d3\5%\23\2\u01d3\u01d4\5\63\32\2"+
		"\u01d4n\3\2\2\2\u01d5\u01d6\5\'\24\2\u01d6\u01d7\5\13\6\2\u01d7\u01d8"+
		"\5\31\r\2\u01d8\u01d9\5\13\6\2\u01d9\u01da\5\7\4\2\u01da\u01db\5)\25\2"+
		"\u01dbp\3\2\2\2\u01dc\u01dd\5)\25\2\u01dd\u01de\5\3\2\2\u01de\u01df\5"+
		"\5\3\2\u01df\u01e0\5\31\r\2\u01e0\u01e1\5\13\6\2\u01e1\u01e2\5\'\24\2"+
		"\u01e2r\3\2\2\2\u01e3\u01e4\5)\25\2\u01e4\u01e5\5\3\2\2\u01e5\u01e6\5"+
		"\5\3\2\u01e6\u01e7\5\31\r\2\u01e7\u01e8\5\13\6\2\u01e8t\3\2\2\2\u01e9"+
		"\u01ea\5+\26\2\u01ea\u01eb\5\35\17\2\u01eb\u01ec\5\23\n\2\u01ec\u01ed"+
		"\5#\22\2\u01ed\u01ee\5+\26\2\u01ee\u01ef\5\13\6\2\u01efv\3\2\2\2\u01f0"+
		"\u01f1\5-\27\2\u01f1\u01f2\5\3\2\2\u01f2\u01f3\5\31\r\2\u01f3\u01f4\5"+
		"+\26\2\u01f4\u01f5\5\13\6\2\u01f5\u01f6\5\'\24\2\u01f6x\3\2\2\2\u01f7"+
		"\u01f8\5-\27\2\u01f8\u01f9\5\3\2\2\u01f9\u01fa\5\31\r\2\u01fa\u01fb\5"+
		"+\26\2\u01fb\u01fc\5\13\6\2\u01fcz\3\2\2\2\u01fd\u01fe\5/\30\2\u01fe\u01ff"+
		"\5\21\t\2\u01ff\u0200\5\13\6\2\u0200\u0201\5%\23\2\u0201\u0202\5\13\6"+
		"\2\u0202|\3\2\2\2\u0203\u0204\5/\30\2\u0204\u0205\5\37\20\2\u0205\u0206"+
		"\5%\23\2\u0206\u0207\5\27\f\2\u0207~\3\2\2\2\u0208\u0209\7?\2\2\u0209"+
		"\u0080\3\2\2\2\u020a\u020b\7<\2\2\u020b\u020c\7?\2\2\u020c\u0082\3\2\2"+
		"\2\u020d\u020e\7>\2\2\u020e\u020f\7?\2\2\u020f\u0210\7@\2\2\u0210\u0084"+
		"\3\2\2\2\u0211\u0212\7@\2\2\u0212\u0213\7?\2\2\u0213\u0086\3\2\2\2\u0214"+
		"\u0215\7@\2\2\u0215\u0088\3\2\2\2\u0216\u0217\7>\2\2\u0217\u0218\7?\2"+
		"\2\u0218\u008a\3\2\2\2\u0219\u021a\7>\2\2\u021a\u008c\3\2\2\2\u021b\u021c"+
		"\7#\2\2\u021c\u021d\7?\2\2\u021d\u008e\3\2\2\2\u021e\u021f\7>\2\2\u021f"+
		"\u0220\7@\2\2\u0220\u0221\3\2\2\2\u0221\u0222\bH\5\2\u0222\u0090\3\2\2"+
		"\2\u0223\u0224\7-\2\2\u0224\u0092\3\2\2\2\u0225\u0226\7/\2\2\u0226\u0094"+
		"\3\2\2\2\u0227\u0228\7,\2\2\u0228\u0096\3\2\2\2\u0229\u022a\7\61\2\2\u022a"+
		"\u0098\3\2\2\2\u022b\u022c\7\'\2\2\u022c\u009a\3\2\2\2\u022d\u022e\7#"+
		"\2\2\u022e\u009c\3\2\2\2\u022f\u0230\7\u0080\2\2\u0230\u009e\3\2\2\2\u0231"+
		"\u0232\7>\2\2\u0232\u0233\7>\2\2\u0233\u00a0\3\2\2\2\u0234\u0235\7@\2"+
		"\2\u0235\u0236\7@\2\2\u0236\u00a2\3\2\2\2\u0237\u0238\7(\2\2\u0238\u0239"+
		"\7(\2\2\u0239\u00a4\3\2\2\2\u023a\u023b\7(\2\2\u023b\u00a6\3\2\2\2\u023c"+
		"\u023d\7`\2\2\u023d\u00a8\3\2\2\2\u023e\u023f\7~\2\2\u023f\u0240\7~\2"+
		"\2\u0240\u0241\3\2\2\2\u0241\u0242\bU\6\2\u0242\u00aa\3\2\2\2\u0243\u0244"+
		"\7~\2\2\u0244\u00ac\3\2\2\2\u0245\u0246\7\60\2\2\u0246\u00ae\3\2\2\2\u0247"+
		"\u0248\7.\2\2\u0248\u00b0\3\2\2\2\u0249\u024a\7=\2\2\u024a\u00b2\3\2\2"+
		"\2\u024b\u024c\7<\2\2\u024c\u00b4\3\2\2\2\u024d\u024e\7*\2\2\u024e\u00b6"+
		"\3\2\2\2\u024f\u0250\7+\2\2\u0250\u00b8\3\2\2\2\u0251\u0252\7}\2\2\u0252"+
		"\u00ba\3\2\2\2\u0253\u0254\7\177\2\2\u0254\u00bc\3\2\2\2\u0255\u0256\7"+
		"a\2\2\u0256\u00be\3\2\2\2\u0257\u0259\59\35\2\u0258\u0257\3\2\2\2\u0259"+
		"\u025a\3\2\2\2\u025a\u0258\3\2\2\2\u025a\u025b\3\2\2\2\u025b\u025c\3\2"+
		"\2\2\u025c\u0264\t\6\2\2\u025d\u0261\5\u00e7t\2\u025e\u0260\5\u00e5s\2"+
		"\u025f\u025e\3\2\2\2\u0260\u0263\3\2\2\2\u0261\u025f\3\2\2\2\u0261\u0262"+
		"\3\2\2\2\u0262\u0265\3\2\2\2\u0263\u0261\3\2\2\2\u0264\u025d\3\2\2\2\u0264"+
		"\u0265\3\2\2\2\u0265\u027a\3\2\2\2\u0266\u0268\59\35\2\u0267\u0266\3\2"+
		"\2\2\u0268\u0269\3\2\2\2\u0269\u0267\3\2\2\2\u0269\u026a\3\2\2\2\u026a"+
		"\u026b\3\2\2\2\u026b\u026f\5\u00e9u\2\u026c\u026e\5\u00e5s\2\u026d\u026c"+
		"\3\2\2\2\u026e\u0271\3\2\2\2\u026f\u026d\3\2\2\2\u026f\u0270\3\2\2\2\u0270"+
		"\u027a\3\2\2\2\u0271\u026f\3\2\2\2\u0272\u0276\5\u00e7t\2\u0273\u0275"+
		"\5\u00e5s\2\u0274\u0273\3\2\2\2\u0275\u0278\3\2\2\2\u0276\u0274\3\2\2"+
		"\2\u0276\u0277\3\2\2\2\u0277\u027a\3\2\2\2\u0278\u0276\3\2\2\2\u0279\u0258"+
		"\3\2\2\2\u0279\u0267\3\2\2\2\u0279\u0272\3\2\2\2\u027a\u00c0\3\2\2\2\u027b"+
		"\u027c\t\17\2\2\u027c\u027d\5\u00cdg\2\u027d\u00c2\3\2\2\2\u027e\u027f"+
		"\7b\2\2\u027f\u00c4\3\2\2\2\u0280\u0281\7)\2\2\u0281\u00c6\3\2\2\2\u0282"+
		"\u0283\7$\2\2\u0283\u00c8\3\2\2\2\u0284\u028c\5\u00c3b\2\u0285\u0286\6"+
		"e\2\2\u0286\u0288\7^\2\2\u0287\u0285\3\2\2\2\u0287\u0288\3\2\2\2\u0288"+
		"\u0289\3\2\2\2\u0289\u028b\13\2\2\2\u028a\u0287\3\2\2\2\u028b\u028e\3"+
		"\2\2\2\u028c\u028d\3\2\2\2\u028c\u028a\3\2\2\2\u028d\u028f\3\2\2\2\u028e"+
		"\u028c\3\2\2\2\u028f\u0290\5\u00c3b\2\u0290\u00ca\3\2\2\2\u0291\u029a"+
		"\5\u00c7d\2\u0292\u0293\6f\3\2\u0293\u0294\7^\2\2\u0294\u0296\13\2\2\2"+
		"\u0295\u0292\3\2\2\2\u0295\u0296\3\2\2\2\u0296\u0297\3\2\2\2\u0297\u0299"+
		"\13\2\2\2\u0298\u0295\3\2\2\2\u0299\u029c\3\2\2\2\u029a\u029b\3\2\2\2"+
		"\u029a\u0298\3\2\2\2\u029b\u029d\3\2\2\2\u029c\u029a\3\2\2\2\u029d\u029e"+
		"\5\u00c7d\2\u029e\u02a0\3\2\2\2\u029f\u0291\3\2\2\2\u02a0\u02a1\3\2\2"+
		"\2\u02a1\u029f\3\2\2\2\u02a1\u02a2\3\2\2\2\u02a2\u00cc\3\2\2\2\u02a3\u02ab"+
		"\5\u00c5c\2\u02a4\u02a5\6g\4\2\u02a5\u02a7\7^\2\2\u02a6\u02a4\3\2\2\2"+
		"\u02a6\u02a7\3\2\2\2\u02a7\u02a8\3\2\2\2\u02a8\u02aa\13\2\2\2\u02a9\u02a6"+
		"\3\2\2\2\u02aa\u02ad\3\2\2\2\u02ab\u02ac\3\2\2\2\u02ab\u02a9\3\2\2\2\u02ac"+
		"\u02ae\3\2\2\2\u02ad\u02ab\3\2\2\2\u02ae\u02af\5\u00c5c\2\u02af\u02b1"+
		"\3\2\2\2\u02b0\u02a3\3\2\2\2\u02b1\u02b2\3\2\2\2\u02b2\u02b0\3\2\2\2\u02b2"+
		"\u02b3\3\2\2\2\u02b3\u00ce\3\2\2\2\u02b4\u02b5\7\61\2\2\u02b5\u02b6\7"+
		",\2\2\u02b6\u02b7\7#\2\2\u02b7\u02b8\3\2\2\2\u02b8\u02b9\59\35\2\u02b9"+
		"\u02c3\3\2\2\2\u02ba\u02c4\6h\5\2\u02bb\u02bd\13\2\2\2\u02bc\u02bb\3\2"+
		"\2\2\u02bd\u02c0\3\2\2\2\u02be\u02bf\3\2\2\2\u02be\u02bc\3\2\2\2\u02bf"+
		"\u02c1\3\2\2\2\u02c0\u02be\3\2\2\2\u02c1\u02c2\7,\2\2\u02c2\u02c4\7\61"+
		"\2\2\u02c3\u02ba\3\2\2\2\u02c3\u02be\3\2\2\2\u02c4\u02c5\3\2\2\2\u02c5"+
		"\u02c6\bh\7\2\u02c6\u00d0\3\2\2\2\u02c7\u02c8\7\61\2\2\u02c8\u02c9\7,"+
		"\2\2\u02c9\u02ca\7#\2\2\u02ca\u02cb\3\2\2\2\u02cb\u02cc\bi\b\2\u02cc\u02cd"+
		"\3\2\2\2\u02cd\u02ce\bi\7\2\u02ce\u00d2\3\2\2\2\u02cf\u02d0\7,\2\2\u02d0"+
		"\u02d1\7\61\2\2\u02d1\u02d2\3\2\2\2\u02d2\u02d3\6j\6\2\u02d3\u02d4\bj"+
		"\t\2\u02d4\u02d5\3\2\2\2\u02d5\u02d6\bj\7\2\u02d6\u00d4\3\2\2\2\u02d7"+
		"\u02d8\7\61\2\2\u02d8\u02d9\7,\2\2\u02d9\u02da\7,\2\2\u02da\u02e8\7\61"+
		"\2\2\u02db\u02dc\7\61\2\2\u02dc\u02dd\7,\2\2\u02dd\u02de\3\2\2\2\u02de"+
		"\u02e2\n\37\2\2\u02df\u02e1\13\2\2\2\u02e0\u02df\3\2\2\2\u02e1\u02e4\3"+
		"\2\2\2\u02e2\u02e3\3\2\2\2\u02e2\u02e0\3\2\2\2\u02e3\u02e5\3\2\2\2\u02e4"+
		"\u02e2\3\2\2\2\u02e5\u02e6\7,\2\2\u02e6\u02e8\7\61\2\2\u02e7\u02d7\3\2"+
		"\2\2\u02e7\u02db\3\2\2\2\u02e8\u02e9\3\2\2\2\u02e9\u02ea\bk\7\2\u02ea"+
		"\u00d6\3\2\2\2\u02eb\u02ef\7%\2\2\u02ec\u02ee\n \2\2\u02ed\u02ec\3\2\2"+
		"\2\u02ee\u02f1\3\2\2\2\u02ef\u02ed\3\2\2\2\u02ef\u02f0\3\2\2\2\u02f0\u02f2"+
		"\3\2\2\2\u02f1\u02ef\3\2\2\2\u02f2\u02f3\bl\7\2\u02f3\u00d8\3\2\2\2\u02f4"+
		"\u02fe\5\u00dbn\2\u02f5\u02f9\t!\2\2\u02f6\u02f8\n \2\2\u02f7\u02f6\3"+
		"\2\2\2\u02f8\u02fb\3\2\2\2\u02f9\u02f7\3\2\2\2\u02f9\u02fa\3\2\2\2\u02fa"+
		"\u02ff\3\2\2\2\u02fb\u02f9\3\2\2\2\u02fc\u02ff\5\u00ddo\2\u02fd\u02ff"+
		"\7\2\2\3\u02fe\u02f5\3\2\2\2\u02fe\u02fc\3\2\2\2\u02fe\u02fd\3\2\2\2\u02ff"+
		"\u0300\3\2\2\2\u0300\u0301\bm\7\2\u0301\u00da\3\2\2\2\u0302\u0303\7/\2"+
		"\2\u0303\u0304\7/\2\2\u0304\u00dc\3\2\2\2\u0305\u0306\t \2\2\u0306\u00de"+
		"\3\2\2\2\u0307\u030b\5\67\34\2\u0308\u030b\t\"\2\2\u0309\u030b\5\u00ad"+
		"W\2\u030a\u0307\3\2\2\2\u030a\u0308\3\2\2\2\u030a\u0309\3\2\2\2\u030b"+
		"\u030c\3\2\2\2\u030c\u030a\3\2\2\2\u030c\u030d\3\2\2\2\u030d\u00e0\3\2"+
		"\2\2\u030e\u030f\7\61\2\2\u030f\u0310\7,\2\2\u0310\u00e2\3\2\2\2\u0311"+
		"\u0312\7,\2\2\u0312\u0313\7\61\2\2\u0313\u00e4\3\2\2\2\u0314\u0317\5\67"+
		"\34\2\u0315\u0317\5\u00e7t\2\u0316\u0314\3\2\2\2\u0316\u0315\3\2\2\2\u0317"+
		"\u00e6\3\2\2\2\u0318\u0319\t#\2\2\u0319\u00e8\3\2\2\2\u031a\u031b\t$\2"+
		"\2\u031b\u00ea\3\2\2\2(\2\u0124\u012e\u0136\u013a\u0142\u014a\u014d\u0153"+
		"\u0159\u015c\u0162\u016b\u025a\u0261\u0264\u0269\u026f\u0276\u0279\u0287"+
		"\u028c\u0295\u029a\u02a1\u02a6\u02ab\u02b2\u02be\u02c3\u02e2\u02e7\u02ef"+
		"\u02f9\u02fe\u030a\u030c\u0316\n\3!\2\3$\3\tB\2\t*\2\3U\4\2\3\2\3i\5\3"+
		"j\6";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}