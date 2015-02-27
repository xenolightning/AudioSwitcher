using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Sandbox
{
    public class SandboxAudioController : AudioController<SandboxDevice>
    {
        private List<SandboxDevice> _devices;
        private Guid _defaultPlaybackDeviceId;
        private Guid _defaultPlaybackCommDeviceId;
        private Guid _defaultCaptureDeviceId;
        private Guid _defaultCaptureCommDeviceId;

        public SandboxAudioController(IAudioController source)
        {
            _devices = new List<SandboxDevice>();

            //Get a copy of the current system audio devices
            //then create a copy of the current state of the system
            //this allows us to "debug" macros against a "test" system
            _defaultPlaybackDeviceId = source.DefaultPlaybackDevice == null
                ? Guid.Empty
                : source.DefaultPlaybackDevice.Id;
            _defaultPlaybackCommDeviceId = source.DefaultPlaybackCommunicationsDevice == null
                ? Guid.Empty
                : source.DefaultPlaybackCommunicationsDevice.Id;
            _defaultCaptureDeviceId = source.DefaultCaptureDevice == null ? Guid.Empty : source.DefaultCaptureDevice.Id;
            _defaultCaptureCommDeviceId = source.DefaultCaptureCommunicationsDevice == null
                ? Guid.Empty
                : source.DefaultCaptureCommunicationsDevice.Id;

            foreach (
                IDevice sourceDev in
                    source.GetDevices(DeviceType.All,
                        DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled))
            {
                var dev = new SandboxDevice(this)
                {
                    id = sourceDev.Id,
                    interfaceName = sourceDev.InterfaceName,
                    icon = sourceDev.Icon,
                    name = sourceDev.Name,
                    fullName = sourceDev.FullName,
                    type = sourceDev.DeviceType,
                    state = sourceDev.State,
                    Volume = sourceDev.Volume
                };
                _devices.Add(dev);
            }
        }

        public override SandboxDevice DefaultPlaybackDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackDeviceId); }
        }

        public override SandboxDevice DefaultPlaybackCommunicationsDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultPlaybackCommDeviceId); }
        }

        public override SandboxDevice DefaultCaptureDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureDeviceId); }
        }

        public override SandboxDevice DefaultCaptureCommunicationsDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureCommDeviceId); }
        }

        public SandboxDevice DefaultCommunicationsCaptureDevice
        {
            get { return _devices.FirstOrDefault(x => x.Id == _defaultCaptureCommDeviceId); }
        }

        public override SandboxDevice GetDevice(Guid id)
        {
            return GetDevice(id, DeviceState.All);
        }

        public override SandboxDevice GetDevice(Guid id, DeviceState state)
        {
            return _devices.FirstOrDefault(x => x.Id == id && state.HasFlag(x.State));
        }

        public override SandboxDevice GetDefaultDevice(DeviceType deviceType, Role role)
        {
            switch (deviceType)
            {
                case DeviceType.Capture:
                    if (role == Role.Console || role == Role.Multimedia)
                        return DefaultCaptureDevice;

                    return DefaultCommunicationsCaptureDevice;
                case DeviceType.Playback:
                    if (role == Role.Console || role == Role.Multimedia)
                        return DefaultPlaybackDevice;

                    return DefaultPlaybackCommunicationsDevice;
            }

            return null;
        }

        public override IEnumerable<SandboxDevice> GetDevices(DeviceState state)
        {
            return _devices.Where(x => state.HasFlag(x.State));
        }

        public override IEnumerable<SandboxDevice> GetDevices(DeviceType deviceType, DeviceState state)
        {
            return _devices.Where(x =>
                (x.type == deviceType || deviceType == DeviceType.All)
                && state.HasFlag(x.State)
                );
        }

        public override IEnumerable<SandboxDevice> GetPlaybackDevices(DeviceState state)
        {
            return _devices.Where(x => state.HasFlag(x.State));
        }

        public override IEnumerable<SandboxDevice> GetCaptureDevices(DeviceState state)
        {
            return _devices.Where(x => state.HasFlag(x.State));
        }

        public override bool SetDefaultDevice(SandboxDevice dev)
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

        public override bool SetDefaultCommunicationsDevice(SandboxDevice dev)
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

        public override bool SetDefaultDevice(IDevice dev)
        {
            var device = dev as SandboxDevice;
            if (device != null)
                return SetDefaultDevice(device);

            return false;
        }

        public override bool SetDefaultCommunicationsDevice(IDevice dev)
        {
            var device = dev as SandboxDevice;
            if (device != null)
                return SetDefaultCommunicationsDevice(device);

            return false;
        }

        public override Task<bool> SetDefaultDeviceAsync(IDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultDevice(dev));
        }

        public override Task<bool> SetDefaultCommunicationsDeviceAsync(IDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultCommunicationsDevice(dev));
        }

        protected override void Dispose(bool disposing)
        {
            if (_devices != null)
            {
                _devices.Clear();
            }
        }
    }
}