using System;

namespace AudioSwitcher.AudioApi.Isolated
{
    public class IsolatedAudioDevice : AudioDevice
    {
        public DataFlow dataFlow;
        public string description;
        public string fullName;
        public string iconPath;
        public Guid id;
        public bool isMuted;
        public string shortName;
        public DeviceState state;
        public string systemName;

        public IsolatedAudioDevice(IDeviceEnumerator enumerator)
            : base(enumerator)
        {
        }

        public override Guid Id
        {
            get { return id; }
        }

        public override string SystemId
        {
            get { return Id.ToString(); }
        }

        public override string Description
        {
            get { return description; }
        }

        public override string ShortName
        {
            get { return shortName; }
            set { }
        }

        public override string SystemName
        {
            get { return systemName; }
        }

        public override string FullName
        {
            get { return fullName; }
        }

        public override string IconPath
        {
            get { return iconPath; }
        }

        public override DeviceState State
        {
            get { return state; }
        }

        public override DataFlow DataFlow
        {
            get { return dataFlow; }
        }

        public override bool IsMuted
        {
            get { return isMuted; }
        }

        public override int Volume { get; set; }

        public override bool Mute()
        {
            return isMuted = true;
        }

        public override bool UnMute()
        {
            return isMuted = false;
        }
    }
}