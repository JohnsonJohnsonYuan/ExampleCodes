namespace MathParserTK
{
	using System;
    using System.Diagnostics;

    class Program
    {
        static Stopwatch stopWatch = new Stopwatch();

        static void Main(string[] args)
        {			
            // Check performance

            string s1 = "pi+5*5+5*3-5*5-5*3+1E1";
			string s2 = "sin(cos(tg(sh(ch(th(100))))))";
            int numberOfTests = 100000;
            long sum = 0;

            MathParser parser = new MathParser();
            var rpn = parser.ConvertToRPN(s2.ToLower()); 
            Console.WriteLine(rpn);
            return;


            for (int i = 0; i < numberOfTests; i++)
            {
                sum += Parse(s1, s2);
            }            
            Console.WriteLine("Average performance of math parser is {0} ticks",
                sum / numberOfTests);

			Console.ReadKey(true);
        }

        static long Parse(string expr1, string expr2)
        {
            MathParser parser = new MathParser();
            bool isRadians = false;

            stopWatch.Restart();
            double d1 = parser.Parse(expr1, isRadians);
            double d2 = parser.Parse(expr2, isRadians);
            stopWatch.Stop();

            return stopWatch.ElapsedTicks;
        }
    }
}
