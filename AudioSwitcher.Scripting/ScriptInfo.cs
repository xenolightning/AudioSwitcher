using System;

namespace AudioSwitcher.Scripting
{
    public interface IScriptInfo
    {

        string CommonName
        {
            get;
        }

        string OfficialName
        {
            get;
        }

        string VersionString
        {
            get;
        }

        string SyntaxHighlightingCode
        {
            get;
        }

        Type ScriptEngineType
        {
            get;
        }

    }
}