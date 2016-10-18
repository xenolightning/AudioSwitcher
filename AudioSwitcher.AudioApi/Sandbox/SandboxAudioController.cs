using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioSwitcher.AudioApi.Sandbox
{
    public class SandboxAudioController : AudioController<SandboxDevice>
    {
        private readonly List<SandboxDevice> _devices;
        private Guid _defaultCaptureCommDeviceId;
        private Guid _defaultCaptureDeviceId;
        private Guid _defaultPlaybackCommDeviceId;
        private Guid _defaultPlaybackDeviceId;

        public SandboxAudioController(IAudioController source)
        {
            _devices = new List<SandboxDevice>();

            //Get a copy of the current system audio devices
            //then create a copy of the current state of the system
            //this allows us to "debug" macros against a "test" system
            _defaultPlaybackDeviceId = source.DefaultPlaybackDevice?.Id ?? Guid.Empty;
            _defaultPlaybackCommDeviceId = source.DefaultPlaybackCommunicationsDevice?.Id ?? Guid.Empty;
            _defaultCaptureDeviceId = source.DefaultCaptureDevice?.Id ?? Guid.Empty;
            _defaultCaptureCommDeviceId = source.DefaultCaptureCommunicationsDevice?.Id ?? Guid.Empty;

            foreach (var sourceDev in source.GetDevices(DeviceType.All, DeviceState.All))
            {
                var dev = new SandboxDevice(this)
                {
                    id = sourceDev.Id,
                    interfaceName = sourceDev.InterfaceName,
                    icon = sourceDev.Icon,
                    name = sourceDev.Name,
                    isMuted = sourceDev.IsMuted,
                    fullName = sourceDev.FullName,
                    type = sourceDev.DeviceType,
                    state = sourceDev.State,
                    volume = sourceDev.Volume,
                    iconPath = sourceDev.IconPath
                };
                _devices.Add(dev);
            }
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

        internal bool SetDefaultDevice(SandboxDevice dev)
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

        internal bool SetDefaultCommunicationsDevice(SandboxDevice dev)
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

        protected override void Dispose(bool disposing)
        {
            _devices?.Clear();
        }
    }
}