using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AudioSwitcher.AudioApi.Sandbox
{
    public sealed class SandboxDeviceEnumerator : IDeviceEnumerator<SandboxDevice>
    {
        private readonly ConcurrentBag<SandboxDevice> _devices;
        private Guid _defaultCaptureCommDeviceID;
        private Guid _defaultCaptureDeviceID;
        private Guid _defaultPlaybackCommDeviceID;
        private Guid _defaultPlaybackDeviceID;


        public SandboxDeviceEnumerator(IDeviceEnumerator source)
        {
            _devices = new ConcurrentBag<SandboxDevice>();

            //Get a copy of the current system audio devices
            //then create a copy of the current state of the system
            //this allows us to "debug" macros against a "test" system
            _defaultPlaybackDeviceID = source.DefaultPlaybackDevice == null
                ? Guid.Empty
                : source.DefaultPlaybackDevice.Id;
            _defaultPlaybackCommDeviceID = source.DefaultCommunicationsPlaybackDevice == null
                ? Guid.Empty
                : source.DefaultCommunicationsPlaybackDevice.Id;
            _defaultCaptureDeviceID = source.DefaultCaptureDevice == null ? Guid.Empty : source.DefaultCaptureDevice.Id;
            _defaultCaptureCommDeviceID = source.DefaultCommunicationsCaptureDevice == null
                ? Guid.Empty
                : source.DefaultCommunicationsCaptureDevice.Id;

            foreach (
                IDevice sourceDev in
                    source.GetDevices(DataFlow.All,
                        DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled))
            {
                var dev = new SandboxDevice(this)
                {
                    id = sourceDev.Id,
                    description = sourceDev.Description,
                    shortName = sourceDev.ShortName,
                    systemName = sourceDev.SystemName,
                    fullName = sourceDev.FullName,
                    dataFlow = sourceDev.DataFlow,
                    state = sourceDev.State,
                    Volume = sourceDev.Volume
                };
                _devices.Add(dev);
            }
        }

        public AudioController AudioController { get; set; }

        public SandboxDevice DefaultPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackDeviceID); }
        }

        public SandboxDevice DefaultCommunicationsPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackCommDeviceID); }
        }

        public SandboxDevice DefaultCaptureDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureDeviceID); }
        }

        public SandboxDevice DefaultCommunicationsCaptureDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureCommDeviceID); }
        }

        public SandboxDevice GetDevice(Guid id)
        {
            return _devices.FirstOrDefault(x => x.Id == id);
        }

        public SandboxDevice GetDefaultDevice(DataFlow dataflow, Role eRole)
        {
            switch (dataflow)
            {
                case DataFlow.Capture:
                    if (eRole == Role.Console || eRole == Role.Multimedia)
                        return DefaultCaptureDevice;

                    return DefaultCommunicationsCaptureDevice;
                case DataFlow.Render:
                    if (eRole == Role.Console || eRole == Role.Multimedia)
                        return DefaultPlaybackDevice;

                    return DefaultCommunicationsPlaybackDevice;
            }

            return null;
        }

        public IEnumerable<SandboxDevice> GetDevices(DataFlow dataflow, DeviceState eRole)
        {
            return _devices.Where(x =>
                (x.dataFlow == dataflow || dataflow == DataFlow.All)
                && (x.State & eRole) > 0
                );
        }

        public bool SetDefaultDevice(SandboxDevice dev)
        {
            if (dev.IsPlaybackDevice)
            {
                _defaultPlaybackDeviceID = dev.Id;
                return true;
            }

            if (dev.IsCaptureDevice)
            {
                _defaultCaptureDeviceID = dev.Id;
                return true;
            }

            return false;
        }

        public bool SetDefaultCommunicationsDevice(SandboxDevice dev)
        {
            if (dev.IsPlaybackDevice)
            {
                _defaultPlaybackCommDeviceID = dev.Id;
                return true;
            }

            if (dev.IsCaptureDevice)
            {
                _defaultCaptureCommDeviceID = dev.Id;
                return true;
            }

            return false;
        }

        IDevice IDeviceEnumerator.DefaultPlaybackDevice
        {
            get { return DefaultPlaybackDevice; }
        }

        IDevice IDeviceEnumerator.DefaultCommunicationsPlaybackDevice
        {
            get { return DefaultCommunicationsPlaybackDevice; }
        }

        IDevice IDeviceEnumerator.DefaultCaptureDevice
        {
            get { return DefaultCaptureDevice; }
        }

        IDevice IDeviceEnumerator.DefaultCommunicationsCaptureDevice
        {
            get { return DefaultCommunicationsCaptureDevice; }
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        IDevice IDeviceEnumerator.GetDevice(Guid id)
        {
            return GetDevice(id);
        }

        IDevice IDeviceEnumerator.GetDefaultDevice(DataFlow dataflow, Role eRole)
        {
            return GetDefaultDevice(dataflow, eRole);
        }

        IEnumerable<IDevice> IDeviceEnumerator.GetDevices(DataFlow dataflow, DeviceState state)
        {
            return GetDevices(dataflow, state);
        }

        public bool SetDefaultDevice(IDevice dev)
        {
            var device = dev as SandboxDevice;
            if (device != null)
                return SetDefaultDevice(device);

            return false;
        }

        public bool SetDefaultCommunicationsDevice(IDevice dev)
        {
            var device = dev as SandboxDevice;
            if (device != null)
                return SetDefaultCommunicationsDevice(device);

            return false;
        }
    }
}