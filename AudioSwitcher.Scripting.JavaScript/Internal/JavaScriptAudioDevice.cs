using System;
using AudioSwitcher.AudioApi;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    internal sealed partial class JavaScriptAudioDevice : ObjectInstance
    {
        internal JavaScriptAudioDevice(ScriptEngine engine, AudioController controller, IDevice device)
            : base(engine)
        {
            Controller = controller;
            Id = device.Id.ToString();
            Name = device.Name;
            Interface = device.InterfaceName;
            FullName = device.FullName;
            IsPlayback = device.IsPlaybackDevice;
            IsCapture = device.IsPlaybackDevice;
            Type = JavaScriptDeviceType.GetJavascriptDeviceType(device.DeviceType);
            State = JavaScriptDeviceState.GetJavascriptDeviceState(device.State);
            IsDefault = device.IsDefaultDevice;
            IsDefaultComm = device.IsDefaultCommunicationsDevice;

            PopulateFields();
            PopulateFunctions();
        }

        [JSProperty(Name = "id")]
        public string Id
        {
            get;
            internal set;
        }

        [JSProperty(Name = "name")]
        public string Name
        {
            get;
            internal set;
        }

        [JSProperty(Name = "interface")]
        public string Interface
        {
            get;
            internal set;
        }

        [JSProperty(Name = "fullName")]
        public string FullName
        {
            get;
            internal set;
        }

        [JSProperty(Name = "type")]
        public string Type
        {
            get;
            internal set;
        }

        [JSProperty(Name = "isPlayback")]
        public bool IsPlayback
        {
            get;
            internal set;
        }

        [JSProperty(Name = "isCapture")]
        public bool IsCapture
        {
            get;
            internal set;
        }

        [JSProperty(Name = "state")]
        public string State
        {
            get;
            internal set;
        }

        [JSProperty(Name = "isDefault")]
        public bool IsDefault
        {
            get;
            internal set;
        }

        [JSProperty(Name = "isDefaultComm")]
        public bool IsDefaultComm
        {
            get;
            internal set;
        }

        [JSProperty(Name = "isMuted")]
        public bool IsMuted
        {
            get { return Device.IsMuted; }
        }

        private AudioController Controller
        {
            get;
            set;
        }

        private IDevice Device
        {
            get
            {
                //Ensures that the AudioController is always referencing the correct device
                //instance
                return Controller.GetAudioDevice(new Guid(Id));
            }
        }
    }
}