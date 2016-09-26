using System;
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
        public delegate void StatusChangedEventHandler(EHookStatus status);

        private readonly Func<DataFlow, Role, string> _systemDeviceId;
        private readonly ManualResetEvent _unhookWaitEvent;
        private string _channelName;
        private IpcServerChannel _ipcChannel;
        private Timer _hookIsLiveTimer;
        private RemoteInterface _interface;
        private int _lastMessageCount;
        private bool _completeSignalled;
        private int _hookedProcessId;
        private EHookStatus _status;

        public event StatusChangedEventHandler StatusChanged;
        public event CompleteEventHandler Complete;
        public event ErrorEventHandler Error;

        public EHookStatus Status
        {
            get
            {
                return _status;
            }
            private set
            {
                _status = value;

                OnStatusChanged(_status);
            }
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
            _unhookWaitEvent.Close();
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

                return true;
            }
            catch (Exception ex)
            {
                Status = EHookStatus.Inactive;
                Error?.Invoke(_hookedProcessId, ex);
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
                if (device != null)
                {
                    device.GetId(out devId);
                    policyConfig.SetDefaultEndpoint(devId, Role.Communications);

                }

                deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console, out devptr);
                device = Marshal.GetObjectForIUnknown(devptr) as IMultimediaDevice;
                if (device != null)
                {
                    device.GetId(out devId);
                    policyConfig.SetDefaultEndpoint(devId, Role.Console);
                }
            }
            catch (Exception)
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
            {
                _lastMessageCount = messageCount;
            }
            else
            {
                OnComplete(_hookedProcessId);
            }
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

            if (_ipcChannel != null)
            {
                ChannelServices.UnregisterChannel(_ipcChannel);
                _ipcChannel.StopListening(null);
                _ipcChannel = null;
            }

            Thread.Sleep(100);

            SetDefaults();

            return true;
        }

        private void OnStatusChanged(EHookStatus status)
        {
            var handler = StatusChanged;
            if (handler != null)
                handler(status);
        }

        private void OnComplete(int processId)
        {
            if (_completeSignalled)
                return;

            _completeSignalled = true;
            _unhookWaitEvent.Set();

            UnHook();

            var handler = Complete;
            if (handler != null)
                handler(processId);
        }

        private void OnError(int processId, Exception exception)
        {
            var handler = Error;

            _unhookWaitEvent.Set();
            UnHook();

            if (handler != null)
                handler(processId, exception);
        }

    }
}