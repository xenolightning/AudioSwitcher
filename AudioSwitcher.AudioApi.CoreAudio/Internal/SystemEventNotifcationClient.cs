using System;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal sealed class SystemEventNotifcationClient : IMultimediaNotificationClient, IDisposable
    {
        private readonly IMultimediaDeviceEnumerator _enumerator;

        internal class DeviceStateChangedArgs
        {
            public string DeviceId { get; set; }
            public EDeviceState State { get; set; }
        }

        internal class DeviceAddedArgs
        {
            public string DeviceId { get; set; }
        }

        internal class DeviceRemovedArgs
        {
            public string DeviceId { get; set; }
        }

        internal class DefaultChangedArgs
        {
            public string DeviceId { get; set; }
            public EDataFlow DataFlow { get; set; }
            public ERole DeviceRole { get; set; }
        }

        internal class PropertyChangedArgs
        {
            public string DeviceId { get; set; }
            public PropertyKey PropertyKey { get; set; }
        }

        private readonly AsyncBroadcaster<DeviceStateChangedArgs> _deviceStateChanged;
        private readonly AsyncBroadcaster<DeviceAddedArgs> _deviceAdded;
        private readonly AsyncBroadcaster<DeviceRemovedArgs> _deviceRemoved;
        private readonly AsyncBroadcaster<DefaultChangedArgs> _defaultDeviceChanged;
        private readonly AsyncBroadcaster<PropertyChangedArgs> _propertyChanged;
        private bool _isDisposed;

        public IObservable<DeviceStateChangedArgs> DeviceStateChanged
        {
            get
            {
                return _deviceStateChanged.AsObservable();
            }
        }

        public IObservable<DeviceAddedArgs> DeviceAdded
        {
            get
            {
                return _deviceAdded.AsObservable();
            }
        }

        public IObservable<DeviceRemovedArgs> DeviceRemoved
        {
            get
            {
                return _deviceRemoved.AsObservable();
            }
        }

        public IObservable<DefaultChangedArgs> DefaultDeviceChanged
        {
            get
            {
                return _defaultDeviceChanged.AsObservable();
            }
        }

        public IObservable<PropertyChangedArgs> PropertyChanged
        {
            get
            {
                return _propertyChanged.AsObservable();
            }
        }

        public SystemEventNotifcationClient(IMultimediaDeviceEnumerator enumerator)
        {
            _enumerator = enumerator;

            _deviceStateChanged = new AsyncBroadcaster<DeviceStateChangedArgs>();
            _deviceAdded = new AsyncBroadcaster<DeviceAddedArgs>();
            _deviceRemoved = new AsyncBroadcaster<DeviceRemovedArgs>();
            _defaultDeviceChanged = new AsyncBroadcaster<DefaultChangedArgs>();
            _propertyChanged = new AsyncBroadcaster<PropertyChangedArgs>();

            enumerator.RegisterEndpointNotificationCallback(this);
        }

        void IMultimediaNotificationClient.OnDeviceStateChanged(string deviceId, EDeviceState newState)
        {
            _deviceStateChanged.OnNext(new DeviceStateChangedArgs
            {
                DeviceId = deviceId,
                State = newState
            });
        }

        void IMultimediaNotificationClient.OnDeviceAdded(string deviceId)
        {
            _deviceAdded.OnNext(new DeviceAddedArgs
            {
                DeviceId =  deviceId
            });
        }

        void IMultimediaNotificationClient.OnDeviceRemoved(string deviceId)
        {
            _deviceRemoved.OnNext(new DeviceRemovedArgs
            {
                DeviceId = deviceId
            });
        }

        void IMultimediaNotificationClient.OnDefaultDeviceChanged(EDataFlow dataFlow, ERole deviceRole, string defaultDeviceId)
        {
            _defaultDeviceChanged.OnNext(new DefaultChangedArgs()
            {
                DeviceId = defaultDeviceId,
                DataFlow = dataFlow,
                DeviceRole = deviceRole
            });

        }

        void IMultimediaNotificationClient.OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
        {
            _propertyChanged.OnNext(new PropertyChangedArgs
            {
                DeviceId = deviceId,
                PropertyKey = propertyKey
            });
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _enumerator.UnregisterEndpointNotificationCallback(this);

            _deviceStateChanged.Dispose();
            _deviceAdded.Dispose();
            _deviceRemoved.Dispose();
            _defaultDeviceChanged.Dispose();
            _propertyChanged.Dispose();

            _isDisposed = true;
        }
    }
}
