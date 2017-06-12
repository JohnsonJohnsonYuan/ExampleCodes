using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ZeroMQ;
using ZeroMQ.lib;

namespace ZmqSendFile_PUSHPULL
{
    public partial class Program
    {
        // INFO: You will find a "static int Main(string[] args)" in ProgramRunner.cs

        public static bool Verbose = false;

        static void Main2(string[] args)
        {
            string name = "World";

            using (var context = new ZContext())
            using (var responder = new ZSocket(context, ZSocketType.REP))
            {
                // Bind
                responder.Bind("tcp://*:5555");
                while (true)
                {
                    // Receive
                    using (ZFrame request = responder.ReceiveFrame())
                    {
                        Console.WriteLine("Received {0}", request.ReadString());
                        // Do some work
                        System.Threading.Thread.Sleep(1);
                        // Send
                        responder.Send(new ZFrame(name));
                    }
                }
            }
            return;


            var filePath = @"E:\迅雷下载\tusplus\TutsPlus-30-Days-to-Learn-jQuery\30 Days to Learn jQuery\3-The-Basics-of-Querying-the-Dom.mov";
            var content = File.ReadAllBytes(filePath);
            Console.WriteLine((float)content.Length / 1024/ 1024);
            Console.ReadKey();
            var newContents = new byte[content.Length * 2];
            Array.Copy(content, newContents, content.Length);
            Array.Copy(content, 0, newContents, content.Length, content.Length);
            File.WriteAllBytes("demo1.mov", content);
            File.WriteAllBytes("demo2.mov", newContents);
        }
    }
}
