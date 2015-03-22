using System;

namespace AudioSwitcher.Scripting
{
    public interface IScript
    {
        string Name
        {
            get;
        }

        IScriptSource Source
        {
            get;
        }

        Type ScriptInfoType
        {
            get;
        }
    }
}