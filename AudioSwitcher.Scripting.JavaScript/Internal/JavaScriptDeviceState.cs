using System;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    public sealed class JavaScriptDeviceState
    {

        public const string ACTIVE = "ACTIVE";

        public const string DISABLED = "DISABLED";

        public const string NOTPRESENT = "NOTPRESENT";

        public const string UNPLUGGED = "UNPLUGGED";

        public const string ALL = "ALL";


        internal JavaScriptDeviceState()
        {
        }

        internal static string GetJavascriptDeviceState(DeviceState state)
        {
            switch (state)
            {
                case DeviceState.Active:
                    return ACTIVE;
                case DeviceState.NotPresent:
                    return NOTPRESENT;
                case DeviceState.Unplugged:
                    return UNPLUGGED;
                case DeviceState.Disabled:
                    return DISABLED;
                case DeviceState.All:
                    return ALL;
            }
            throw new ArgumentOutOfRangeException(nameof(state));
        }
    }
}
