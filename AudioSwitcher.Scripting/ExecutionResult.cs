using System;

namespace AudioSwitcher.Scripting
{
    public sealed class ExecutionResult
    {
        public bool Success
        {
            get;
            set;
        }

        public Exception ExecutionException
        {
            get;
            set;
        }
    }
}