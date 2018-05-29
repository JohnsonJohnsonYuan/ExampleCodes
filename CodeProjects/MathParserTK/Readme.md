解析数学表达式
=============

# MathParser project
[ref](https://www.codeproject.com/script/articles/articleversion.aspx?aid=381509&av=559662)

```c#
string s1 = "pi+5*5+5*3-5*5-5*3+1E1";
string s2 = "sin(cos(tg(sh(ch(th(100))))))";
MathParser parser = new MathParser();
double result = parser.Parse(s1);  // 第二个参数用来计算三角函数是时候转换为弧度

// Parse 方法: FormatString: 验证括号个数是否匹配, 转换为小写
// ConvertToRPN: 转换为RPN
// Calculate: 计算RPN
// Calculate(ConvertToRPN(FormatString(expression)));

// 数字之前用#标记, 函数@, 运算符$， 方便解析运算, 支持pi, e
parser.ConvertToRPN(s1);	// #3.14159265358979#5#5$*$+#5#3$*$+#5#5$*$-#5#3$*$-#10$+
parser.ConvertToRPN(s2);	// #100@th@ch@sh@tg@cos@sin
```

# Reverse Polish notation(RPN) 介绍
mathematical notation in which operators follow their operands
RPN 能够去掉表达式中的复杂的括号关系, 转换之后利用stack可以使用stack计算值

e.g.
3-4+5   表示为   3 4 - 5 +
3-(4+5) 表示为   3 4 5 + -

reference:
[Reverse Polish notation(Postfix notation)](https://en.wikipedia.org/wiki/Reverse_Polish_notation)

## 转换方法 [Shunting-yard algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm)
A+B*C-D => A B C * + D -
![Graphical illustration](https://upload.wikimedia.org/wikipedia/commons/2/24/Shunting_yard.svg)

算法:
可以参考MathParserTK\MathParser.cs ConvertToRPN 方法:
1）遍历表达式, LexicalAnalysisInfixNotation 方法得到当前token(完整的数字字符串, 或者函数, 或者运算符号, 括号)
2）根据token类型 （SyntaxAnalysisInfixNotation 方法）
* 如果当前token是数字， 输出
* 如果当前token是函数, 左括号, push to stack
* 如果当前token是右括号, pop operator's in stack until meet '(', and pop '('
* 如果当前token是运算符(+,-,*,/), pop stack中优先级大于当前operator的(判断是left 还是right associate); 当前operator 加入stack
3) 如果stack中还有operator, 输出
   
```c#
while there are tokens to be read:
    read a token.
    if the token is a number, then:
        push it to the output queue.
    if the token is a function then:
        push it onto the operator stack 
    if the token is an operator, then:
        while ((there is a function at the top of the operator stack)
               or (there is an operator at the top of the operator stack with greater precedence)
               or (the operator at the top of the operator stack has equal precedence and is left associative))
              and (the operator at the top of the operator stack is not a left bracket):
            pop operators from the operator stack onto the output queue.
        push it onto the operator stack.
    if the token is a left bracket (i.e. "("), then:
        push it onto the operator stack.
    if the token is a right bracket (i.e. ")"), then:
        while the operator at the top of the operator stack is not a left bracket:
            pop the operator from the operator stack onto the output queue.
        pop the left bracket from the stack.
        /* if the stack runs out without finding a left bracket, then there are mismatched parentheses. */
if there are no more tokens to read:
    while there are still operator tokens on the stack:
        /* if the operator token on the top of the stack is a bracket, then there are mismatched parentheses. */
        pop the operator from the operator stack onto the output queue.
exit.
```


## 计算方法 （using stack, 当表达式遍历完后, stack只有一个元素, 就是结果）
* 如果当前token是数字, push to stack
* 如果当前token是operator, pop(具体根据operator来pop1个或2个, 如+, - 需要pop 2个, tan, sin函数pop 1个即可), calculate, push to stack
* stack.Pop()

```c#
for each token in the postfix expression:
  if token is an operator:
    operand_2 ← pop from the stack
    operand_1 ← pop from the stack
    result ← evaluate token with operand_1 and operand_2
    push result back onto the stack
  else if token is an operand:
    push token onto the stack
result ← pop from the stack
```