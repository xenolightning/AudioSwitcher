using System;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    internal sealed partial class JavaScriptAudioDevice
    {
        internal JavaScriptAudioDevice(IAudioController controller, IDevice device)
        {
            Controller = controller;
            Id = device.Id.ToString();
            Name = device.Name;
            Interface = device.InterfaceName;
            FullName = device.FullName;
            IsPlayback = device.IsPlaybackDevice;
            IsCapture = device.IsCaptureDevice;
            Type = JavaScriptDeviceType.GetJavascriptDeviceType(device.DeviceType);
            State = JavaScriptDeviceState.GetJavascriptDeviceState(device.State);
            IsDefault = device.IsDefaultDevice;
            IsDefaultComm = device.IsDefaultCommunicationsDevice;
        }

        public string Id
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
            internal set;
        }

        public string Interface
        {
            get;
            internal set;
        }

        public string FullName
        {
            get;
            internal set;
        }

        public string Type
        {
            get;
            internal set;
        }

        public bool IsPlayback
        {
            get;
            internal set;
        }

        public bool IsCapture
        {
            get;
            internal set;
        }

        public string State
        {
            get;
            internal set;
        }

        public bool IsDefault
        {
            get;
            internal set;
        }

        public bool IsDefaultComm
        {
            get;
            internal set;
        }

        public bool IsMuted => Device.IsMuted;

        private IAudioController Controller
        {
            get;
            set;
        }

        private IDevice Device => Controller.GetDevice(new Guid(Id));
    }
}