using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AudioSwitcher.AudioApi.Sandbox
{
    public sealed class SandboxDeviceEnumerator : IDeviceEnumerator<SandboxDevice>
    {
        private readonly ConcurrentBag<SandboxDevice> _devices;
        private Guid _defaultCaptureCommDeviceId;
        private Guid _defaultCaptureDeviceId;
        private Guid _defaultPlaybackCommDeviceId;
        private Guid _defaultPlaybackDeviceId;


        public SandboxDeviceEnumerator(IDeviceEnumerator source)
        {
            _devices = new ConcurrentBag<SandboxDevice>();

            //Get a copy of the current system audio devices
            //then create a copy of the current state of the system
            //this allows us to "debug" macros against a "test" system
            _defaultPlaybackDeviceId = source.DefaultPlaybackDevice == null
                ? Guid.Empty
                : source.DefaultPlaybackDevice.Id;
            _defaultPlaybackCommDeviceId = source.DefaultCommunicationsPlaybackDevice == null
                ? Guid.Empty
                : source.DefaultCommunicationsPlaybackDevice.Id;
            _defaultCaptureDeviceId = source.DefaultCaptureDevice == null ? Guid.Empty : source.DefaultCaptureDevice.Id;
            _defaultCaptureCommDeviceId = source.DefaultCommunicationsCaptureDevice == null
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
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackDeviceId); }
        }

        public SandboxDevice DefaultCommunicationsPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackCommDeviceId); }
        }

        public SandboxDevice DefaultCaptureDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureDeviceId); }
        }

        public SandboxDevice DefaultCommunicationsCaptureDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureCommDeviceId); }
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
                _defaultPlaybackDeviceId = dev.Id;
                return true;
            }

            if (dev.IsCaptureDevice)
            {
                _defaultCaptureDeviceId = dev.Id;
                return true;
            }

            return false;
        }

        public bool SetDefaultCommunicationsDevice(SandboxDevice dev)
        {
            if (dev.IsPlaybackDevice)
            {
                _defaultPlaybackCommDeviceId = dev.Id;
                return true;
            }

            if (dev.IsCaptureDevice)
            {
                _defaultCaptureCommDeviceId = dev.Id;
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