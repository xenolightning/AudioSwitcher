using System;
using System.Collections.Generic;
using System.Threading;
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
        public double volume;
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

        public override bool IsDefaultDevice
            => (Controller.DefaultPlaybackDevice != null && Controller.DefaultPlaybackDevice.Id == Id)
               || (Controller.DefaultCaptureDevice != null && Controller.DefaultCaptureDevice.Id == Id);

        public override bool IsDefaultCommunicationsDevice
            =>
                (Controller.DefaultPlaybackCommunicationsDevice != null &&
                 Controller.DefaultPlaybackCommunicationsDevice.Id == Id)
                ||
                (Controller.DefaultCaptureCommunicationsDevice != null &&
                 Controller.DefaultCaptureCommunicationsDevice.Id == Id);

        public override DeviceState State => state;

        public override DeviceType DeviceType => type;

        public override bool IsMuted => isMuted;
        public override double Volume { get { return volume; } }


        public SandboxDevice(SandboxAudioController controller)
            : base(controller)
        {
            _controller = controller;
        }

        public override bool SetAsDefault(CancellationToken cancellationToken)
        {
            _controller.SetDefaultDevice(this);

            return IsDefaultDevice;
        }

        public override Task<bool> SetAsDefaultAsync(CancellationToken cancellationToken)
        {
            _controller.SetDefaultDevice(this);

            return TaskShim.FromResult(IsDefaultDevice);
        }

        public override bool SetAsDefaultCommunications(CancellationToken cancellationToken)
        {
            _controller.SetDefaultCommunicationsDevice(this);

            return IsDefaultCommunicationsDevice;
        }

        public override Task<bool> SetAsDefaultCommunicationsAsync(CancellationToken cancellationToken)
        {
            _controller.SetDefaultCommunicationsDevice(this);

            return TaskShim.FromResult(IsDefaultCommunicationsDevice);
        }

        public override Task<bool> SetMuteAsync(bool mute, CancellationToken cancellationToken)
        {
            return TaskShim.FromResult(isMuted = mute);
        }

        public override Task<double> GetVolumeAsync(CancellationToken cancellationToken)
        {
            return TaskShim.FromResult(volume);
        }

        public override Task<double> SetVolumeAsync(double ivol, CancellationToken cancellationToken)
        {
            return TaskShim.FromResult(volume = ivol);
        }

        public override bool HasCapability<TCapability>()
        {
            return false;
        }

        public override TCapability GetCapability<TCapability>()
        {
            return default(TCapability);
        }

        public override IEnumerable<IDeviceCapability> GetAllCapabilities()
        {
            yield return null;
        }
    }
}