using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace AudioSwitcher.Scripting
{
    public sealed class FileScriptSource : IScriptSource
    {
        public string Path
        {
            get;
            private set;
        }

        private readonly ConcurrentBag<TextReader> _readers;

        internal FileScriptSource()
        {
            _readers = new ConcurrentBag<TextReader>();
        }

        public FileScriptSource(string path)
            :this()
        {
            Path = path;
        }

        public TextReader GetReader()
        {
            var reader = File.OpenText(Path);
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
