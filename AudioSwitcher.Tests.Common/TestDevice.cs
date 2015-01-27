using System;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Tests.Common
{
    public sealed class TestDevice : Device
    {
        private readonly DeviceType _deviceType;
        private Guid _id;
        private bool _muted;

        public TestDevice(Guid id, DeviceType dFlow, IDeviceEnumerator enumerator)
            : base(enumerator)
        {
            _id = id;
            _deviceType = dFlow;
        }

        public override Guid Id
        {
            get { return _id; }
        }

        public override string InterfaceName
        {
            get { return Id.ToString(); }
        }

        public override string Name
        {
            get { return Id.ToString(); }
            set { }
        }

        public override string FullName
        {
            get { return Id.ToString(); }
        }

        public override DeviceIcon Icon
        {
            get { return DeviceIcon.Unknown; }
        }

        public override DeviceState State
        {
            get { return DeviceState.Active; }
        }

        public override DeviceType DeviceType
        {
            get { return _deviceType; }
        }

        public override bool IsMuted
        {
            get { return _muted; }
        }

        public override int Volume
        {
            get;
            set;
        }

        public override bool Mute()
        {
            return _muted = true;
        }

        public override bool UnMute()
        {
            return _muted = false;
        }

        public override event AudioDeviceChangedHandler VolumeChanged;
    }
}