using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Sandbox
{
    public sealed class SandboxDeviceEnumerator : IDeviceEnumerator<SandboxDevice>
    {
        private readonly List<SandboxDevice> _devices;
        private Guid _defaultCaptureCommDeviceId;
        private Guid _defaultCaptureDeviceId;
        private Guid _defaultPlaybackCommDeviceId;
        private Guid _defaultPlaybackDeviceId;


        public SandboxDeviceEnumerator(IDeviceEnumerator source)
        {
            _devices = new List<SandboxDevice>();

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
                    source.GetDevices(DeviceType.All,
                        DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled))
            {
                var dev = new SandboxDevice(this)
                {
                    id = sourceDev.Id,
                    interfaceName = sourceDev.InterfaceName,
                    name = sourceDev.Name,
                    fullName = sourceDev.FullName,
                    type = sourceDev.DeviceType,
                    state = sourceDev.State,
                    Volume = sourceDev.Volume
                };
                _devices.Add(dev);
            }
        }

        public IAudioController AudioController { get; set; }

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
            return GetDevice(id, DeviceState.All);
        }

        public SandboxDevice GetDevice(Guid id, DeviceState state)
        {
            return _devices.FirstOrDefault(x => x.Id == id && (x.state & state) > 0);
        }

        Task<SandboxDevice> IDeviceEnumerator<SandboxDevice>.GetDeviceAsync(Guid id, DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevice(id, state));
        }

        IDevice IDeviceEnumerator.GetDevice(Guid id, DeviceState state)
        {
            return GetDevice(id, state);
        }

        Task<SandboxDevice> IDeviceEnumerator<SandboxDevice>.GetDeviceAsync(Guid id)
        {
            return Task.Factory.StartNew(() => GetDevice(id));
        }

        Task<IDevice> IDeviceEnumerator.GetDeviceAsync(Guid id, DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevice(id, state) as IDevice);
        }

        Task<SandboxDevice> IDeviceEnumerator<SandboxDevice>.GetDefaultDeviceAsync(DeviceType deviceType, Role role)
        {
            return Task.Factory.StartNew(() => GetDefaultDevice(deviceType, role));
        }

        Task<IEnumerable<SandboxDevice>> IDeviceEnumerator<SandboxDevice>.GetDevicesAsync(DeviceType deviceType, DeviceState state)
        {
            return Task.Factory.StartNew(() => GetDevices(deviceType, state));
        }

        public SandboxDevice GetDefaultDevice(DeviceType deviceType, Role eRole)
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

        public IEnumerable<SandboxDevice> GetDevices(DeviceType deviceType, DeviceState eRole)
        {
            return _devices.Where(x =>
                (x.type == deviceType || deviceType == DeviceType.All)
                && (x.State & eRole) > 0
                );
        }

        Task<IDevice> IDeviceEnumerator.GetDeviceAsync(Guid id)
        {
            return Task.Factory.StartNew(() => GetDevice(id) as IDevice);
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

        public Task<bool> SetDefaultDeviceAsync(SandboxDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultDevice(dev));
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

        public Task<bool> SetDefaultCommunicationsDeviceAsync(SandboxDevice dev)
        {
            return Task.Factory.StartNew(() => SetDefaultCommunicationsDevice(dev));
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
            if (_devices != null)
            {
                _devices.Clear();
            }
        }
    }
}