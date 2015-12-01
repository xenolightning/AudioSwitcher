using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using AudioSwitcher.AudioApi.Hooking.ComObjects;
using EasyHook;

namespace AudioSwitcher.AudioApi.Hooking
{
    public sealed class DefaultDeviceHook : IHook, IDisposable
    {
        public delegate void ErrorEventHandler(int processId, Exception exception);

        public delegate void CompleteEventHandler(int processId);

        private readonly Func<DataFlow, Role, string> _systemDeviceId;
        private readonly ManualResetEvent _unhookWaitEvent;
        private string _channelName;
        private IpcServerChannel _ipcChannel;
        private Timer _hookIsLiveTimer;
        private RemoteInterface _interface;
        private int _lastMessageCount;
        private bool _completeSignalled;
        private int _hookedProcessId;

        public EHookStatus Status
        {
            get;
            private set;
        }

        public DefaultDeviceHook(Func<DataFlow, Role, string> systemDeviceId)
        {
            _systemDeviceId = systemDeviceId;
            Status = EHookStatus.Inactive;
            _unhookWaitEvent = new ManualResetEvent(false);
        }

        public void Dispose()
        {
            UnHook();
        }

        public bool Hook(int processId)
        {
            if (Status == EHookStatus.Active || Status == EHookStatus.Pending)
                return false;

            Status = EHookStatus.Pending;
            _hookedProcessId = processId;

            _interface = new RemoteInterface
            (
                (x, y) => _systemDeviceId(x, y),
                CanUnload,
                HookInstalled,
                OnComplete,
                OnError
            );

            _ipcChannel = RemoteHooking.IpcCreateServer(ref _channelName, WellKnownObjectMode.Singleton, _interface);

            try
            {
                RemoteHooking.Inject(
                    processId,
                    InjectionOptions.DoNotRequireStrongName,
                    typeof(IMultimediaDeviceEnumerator).Assembly.Location,
                    typeof(IMultimediaDeviceEnumerator).Assembly.Location,
                    _channelName);

                //2000ms due time. This will give the hook 2seconds to become active
                //then it will check every 500ms
                _hookIsLiveTimer = new Timer(HookIsLive, null, 2000, 500);
                _lastMessageCount = -1;

                _completeSignalled = false;

                SetDefaults();

                return true;
            }
            catch (Exception ex)
            {
                Status = EHookStatus.Inactive;
            }


            return false;
        }

        private void HookInstalled()
        {
            Status = EHookStatus.Active;
            SetDefaults();
        }

        private static void SetDefaults()
        {
            try
            {

                IMultimediaDevice device;
                string devId;
                IntPtr devptr;
                var policyConfig = ComObjectFactory.GetPolicyConfig();
                var deviceEnumerator = ComObjectFactory.GetDeviceEnumerator();

                deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications, out devptr);
                device = Marshal.GetObjectForIUnknown(devptr) as IMultimediaDevice;
                device.GetId(out devId);
                policyConfig.SetDefaultEndpoint(devId, Role.Communications);

                deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console, out devptr);
                device = Marshal.GetObjectForIUnknown(devptr) as IMultimediaDevice;
                device.GetId(out devId);
                policyConfig.SetDefaultEndpoint(devId, Role.Console);

            }
            catch(Exception)
            {
                // not worth failing for
            }
        }

        private bool CanUnload()
        {
            return Status == EHookStatus.Inactive;
        }

        private void HookIsLive(object state)
        {
            var messageCount = _interface.MessageCount;

            if (_interface.MessageCount > _lastMessageCount)
                _lastMessageCount = messageCount;
            else
                UnHook();
        }

        public bool UnHook()
        {
            if (Status == EHookStatus.Inactive)
                return false;

            Status = EHookStatus.Inactive;

            if (_hookIsLiveTimer != null)
            {
                _hookIsLiveTimer.Dispose();
                _hookIsLiveTimer = null;
            }

            _unhookWaitEvent.WaitOne(1000);

            SetDefaults();

            if (_ipcChannel != null)
            {
                ChannelServices.UnregisterChannel(_ipcChannel);
                _ipcChannel.StopListening(null);
                _ipcChannel = null;
            }

            return true;
        }

        public event CompleteEventHandler Complete;

        private void OnComplete(int processId)
        {
            if (_completeSignalled)
                return;

            _completeSignalled = true;
            _unhookWaitEvent.Set();

            var handler = Complete;
            if (handler != null)
                handler(processId);
        }

        public event ErrorEventHandler Error;

        private void OnError(int processId, Exception exception)
        {
            var handler = Error;

            if (handler != null)
                handler(processId, exception);
        }

    }
}