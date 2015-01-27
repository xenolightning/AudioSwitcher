using System;
using System.IO;

namespace AudioSwitcher.Scripting
{
    public interface IScriptSource : IDisposable
    {

        TextReader GetReader();

    }
}
