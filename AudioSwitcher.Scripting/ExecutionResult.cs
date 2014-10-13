using System;

namespace AudioSwitcher.Scripting
{
    public sealed class ExecutionResult<T>
    {
        public T Script
        {
            get;
            set;
        }

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