using System;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public partial class CoreAudioDevice : IAudioSessionEndpoint
    {
        private CoreAudioSessionController _sessionController;

        public IAudioSessionController SessionController
        {
            get { return _sessionController; }
        }

        private void ClearAudioSession()
        {
            if (_sessionController != null)
            {
                _sessionController.Dispose();
                _sessionController = null;
            }
        }

        private void LoadAudioSessionController(IMultimediaDevice device)
        {
            if (SessionController != null)
                return;

            //This should be all on the COM thread to avoid any
            //weird lookups on the result COM object not on an STA Thread
            ComThread.Assert();

            //Need to catch here, as there is a chance that unauthorized is thrown.
            //It's not an HR exception, but bubbles up through the .net call stack
            try
            {
                var clsGuid = new Guid(ComIIds.AUDIO_SESSION_MANAGER2_IID);
                object result;
                Marshal.GetExceptionForHR(device.Activate(ref clsGuid, ClsCtx.Inproc, IntPtr.Zero, out result));

                //This is scoped into the managed object, so disposal is taken care of there.
                var audioSessionManager = result as IAudioSessionManager2;

                if (audioSessionManager != null)
                    _sessionController = new CoreAudioSessionController(this, audioSessionManager);

            }
            catch (Exception)
            {
                if (_sessionController != null)
                    _sessionController.Dispose();

                _sessionController = null;
            }
        }
    }
}