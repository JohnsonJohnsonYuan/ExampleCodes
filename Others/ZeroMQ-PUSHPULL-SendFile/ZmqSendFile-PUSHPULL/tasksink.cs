using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using ZeroMQ;

namespace  ZmqSendFile_PUSHPULL
{
	static partial class Program
	{
		public static void TaskSink(string[] args)
		{
			//
			// Task sink
			// Binds PULL socket to tcp://127.0.0.1:5558
			// Collects results from workers via that socket
			//
			// Author: metadings
			//

			// Prepare our context and socket
			using (var context = new ZContext())
			using (var sink = new ZSocket(context, ZSocketType.PULL))
			{
                sink.Bind(SettingsModel.Instance.SinkConnect);

                Console.WriteLine("Start receive");
				// Wait for start of batch
                var frame = sink.ReceiveFrame();

                var taskCount = frame.ReadInt32();

				// Start our clock now
				var stopwatch = new Stopwatch();
				stopwatch.Start();

                Console.WriteLine("一共有" + taskCount + "个文件需要处理");
                for (int i = 0; i < taskCount; ++i)
                {
                    var reply = sink.ReceiveFrame();

                    if ((i / 10) * 10 == i)
                        Console.Write(":");
                    else
                        Console.Write(".");
                }

				// Calculate and report duration of batch
				stopwatch.Stop();
                Console.WriteLine();
				Console.WriteLine("Total elapsed time: {0} ms", stopwatch.ElapsedMilliseconds);
			}
		}
	}
}