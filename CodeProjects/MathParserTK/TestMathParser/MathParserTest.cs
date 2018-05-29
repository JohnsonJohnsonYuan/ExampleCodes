using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathParserTK;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace TestMathParser
{
    [TestClass]
    public class MathParserTest
    {
        #region Test Parse method

        [TestMethod]
        public void ParseTest1()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "2-2";
            var output = 0d;

            // Act
            var actual = mathParser.Parse(input);
            
            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest2()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "2.5+2";
            var output = 4.5d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest3()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "2.0*3.0";
            var output = 6d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest4()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "10/2.5";
            var output = 4d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest5()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "2*(3+4)";
            var output = 14d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest6()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "2^2+4";
            var output = 8d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest7()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "5*5+5*3-5*5-5*3";
            var output = 0d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest8()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "-5*(+5)+5*(-3)-5*(+5)-5*(-3)";
            var output = -50d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest9()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "-(-1)";
            var output = 1d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest10()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "-1^2";
            var output = -1d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest11()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "(-1)^2";
            var output = 1d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest12()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "pi*1+e*2+.95";
            var output = 9.5281563105078888d;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest13()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "sin(cos(tg(sh(ch(th(100))))))";
            var output = 0.017452402397444194D;

            // Act
            var actual = mathParser.Parse(input, false);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest14()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "th(ch(sh(tg(cos(sin(100))))))";
            var output = 0.76165811649275783D;

            // Act
            var actual = mathParser.Parse(input, false);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest15()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "abs(-2)+exp(1)+(2)Log(4)+√(4)";
            var output = 2 + Math.E + 2 + 2;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ParseTest16()
        {
            // Arrange
            var mathParser = new MathParser('.');
            var input = "-1*(-(+(5*(+5)+5*(+3)-5*(+5)-5*(-3))))";
            var output = 30D;

            // Act
            var actual = mathParser.Parse(input);

            // Assert
            Assert.AreEqual(output, actual);
        }        

        #endregion

        #region Test FormatString method

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatStringTest1()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));

            // Act
            var actual = (string)mathParser.Invoke("FormatString", string.Empty);

            // Assert
        }

        [TestMethod]
        public void FormatStringTest2()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "\t2 - 2  + COS(60)";
            var output = "2-2+cos(60)";

            // Act
            var actual = (string)mathParser.Invoke("FormatString", input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FormatStringTest3()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "\t2 - 2  + COS60)";

            // Act
            var actual = (string)mathParser.Invoke("FormatString", input);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FormatStringTest4()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "\t2 - 2  + COS((60)";

            // Act
            var actual = (string)mathParser.Invoke("FormatString", input);

            // Assert
        }

        #endregion

        #region Test ConvertToRPN method
        
        [TestMethod]
        public void ConvertToRPNTest1()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "2+2";
            var output = "#2#2$+";

            // Act
            var actual = (string)mathParser.Invoke("ConvertToRPN", input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void ConvertToRPNTest2()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "-5*(+5)+5*(-3)-2^2^3/4";
            var output = "#5$un-#5$un+$*#5#3$un-$*$+#2#2#3$^$^#4$/$-";

            // Act
            var actual = (string)mathParser.Invoke("ConvertToRPN", input);

            // Assert
            Assert.AreEqual(output, actual);
        }        

        #endregion

        #region Test LexicalAnalysisInfixNotation method

        [TestMethod]
        public void LexicalAnalysisInfixNotationTest1()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "+1234*3";
            var pos = 0;
            var output = "$un+";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void LexicalAnalysisInfixNotationTest2()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser), args: '.');
            var input = "+1234.535*3";
            var pos = 1;
            var output = "#1234" + CultureInfo.CurrentCulture
                        .NumberFormat.NumberDecimalSeparator + "535";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void LexicalAnalysisInfixNotationTest3()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "-55*5-2^2^33/44";
            var pos = input.Length - 3;
            var output = "$/";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void LexicalAnalysisInfixNotationTest4()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "3+cos(90)";
            var pos = 5;
            var output = "$(";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LexicalAnalysisInfixNotationTest5()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "3+fac(90)";
            var pos = 2;

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LexicalAnalysisInfixNotationTest6()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "_+3";
            var pos = 0;

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
        }

        [TestMethod]
        public void LexicalAnalysisInfixNotationTest7()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "sqrt(25)+3";
            var pos = 0;
            var output = "@sqrt";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void LexicalAnalysisInfixNotationTest8()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "sqrt(25)+3+e";
            var pos = input.Length - 1;
            var output = "#" + Math.E;

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void LexicalAnalysisInfixNotationTest9()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "e+2e+10";
            var pos = 2;
            var output = "#20000000000";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisInfixNotation", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        #endregion

        #region Test SyntaxAnalysisInfixNotation method

        [TestMethod]
        public void SyntaxAnalysisInfixNotationTest1()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var token = "$+";
            var input = new StringBuilder("#3#1");
            var stack = new Stack<string>();
            stack.Push("$-");
            var output = "#3#1$-";

            // Act
            var actual = (StringBuilder)mathParser.Invoke("SyntaxAnalysisInfixNotation", token, input, stack);

            // Assert
            Assert.AreEqual(output, actual.ToString());
            Assert.IsTrue(stack.Count == 1 && stack.Peek() == "$+");
        }

        [TestMethod]
        public void SyntaxAnalysisInfixNotationTest2()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var token = "#1";
            var input = new StringBuilder("#3");
            var stack = new Stack<string>();
            var output = "#3#1";

            // Act
            var actual = (StringBuilder)mathParser.Invoke("SyntaxAnalysisInfixNotation", token, input, stack);

            // Assert
            Assert.AreEqual(output, actual.ToString());
        }

        [TestMethod]
        public void SyntaxAnalysisInfixNotationTest3()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var token = "$*";
            var input = new StringBuilder("#3#1");
            var stack = new Stack<string>();
            stack.Push("$-");
            var output = "#3#1";

            // Act
            var actual = (StringBuilder)mathParser.Invoke("SyntaxAnalysisInfixNotation", token, input, stack);

            // Assert
            Assert.AreEqual(output, actual.ToString());
            Assert.IsTrue(stack.Count == 2 && stack.Peek() == "$*");
        }

        [TestMethod]
        public void SyntaxAnalysisInfixNotationTest4()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var token = "$)";
            var input = new StringBuilder("#3#1#4#5");
            var stack = new Stack<string>();
            stack.Push("$(");
            stack.Push("$-");
            stack.Push("$+");
            stack.Push("$*");
            var output = "#3#1#4#5$*$+$-";

            // Act
            var actual = (StringBuilder)mathParser.Invoke("SyntaxAnalysisInfixNotation", token, input, stack);

            // Assert
            Assert.AreEqual(output, actual.ToString());
            Assert.IsTrue(stack.Count == 0);
        }

        [TestMethod]
        public void SyntaxAnalysisInfixNotationTest5()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var token = "$)";
            var input = new StringBuilder("#3#1#4#5");
            var stack = new Stack<string>();
            stack.Push("@sqrt");
            stack.Push("$(");
            stack.Push("$-");
            stack.Push("$+");
            stack.Push("$*");
            var output = "#3#1#4#5$*$+$-@sqrt";

            // Act
            var actual = (StringBuilder)mathParser.Invoke("SyntaxAnalysisInfixNotation", token, input, stack);

            // Assert
            Assert.AreEqual(output, actual.ToString());
            Assert.IsTrue(stack.Count == 0);
        }

        [TestMethod]
        public void SyntaxAnalysisInfixNotationTest6()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var token = "@sqrt";
            var input = new StringBuilder("#3#1");
            var stack = new Stack<string>();
            stack.Push("$-");
            var output = "#3#1";

            // Act
            var actual = (StringBuilder)mathParser.Invoke("SyntaxAnalysisInfixNotation", token, input, stack);

            // Assert
            Assert.AreEqual(output, actual.ToString());
            Assert.IsTrue(stack.Count == 2 && stack.Peek() == "@sqrt");
        }

        #endregion

        #region Test Calculate method

        [TestMethod]
        public void CalculateTest1()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "#2#2$+";
            var output = 4d;

            // Act
            var actual = (double)mathParser.Invoke("Calculate", input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void CalculateTest2()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "#5$un-#5$un+$*#5#3$un-$*$+#2#2#3$^$^#4$/$-";
            var output = -104d;

            // Act
            var actual = (double)mathParser.Invoke("Calculate", input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void CalculateTest3()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "#1$un-$un-";
            var output = 1d;

            // Act
            var actual = (double)mathParser.Invoke("Calculate", input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void CalculateTest4()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "#1#2$^$un-";
            var output = -1d;

            // Act
            var actual = (double)mathParser.Invoke("Calculate", input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void CalculateTest5()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "#25@sqrt";
            var output = 5d;

            // Act
            var actual = (double)mathParser.Invoke("Calculate", input);

            // Assert
            Assert.AreEqual(output, actual);
        }

        #endregion

        #region Test LexicalAnalysisRPN

        [TestMethod]
        public void LexicalAnalysisRPNTest1()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "#2#2$+";
            var pos = 0;
            var output = "#2";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisRPN", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void LexicalAnalysisRPNTest2()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "#2#2$+";
            var pos = 4;
            var output = "$+";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisRPN", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        [TestMethod]
        public void LexicalAnalysisRPNTest3()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var input = "#2#2$+@sqrt";
            var pos = 6;
            var output = "@sqrt";

            // Act
            var actual = (string)mathParser.Invoke("LexicalAnalysisRPN", input, pos);

            // Assert
            Assert.AreEqual(output, actual);
        }

        #endregion

        #region Test SyntaxAnalysisRPN method

        [TestMethod]
        public void SyntaxAnalysisRPNTest1()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var stack = new Stack<double>();
            stack.Push(2);
            stack.Push(2);
            var token = "$+";
            var output = 4d;

            // Act
            var actual = (Stack<double>)mathParser.Invoke("SyntaxAnalysisRPN", stack, token);

            // Assert
            Assert.IsTrue(actual.Count == 1);
            Assert.AreEqual(output, actual.Peek());
        }

        [TestMethod]
        public void SyntaxAnalysisRPNTest2()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var stack = new Stack<double>();
            stack.Push(2);
            stack.Push(2);
            var token = "$un-";
            var output = -2;

            // Act
            var actual = (Stack<double>)mathParser.Invoke("SyntaxAnalysisRPN", stack, token);

            // Assert
            Assert.IsTrue(actual.Count == 2);
            Assert.AreEqual(output, actual.Peek());
        }

        [TestMethod]
        public void SyntaxAnalysisRPNTest3()
        {
            // Arrange
            var mathParser = new PrivateObject(typeof(MathParser));
            var stack = new Stack<double>();
            stack.Push(2);
            stack.Push(2);
            var token = "#4";
            var output = 4;

            // Act
            var actual = (Stack<double>)mathParser.Invoke("SyntaxAnalysisRPN", stack, token);

            // Assert
            Assert.IsTrue(actual.Count == 3);
            Assert.AreEqual(output, actual.Peek());
        }

        #endregion
    }
}
