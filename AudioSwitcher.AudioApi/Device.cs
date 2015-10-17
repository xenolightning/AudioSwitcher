using System;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     Provides a basic implementation of IDevice so that developers need not implement very common functionality.
    ///     Should be used in place of IDevice where possible
    /// </summary>
    public abstract class Device : IDevice, IDisposable
    {
        private readonly AsyncBroadcaster<DeviceVolumeChangedArgs> _volumeChanged;

        protected Device(IAudioController controller)
        {
            Controller = controller;
            _volumeChanged = new AsyncBroadcaster<DeviceVolumeChangedArgs>();
        }

        public IAudioController Controller { get; private set; }

        public abstract Guid Id { get; }

        public abstract string Name { get; set; }

        public abstract string InterfaceName { get; }

        public abstract string FullName { get; }

        public abstract DeviceIcon Icon { get; }

        public virtual bool IsDefaultDevice
        {
            get
            {
                return (Controller.DefaultPlaybackDevice != null && Controller.DefaultPlaybackDevice.Id == Id)
                       || (Controller.DefaultCaptureDevice != null && Controller.DefaultCaptureDevice.Id == Id);
            }
        }

        public virtual bool IsDefaultCommunicationsDevice
        {
            get
            {
                return (Controller.DefaultPlaybackCommunicationsDevice != null &&
                        Controller.DefaultPlaybackCommunicationsDevice.Id == Id)
                       ||
                       (Controller.DefaultCaptureCommunicationsDevice != null &&
                        Controller.DefaultCaptureCommunicationsDevice.Id == Id);
            }
        }

        public abstract DeviceState State { get; }

        public abstract DeviceType DeviceType { get; }

        public virtual bool IsPlaybackDevice
        {
            get
            {
                return DeviceType == DeviceType.Playback || DeviceType == DeviceType.All;
            }
        }

        public virtual bool IsCaptureDevice
        {
            get
            {
                return DeviceType == DeviceType.Capture || DeviceType == DeviceType.All;
            }
        }

        public abstract bool IsMuted { get; }

        public abstract int Volume { get; set; }

        /// <summary>
        ///     Set this device as the the default device
        /// </summary>
        public virtual bool SetAsDefault()
        {
            return Controller.SetDefaultDevice(this);
        }

        public Task<bool> SetAsDefaultAsync()
        {
            return Controller.SetDefaultDeviceAsync(this);
        }

        /// <summary>
        ///     Set this device as the default communication device
        /// </summary>
        public virtual bool SetAsDefaultCommunications()
        {
            return Controller.SetDefaultCommunicationsDevice(this);
        }

        public Task<bool> SetAsDefaultCommunicationsAsync()
        {
            return Controller.SetDefaultCommunicationsDeviceAsync(this);
        }

        public abstract bool Mute(bool mute);

        public virtual Task<bool> MuteAsync(bool mute)
        {
            return Task.Factory.StartNew(() => Mute(mute));
        }

        public virtual bool ToggleMute()
        {
            Mute(!IsMuted);

            return IsMuted;
        }

        public virtual Task<bool> ToggleMuteAsync()
        {
            return Task.Factory.StartNew(() => ToggleMute());
        }

        public IObservable<DeviceVolumeChangedArgs> VolumeChanged
        {
            get { return _volumeChanged.AsObservable(); }
        }

        protected virtual void OnVolumeChanged(int volume)
        {
            _volumeChanged.OnNext(new DeviceVolumeChangedArgs(this, volume));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            _volumeChanged.Dispose();
        }
    }
}