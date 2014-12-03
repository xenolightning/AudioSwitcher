using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        ~ScriptSourceProxy()
        {
            if(_source != null)
                _source.Dispose();

            _source = null;
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
