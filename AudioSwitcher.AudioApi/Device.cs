using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     Provides a basic implementation of IDevice so that developers need not implement very common functionality.
    ///     Should be used inplace of IDevice where possible
    /// </summary>
    [ComVisible(false)]
    public abstract class Device : IDevice
    {
        protected Device(IDeviceEnumerator enumerator)
        {
            Enumerator = enumerator;
        }

        public IDeviceEnumerator Enumerator { get; private set; }

        public abstract Guid Id { get; }

        public abstract string Description { get; }

        public abstract string ShortName { get; set; }

        public abstract string SystemName { get; }

        public abstract string FullName { get; }

        public abstract string IconPath { get; }

        public virtual bool IsDefaultDevice
        {
            get
            {
                return (Enumerator.DefaultPlaybackDevice != null && Enumerator.DefaultPlaybackDevice.Id == Id)
                       || (Enumerator.DefaultCaptureDevice != null && Enumerator.DefaultCaptureDevice.Id == Id);
            }
        }

        public virtual bool IsDefaultCommunicationsDevice
        {
            get
            {
                return (Enumerator.DefaultCommunicationsPlaybackDevice != null &&
                        Enumerator.DefaultCommunicationsPlaybackDevice.Id == Id)
                       ||
                       (Enumerator.DefaultCommunicationsCaptureDevice != null &&
                        Enumerator.DefaultCommunicationsCaptureDevice.Id == Id);
            }
        }

        public abstract DeviceState State { get; }

        public abstract DeviceType DeviceType { get; }

        public virtual bool IsPlaybackDevice
        {
            get { return DeviceType == DeviceType.Playback || DeviceType == DeviceType.All; }
        }

        public virtual bool IsCaptureDevice
        {
            get { return DeviceType == DeviceType.Capture || DeviceType == DeviceType.All; }
        }

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

        public virtual bool ToggleMute()
        {
            if (IsMuted)
                UnMute();
            else
                Mute();

            return IsMuted;
        }

        public abstract event AudioDeviceChangedHandler VolumeChanged;
    }
}