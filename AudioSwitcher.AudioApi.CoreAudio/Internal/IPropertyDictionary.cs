using System;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public interface IPropertyDictionary : IDisposable
    {
        AccessMode Mode { get; }

        int Count { get; }

        object this[PropertyKey key] { get; set; }

        bool Contains(PropertyKey key);
    }

    [Flags]
    public enum AccessMode
    {
        Read,
        Write,
        ReadWrite = Read | Write
    }
}