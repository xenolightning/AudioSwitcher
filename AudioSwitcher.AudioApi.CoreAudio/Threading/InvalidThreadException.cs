using System;

namespace AudioSwitcher.AudioApi.CoreAudio.Threading
{
    [Serializable]
    public sealed class InvalidThreadException : Exception
    {
        public InvalidThreadException(string message)
            : base(message)
        {
        }
    }
}