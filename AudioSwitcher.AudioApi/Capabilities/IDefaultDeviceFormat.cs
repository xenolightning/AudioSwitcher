namespace AudioSwitcher.AudioApi.Capabilities
{
    public interface IDefaultDeviceFormat : IDeviceCapability
    {
        SampleRate SampleRate { get; }

        BitDepth BitDepth { get; }

        bool IsSupported(SampleRate sampleRate, BitDepth bitDepth);

        void SetDeviceFormat(SampleRate sampleRate, BitDepth bitDepth);
    }
}