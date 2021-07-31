using Reflection = System.Reflection;
using System;

namespace ConsoleAppHelloWorld.App.CsharpClassTest
{
    class AppMain
    {
        public static void Run()
        {
            Type type1 = typeof(SimpleClassWithOverride);
            GetMemberInfoFromClass(ref type1);
            Type type2 = typeof(SimpleClassWithNew);
            GetMemberInfoFromClass(ref type2);
        }

        public static void GetMemberInfoFromClass(ref Type type)
        {
            Reflection.BindingFlags flags = Reflection.BindingFlags.Instance | Reflection.BindingFlags.Static |
                    Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.FlattenHierarchy;
            Reflection.MemberInfo[] memberInfos = type.GetMembers(flags);
            Console.WriteLine($"----------\nType {type.Name} has {memberInfos.Length} members:\n----------");
            foreach (var member in memberInfos)
            {
                System.Text.StringBuilder str = new();
                str.Append($"{member.Name} ({member.MemberType}): ");
                var method = member as Reflection.MethodBase;
                if (method is not null)
                {
                    bool hasAppended = false;
                    if (method.IsPublic)
                        SetMethodType(" Public");
                    else if (method.IsPrivate)
                        SetMethodType(" Private");
                    else if (method.IsFamily)
                        SetMethodType(" Protected");
                    else if (method.IsAssembly)
                        SetMethodType(" Internal");
                    else if (method.IsFamilyOrAssembly)
                        SetMethodType(" Protected Internal");
                    if (method.IsStatic) SetMethodType(" Static");
                    if (hasAppended) SetMethodType(", ");


                    void SetMethodType(string t)
                    {
                        str.Append(t);
                        hasAppended = true;
                    }
                }
                str.Append($"Declared by {member.DeclaringType}");
                Console.WriteLine(str.ToString());
            }
        }
    }

    class SimpleClassBase
    {
        public virtual void TestMethod()
        {
            //
        }
    }

    class SimpleClassWithNew : SimpleClassBase
    {
        public new void TestMethod()
        {
        }
    }

    class SimpleClassWithOverride : SimpleClassBase
    {
        public override void TestMethod()
        {
        }
    }
}
