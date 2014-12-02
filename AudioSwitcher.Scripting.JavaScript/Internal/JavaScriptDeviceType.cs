using System;
using AudioSwitcher.AudioApi;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    public sealed class JavaScriptDeviceType : ObjectInstance
    {
        [JSField]
        public const string ALL = "ALL";

        [JSField]
        public const string PLAYBACK = "PLAYBACK";

        [JSField]
        public const string CAPTURE = "CAPTURE";

        internal JavaScriptDeviceType(ScriptEngine engine)
            : base(engine)
        {
            PopulateFields();
            PopulateFunctions();
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

            throw new ArgumentOutOfRangeException("type");
        }
    }
}
