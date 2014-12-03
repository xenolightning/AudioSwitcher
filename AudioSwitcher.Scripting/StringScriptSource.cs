using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AudioSwitcher.Scripting
{
    public sealed class StringScriptSource : IScriptSource
    {
        private StringReader _reader;

        public StringScriptSource(string script)
        {
            _reader = new StringReader(script);
        }

        public TextReader GetReader()
        {
            return _reader;
        }

        public void Dispose()
        {
            if(_reader != null)
                _reader.Dispose();

            _reader = null;
        }
    }
}
