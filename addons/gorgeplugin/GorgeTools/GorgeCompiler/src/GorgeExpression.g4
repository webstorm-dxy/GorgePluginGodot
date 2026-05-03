/*
    Gorge表达式语法
*/
grammar GorgeExpression;
import GorgeLexerRules,GorgeStatement;

/*
    条件级表达式
*/
expression
    : assignmentLevelExpression
    ;

/*
    赋值级表达式，同级右结合，向左泵出其他条件表达式
*/
assignmentLevelExpression
    : conditionalLevelExpression                        # ConditionalLevel
    | assignmentTarget '=' conditionalLevelExpression   # Assignment
    ;

/*
    条件级运算符，同级右结合
      a ? b : c
      结合规则：
        a ? b : c ? d : e = a ? b : (c ? d : e)
        a ? b ? c : d : e = a ? (b ? c : d) : e
*/
conditionalLevelExpression
    : logicalOrLevelExpression                              # LoginOrLevel
    | logicalOrLevelExpression '?' expression ':' expression # Conditional
    ;

/*
    逻辑或级表达式，同级左结合，向右泵出逻辑与级表达式
      a || b
*/
logicalOrLevelExpression
    : logicalAndLevelExpression                                 # LogicalAndLevel
    | logicalOrLevelExpression '||' logicalAndLevelExpression   # LogicalOr
    ;

/*
    逻辑与级表达式，同级左结合，向右泵出相等级表达式
      a && b
*/
logicalAndLevelExpression
    : equalityLevelExpression                                   # EqualityLevel
    | logicalAndLevelExpression '&&' equalityLevelExpression    # LogicalAnd
    ;

/*
    相等级表达式，同级左结合，向右泵出比较级表达式
      a == b
      a != b
*/
equalityLevelExpression
    : comparisonLevelExpression                                 # ComparisonLevel
    | equalityLevelExpression '==' comparisonLevelExpression    # Equality
    | equalityLevelExpression '!=' comparisonLevelExpression    # Inequality
    ;

/*
    比较级表达式，同级左结合，向右泵出加法级表达式
      a < b
      a > b
      a <= b
      a >= b
*/
comparisonLevelExpression
    : additionLevelExpression                                   # AdditionLevel
    | comparisonLevelExpression '<' additionLevelExpression     # Less
    | comparisonLevelExpression '>' additionLevelExpression     # Greater
    | comparisonLevelExpression '<=' additionLevelExpression    # LessEqual
    | comparisonLevelExpression '>=' additionLevelExpression    # GreaterEqual
    ;

/*
    加法级表达式，同级左结合，向右泵出乘法级表达式
      a + b
      a - b
*/
additionLevelExpression
    : multiplicationLevelExpression                             # MultiplicationLevel
    | additionLevelExpression '+' multiplicationLevelExpression # Addition
    | additionLevelExpression '-' multiplicationLevelExpression # Subtraction
    ;

/*
    乘法级表达式，同级左结合，向右泵出一元级表达式
      a * b
      a / b
      a % b
*/
multiplicationLevelExpression
    : unaryRightAssociativityLevelExpression                                      # UnaryRightAssociativityLevel
    | multiplicationLevelExpression '*' unaryRightAssociativityLevelExpression    # Multiplication
    | multiplicationLevelExpression '/' unaryRightAssociativityLevelExpression    # Division
    | multiplicationLevelExpression '%' unaryRightAssociativityLevelExpression    # Remainder
    ;

/*
    一元右结合级表达式，同级右结合，向左泵出操作符
      -a
      !a
      (int)a
*/
unaryRightAssociativityLevelExpression
    : unaryLeftAssociativityLevelExpression                         # UnaryLeftAssociativityLevel
    | '-' unaryRightAssociativityLevelExpression                    # Opposite
    | '!' unaryRightAssociativityLevelExpression                    # LogicalNot
    | '(' expression ')' unaryRightAssociativityLevelExpression     # Cast
    ;

/*
    一元左结合级表达式，同级左结合，向右泵出操作符
      a.fieldName
*/
unaryLeftAssociativityLevelExpression
    : primaryLevelExpression                                                                                # PrimaryLevel
    | unaryLeftAssociativityLevelExpression '.' Identifier                                                  # MemberAccess
    | unaryLeftAssociativityLevelExpression '.' '^' Identifier                                              # InjectorMemberAccess
    | unaryLeftAssociativityLevelExpression '(' (expression (Comma expression)*)? ')'                       # MethodInvocation
    | unaryLeftAssociativityLevelExpression ':' objectInjector                                              # InjectorLiteral
    | unaryLeftAssociativityLevelExpression ':' arrayInjector                                               # ArrayInjectorLiteral
    | unaryLeftAssociativityLevelExpression '[' expression ']'                                              # ArrayAccess
    | unaryLeftAssociativityLevelExpression ':' '(' (lambdaExpressionParameter (Comma lambdaExpressionParameter)*)? ')' '->' codeBlockList # LambdaExpression
    | unaryLeftAssociativityLevelExpression '[' ']'             # TypeArray
    | unaryLeftAssociativityLevelExpression '^'                 # TypeInjector
    | unaryLeftAssociativityLevelExpression '<' expression '>'     # TypeGenerics
    ;

/*
    主表达式级表达式，不可细分，不存在集合
      直接数(含引用)
      括号括起来的表达式
*/ 
primaryLevelExpression
    : literal               # LiteralExpression
    | '(' expression ')'    # SeparateExpression
    | New expression '(' (expression (Comma expression)*)? ')' objectInjector?     # ConstructorInvocation
    | New expression '[' expression  ']' arrayInjector?                          # ArrayConstructorInvocation
    | Delegate '<' expression (':' deletageParameterTypes)? '>' # TypeDelegate
    ;
    
lambdaExpressionParameter
    : expression Identifier
    ;

/*
    赋值目标表达式，同级左结合，向右泵出操作符
*/
assignmentTarget
    : This                                  # ThisAssignmentTarget
    | Identifier                            # ReferenceAssignmentTarget
    | assignmentTarget '.' Identifier       # MemberAccessAssignmentTarget
    | assignmentTarget '.' '^' Identifier   # InjectorMemberAccessAssignmentTarget
    | assignmentTarget '[' expression ']'   # ArrayAccessAssignmentTarget
    ;

// 普通类注入器
objectInjector
    : LeftCurlyBracket keyValuePair (Comma keyValuePair)* Comma? RightCurlyBracket     # NonemptyObjectInjector
    | LeftCurlyBracket ':' RightCurlyBracket                                       # EmptyObjectInjector
    ;

// 数组注入器
arrayInjector
    : LeftCurlyBracket expression (Comma expression)* Comma? RightCurlyBracket         # NonemptyArrayInjector
    | LeftCurlyBracket Comma RightCurlyBracket                                       # EmptyArrayInjector
    ;
    
// 对象注入器使用的键值对
keyValuePair
    : Identifier ':' expression
    ;
    
deletageParameterTypes
    : expression (Comma expression)*
    ;

// 字面量表达式
literal
    : valueLiteral
    | referenceLiteral
    | typeLiteral
    ;

// 值字面量
valueLiteral
    : IntLiteral        # LiteralInt
    | FloatLiteral      # LiteralFloat
    | BoolLiteral       # LiteralBool
    | StringLiteral     # LiteralString
    ;

// 引用字面量
referenceLiteral
    : This                  # ThisExpression
    | Null                  # NullExpression
    | Default               # DefaultExpression
    | Identifier            # ReferenceExpression
    | '^' Identifier        # InjectorReferenceExpression
    ;

// 类型字面量
typeLiteral
    : Int                               # TypeInt
    | Float                             # TypeFloat
    | Bool                              # TypeBool
    | String                            # TypeString
    | Object                            # TypeBaseObject
    | Auto                              # TypeAuto
    | Void                              # TypeVoid
    ;