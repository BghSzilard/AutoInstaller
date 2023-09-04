grammar AISL;

/*
 * Parser Rules
 */

script : findInstruction hasBlock? uninstallInstruction? executeInstruction invokeInstruction? EOF ;

findInstruction : FIND programName AT installationsPath SEMICOLON NEWLINE ;

hasBlock : HAS OPEN_PARENTHESIS NEWLINE parameterList CLOSE_PARENTHESIS AS INSTALLATION_PARAMETERS SEMICOLON NEWLINE ;

parameterList : parameter (COMMA NEWLINE parameter)* NEWLINE ;

parameter : parameterIsOptional? parameterType parameterName (((FROM optionList)? (WITH DEFAULT parameterDefaultValue)?) | (EQUALS parameterFixedValue)) ;

uninstallInstruction : UNINSTALL programName SEMICOLON NEWLINE ;

executeInstruction: EXECUTE installerPath WITH INSTALLATION_PARAMETERS SEMICOLON NEWLINE ;

invokeInstruction: INVOKE OPEN_CURLY_BRACKET NEWLINE invokeBlock '} AT' invokePath SEMICOLON (NEWLINE)*;
invokeBlock : (invokeLine NEWLINE)+ ;

invokeLine : ANY_TEXT ;

invokePath : QUOTED_TEXT | ANY_TEXT ;

installerPath : QUOTED_TEXT ;

programName : QUOTED_TEXT ;

installationsPath : QUOTED_TEXT ;

parameterType : WORD ;

parameterName : WORD ;

parameterDefaultValue : WORD | QUOTED_TEXT ;

parameterFixedValue : WORD | QUOTED_TEXT ;

parameterIsOptional : OPTIONAL ;

optionList: OPEN_SQUARE_BRACKET option (COMMA option)* CLOSE_SQUARE_BRACKET;

option: WORD | QUOTED_TEXT ;

/*
 * Lexer Rules
 */

fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment DIGIT      : [0-9] ;

FIND : 'FIND' | 'find' ;

AT : 'AT' | 'at' ;

HAS : 'HAS' | 'has' ;

WITH : 'WITH' | 'with' ;

DEFAULT : 'DEFAULT' | 'default' ;

AS : 'AS' | 'as' ;

FROM : 'FROM' | 'from' ;

UNINSTALL : 'UNINSTALL' | 'uninstall' ;

EXECUTE : 'EXECUTE' | 'execute' ;

INVOKE : 'INVOKE' | 'invoke' ;

INSTALLATION_PARAMETERS : 'installation_parameters' ;

OPTIONAL : 'optional' ;

WORD : (LOWERCASE | UPPERCASE | DIGIT)+ ;

QUOTED_TEXT : '"' .+? '"' ;

SEMICOLON : ';' ;

COMMA : ',' ;

OPEN_PARENTHESIS : '(' ;

CLOSE_PARENTHESIS : ')' ;

OPEN_SQUARE_BRACKET : '[' ;

CLOSE_SQUARE_BRACKET : ']' ;

OPEN_CURLY_BRACKET : '{' ;

CLOSE_CURLY_BRACKET : '}' ;

EQUALS : '=' ;

NEWLINE : ('\r'? '\n' | '\r')+ ;

ANY_TEXT : (~[\r\n])+? ;

WHITESPACE : [ \t\n]+ -> skip ;