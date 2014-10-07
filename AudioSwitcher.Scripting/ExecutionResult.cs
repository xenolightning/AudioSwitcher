using System;
using System.Collections;
using System.Collections.Generic;

namespace AudioSwitcher.Scripting
{
    public sealed class ExecutionResult
    {

        public IScript Script { get; set; }

        public bool Success { get; set; }

        public Exception ExecutionException { get; set; }

    }
}