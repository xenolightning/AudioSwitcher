using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitcher.AudioApi.Isolated
{
    public sealed class DebugSystemDeviceEnumerator : IDeviceEnumerator<IsolatedDevice>
    {
        private readonly ConcurrentBag<IsolatedDevice> _devices;
        private Guid _defaultPlaybackCommDeviceID;
        private Guid _defaultPlaybackDeviceID;
        private Guid _defaultRecordingCommDeviceID;
        private Guid _defaultRecordingDeviceID;


        public DebugSystemDeviceEnumerator()
        {
            _devices = new ConcurrentBag<IsolatedDevice>();

            //Get a copy of the current system audio devices
            //then create a copy of the current state of the system
            //this allows us to "debug" macros against a "test" system
            var devEnum = new CoreAudioDeviceEnumerator();
            _defaultPlaybackDeviceID = devEnum.DefaultPlaybackDevice == null ? Guid.Empty : devEnum.DefaultPlaybackDevice.Id;
            _defaultPlaybackCommDeviceID = devEnum.DefaultCommunicationsPlaybackDevice == null ? Guid.Empty : devEnum.DefaultCommunicationsPlaybackDevice.Id;
            _defaultRecordingDeviceID = devEnum.DefaultRecordingDevice == null ? Guid.Empty : devEnum.DefaultRecordingDevice.Id;
            _defaultRecordingCommDeviceID = devEnum.DefaultCommunicationsRecordingDevice == null ? Guid.Empty : devEnum.DefaultCommunicationsRecordingDevice.Id;

            foreach (
                CoreAudioDevice sysDev in
                    devEnum.GetDevices(DataFlow.All,
                        DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled))
            {
                var dev = new IsolatedDevice(this)
                {
                    id = sysDev.Id,
                    description = sysDev.Description,
                    shortName = sysDev.ShortName,
                    systemName = sysDev.SystemName,
                    fullName = sysDev.FullName,
                    dataFlow = sysDev.DataFlow,
                    state = sysDev.State,
                    Volume = sysDev.Volume
                };
                _devices.Add(dev);
            }
        }

        public Controller Controller { get; set; }

        public IsolatedDevice DefaultPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackDeviceID); }
        }

        public IsolatedDevice DefaultCommunicationsPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackCommDeviceID); }
        }

        public IsolatedDevice DefaultRecordingDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultRecordingDeviceID); }
        }

        public IsolatedDevice DefaultCommunicationsRecordingDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultRecordingCommDeviceID); }
        }

        public IsolatedDevice GetDevice(Guid id)
        {
            return _devices.FirstOrDefault(x => x.Id == id);
        }

        public IsolatedDevice GetDefaultDevice(DataFlow dataflow, Role eRole)
        {
            switch (dataflow)
            {
                case DataFlow.Capture:
                    if (eRole == Role.Console || eRole == Role.Multimedia)
                        return DefaultRecordingDevice;

                    return DefaultCommunicationsRecordingDevice;
                case DataFlow.Render:
                    if (eRole == Role.Console || eRole == Role.Multimedia)
                        return DefaultPlaybackDevice;

                    return DefaultCommunicationsPlaybackDevice;
            }

            return null;
        }

        public IEnumerable<IsolatedDevice> GetDevices(DataFlow dataflow, DeviceState eRole)
        {
            return _devices.Where(x =>
                (x.dataFlow == dataflow || dataflow == DataFlow.All)
                && (x.State & eRole) > 0
                );
        }

        public bool SetDefaultDevice(IsolatedDevice dev)
        {
            if (dev.IsPlaybackDevice)
            {
                _defaultPlaybackDeviceID = dev.Id;
                return true;
            }

            if (dev.IsRecordingDevice)
            {
                _defaultRecordingDeviceID = dev.Id;
                return true;
            }

            return false;
        }

        public bool SetDefaultCommunicationsDevice(IsolatedDevice dev)
        {
            if (dev.IsPlaybackDevice)
            {
                _defaultPlaybackCommDeviceID = dev.Id;
                return true;
            }

            if (dev.IsRecordingDevice)
            {
                _defaultRecordingCommDeviceID = dev.Id;
                return true;
            }

            return false;
        }

        Device IDeviceEnumerator.DefaultPlaybackDevice
        {
            get { return DefaultPlaybackDevice; }
        }

        Device IDeviceEnumerator.DefaultCommunicationsPlaybackDevice
        {
            get { return DefaultCommunicationsPlaybackDevice; }
        }

        Device IDeviceEnumerator.DefaultRecordingDevice
        {
            get { return DefaultRecordingDevice; }
        }

        Device IDeviceEnumerator.DefaultCommunicationsRecordingDevice
        {
            get { return DefaultCommunicationsRecordingDevice; }
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        Device IDeviceEnumerator.GetDevice(Guid id)
        {
            return GetDevice(id);
        }

        Device IDeviceEnumerator.GetDefaultDevice(DataFlow dataflow, Role eRole)
        {
            return GetDefaultDevice(dataflow, eRole);
        }

        IEnumerable<Device> IDeviceEnumerator.GetDevices(DataFlow dataflow, DeviceState state)
        {
            return GetDevices(dataflow, state);
        }

        public bool SetDefaultDevice(Device dev)
        {
            var device = dev as IsolatedDevice;
            if (device != null)
                return SetDefaultDevice(device);

            return false;
        }

        public bool SetDefaultCommunicationsDevice(Device dev)
        {
            var device = dev as IsolatedDevice;
            if (device != null)
                return SetDefaultDevice(device);

            return false;
        }
    }
}