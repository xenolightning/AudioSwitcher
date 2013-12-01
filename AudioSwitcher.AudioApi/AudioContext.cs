namespace AudioSwitcher.AudioApi
{
    public abstract class AudioContext
    {

        public AudioController Controller
        {
            get;
            private set;
        }

        public PreferredDeviceManager PreferredDeviceManager
        {
            get;
            private set;
        }

        protected AudioContext(AudioController controller, PreferredDeviceManager preferredDeviceManager)
        {
            Controller = controller;
            PreferredDeviceManager = preferredDeviceManager;

            if (Controller != null)
                Controller.Context = this;

            if (PreferredDeviceManager != null)
                PreferredDeviceManager.Context = this;
        }
    }
}