using System;

namespace ConsoleAppHelloWorld.App.DeligatesAreDelicate
{
    class AppMain
    {
        public static void Run()
        {
            LogPrinter logPrinter = new();
            logPrinter.Print();
        }
    }

    class LogPrinter
    {
        private Logger logger;

        private string Writer1(string msg)
        {
            Console.WriteLine($"Writer 1 :: {msg}");
            return $"Writer 1 :: {msg}";
        }

        private string Writer2(string msg)
        {
            Console.WriteLine($"Writer 2 :: {msg}");
            return $"Writer 2 :: {msg}";
        }

        public LogPrinter()
        {
            logger = new();
            logger.LogWriter += Writer1;
            logger.LogWriter += Writer2;
        }

        ~LogPrinter()
        {
            System.Diagnostics.Trace.WriteLine("Removing");
            logger.LogWriter -= Writer1;
            logger.LogWriter -= Writer2;
        }

        public void Print()
        {
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
