using AppHelloWorldStack = ConsoleAppHelloWorld.App.HelloWorld.Stack;

namespace ConsoleAppHelloWorld.App.HelloWorld
{
    class AppMain
    {
        public static void Run()
        {
            System.Console.WriteLine("Simple Stack in C#");
            AppHelloWorldStack.GenericStack<int> stack = new AppHelloWorldStack.GenericStack<int>();

            for (int i = 0; i < 10; i++)
            {
                stack.Push(i + 1);
            }

            try
            {
                for (int i = 0; i <= 10; i++)
                {
                    System.Console.WriteLine($"POP - {i + 1} : {stack.Pop()}");
                }
            }
            catch (System.InvalidOperationException e)
            {
                System.Console.WriteLine($"InvalidOperationException Caught: {e.Message}");
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Exception Caught: {e.Message}");
            }
        }
    }
}
