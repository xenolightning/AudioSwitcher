using System;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public class ClrObject
    {
        public int Field => 1;
    }

    public sealed class TestLibrary : IScriptLibrary
    {

        public TestLibrary()
        {
            Delegate = () => Property;
            DelegateWithArguments = x => x;
        }

        public int Property { get; set; }

        public Func<int> Delegate { get; private set; }

        public Func<string, string> DelegateWithArguments { get; private set; }

        public string Method()
        {
            return "Hello";
        }

        public ClrObject MethodReturnClr()
        {
            return new ClrObject();
        }
        public ClrObject[] MethodReturnClrArray()
        {
            return new[] { new ClrObject(), new ClrObject() };
        }

        public string MethodWithArg(string x)
        {
            return x;
        }

        public string MethodWithArgs(string x, string y)
        {
            return x + " + " + y;
        }

        public Func<int> MethodFunc()
        {
            return () => 1;
        }

    }
}
