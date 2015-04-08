using System;

namespace AudioSwitcher.Scripting
{
    public abstract class ScriptInfo : IScriptInfo
    {
        public abstract string CommonName { get; }
        public abstract string OfficialName { get; }
        public abstract string VersionString { get; }
        public abstract string SyntaxHighlightingCode { get; }
        public abstract string FileExtension { get; }
        public abstract Type ScriptEngineType { get; }

        public ScriptInfo()
        {
        }

    }
}