using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Tests.Common
{
    public class TestDeviceEnumerator : IDeviceEnumerator<TestDevice>
    {
        private readonly ConcurrentBag<TestDevice> _devices;
        private Guid _defaultCaptureCommDeviceId;
        private Guid _defaultCaptureDeviceId;
        private Guid _defaultPlaybackCommDeviceId;
        private Guid _defaultPlaybackDeviceId;


        public TestDeviceEnumerator(int numPlaybackDevices, int numCaptureDevices)
        {
            _devices = new ConcurrentBag<TestDevice>();

            for (int i = 0; i < numPlaybackDevices; i++)
            {
                var id = Guid.NewGuid();
                var dev = new TestDevice(id, DeviceType.Playback, this);
                _devices.Add(dev);
            }

            for (int i = 0; i < numCaptureDevices; i++)
            {
                var id = Guid.NewGuid();
                var dev = new TestDevice(id, DeviceType.Capture, this);
                _devices.Add(dev);
            }
        }

        public IAudioController AudioController
        {
            get;
            set;
        }

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

        public TestDevice GetDevice(Guid id)
        {
            return GetDevice(id, DeviceState.All);
        }

        public TestDevice GetDevice(Guid id, DeviceState state)
        {
            return _devices.FirstOrDefault(x => x.Id == id && (x.State & state) > 0);
        }

        Task<TestDevice> IDeviceEnumerator<TestDevice>.GetDeviceAsync(Guid id, DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevice(id, state));
        }

        IDevice IDeviceEnumerator.GetDevice(Guid id, DeviceState state)
        {
            return GetDevice(id, state);
        }

        Task<IDevice> IDeviceEnumerator.GetDeviceAsync(Guid id)
        {
            return Task.Factory.StartNew(() => GetDevice(id) as IDevice);
        }

        Task<IDevice> IDeviceEnumerator.GetDeviceAsync(Guid id, DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevice(id, state) as IDevice);
        }

        Task<IDevice> IDeviceEnumerator.GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return Task.Factory.StartNew(() => GetDefaultDevice(deviceType, role) as IDevice);
        }

        Task<IEnumerable<IDevice>> IDeviceEnumerator.GetDevicesAsync(DeviceType deviceType, DeviceState state)
        {
            // ReSharper disable once RedundantEnumerableCastCall
            return Task.Factory.StartNew(() => GetDevices(deviceType, state).Cast<IDevice>());
        }

        public TestDevice GetDefaultDevice(DeviceType deviceType, Role eRole)
        {
            switch (deviceType)
            {
                case DeviceType.Capture:
                    if (eRole == Role.Console || eRole == Role.Multimedia)
                        return DefaultCaptureDevice;

                    return DefaultCommunicationsCaptureDevice;
                case DeviceType.Playback:
                    if (eRole == Role.Console || eRole == Role.Multimedia)
                        return DefaultPlaybackDevice;

                    return DefaultCommunicationsPlaybackDevice;
            }

            return null;
        }

        public IEnumerable<TestDevice> GetDevices(DeviceType deviceType, DeviceState eRole)
        {
            return _devices.Where(x =>
                (x.DeviceType == deviceType || deviceType == DeviceType.All)
                && (x.State & eRole) > 0
                );
        }

        public Task<TestDevice> GetDeviceAsync(Guid id)
        {
            return Task.Factory.StartNew(() => GetDevice(id));
        }

        public Task<TestDevice> GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return Task.Factory.StartNew(() => GetDefaultDevice(deviceType, role));
        }

        public Task<IEnumerable<TestDevice>> GetDevicesAsync(DeviceType deviceType, DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevices(deviceType, state));
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

        public Task<bool> SetDefaultDeviceAsync(TestDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultDevice(dev));
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

        public Task<bool> SetDefaultCommunicationsDeviceAsync(TestDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultCommunicationsDevice(dev));
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        IDevice IDeviceEnumerator.GetDevice(Guid id)
        {
            return GetDevice(id);
        }

        IDevice IDeviceEnumerator.GetDefaultDevice(DeviceType deviceType, Role eRole)
        {
            return GetDefaultDevice(deviceType, eRole);
        }

        IEnumerable<IDevice> IDeviceEnumerator.GetDevices(DeviceType deviceType, DeviceState state)
        {
            return GetDevices(deviceType, state);
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

        public Task<bool> SetDefaultDeviceAsync(IDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultDevice(dev));
        }

        public Task<bool> SetDefaultCommunicationsDeviceAsync(IDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultCommunicationsDevice(dev));
        }

        public void Dispose()
        {
        }
    }
}