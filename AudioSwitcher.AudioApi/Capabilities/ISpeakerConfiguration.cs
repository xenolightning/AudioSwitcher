namespace AudioSwitcher.AudioApi.Capabilities
{
    public interface ISpeakerConfiguration : IDeviceCapability
    {
        SpeakerConfiguration SpeakerConfiguration { get; }

        void IsSupported(SpeakerConfiguration configuration);

        void SetSpeakerConfiguration(SpeakerConfiguration configuration);
    }
}