using System;

namespace AudioSwitcher.AudioApi
{
    public abstract class AudioController
    {
        public virtual AudioDevice DefaultPlaybackDevice { get; protected set; }

        public virtual AudioDevice DefaultCommunicationsPlaybackDevice { get; protected set; }

        public virtual AudioDevice DefaultRecordingDevice { get; protected set; }

        public virtual AudioDevice DefaultCommunicationsRecordingDevice { get; protected set; }

        public event AudioDeviceChangedHandler AudioDeviceChanged;

        protected virtual void OnAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            //Null check and fire if the event is registered
            if (AudioDeviceChanged != null)
                AudioDeviceChanged(sender, e);
        }

        public abstract AudioDevice GetAudioDevice(Guid id);

        public abstract bool SetDefaultDevice(AudioDevice dev);

        public abstract bool SetDefaultCommunicationsDevice(AudioDevice dev);
    }
}