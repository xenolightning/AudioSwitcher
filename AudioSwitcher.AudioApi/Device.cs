using System;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     Provides a basic implementation of IDevice so that developers need not implement very common functionality.
    ///     Should be used in place of IDevice where possible
    /// </summary>
    public abstract class Device : IDevice
    {
        protected Device(IDeviceEnumerator enumerator)
        {
            Enumerator = enumerator;
        }

        public IDeviceEnumerator Enumerator { get; private set; }

        public abstract Guid Id { get; }

        public abstract string Name { get; set; }

        public abstract string InterfaceName { get; }

        public abstract string FullName { get; }

        public abstract DeviceIcon Icon { get; }

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

        public Task<bool> SetAsDefaultAsync()
        {
            return Enumerator.SetDefaultDeviceAsync(this);
        }

        /// <summary>
        ///     Set this device as the default communication device
        /// </summary>
        public virtual bool SetAsDefaultCommunications()
        {
            return Enumerator.SetDefaultCommunicationsDevice(this);
        }

        public Task<bool> SetAsDefaultCommunicationsAsync()
        {
            return Enumerator.SetDefaultCommunicationsDeviceAsync(this);
        }

        public abstract bool Mute();
        public virtual Task<bool> MuteAsync()
        {
            return Task.Factory.StartNew(() => Mute());
        }

        public abstract bool UnMute();
        public virtual Task<bool> UnMuteAsync()
        {
            return Task.Factory.StartNew(() => UnMute());
        }

        public virtual bool ToggleMute()
        {
            if (IsMuted)
                UnMute();
            else
                Mute();

            return IsMuted;
        }

        public virtual Task<bool> ToggleMuteAsync()
        {
            return Task.Factory.StartNew(() => ToggleMute());
        }

        public abstract event AudioDeviceChangedHandler VolumeChanged;
    }
}