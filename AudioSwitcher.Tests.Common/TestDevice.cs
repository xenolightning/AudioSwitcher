using System;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Tests.Common
{
    public sealed class TestDevice : Device
    {
        private readonly DeviceType _deviceType;
        private Guid _id;
        private bool _muted;

        public TestDevice(Guid id, DeviceType dFlow, IAudioController<TestDevice> controller)
            : base(controller)
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

        public override SpeakerConfiguration SpeakerConfiguration
        {
            get { return SpeakerConfiguration.NotSupported; }
            set { }
        }

        public override bool Mute(bool mute)
        {
            return _muted = mute;
        }

        public override event EventHandler<DeviceVolumeChangedEventArgs> VolumeChanged;

        public override int VolumeStepUp()
        {
            RaiseVolumeChanged();
            return Volume;
        }

        private void RaiseVolumeChanged()
        {
            var handler = VolumeChanged;
            if(handler != null)
                handler(this, new DeviceVolumeChangedEventArgs(this, Volume, IsMuted));
        }


        public override int VolumeStepDown()
        {
            return Volume;
        }
    }
}