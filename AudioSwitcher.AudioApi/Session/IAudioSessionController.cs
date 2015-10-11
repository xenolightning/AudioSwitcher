using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSessionController : IEnumerable<IAudioSession>, IDisposable
    {

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
        IEnumerable<IAudioSession> GetActiveSessions();
        Task<IEnumerable<IAudioSession>> GetActiveSessionsAsync();

        /// <summary>
        /// All inactive audio sessions
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAudioSession> GetInactiveSessions();
        Task<IEnumerable<IAudioSession>> GetInactiveSessionsAsync();

        /// <summary>
        /// All expired audio sessions
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAudioSession> GetExpiredSessions();
        Task<IEnumerable<IAudioSession>> GetExpiredSessionsAsync();

        IObservable<AudioSessionChanged> SessionChanged { get; }
    }
}