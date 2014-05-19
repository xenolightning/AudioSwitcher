using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AudioSwitcher.AudioApi.Plugins
{
    public class SystemEventLog : IControllerPlugin
    {

        private const string EVENT_SOURCE = "AudioSwitcher";
        private const string EVENT_LOG_LOCATION = "Application";

        public string Name
        {
            get { return "SystemEventLog"; }
        }

        public Controller Controller
        {
            get;
            set;
        }

        public void OnRegister()
        {
            Controller.AudioDeviceChanged += ControllerOnAudioDeviceChanged;
        }

        private void ControllerOnAudioDeviceChanged(object sender, AudioDeviceChangedEventArgs e)
        {
            if (!EventLog.SourceExists(EVENT_SOURCE))
                EventLog.CreateEventSource(EVENT_SOURCE, EVENT_LOG_LOCATION);

            var eventType = 1000 + (int)e.EventType;

            EventLog.WriteEntry(EVENT_SOURCE, e.Device.ShortName, EventLogEntryType.Information, eventType);
        }

        public void OnUnregister()
        {
            Controller.AudioDeviceChanged -= ControllerOnAudioDeviceChanged;
        }
    }
}
