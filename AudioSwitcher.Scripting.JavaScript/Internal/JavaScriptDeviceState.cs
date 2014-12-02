using System;
using AudioSwitcher.AudioApi;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    public sealed class JavaScriptDeviceState : ObjectInstance
    {

        [JSField]
        public const string ACTIVE = "ACTIVE";

        [JSField]
        public const string DISABLED = "DISABLED";

        [JSField]
        public const string NOTPRESENT = "NOTPRESENT";

        [JSField]
        public const string UNPLUGGED = "UNPLUGGED";

        [JSField]
        public const string ALL = "ALL";


        internal JavaScriptDeviceState(ScriptEngine engine)
            : base(engine)
        {
            PopulateFields();
            PopulateFunctions();
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
            throw new ArgumentOutOfRangeException("state");
        }
    }
}
