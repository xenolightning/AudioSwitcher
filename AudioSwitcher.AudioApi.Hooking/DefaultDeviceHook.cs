using System;
using System.Runtime.Remoting;
using AudioSwitcher.AudioApi.Hooking.ComObjects;
using EasyHook;

namespace AudioSwitcher.AudioApi.Hooking
{
    public class DefaultDeviceHook : IHook, IDisposable
    {
        public delegate void OnErrorHandler(int processId, Exception exception);

        private readonly int _processId;
        private readonly Func<DataFlow, Role, string> _systemDeviceId;
        private string _channelName;

        public DefaultDeviceHook(int processId, Func<DataFlow, Role, string> systemDeviceId)
        {
            _processId = processId;
            _systemDeviceId = systemDeviceId;
        }

        public bool IsHooked
        {
            get;
            private set;
        }

        public void Dispose()
        {
            UnHook();
        }

        public void Hook()
        {
            if (IsHooked)
                return;

            var ri = new RemoteInterface
            {
                //Wrap the target delegate in our own delegate for reference safety
                SystemId = (x, y) => _systemDeviceId(x, y),
                Unload = () =>
                {
                    return !IsHooked;
                },
                ErrorHandler = RaiseOnError
            };

            RemoteHooking.IpcCreateServer(ref _channelName, WellKnownObjectMode.Singleton, ri);

            RemoteHooking.Inject(
                _processId,
                InjectionOptions.DoNotRequireStrongName,
                typeof(IMMDeviceEnumerator).Assembly.Location,
                typeof(IMMDeviceEnumerator).Assembly.Location,
                _channelName);

            IsHooked = true;
        }

        public void UnHook()
        {
            if (!IsHooked)
                return;

            IsHooked = false;
        }

        public event OnErrorHandler OnError;

        private void RaiseOnError(int processId, Exception exception)
        {
            if (OnError != null)
                OnError(processId, exception);
        }

    }
}