using System;
using System.Diagnostics;
using System.Reflection;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    public delegate void UnregisterCallback<E>(EventHandler<E> eventHandler) where E : EventArgs;

    public interface IWeakEventHandler<E>
        where E : EventArgs
    {
        EventHandler<E> Handler { get; }
    }

    [DebuggerNonUserCode]
    public class WeakEventHandler<T, E> : IWeakEventHandler<E>
        where T : class
        where E : EventArgs
    {
        private delegate void OpenEventHandler(T @this, object sender, E e);

        private WeakReference _targetRef;
        private OpenEventHandler _openHandler;
        private EventHandler<E> _handler;
        private UnregisterCallback<E> _unregister;

        [DebuggerNonUserCode]
        public WeakEventHandler(EventHandler<E> eventHandler, UnregisterCallback<E> unregister)
        {
            _targetRef = new WeakReference(eventHandler.Target);
            _openHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler),
              null, eventHandler.Method);
            _handler = Invoke;
            _unregister = unregister;
        }

        [DebuggerNonUserCode]
        public void Invoke(object sender, E e)
        {
            T target = (T)_targetRef.Target;

            if (target != null)
                _openHandler.Invoke(target, sender, e);
            else if (_unregister != null)
            {
                _unregister(_handler);
                _unregister = null;
            }
        }

        [DebuggerNonUserCode]
        public EventHandler<E> Handler
        {
            get { return _handler; }
        }

        [DebuggerNonUserCode]
        public static implicit operator EventHandler<E>(WeakEventHandler<T, E> weh)
        {
            return weh._handler;
        }
    }

    public static class EventHandlerUtils
    {
        public static EventHandler<E> MakeWeak<E>(this EventHandler<E> eventHandler, UnregisterCallback<E> unregister)
            where E : EventArgs
        {
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler");
            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
                throw new ArgumentException("Only instance methods are supported.", "eventHandler");

            Type wehType = typeof(WeakEventHandler<,>).MakeGenericType(eventHandler.Method.DeclaringType, typeof(E));
            ConstructorInfo wehConstructor = wehType.GetConstructor(new Type[] { typeof(EventHandler<E>),
      typeof(UnregisterCallback<E>) });

            IWeakEventHandler<E> weh = (IWeakEventHandler<E>)wehConstructor.Invoke(
              new object[] { eventHandler, unregister });

            return weh.Handler;
        }
    }

}
