using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public abstract class AudioController
    {
        protected const DeviceState DefaultDeviceStateFilter =
            DeviceState.Active | DeviceState.Unplugged | DeviceState.Disabled;

        private readonly List<IControllerPlugin> _plugins = new List<IControllerPlugin>();

        public IEnumerable<IControllerPlugin> Plugins
        {
            get { return _plugins; }
        } 

        protected AudioController(IDeviceEnumerator enumerator)
        {
            DeviceEnumerator = enumerator;
            DeviceEnumerator.AudioDeviceChanged += DeviceEnumerator_AudioDeviceChanged;

            enumerator.AudioController = this;
        }

        public bool AddPlugin(IControllerPlugin plugin)
        {
            if (plugin == null || plugin.AudioController != null || _plugins.Contains(plugin))
                return false;

            _plugins.Add(plugin);
            return true;
        }

        public bool RemovePlugin(IControllerPlugin plugin)
        {
            return _plugins.Remove(plugin);
        }

        public T GetPlugin<T>()
            where T : IControllerPlugin
        {
            return _plugins.OfType<T>().FirstOrDefault();
        }

        public T GetPlugin<T>(string name)
            where T : IControllerPlugin
        {
            return _plugins.OfType<T>().FirstOrDefault(x => x.Name == name);
        }

        protected IDeviceEnumerator DeviceEnumerator
        {
            get;
            set;
        }

        public virtual Device DefaultPlaybackDevice
        {
            get { return DeviceEnumerator.DefaultPlaybackDevice; }
        }

        public virtual Device DefaultPlaybackCommunicationsDevice
        {
            get { return DeviceEnumerator.DefaultCommunicationsPlaybackDevice; }
        }

        public virtual Device DefaultCaptureDevice
        {
            get { return DeviceEnumerator.DefaultCaptureDevice; }
        }

        public virtual Device DefaultCaptureCommunicationsDevice
        {
            get { return DeviceEnumerator.DefaultCommunicationsCaptureDevice; }
        }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        private void DeviceEnumerator_AudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            //Bubble the event
            if (AudioDeviceChanged != null)
                AudioDeviceChanged(sender, e);

            OnAudioDeviceChanged(sender, e);
        }

        protected virtual void OnAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
        }

        public IEnumerable<Device> GetAllDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DataFlow.All, deviceState);
        }

        public IEnumerable<Device> GetPlaybackDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DataFlow.Render, deviceState);
        }

        public IEnumerable<Device> GetCaptureDevices(DeviceState deviceState = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DataFlow.Capture, deviceState);
        }

        public virtual Device GetAudioDevice(Guid id, DeviceState state = DefaultDeviceStateFilter)
        {
            return DeviceEnumerator.GetDevices(DataFlow.All, state).FirstOrDefault(dev => dev.Id == id);
        }

        public virtual bool SetDefaultDevice(Device dev)
        {
            return DeviceEnumerator.SetDefaultDevice(dev);
        }

        public virtual bool SetDefaultCommunicationsDevice(Device dev)
        {
            return DeviceEnumerator.SetDefaultCommunicationsDevice(dev);
        }
    }
}