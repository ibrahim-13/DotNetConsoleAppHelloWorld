using System;

namespace ConsoleAppHelloWorld.App.DeligatesAreDelicate
{
    class AppMain
    {
        public static void Run()
        {
            Logger logger = new();
            string writer1(string msg)
            {
                Console.WriteLine($"Writer 1 :: {msg}");
                return $"Writer 1 :: {msg}";
            };
            logger.LogWriter += writer1;
            string writer2(string msg)
            {
                Console.WriteLine($"Writer 2 :: {msg}");
                return $"Writer 2 :: {msg}";
            }
            logger.LogWriter += writer2;
            logger.WriteLog("This is a test 1");
            logger.WriteLog("This is a test 2");
        }
    }

    class Logger
    {
        public Func<string, string>? LogWriter;

        public void WriteLog(string msg)
        {
            foreach (MulticastDelegate writer in LogWriter?.GetInvocationList() ?? Array.Empty<Delegate>())
            {
                var ret = writer.DynamicInvoke(msg);
                Console.WriteLine($"Output Return :: {ret}");
            }
        }
    }
}
