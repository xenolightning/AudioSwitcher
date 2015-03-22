using System.IO;
using Jurassic;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    internal sealed class ScriptSourceProxy : ScriptSource
    {
        private IScriptSource _source;

        public ScriptSourceProxy(IScriptSource source)
        {
            _source = source;
        }

        public override TextReader GetReader()
        {
            return _source.GetReader();
        }

        public override string Path
        {
            get { return ""; }
        }
    }
}
