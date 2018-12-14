using System;
using System.Net;
using System.Text;
using System.Threading;

namespace SimpleWebServer
{
    ///<summary>
    /// https://www.codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server
    /// 
	/// HttpListener.GetContext method blocks while waiting for a request. 
    /// 
	/// ThreadPool.QueueUserWorkItem 可以替换为Task.Factory.StartNew()
    /// Run 方发在创建了嵌套的thread，目的是当有第二个请求时能立即处理，否则得等第一个请求处理完才执行
    /// while (_listener.IsListening) 来确保网址listen中
    ///
    /// Simple-http: 内部就是调用了HttpListener来实现的
    /// http://simple-http.net/ (https://github.com/dajuric/simple-http)
    ///
    ///</summary>
    ///<examples>
    ///static void Main(string[] args)
    ///{
    ///    WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/");
    ///    ws.Run();
    ///    Console.WriteLine("A simple webserver. Press a key to quit.");
    ///    Console.ReadKey();
    ///    ws.Stop();
    ///}
    /// public static string SendResponse(HttpListenerRequest request)
    ///{
    ///    return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);    
    ///}
    ///</examples>
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        System.Console.WriteLine("Listening...");

                        // Note: The _listener.GetContext method blocks while waiting for a request. 
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            System.Console.WriteLine("Create Listening task ... ");

                            var ctx = c as HttpListenerContext;
                            try
                            {
                                bool isEdge = ctx.Request.UserAgent.IndexOf("edge", StringComparison.InvariantCultureIgnoreCase) > -1;
                                var browserStr = isEdge ? "Edge" : "Chrome";
                                System.Console.WriteLine($"{DateTime.Now.ToLongTimeString()} receive request from: {browserStr}");

                                string rstr = _responderMethod(ctx.Request);

                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}