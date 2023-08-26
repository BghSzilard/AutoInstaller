grammar AISL;

/*
 * Parser Rules
 */

script : findInstruction hasBlock? EOF ;

findInstruction : FIND programName AT installationsPath SEMICOLON NEWLINE ;

hasBlock : HAS OPEN_PARENTHESIS NEWLINE parameterList CLOSE_PARENTHESIS AS INSTALLATION_PARAMETERS SEMICOLON NEWLINE ;

parameterList : parameter (COMMA NEWLINE parameter)* NEWLINE ;

parameter : parameterType parameterName (WITH DEFAULT parameterDefaultValue)? ;

programName : QUOTED_TEXT ;

installationsPath : QUOTED_TEXT ;

parameterType : WORD ;

parameterName : WORD ;

parameterDefaultValue : WORD ;

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

INSTALLATION_PARAMETERS : 'installation_parameters' ;

WORD : (LOWERCASE | UPPERCASE | DIGIT)+ ;

QUOTED_TEXT : '"' .*? '"' ;

SEMICOLON : ';' ;

COMMA : ',' ;

OPEN_PARENTHESIS : '(' ;

CLOSE_PARENTHESIS : ')' ;

WHITESPACE : [ \t\n]+ -> skip ;

NEWLINE : ('\r'? '\n' | '\r')+ ;