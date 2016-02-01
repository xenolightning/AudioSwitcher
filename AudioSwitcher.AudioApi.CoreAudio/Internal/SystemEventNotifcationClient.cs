using System;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.Observables;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal sealed class SystemEventNotifcationClient : IMultimediaNotificationClient
    {
        private AsyncBroadcaster<DeviceStateChangedArgs> _deviceStateChanged;

        public IObservable<DeviceStateChangedArgs> DeviceStateChanged
        {
            get
            {
                return _deviceStateChanged.AsObservable();
            }
        }


        void IMultimediaNotificationClient.OnDeviceStateChanged(string deviceId, EDeviceState newState)
        {
            //_deviceStateChanged.OnNext(new DeviceStateChangedArgs());
        }

        void IMultimediaNotificationClient.OnDeviceAdded(string deviceId)
        {
            throw new NotImplementedException();
        }

        void IMultimediaNotificationClient.OnDeviceRemoved(string deviceId)
        {
            throw new NotImplementedException();
        }

        void IMultimediaNotificationClient.OnDefaultDeviceChanged(EDataFlow dataFlow, ERole deviceRole, string defaultDeviceId)
        {
            throw new NotImplementedException();
        }

        void IMultimediaNotificationClient.OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
        {
            throw new NotImplementedException();
        }
    }
}
