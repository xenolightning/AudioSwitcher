using System.IO;

namespace AudioSwitcher.Scripting
{
    public sealed class StringScriptSource : IScriptSource
    {
        public string Script
        {
            get;
            private set;
        }

        public StringScriptSource(string script)
        {
            Script = script;
        }

        public TextReader GetReader()
        {
            return new StringReader(Script);
        }

        public void Dispose()
        {
        }
    }
}
