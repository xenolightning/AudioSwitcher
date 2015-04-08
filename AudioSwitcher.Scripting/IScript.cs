using System;

namespace AudioSwitcher.Scripting
{
    public interface IScript
    {
        Guid Id
        {
            get;
        }

        string Name
        {
            get;
        }

        IScriptSource Source
        {
            get;
        }

        IScriptInfo ScriptInfo
        {
            get;
        }

        IScript Clone();
    }
}