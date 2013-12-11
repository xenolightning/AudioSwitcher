using System;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.AudioApi.Isolated
{
    public class IsolatedAudioDevice : AudioDevice
    {
        public IsolatedAudioDevice(IDeviceEnumerator enumerator)
            : base(enumerator)
        {

        }

        public Guid id;
        public override Guid ID
        {
            get { return id; }
        }

        public string description;
        public override string Description
        {
            get { return description; }
        }

        public string shortName;
        public override string ShortName
        {
            get { return shortName; }
        }

        public string systemName;
        public override string SystemName
        {
            get { return systemName; }
        }

        public string fullName;
        public override string FullName
        {
            get { return fullName; }
        }

        public string iconPath;
        public override string IconPath
        {
            get { return iconPath; }
        }

        public DeviceState state;
        public override DeviceState State
        {
            get { return state; }
        }

        public DataFlow dataFlow;
        public override DataFlow DataFlow
        {
            get { return dataFlow; }
        }

        public bool isMuted;

        public override bool IsMuted
        {
            get { return isMuted; }
        }

        public override int Volume
        {
            get;
            set;
        }

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
