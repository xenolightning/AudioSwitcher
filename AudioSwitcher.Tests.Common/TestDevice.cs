using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Tests.Common
{
    public sealed class TestDevice : Device
    {
        private readonly DeviceType _deviceType;
        private readonly TestDeviceController _controller;
        private Guid _id;
        private bool _muted;
        private double _volume;

        public TestDevice(Guid id, DeviceType dFlow, TestDeviceController controller)
            : base(controller)
        {
            _id = id;
            _deviceType = dFlow;
            _controller = controller;
        }

        public override Guid Id => _id;

        public override string InterfaceName => Id.ToString();

        public override string Name
        {
            get { return Id.ToString(); }
            set { }
        }

        public override string FullName => Id.ToString();

        public override DeviceIcon Icon => DeviceIcon.Unknown;

        public override string IconPath => "";

        public override bool IsDefaultDevice => (Controller.DefaultPlaybackDevice != null && Controller.DefaultPlaybackDevice.Id == Id)
                                                || (Controller.DefaultCaptureDevice != null && Controller.DefaultCaptureDevice.Id == Id);

        public override bool IsDefaultCommunicationsDevice => (Controller.DefaultPlaybackCommunicationsDevice != null && Controller.DefaultPlaybackCommunicationsDevice.Id == Id)
                                                              || (Controller.DefaultCaptureCommunicationsDevice != null && Controller.DefaultCaptureCommunicationsDevice.Id == Id);

        public override DeviceState State => DeviceState.Active;

        public override DeviceType DeviceType => _deviceType;

        public override bool IsMuted => _muted;

        public override double Volume => _volume;

        public override bool SetAsDefault(CancellationToken cancellationToken)
        {
            _controller.SetDefaultDevice(this);

            return IsDefaultDevice;
        }

        public override Task<bool> SetAsDefaultAsync(CancellationToken cancellationToken)
        {
            _controller.SetDefaultDevice(this);

            return Task.FromResult(IsDefaultDevice);
        }

        public override bool SetAsDefaultCommunications(CancellationToken cancellationToken)
        {
            _controller.SetDefaultCommunicationsDevice(this);

            return IsDefaultCommunicationsDevice;
        }

        public override Task<bool> SetAsDefaultCommunicationsAsync(CancellationToken cancellationToken)
        {
            _controller.SetDefaultCommunicationsDevice(this);

            return Task.FromResult(IsDefaultCommunicationsDevice);
        }

        public override Task<bool> SetMuteAsync(bool mute, CancellationToken cancellationToken)
        {
            return Task.FromResult(_muted = mute);
        }

        public override Task<double> GetVolumeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_volume);
        }

        public override Task<double> SetVolumeAsync(double volume, CancellationToken cancellationToken)
        {
            return Task.FromResult(_volume = volume);
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