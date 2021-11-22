using ConsoleAppHelloWorld.App.HelloWorld.Stack;
using Xunit;

namespace ConsoleAppHelloWorldTest;

public class HelloWorldStack
{
  [Fact]
  public void PushPop()
  {
    GenericStack<int> v1 = new ();
    v1.Push(1);
    v1.Push(2);
    v1.Push(3);
    Assert.Equal(3, v1.Pop());
    Assert.Equal(2, v1.Pop());
    Assert.Equal(1, v1.Pop());
  }

  [Theory]
  [InlineData(101)]
  [InlineData(151)]
  public void StoreGenericDataInt(int data)
  {
    GenericStack<int> v1 = new();
    v1.Push(data);
    Assert.Equal(data, v1.Pop());
  }

  [Theory]
  [InlineData("data1")]
  [InlineData("data2")]
  public void StoreGenericDataString(string data)
  {
    GenericStack<string> v1 = new();
    v1.Push(data);
    Assert.Equal(data, v1.Pop());
  }
}