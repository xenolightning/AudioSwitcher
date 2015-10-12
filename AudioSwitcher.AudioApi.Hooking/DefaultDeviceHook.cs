using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using AudioSwitcher.AudioApi.Hooking.ComObjects;
using EasyHook;

namespace AudioSwitcher.AudioApi.Hooking
{
    public class DefaultDeviceHook : IHook, IDisposable
    {
        public delegate void OnErrorHandler(int processId, Exception exception);

        public delegate void OnCompleteHandler();

        private readonly Func<DataFlow, Role, string> _systemDeviceId;
        private string _channelName;
        private IpcServerChannel _ipcChannel;
        private Timer _hookIsLiveTimer;
        private RemoteInterface _interface;
        private int _lastMessageCount;
        private bool _completeSignalled;

        public EHookStatus Status
        {
            get;
            private set;
        }

        public DefaultDeviceHook(Func<DataFlow, Role, string> systemDeviceId)
        {
            _systemDeviceId = systemDeviceId;
            Status = EHookStatus.Inactive;
        }

        public void Dispose()
        {
            UnHook();
        }

        public void Hook(int processId)
        {
            if (Status == EHookStatus.Active || Status == EHookStatus.Pending)
                return;

            Status = EHookStatus.Pending;

            _interface = new RemoteInterface
            (
                (x, y) => _systemDeviceId(x, y),
                () => Status == EHookStatus.Inactive,
                () => Status = EHookStatus.Active,
                RaiseOnComplete,
                RaiseOnError
            );

            _ipcChannel = RemoteHooking.IpcCreateServer(ref _channelName, WellKnownObjectMode.Singleton, _interface);

            RemoteHooking.Inject(
                processId,
                InjectionOptions.DoNotRequireStrongName,
                typeof(IMultimediaDeviceEnumerator).Assembly.Location,
                typeof(IMultimediaDeviceEnumerator).Assembly.Location,
                _channelName);

            _hookIsLiveTimer = new Timer(HookIsLive, null, 0, 2000);
            _lastMessageCount = -1;

            _completeSignalled = false;
        }

        private void HookIsLive(object state)
        {
            var messageCount = _interface.MessageCount;

            if (_interface.MessageCount > _lastMessageCount)
                _lastMessageCount = messageCount;
            else
                UnHook();
        }

        public void UnHook()
        {
            if (Status == EHookStatus.Inactive)
                return;

            Status = EHookStatus.Inactive;

            if (_hookIsLiveTimer != null)
            {
                _hookIsLiveTimer.Dispose();
                _hookIsLiveTimer = null;
            }

            if (_ipcChannel != null)
            {
                ChannelServices.UnregisterChannel(_ipcChannel);
                _ipcChannel.StopListening(null);
                _ipcChannel = null;

                RaiseOnComplete();
            }
        }

        public event OnCompleteHandler OnComplete;

        private void RaiseOnComplete()
        {
            if (_completeSignalled)
                return;

            var handler = OnComplete;

            if (handler != null)
            {
                _completeSignalled = true;
                handler();
            }
        }

        public event OnErrorHandler OnError;

        private void RaiseOnError(int processId, Exception exception)
        {
            var handler = OnError;

            if (handler != null)
                handler(processId, exception);
        }

    }
}