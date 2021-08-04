using System;

namespace ConsoleAppHelloWorld.App.InstanceWithActivator
{
    class AppMain
    {
        public static void Run()
        {
            TestClass? testClass1 = Activator.CreateInstance(typeof(TestClass)) as TestClass;
            if (testClass1 is not null)
            {
                Console.WriteLine($"[Constructor][Type][Default]: {testClass1.Data}");
            }
            string? currentAssembly = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            TestClass? testClass2 = currentAssembly != null
                ? Activator.CreateInstanceFrom(currentAssembly, "ConsoleAppHelloWorld.App.InstanceWithActivator.AppMain+TestClass")?.Unwrap() as TestClass
                : null;
            if (testClass2 is not null)
            {
                Console.WriteLine($"[Constructor][string][Default]: {testClass2.Data}");
            }
            TestClass? testClass3 = Activator.CreateInstance(typeof(TestClass), "new data on create") as TestClass;
            if (testClass3 is not null)
            {
                Console.WriteLine($"[Constructor][Type][InitData]: {testClass3.Data}");
            }
        }

        class TestClass
        {
            public string Data { get; set; } = "init data";

            public TestClass()
            { }

            public TestClass(string init)
            {
                Data = init;
            }
        }
    }
}
