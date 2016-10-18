using System;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal sealed class SystemEventNotifcationClient : IDisposable
    {

        private readonly Broadcaster<DeviceStateChangedArgs> _deviceStateChanged;
        private readonly Broadcaster<DeviceAddedArgs> _deviceAdded;
        private readonly Broadcaster<DeviceRemovedArgs> _deviceRemoved;
        private readonly Broadcaster<DefaultChangedArgs> _defaultDeviceChanged;
        private readonly Broadcaster<PropertyChangedArgs> _propertyChanged;
        private ComMultimediaNotificationClient _innerClient;
        private bool _isDisposed;

        public IObservable<DeviceStateChangedArgs> DeviceStateChanged => _deviceStateChanged.AsObservable();

        public IObservable<DeviceAddedArgs> DeviceAdded => _deviceAdded.AsObservable();

        public IObservable<DeviceRemovedArgs> DeviceRemoved => _deviceRemoved.AsObservable();

        public IObservable<DefaultChangedArgs> DefaultDeviceChanged => _defaultDeviceChanged.AsObservable();

        public IObservable<PropertyChangedArgs> PropertyChanged => _propertyChanged.AsObservable();

        public SystemEventNotifcationClient(Func<IMultimediaDeviceEnumerator> enumerator)
        {
            _deviceStateChanged = new Broadcaster<DeviceStateChangedArgs>();
            _deviceAdded = new Broadcaster<DeviceAddedArgs>();
            _deviceRemoved = new Broadcaster<DeviceRemovedArgs>();
            _defaultDeviceChanged = new Broadcaster<DefaultChangedArgs>();
            _propertyChanged = new Broadcaster<PropertyChangedArgs>();

            _innerClient = new ComMultimediaNotificationClient(this);
            _innerClient.RegisterEvents(enumerator);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _innerClient.Unregister();
            _deviceStateChanged.Dispose();
            _deviceAdded.Dispose();
            _deviceRemoved.Dispose();
            _defaultDeviceChanged.Dispose();
            _propertyChanged.Dispose();

            _innerClient = null;

            _isDisposed = true;
        }

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

        private sealed class ComMultimediaNotificationClient : IMultimediaNotificationClient
        {
            private readonly SystemEventNotifcationClient _client;
            private Func<IMultimediaDeviceEnumerator> _enumeratorFunc;
            private bool _isRegistered;

            public ComMultimediaNotificationClient(SystemEventNotifcationClient client)
            {
                _client = client;
            }

            public void RegisterEvents(Func<IMultimediaDeviceEnumerator> enumerator)
            {
                //Possible race condition
                if (_isRegistered)
                    return;

                ComThread.Assert();

                _enumeratorFunc = enumerator;

                _enumeratorFunc().RegisterEndpointNotificationCallback(this);

                _isRegistered = true;
            }

            public void Unregister()
            {
                if (!_isRegistered)
                    return;

                ComThread.Assert();

                _enumeratorFunc().UnregisterEndpointNotificationCallback(this);
            }

            void IMultimediaNotificationClient.OnDeviceStateChanged(string deviceId, EDeviceState newState)
            {
                _client._deviceStateChanged.OnNext(new DeviceStateChangedArgs
                {
                    DeviceId = deviceId,
                    State = newState
                });
            }

            void IMultimediaNotificationClient.OnDeviceAdded(string deviceId)
            {
                _client._deviceAdded.OnNext(new DeviceAddedArgs
                {
                    DeviceId = deviceId
                });
            }

            void IMultimediaNotificationClient.OnDeviceRemoved(string deviceId)
            {
                _client._deviceRemoved.OnNext(new DeviceRemovedArgs
                {
                    DeviceId = deviceId
                });
            }

            void IMultimediaNotificationClient.OnDefaultDeviceChanged(EDataFlow dataFlow, ERole deviceRole, string defaultDeviceId)
            {
                _client._defaultDeviceChanged.OnNext(new DefaultChangedArgs
                {
                    DeviceId = defaultDeviceId,
                    DataFlow = dataFlow,
                    DeviceRole = deviceRole
                });
            }

            void IMultimediaNotificationClient.OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
            {
                _client._propertyChanged.OnNext(new PropertyChangedArgs
                {
                    DeviceId = deviceId,
                    PropertyKey = propertyKey
                });
            }
        }
    }
}
