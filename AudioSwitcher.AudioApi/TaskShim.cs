using System;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    internal static class TaskShim
    {

        public static Task<T> FromResult<T>(T result)
        {
#if NET40
            return TaskEx.FromResult(result);
#elif NET45
            return Task.FromResult(result);
#endif
        }

        public static Task Run(Action action)
        {
#if NET40
            return TaskEx.Run(action);
#elif NET45
            return Task.Run(action);
#endif
        }

        public static Task Run(Action action, CancellationToken cancellationToken)
        {
#if NET40
            return TaskEx.Run(action, cancellationToken);
#elif NET45
            return Task.Run(action, cancellationToken);
#endif
        }

        public static Task<T> Run<T>(Func<T> function)
        {
#if NET40
            return TaskEx.Run(function);
#elif NET45
            return Task.Run(function);
#endif
        }

        public static Task<T> Run<T>(Func<T> function, CancellationToken cancellationToken)
        {
#if NET40
            return TaskEx.Run(function, cancellationToken);
#elif NET45
            return Task.Run(function, cancellationToken);
#endif
        }

        public static Task<T> Run<T>(Func<Task<T>> function)
        {
#if NET40
            return TaskEx.Run(function);
#elif NET45
            return Task.Run(function);
#endif
        }

        public static Task<T> Run<T>(Func<Task<T>> function, CancellationToken cancellationToken)
        {
#if NET40
            return TaskEx.Run(function, cancellationToken);
#elif NET45
            return Task.Run(function, cancellationToken);
#endif
        }

        public static Task Run(Func<Task> function)
        {
#if NET40
            return TaskEx.Run(function);
#elif NET45
            return Task.Run(function);
#endif
        }

        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
#if NET40
            return TaskEx.Run(function, cancellationToken);
#elif NET45
            return Task.Run(function, cancellationToken);
#endif
        }

    }
}
