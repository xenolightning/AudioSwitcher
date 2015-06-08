using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;

namespace AudioSwitcher.Tests.Common
{
    public class TestDeviceController : AudioController<TestDevice>
    {
        private readonly ConcurrentBag<TestDevice> _devices;
        private Guid _defaultCaptureCommDeviceId;
        private Guid _defaultCaptureDeviceId;
        private Guid _defaultPlaybackCommDeviceId;
        private Guid _defaultPlaybackDeviceId;


        public TestDeviceController(int numPlaybackDevices, int numCaptureDevices)
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

        public override TestDevice GetDevice(Guid id)
        {
            return GetDevice(id, DeviceState.All);
        }

        public override TestDevice GetDevice(Guid id, DeviceState state)
        {
            return _devices.FirstOrDefault(x => x.Id == id && (x.State & state) > 0);
        }

        public override TestDevice GetDefaultDevice(DeviceType deviceType, Role role)
        {
            Guid devId = Guid.Empty;
            switch (deviceType)
            {
                case DeviceType.Capture:
                    if (role == Role.Console || role == Role.Multimedia)
                        devId = _defaultCaptureDeviceId;
                    else
                        devId = _defaultCaptureCommDeviceId;
                    break;
                case DeviceType.Playback:
                    if (role == Role.Console || role == Role.Multimedia)
                        devId = _defaultPlaybackDeviceId;
                    else
                        devId = _defaultPlaybackCommDeviceId;
                    break;
            }

            return _devices.FirstOrDefault(x => x.Id == devId);
        }

        public override IEnumerable<TestDevice> GetDevices(DeviceType deviceType, DeviceState state)
        {
            return _devices.Where(x => deviceType.HasFlag(x.DeviceType) && state.HasFlag(x.State));
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

        public override bool SetDefaultDevice(TestDevice dev)
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

        public override bool SetDefaultCommunicationsDevice(TestDevice dev)
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

        public event EventHandler<DeviceChangedEventArgs> AudioDeviceChanged;

        public override bool SetDefaultDevice(IDevice dev)
        {
            var device = dev as TestDevice;
            if (device != null)
                return SetDefaultDevice(device);

            return false;
        }

        public override bool SetDefaultCommunicationsDevice(IDevice dev)
        {
            var device = dev as TestDevice;
            if (device != null)
                return SetDefaultCommunicationsDevice(device);

            return false;
        }

    }
}