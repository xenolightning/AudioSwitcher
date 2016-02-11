using System;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    public sealed class JavaScriptDeviceType
    {
        public const string ALL = "ALL";

        public const string PLAYBACK = "PLAYBACK";

        public const string CAPTURE = "CAPTURE";

        internal JavaScriptDeviceType()
        {
        }

        internal static string GetJavascriptDeviceType(DeviceType type)
        {
            switch (type)
            {
                case DeviceType.Playback:
                    return PLAYBACK;
                case DeviceType.Capture:
                    return CAPTURE;
                case DeviceType.All:
                    return ALL;
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
