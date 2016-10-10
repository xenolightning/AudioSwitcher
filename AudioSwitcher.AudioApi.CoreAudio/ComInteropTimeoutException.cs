using System;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [Serializable]
    public class ComInteropTimeoutException : ApplicationException
    {

        private const string DefaultExceptionMessage = "COM Interop Operation did not complete in the requested time period.";

        public ComInteropTimeoutException()
            :base(DefaultExceptionMessage)
        {
            
        }

        public ComInteropTimeoutException(Exception innerException)
            : base(DefaultExceptionMessage, innerException)
        {

        }

        public ComInteropTimeoutException(string message)
            : base(message)
        {

        }

        public ComInteropTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

    }
}
