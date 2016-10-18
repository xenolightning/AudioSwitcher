using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

            for (var i = 0; i < numPlaybackDevices; i++)
            {
                var id = Guid.NewGuid();
                var dev = new TestDevice(id, DeviceType.Playback, this);
                _devices.Add(dev);
            }

            for (var i = 0; i < numCaptureDevices; i++)
            {
                var id = Guid.NewGuid();
                var dev = new TestDevice(id, DeviceType.Capture, this);
                _devices.Add(dev);
            }
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
            var devId = Guid.Empty;
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

        internal bool SetDefaultDevice(TestDevice dev)
        {
            if (dev.IsPlaybackDevice)
            {
                _defaultPlaybackDeviceId = dev.Id;
                OnAudioDeviceChanged(new DefaultDeviceChangedArgs(dev));
                return true;
            }

            if (dev.IsCaptureDevice)
            {
                _defaultCaptureDeviceId = dev.Id;
                OnAudioDeviceChanged(new DefaultDeviceChangedArgs(dev));
                return true;
            }

            return false;
        }

        internal bool SetDefaultCommunicationsDevice(TestDevice dev)
        {
            if (dev.IsPlaybackDevice)
            {
                _defaultPlaybackCommDeviceId = dev.Id;
                OnAudioDeviceChanged(new DefaultDeviceChangedArgs(dev));
                return true;
            }

            if (dev.IsCaptureDevice)
            {
                _defaultCaptureCommDeviceId = dev.Id;
                OnAudioDeviceChanged(new DefaultDeviceChangedArgs(dev));
                return true;
            }

            return false;
        }

    }
}