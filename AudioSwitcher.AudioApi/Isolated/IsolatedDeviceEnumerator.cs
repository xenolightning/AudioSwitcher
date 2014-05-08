using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitcher.AudioApi.Isolated
{
    public sealed class DebugSystemDeviceEnumerator : IDeviceEnumerator<IsolatedAudioDevice>
    {
        private readonly ConcurrentBag<IsolatedAudioDevice> _devices;
        private Guid _defaultPlaybackCommDeviceID;
        private Guid _defaultPlaybackDeviceID;
        private Guid _defaultRecordingCommDeviceID;
        private Guid _defaultRecordingDeviceID;


        public DebugSystemDeviceEnumerator()
        {
            _devices = new ConcurrentBag<IsolatedAudioDevice>();

            //Get a copy of the current system audio devices
            //then create a copy of the current state of the system
            //this allows us to "debug" macros against a "test" system
            var devEnum = new CoreAudioDeviceEnumerator();
            _defaultPlaybackDeviceID = devEnum.DefaultPlaybackDevice.Id;
            _defaultPlaybackCommDeviceID = devEnum.DefaultCommunicationsPlaybackDevice.Id;
            _defaultRecordingDeviceID = devEnum.DefaultRecordingDevice.Id;
            _defaultRecordingCommDeviceID = devEnum.DefaultCommunicationsRecordingDevice.Id;

            foreach (
                CoreAudioDevice sysDev in
                    devEnum.GetAudioDevices(DataFlow.All,
                        DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled))
            {
                var dev = new IsolatedAudioDevice(this)
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

        public AudioController Controller { get; set; }

        public IsolatedAudioDevice DefaultPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackDeviceID); }
        }

        public IsolatedAudioDevice DefaultCommunicationsPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackCommDeviceID); }
        }

        public IsolatedAudioDevice DefaultRecordingDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultRecordingDeviceID); }
        }

        public IsolatedAudioDevice DefaultCommunicationsRecordingDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultRecordingCommDeviceID); }
        }

        public IsolatedAudioDevice GetAudioDevice(Guid id)
        {
            return _devices.FirstOrDefault(x => x.Id == id);
        }

        public IsolatedAudioDevice GetDefaultAudioDevice(DataFlow dataflow, Role eRole)
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

        public IEnumerable<IsolatedAudioDevice> GetAudioDevices(DataFlow dataflow, DeviceState eRole)
        {
            return _devices.Where(x =>
                (x.dataFlow == dataflow || dataflow == DataFlow.All)
                && (x.State & eRole) > 0
                );
        }

        public bool SetDefaultDevice(IsolatedAudioDevice dev)
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

        public bool SetDefaultCommunicationsDevice(IsolatedAudioDevice dev)
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

        AudioDevice IDeviceEnumerator.DefaultPlaybackDevice
        {
            get { return DefaultPlaybackDevice; }
        }

        AudioDevice IDeviceEnumerator.DefaultCommunicationsPlaybackDevice
        {
            get { return DefaultCommunicationsPlaybackDevice; }
        }

        AudioDevice IDeviceEnumerator.DefaultRecordingDevice
        {
            get { return DefaultRecordingDevice; }
        }

        AudioDevice IDeviceEnumerator.DefaultCommunicationsRecordingDevice
        {
            get { return DefaultCommunicationsRecordingDevice; }
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        AudioDevice IDeviceEnumerator.GetAudioDevice(Guid id)
        {
            return GetAudioDevice(id);
        }

        AudioDevice IDeviceEnumerator.GetDefaultAudioDevice(DataFlow dataflow, Role eRole)
        {
            return GetDefaultAudioDevice(dataflow, eRole);
        }

        IEnumerable<AudioDevice> IDeviceEnumerator.GetAudioDevices(DataFlow dataflow, DeviceState state)
        {
            return GetAudioDevices(dataflow, state);
        }

        public bool SetDefaultDevice(AudioDevice dev)
        {
            var device = dev as IsolatedAudioDevice;
            if (device != null)
                return SetDefaultDevice(device);

            return false;
        }

        public bool SetDefaultCommunicationsDevice(AudioDevice dev)
        {
            var device = dev as IsolatedAudioDevice;
            if (device != null)
                return SetDefaultDevice(device);

            return false;
        }
    }
}