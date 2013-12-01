using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public abstract class AudioDevice : IAudioDevice
    {
        protected AudioDevice(IDeviceEnumerator enumerator)
        {
            Enumerator = enumerator;
        }

        public IDeviceEnumerator Enumerator { get; private set; }

        public abstract Guid ID { get; }

        public abstract string Description { get; }

        public abstract string ShortName { get; }

        public abstract string SystemName { get; }

        public abstract string FullName { get; }

        public abstract bool IsDefaultDevice { get; }

        public abstract bool IsDefaultCommunicationsDevice { get; }

        public abstract DeviceState State { get; }

        public abstract DataFlow DataFlow { get; }

        public abstract bool IsPlaybackDevice { get; }

        public abstract bool IsRecordingDevice { get; }

        public abstract bool IsMuted { get; }

        public abstract int Volume { get; set; }

        /// <summary>
        ///     Set this device as the the default device
        /// </summary>
        public virtual bool SetAsDefault()
        {
            return Enumerator.SetDefaultDevice(this);
        }

        /// <summary>
        ///     Set this device as the default communication device
        /// </summary>
        public virtual bool SetAsDefaultCommunications()
        {
            return Enumerator.SetDefaultCommunicationsDevice(this);
        }

        public abstract bool Mute();

        public abstract bool UnMute();

        public abstract bool ToggleMute();

        public abstract Icon GetIcon();

        public abstract Icon GetIcon(int width, int height);
    }
}