using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConsoleAppHelloWorldTest")]

namespace ConsoleAppHelloWorld.App.HelloWorld.Stack
{
    internal class GenericStack<T>
    {
        private Entry entry;

        public void Push(T data)
        {
            entry = new Entry(entry, data);
        }

        public T Pop()
        {
            if (entry == null)
            {
                throw new System.InvalidOperationException("Can not execute Pop() on empty stack");
            }
            T result = entry.Data;
            entry = entry.Next;

            return result;
        }

        private class Entry
        {
            public Entry Next { get; set; }
            public T Data { get; set; }

            public Entry(Entry next, T data)
            {
                Next = next;
                Data = data;
            }
    }
    }
}
