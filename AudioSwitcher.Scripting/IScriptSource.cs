using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AudioSwitcher.Scripting
{
    public interface IScriptSource : IDisposable
    {

        TextReader GetReader();

    }
}
