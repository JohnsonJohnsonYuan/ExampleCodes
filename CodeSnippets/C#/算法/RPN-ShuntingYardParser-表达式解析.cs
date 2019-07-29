using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// source:
/// https://gist.github.com/istupakov/c49ef290c3bc3dbe329bf68f67971470
/// 
///  表达式支持数字， 字母(a-z)， 
///  var expr2 = "4 + 18 / cos(a+3) - 3";
///  var expr = "cos(a+3)-3";
///  "(a&(((b & c)&d)|e))|f";
/// "(a&(((b & c)&d)|e))|f";
/// "a|cos(f&a)"; 
/// "((cos(da&f))|ca)" // 变量a, da支持
/// 
///  var parser = new Parser();
///  var tokens = parser.Tokenize(expr).ToList();  // 读取每个Operator, Variable, Function
///  //Console.WriteLine(string.Join("\n", tokens));
///  
///  var rpn = parser.ShuntingYard(tokens);         // 转换为RPN表达式
///  Console.WriteLine(string.Join(" ", rpn.Select(t => t.Value)));
///  
///  var result2 = parser.EvalRPN(rpn);             // 计算RPN
///  Console.WriteLine(result2);
/// 
/// 
/// </summary>
namespace RPN_Algorithm
{
    enum TokenType { Number, Variable, Function, Parenthesis, Operator, Comma, WhiteSpace };

    struct Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public override string ToString() => $"{Type}: {Value}";

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    class Operator
    {
        public string Name { get; set; }
        public int Precedence { get; set; }
        public bool RightAssociative { get; set; }
    }

    class Parser
    {

        /// c# 6.0: https://dailydotnettips.com/initialization-of-dictionary-dictionary-initializer-in-c-6-0/
        private IDictionary<string, Operator> operators = new Dictionary<string, Operator>
        {
            // 新增 &, |, ^ 运算
            ["&"] = new Operator { Name = "&", Precedence = 1 },
            ["|"] = new Operator { Name = "|", Precedence = 1 },
            ["^"] = new Operator { Name = "^", Precedence = 1 },

            ["+"] = new Operator { Name = "+", Precedence = 1 },
            ["-"] = new Operator { Name = "-", Precedence = 1 },
            ["*"] = new Operator { Name = "*", Precedence = 2 },
            ["/"] = new Operator { Name = "/", Precedence = 2 },
            ["^"] = new Operator { Name = "/", Precedence = 3, RightAssociative = true }
        };

        private bool CompareOperators(Operator op1, Operator op2)
        {
            return op1.RightAssociative ? op1.Precedence < op2.Precedence : op1.Precedence <= op2.Precedence;
        }

        private bool CompareOperators(string op1, string op2) => CompareOperators(operators[op1], operators[op2]);

        private TokenType DetermineType(char ch)
        {
            if (char.IsLetter(ch))
                return TokenType.Variable;
            if (char.IsDigit(ch))
                return TokenType.Number;
            if (char.IsWhiteSpace(ch))
                return TokenType.WhiteSpace;
            if (ch == ',')
                return TokenType.Comma;
            if (ch == '(' || ch == ')')
                return TokenType.Parenthesis;
            if (operators.ContainsKey(Convert.ToString(ch)))
                return TokenType.Operator;

            throw new Exception("Wrong character");
        }

        public IEnumerable<Token> Tokenize(string input)
        {
            return Tokenize(new StringReader(input));
        }
        public IEnumerable<Token> Tokenize(TextReader reader)
        {
            var token = new StringBuilder();

            int curr;
            while ((curr = reader.Read()) != -1)
            {
                var ch = (char)curr;
                var currType = DetermineType(ch);
                if (currType == TokenType.WhiteSpace)
                    continue;

                token.Append(ch);

                var next = reader.Peek();
                var nextType = next != -1 ? DetermineType((char)next) : TokenType.WhiteSpace;
                if (currType != nextType)
                {
                    // 原版本基础上增加 currType == TokenType.Variable 判断
                    // 否则当表达式 a | (a & f) 时, 会把|判断为Function, 在后面计算RPN时会因为Function缺少Operator报错, 应该判断为Operator
                    // 当currentType 是变量时才判断为Function, a | cos(a&f)
                    if (currType == TokenType.Variable && next == '(')
                        yield return new Token(TokenType.Function, token.ToString());
                    else
                        yield return new Token(currType, token.ToString());
                    token.Clear();
                }
            }
        }

        public IEnumerable<Token> ShuntingYard(IEnumerable<Token> tokens)
        {
            var stack = new Stack<Token>();
            foreach (var tok in tokens)
            {
                switch (tok.Type)
                {
                    case TokenType.Number:
                    case TokenType.Variable:
                        yield return tok;
                        break;
                    case TokenType.Function:
                        stack.Push(tok);
                        break;
                    case TokenType.Comma:
                        while (stack.Peek().Value != "(")
                            yield return stack.Pop();
                        break;
                    case TokenType.Operator:
                        while (stack.Any() &&
                            (stack.Peek().Type == TokenType.Operator && CompareOperators(tok.Value, stack.Peek().Value)))
                            yield return stack.Pop();
                        stack.Push(tok);
                        break;
                    case TokenType.Parenthesis:
                        if (tok.Value == "(")
                            stack.Push(tok);
                        else
                        {
                            while (stack.Peek().Value != "(")
                                yield return stack.Pop();
                            stack.Pop();
                            if (stack.Peek().Type == TokenType.Function)
                                yield return stack.Pop();
                        }
                        break;
                    default:
                        throw new Exception("Wrong token");
                }
            }
            while (stack.Any())
            {
                var tok = stack.Pop();
                if (tok.Type == TokenType.Parenthesis)
                    throw new Exception("Mismatched parentheses");
                yield return tok;
            }
        }

        #region 计算RPN表达式：

        /// 计算RPN表达式：
        /// Source:
        /// https://helloacm.com/cc-coding-exercise-evaluate-reverse-polish-notation-using-stack/
        public string EvalRPN(IEnumerable<Token> tokens)
        {
            Stack<string> operands = new Stack<string>();

            foreach (var tok in tokens)
            {
                var tokenVal = tok.Value;

                System.Console.Write(tokenVal + " ->");

                if (tok.Type == TokenType.Operator)
                {
                    var o1 = operands.Pop();
                    var o2 = operands.Pop();

                    System.Console.WriteLine($"operator, calc: \"{o1}\", \"{o2}\"");

                    // push result to stack
                    // a & b
                    operands.Push($"({o1}{tokenVal}{o2})");

                    System.Console.WriteLine($"({o1}{tokenVal}{o2})");
                }
                else if (tok.Type == TokenType.Function)
                {
                    var o1 = operands.Pop();
                    System.Console.WriteLine($"function: \"{tokenVal}\"");

                    // add function
                    System.Console.WriteLine("push " + tokenVal);
                    operands.Push($"({tokenVal}{o1})");
                }
                else if (tok.Type == TokenType.Variable
                    || tok.Type == TokenType.Number)
                {
                    System.Console.WriteLine("push " + tokenVal);
                    operands.Push(tokenVal);
                }
                else
                {
                    throw new Exception($"token {tokenVal} invalid");
                }
            }

            if (operands.Count == 1)
                return operands.Pop();
            else
                throw new Exception("expr error");
        }

        #endregion
    }
}