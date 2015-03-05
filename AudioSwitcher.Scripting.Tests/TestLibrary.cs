using System;

namespace AudioSwitcher.Scripting.JavaScript.Tests
{
    public sealed class TestLibrary : IScriptLibrary
    {

        public TestLibrary()
        {
            Delegate = () => Property;
            DelegateWithArguments = x =>
            {
                return x;
            };
        }

        public int Property { get; set; }

        public Func<int> Delegate { get; private set; }

        public Func<string, string> DelegateWithArguments { get; private set; }

        public string Method()
        {
            return "Hello";
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
