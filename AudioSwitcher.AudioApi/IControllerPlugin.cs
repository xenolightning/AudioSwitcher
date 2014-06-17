namespace AudioSwitcher.AudioApi
{
    public interface IControllerPlugin
    {

        string Name
        {
            get;
        }

        AudioController AudioController
        {
            get;
            set;
        }

        void OnRegister();

        void OnUnregister();

    }
}
