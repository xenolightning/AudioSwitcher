using System;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Sandbox
{
    public class SandboxDevice : Device
    {
        public string fullName;
        public DeviceIcon icon;
        public string iconPath;
        public Guid id;
        public string interfaceName;
        public bool isMuted;
        public string name;
        public DeviceState state;
        public DeviceType type;
        private SandboxAudioController _controller;

        public override Guid Id => id;

        public override string InterfaceName => interfaceName;

        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
            }
        }

        public override string FullName => fullName;

        public override DeviceIcon Icon => icon;

        public override string IconPath => iconPath;

        public override bool IsDefaultDevice => (Controller.DefaultPlaybackDevice != null && Controller.DefaultPlaybackDevice.Id == Id)
                                                || (Controller.DefaultCaptureDevice != null && Controller.DefaultCaptureDevice.Id == Id);

        public override bool IsDefaultCommunicationsDevice => (Controller.DefaultPlaybackCommunicationsDevice != null && Controller.DefaultPlaybackCommunicationsDevice.Id == Id)
                                                              || (Controller.DefaultCaptureCommunicationsDevice != null && Controller.DefaultCaptureCommunicationsDevice.Id == Id);

        public override DeviceState State => state;

        public override DeviceType DeviceType => type;

        public override bool IsMuted => isMuted;

        public override double Volume { get; set; }

        public SandboxDevice(SandboxAudioController controller)
            : base(controller)
        {
            _controller = controller;
        }

        public override bool SetAsDefault()
        {
            if (IsPlaybackDevice)
                _controller.DefaultPlaybackDevice = this;
            else
                _controller.DefaultCaptureDevice = this;

            return IsDefaultDevice;
        }

        public override Task<bool> SetAsDefaultAsync()
        {
            if (IsPlaybackDevice)
                _controller.DefaultPlaybackDevice = this;
            else
                _controller.DefaultCaptureDevice = this;

            return Task.FromResult(IsDefaultDevice);
        }

        public override bool SetAsDefaultCommunications()
        {
            if (IsPlaybackDevice)
                _controller.DefaultPlaybackCommunicationsDevice = this;
            else
                _controller.DefaultCaptureCommunicationsDevice = this;

            return IsDefaultCommunicationsDevice;
        }

        public override Task<bool> SetAsDefaultCommunicationsAsync()
        {
            if (IsPlaybackDevice)
                _controller.DefaultPlaybackCommunicationsDevice = this;
            else
                _controller.DefaultCaptureCommunicationsDevice = this;

            return Task.FromResult(IsDefaultCommunicationsDevice);
        }

        public override bool Mute(bool mute)
        {
            return isMuted = mute;
        }

        public override Task<bool> MuteAsync(bool mute)
        {
            return Task.FromResult(isMuted = mute);
        }
    }
}