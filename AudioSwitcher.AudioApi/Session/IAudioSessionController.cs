using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSessionController : IDeviceCapability, IEnumerable<IAudioSession>
    {
        IObservable<IAudioSession> SessionCreated { get; }

        IObservable<string> SessionDisconnected { get; }

        /// <summary>
        /// All audio sessions
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAudioSession> All();

        Task<IEnumerable<IAudioSession>> AllAsync();

        /// <summary>
        /// All active audio sessions
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAudioSession> ActiveSessions();

        Task<IEnumerable<IAudioSession>> ActiveSessionsAsync();

        /// <summary>
        /// All inactive audio sessions
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAudioSession> InactiveSessions();

        Task<IEnumerable<IAudioSession>> InactiveSessionsAsync();

        /// <summary>
        /// All expired audio sessions
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAudioSession> ExpiredSessions();

        Task<IEnumerable<IAudioSession>> ExpiredSessionsAsync();
    }
}