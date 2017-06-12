using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ZeroMQ;
using System.IO;

namespace ZmqSendFile_PUSHPULL
{
    static partial class Program
    {
        /// <summary>
        /// Worker for save file.
        /// </summary>
        /// <param name="args">
        /// 
        /// </param>
        public static void TaskWork(string[] args)
        {
            //
            // Task worker
            // Connects PULL socket to tcp://127.0.0.1:5557
            // Collects workloads from ventilator via that socket
            // Connects PUSH socket to tcp://127.0.0.1:5558
            // Sends results to sink via that socket
            //
            // Author: metadings
            //

            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                throw new ArgumentException("请输入保存地址参数");
            // Read task list
            if (!Directory.Exists(args[0]))
                Directory.CreateDirectory(args[0]);

            // Socket to receive messages on and
            // Socket to send messages to
            using (var context = new ZContext())
            using (var receiver = new ZSocket(context, ZSocketType.PULL))
            using (var sink = new ZSocket(context, ZSocketType.PUSH))
            {
                receiver.Connect(SettingsModel.Instance.VentConnect);
                sink.Connect(SettingsModel.Instance.SinkConnect);

                // Initialize random number generator
                var rnd = new Random();

                // Process tasks forever
                while (true)
                {
                    // file path
                    ZFrame reply = receiver.ReceiveFrame();

                    var filePath = reply.ReadString();

                    // file contents
                    var replyBytes = File.ReadAllBytes(filePath);

                    // 模拟延迟
                    //int workload = rnd.Next(5000) + 1;
                    //Thread.Sleep(workload);
                    Thread.Sleep(500);

                    File.WriteAllBytes(Path.Combine(args[0], Path.GetFileName(filePath)), replyBytes);

                    Console.WriteLine("复制完成文件 " + filePath);

                    sink.Send(new byte[0], 0, 0);	// Send results to sink
                }
            }
        }
    }
}