using System;
using System.Threading;
using AudioSwitcher.AudioApi.Hooking.ComObjects;

namespace AudioSwitcher.AudioApi.Hooking
{
    public class RemoteInterface : MarshalByRefObject
    {
        private readonly Func<DataFlow, Role, string> _systemId;
        private readonly Func<bool> _canUnload;
        private readonly Action _hookInstalled;
        private readonly Action<int> _hookUninstalled;
        private readonly Action<int, Exception> _errorHandler;
        private int _messageCount;

        public int MessageCount => _messageCount;

        public RemoteInterface(
            Func<DataFlow, Role, string> systemId,
            Func<bool> canUnload,
            Action hookInstalled,
            Action<int> hookUninstalled,
            Action<int, Exception> errorHandler
            )
        {
            if (systemId == null) throw new ArgumentNullException(nameof(systemId));
            if (canUnload == null) throw new ArgumentNullException(nameof(canUnload));

            _systemId = systemId;
            _canUnload = canUnload;
            _hookInstalled = hookInstalled;
            _hookUninstalled = hookUninstalled;
            _errorHandler = errorHandler;
            _messageCount = 0;
        }

        public bool CanUnload()
        {
            Interlocked.Increment(ref _messageCount);

            if (_canUnload == null)
                return true;

            return _canUnload();
        }

        public bool HookInstalled()
        {
            Interlocked.Increment(ref _messageCount);

            ThreadPool.QueueUserWorkItem(x =>
            {
                if (_hookInstalled != null)
                    _hookInstalled();
            });

            return true;
        }

        public void HookUninstalled(int processId)
        {
            Interlocked.Increment(ref _messageCount);

            ThreadPool.QueueUserWorkItem(x =>
            {
                if (_hookUninstalled != null)
                    _hookUninstalled(processId);
            });
        }

        public void ReportError(int processId, Exception e)
        {
            Interlocked.Increment(ref _messageCount);

            ThreadPool.QueueUserWorkItem(x =>
            {
                if (_errorHandler != null)
                    _errorHandler(processId, e);
            });
        }

        public string GetDefaultDevice(DataFlow dataFlow, Role role)
        {
            Interlocked.Increment(ref _messageCount);

            if (_systemId == null)
                return null;

            return _systemId(dataFlow, role);
        }
    }
}