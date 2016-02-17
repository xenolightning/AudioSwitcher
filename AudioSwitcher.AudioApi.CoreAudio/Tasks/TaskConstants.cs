using System.Threading.Tasks;

namespace Nito.AsyncEx
{
    /// <summary>
    /// Provides completed task constants.
    /// </summary>
    public static class TaskConstants
    {
        private static readonly Task<bool> booleanTrue = Task.FromResult(true);
        private static readonly Task<int> intNegativeOne = Task.FromResult(-1);

        /// <summary>
        /// A task that has been completed with the value <c>true</c>.
        /// </summary>
        public static Task<bool> BooleanTrue => booleanTrue;

        /// <summary>
        /// A task that has been completed with the value <c>false</c>.
        /// </summary>
        public static Task<bool> BooleanFalse => TaskConstants<bool>.Default;

        /// <summary>
        /// A task that has been completed with the value <c>0</c>.
        /// </summary>
        public static Task<int> Int32Zero => TaskConstants<int>.Default;

        /// <summary>
        /// A task that has been completed with the value <c>-1</c>.
        /// </summary>
        public static Task<int> Int32NegativeOne => intNegativeOne;

        /// <summary>
        /// A <see cref="Task"/> that has been completed.
        /// </summary>
        public static Task Completed => booleanTrue;

        /// <summary>
        /// A <see cref="Task"/> that will never complete.
        /// </summary>
        public static Task Never => TaskConstants<bool>.Never;

        /// <summary>
        /// A task that has been canceled.
        /// </summary>
        public static Task Canceled => TaskConstants<bool>.Canceled;
    }

    /// <summary>
    /// Provides completed task constants.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    public static class TaskConstants<T>
    {
        private static Task<T> CanceledTask()
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetCanceled();
            return tcs.Task;
        }

        /// <summary>
        /// A task that has been completed with the default value of <typeparamref name="T"/>.
        /// </summary>
        public static Task<T> Default { get; } = Task.FromResult(default(T));

        /// <summary>
        /// A <see cref="Task"/> that will never complete.
        /// </summary>
        public static Task<T> Never { get; } = new TaskCompletionSource<T>().Task;

        /// <summary>
        /// A task that has been canceled.
        /// </summary>
        public static Task<T> Canceled { get; } = CanceledTask();
    }
}
