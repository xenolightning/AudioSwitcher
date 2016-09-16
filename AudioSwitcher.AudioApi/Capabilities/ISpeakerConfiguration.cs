namespace AudioSwitcher.AudioApi.Capabilities
{
    public interface ISpeakerConfiguration : IDeviceCapability
    {
        bool IsSupported(SpeakerConfiguration configuration);

        SpeakerConfiguration Get();

        void Set(SpeakerConfiguration configuration);
    }
}