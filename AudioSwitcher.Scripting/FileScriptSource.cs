using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AudioSwitcher.Scripting
{
    public sealed class FileScriptSource : IScriptSource
    {
        private readonly string _path;
        private readonly ConcurrentBag<TextReader> _readers;

        public FileScriptSource(string path)
        {
            _path = path;
            _readers = new ConcurrentBag<TextReader>();
        }

        public TextReader GetReader()
        {
            var reader = File.OpenText(_path);
            _readers.Add(reader);

            return reader;
        }

        public void Dispose()
        {
            foreach (var reader in _readers.Where(reader => reader != null))
            {
                reader.Dispose();
            }
        }
    }
}
