using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioSwitcher.Scripting
{
    public interface IScriptOutput
    {

        void Log(string message);

        void Error(string message, Exception ex = null);

        void Clear();

    }
}
