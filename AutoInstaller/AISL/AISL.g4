grammar AISL;
 
script : 
  findInstruction 
  hasBlock? 
  uninstallInstruction? 
  executeInstruction 
  invokeInstallInstruction?
  invokeUninstallInstruction?
  EOF ;
 
findInstruction : 
  'FIND ' QUOTED_TEXT ' AT ' QUOTED_TEXT LINE_END;
hasBlock : 
  'HAS (' NEWLINE parameterList ') AS installation_parameters' LINE_END;
parameterList : 
  ('    ' parameter ',' NEWLINE)+ ;
parameter :
  choiceParameter | nonChoiceParameter;
nonChoiceParameter :
  OPTIONAL? TYPE WORD defaultOrFixed?;
choiceParameter : 
  OPTIONAL? 'choice ' WORD ' FROM ' optionList defaultOrFixed?;
defaultOrFixed :
  defaultParamValue | fixedParamValue;
defaultParamValue :
  ' WITH DEFAULT ' valueOrString;
fixedParamValue :
  ' = ' valueOrString;
uninstallInstruction : 
  'UNINSTALL ' QUOTED_TEXT LINE_END;
executeInstruction: 
  'EXECUTE ' QUOTED_TEXT 'WITH installation_parameters' LINE_END;
invokeInstallInstruction: 
  'INVOKE AS INSTALL {' anything '} AT ' QUOTED_TEXT LINE_END;
invokeUninstallInstruction: 
  'INVOKE AS UNINSTALL {' anything '} AT ' QUOTED_TEXT LINE_END;
anything : 
  ANY_AND_ESCAPED_CURLY+ ;
  
valueOrString : 
  WORD | QUOTED_TEXT ;
optionList : 
  '[' valueOrString (',' valueOrString)* ']';
 
TYPE : 'number' | 'string' | 'flag';
WORD : (LOWERCASE | UPPERCASE | DIGIT)+ ;
QUOTED_TEXT : '"' ANY+? '"' ;
OPTIONAL : 'OPTIONAL ' ;
ANY : ~[\r\n\t] ;
ANY_AND_ESCAPED_CURLY : (~[{}] | '\\}' | '\\{') ;

fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment DIGIT      : [0-9] ;
fragment SEMICOLON  : ';' ;
 
LINE_END   : SEMICOLON NEWLINE ;
NEWLINE    : ('\r'? '\n' | '\r')+ ;
 
WHITESPACE : [ \t\n]+ -> skip ;