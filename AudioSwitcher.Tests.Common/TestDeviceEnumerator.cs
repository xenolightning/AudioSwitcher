using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Tests.Common
{
    public class TestDeviceEnumerator : IDeviceEnumerator<TestDevice>
    {
        private readonly ConcurrentBag<TestDevice> _devices;
        private Guid _defaultPlaybackCommDeviceId;
        private Guid _defaultPlaybackDeviceId;
        private Guid _defaultCaptureCommDeviceId;
        private Guid _defaultCaptureDeviceId;


        public TestDeviceEnumerator(int numPlaybackDevices, int numRecordingDevices)
        {
            _devices = new ConcurrentBag<TestDevice>();

            for (int i = 0; i < numPlaybackDevices; i++)
            {
                var id = Guid.NewGuid();
                var dev = new TestDevice(id, DataFlow.Render, this);
                _devices.Add(dev);
            }

            for (int i = 0; i < numRecordingDevices; i++)
            {
                var id = Guid.NewGuid();
                var dev = new TestDevice(id, DataFlow.Capture, this);
                _devices.Add(dev);
            }
        }

        public AudioController AudioController { get; set; }

        public TestDevice DefaultPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackDeviceId); }
        }

        public TestDevice DefaultCommunicationsPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackCommDeviceId); }
        }

        public TestDevice DefaultCaptureDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureDeviceId); }
        }

        public TestDevice DefaultCommunicationsCaptureDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureCommDeviceId); }
        }

        public TestDevice GetDevice(Guid id)
        {
            return _devices.FirstOrDefault(x => x.Id == id);
        }

        public TestDevice GetDefaultDevice(DataFlow dataflow, Role eRole)
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

        public IEnumerable<TestDevice> GetDevices(DataFlow dataflow, DeviceState eRole)
        {
            return _devices.Where(x =>
                (x.DataFlow == dataflow || dataflow == DataFlow.All)
                && (x.State & eRole) > 0
                );
        }

        public bool SetDefaultDevice(TestDevice dev)
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

        public bool SetDefaultCommunicationsDevice(TestDevice dev)
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
            var device = dev as TestDevice;
            if (device != null)
                return SetDefaultDevice(device);

            return false;
        }

        public bool SetDefaultCommunicationsDevice(IDevice dev)
        {
            var device = dev as TestDevice;
            if (device != null)
                return SetDefaultCommunicationsDevice(device);

            return false;
        }
    }
}
