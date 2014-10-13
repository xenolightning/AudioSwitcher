using System;
using AudioSwitcher.AudioApi;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal
{
    internal sealed partial class JavaScriptAudioDevice : ObjectInstance
    {
        internal JavaScriptAudioDevice(ScriptEngine engine, AudioController controller, IDevice device)
            : base(engine)
        {
            Controller = controller;
            ID = device.Id.ToString();
            Name = device.ShortName;
            Flags = device.IsPlaybackDevice ? 1 : 2;
            IsDefault = device.IsDefaultDevice;
            IsDefaultComm = device.IsDefaultCommunicationsDevice;

            PopulateFields();
            PopulateFunctions();
        }

        [JSProperty(Name = "id")]
        public string ID
        {
            get;
            internal set;
        }

        [JSProperty(Name = "name")]
        public string Name
        {
            get;
            internal set;
        }

        [JSProperty(Name = "flags")]
        public int Flags
        {
            get;
            internal set;
        }

        [JSProperty(Name = "isDefault")]
        public bool IsDefault
        {
            get;
            internal set;
        }

        [JSProperty(Name = "isDefaultComm")]
        public bool IsDefaultComm
        {
            get;
            internal set;
        }

        [JSProperty(Name = "isMuted")]
        public bool IsMuted
        {
            get { return Device.IsMuted; }
        }

        private AudioController Controller
        {
            get;
            set;
        }

        private IDevice Device
        {
            get
            {
                //Ensures that the AudioController is always referencing the correct device
                //instance
                return Controller.GetAudioDevice(new Guid(ID));
            }
        }
    }
}