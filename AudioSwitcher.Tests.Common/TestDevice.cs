using System;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Tests.Common
{
    public sealed class TestDevice : Device
    {
        public TestDevice(Guid id, DeviceType dFlow, IDeviceEnumerator enumerator)
            : base(enumerator)
        {
            _id = id;
            _deviceType = dFlow;
        }

        public Guid _id;
        public override Guid Id
        {
            get
            {
                return _id;
            }
        }

        public override string RealId
        {
            get { return Id.ToString(); }
        }

        public override string Description
        {
            get { return Id.ToString(); }
        }

        public override string ShortName
        {
            get { return Id.ToString(); }
            set { }
        }

        public override string SystemName
        {
            get { return Id.ToString(); }
        }

        public override string FullName
        {
            get { return Id.ToString(); }
        }

        public override string IconPath
        {
            get { return Id.ToString(); }
        }

        public override DeviceState State
        {
            get { return DeviceState.Active; }
        }

        private readonly DeviceType _deviceType;
        public override DeviceType DeviceType
        {
            get
            {
                return _deviceType;
            }
        }

        private bool _muted;
        public override bool IsMuted
        {
            get
            {
                return _muted;
            }
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
