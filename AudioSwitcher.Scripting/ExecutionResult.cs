using System;

namespace AudioSwitcher.Scripting
{
    public class ExecutionResult
    {
        public bool Success
        {
            get;
            set;
        }

        public ExecutionException ExecutionException
        {
            get;
            set;
        }
    }

    public sealed class ExecutionResult<T> : ExecutionResult
    {

        public ExecutionResult()
        {

        }

        public ExecutionResult(T value)
        {
            Value = value;
        }

        public T Value
        {
            get;
            set;
        }

    }

    [Serializable]
    public class ExecutionException : Exception
    {
        public int LineNumber
        {
            get;
            set;
        }

        public ExecutionException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }
    }
}