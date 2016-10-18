using System;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public partial class CoreAudioDevice
    {

        private Lazy<CoreAudioSessionController> _sessionController;

        private void ClearAudioSession()
        {
            if (_sessionController?.IsValueCreated == true)
                _sessionController?.Value?.Dispose();

            _sessionController = null;
        }

        private void LoadAudioSessionController()
        {
            if (_sessionController?.IsValueCreated == true)
                return;

            _sessionController = new Lazy<CoreAudioSessionController>(() =>
            {
                return ComThread.Invoke(() =>
                {
                    //Need to catch here, as there is a chance that unauthorized is thrown.
                    //It's not an HR exception, but bubbles up through the .net call stack
                    try
                    {
                        var clsGuid = new Guid(ComInterfaceIds.AUDIO_SESSION_MANAGER2_IID);
                        object result;
                        Marshal.GetExceptionForHR(Device.Activate(ref clsGuid, ClassContext.Inproc, IntPtr.Zero, out result));

                        //This is scoped into the managed object, so disposal is taken care of there.
                        var audioSessionManager = result as IAudioSessionManager2;

                        if (audioSessionManager != null)
                            return new CoreAudioSessionController(this, audioSessionManager);
                    }
                    catch (Exception)
                    {
                        if (_sessionController?.IsValueCreated == true)
                            _sessionController?.Value?.Dispose();

                        _sessionController = null;
                    }

                    return null;
                });
            });
        }
    }
}