������ѧ���ʽ
=============

# MathParser project
[ref](https://www.codeproject.com/script/articles/articleversion.aspx?aid=381509&av=559662)

```c#
string s1 = "pi+5*5+5*3-5*5-5*3+1E1";
string s2 = "sin(cos(tg(sh(ch(th(100))))))";
MathParser parser = new MathParser();
double result = parser.Parse(s1);  // �ڶ������������������Ǻ�����ʱ��ת��Ϊ����

// Parse ����: FormatString: ��֤���Ÿ����Ƿ�ƥ��, ת��ΪСд
// ConvertToRPN: ת��ΪRPN
// Calculate: ����RPN
// Calculate(ConvertToRPN(FormatString(expression)));

// ����֮ǰ��#���, ����@, �����$�� �����������, ֧��pi, e
parser.ConvertToRPN(s1);	// #3.14159265358979#5#5$*$+#5#3$*$+#5#5$*$-#5#3$*$-#10$+
parser.ConvertToRPN(s2);	// #100@th@ch@sh@tg@cos@sin
```

# Reverse Polish notation(RPN) ����
mathematical notation in which operators follow their operands
RPN �ܹ�ȥ�����ʽ�еĸ��ӵ����Ź�ϵ, ת��֮������stack����ʹ��stack����ֵ

e.g.
3-4+5   ��ʾΪ   3 4 - 5 +
3-(4+5) ��ʾΪ   3 4 5 + -

reference:
[Reverse Polish notation(Postfix notation)](https://en.wikipedia.org/wiki/Reverse_Polish_notation)

## ת������ [Shunting-yard algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm)
A+B*C-D => A B C * + D -
![Graphical illustration](https://upload.wikimedia.org/wikipedia/commons/2/24/Shunting_yard.svg)

�㷨:
���Բο�MathParserTK\MathParser.cs ConvertToRPN ����:
1���������ʽ, LexicalAnalysisInfixNotation �����õ���ǰtoken(�����������ַ���, ���ߺ���, �����������, ����)
2������token���� ��SyntaxAnalysisInfixNotation ������
* �����ǰtoken�����֣� ���
* �����ǰtoken�Ǻ���, ������, push to stack
* �����ǰtoken��������, pop operator's in stack until meet '(', and pop '('
* �����ǰtoken�������(+,-,*,/), pop stack�����ȼ����ڵ�ǰoperator��(�ж���left ����right associate); ��ǰoperator ����stack
3) ���stack�л���operator, ���
   
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


## ���㷽�� ��using stack, �����ʽ�������, stackֻ��һ��Ԫ��, ���ǽ����
* �����ǰtoken������, push to stack
* �����ǰtoken��operator, pop(�������operator��pop1����2��, ��+, - ��Ҫpop 2��, tan, sin����pop 1������), calculate, push to stack
* stack.Pop()

```c#
for each token in the postfix expression:
  if token is an operator:
    operand_2 �� pop from the stack
    operand_1 �� pop from the stack
    result �� evaluate token with operand_1 and operand_2
    push result back onto the stack
  else if token is an operand:
    push token onto the stack
result �� pop from the stack
```