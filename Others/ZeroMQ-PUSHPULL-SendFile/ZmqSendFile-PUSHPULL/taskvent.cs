using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ZeroMQ;
using System.IO;

namespace  ZmqSendFile_PUSHPULL
{
	static partial class Program
	{
        /// <summary>
        /// Ventilator for send tasks.
        /// </summary>
        /// <param name="args">
        /// 1st arg: File list path will be copied from.
        /// </param>
		public static void TaskVent(string[] args)
		{
			//
			// Task ventilator
			// Binds PUSH socket to tcp://127.0.0.1:5557
			// Sends batch of tasks to workers via that socket
			//
			// Author: metadings
			//

			// Socket to send messages on and
			// Socket to send start of batch message on

            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                throw new ArgumentException("请输入文件列表参数");
            // Read task list
            if (!File.Exists(args[0]))
                throw new FileNotFoundException(args[0]);

			using (var context = new ZContext())
			using (var sender = new ZSocket(context, ZSocketType.PUSH))
			using (var sink = new ZSocket(context, ZSocketType.PUSH))
			{
				sender.Bind(SettingsModel.Instance.VentBind);
				sink.Connect(SettingsModel.Instance.SinkConnect);

				Console.WriteLine("Press ENTER when the workers are ready...");
				Console.ReadKey(true);
				Console.WriteLine("Sending tasks to workers...");

                var tasks = File.ReadAllLines(args[0]).Where(line => !string.IsNullOrEmpty(line) && File.Exists(line));

                var taskCount = tasks.Count();
				// The first message is task count
				sink.Send(new ZFrame(taskCount));

                foreach (var task in tasks)
                {
                    Console.WriteLine("Sending: " + task);

                    // Send file name first
                    sender.Send(new ZFrame(task));
                }

                Console.WriteLine("共发送文件个数: " + taskCount);
			}
		}
	}
}