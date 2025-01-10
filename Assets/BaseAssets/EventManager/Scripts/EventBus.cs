using System;
using System.Collections.Generic;
using XUnityLibrary.Singleton;

namespace FirstVillain.EventBus
{
    public class EventBus : UnitySingleton<EventBus>
    {
        private readonly Dictionary<Type, EventDelegate> _delegateDict = new Dictionary<Type, EventDelegate>();
        private readonly Dictionary<Delegate, EventDelegate> _delegateLookupDict = new Dictionary<Delegate, EventDelegate>();

        public delegate void EventDelegate<T>(T myEvent) where T : EventBase;
        private delegate void EventDelegate(EventBase myEvent);

        public void Subscribe<T>(EventDelegate<T> callback) where T : EventBase
        {
            EventDelegate newDelegate = e => callback(e as T);
            _delegateLookupDict[callback] = newDelegate;

            var type = typeof(T);
            if (!_delegateDict.TryGetValue(type, out EventDelegate tempDeletage))
            {
                _delegateDict[type] = tempDeletage;
            }

            _delegateDict[type] += newDelegate;
        }
       
        public void Unsubscribe<T>(EventDelegate<T> callback) where T : EventBase
        {
            if (_delegateLookupDict.TryGetValue(callback, out EventDelegate targetDelegate))
            {
                var type = typeof(T);
                if (_delegateDict.TryGetValue(type, out EventDelegate tempDelegate))
                {
                    tempDelegate -= targetDelegate;
                    if (tempDelegate == null)
                    {
                        _delegateDict.Remove(type);
                    }
                    else
                    {
                        _delegateDict[type] = tempDelegate;
                    }
                }

                _delegateLookupDict.Remove(callback);
            }
        }

        public void Publish(EventBase eventType)
        {
            if (_delegateDict.TryGetValue(eventType.GetType(), out EventDelegate callback))
            {
                callback.Invoke(eventType);
            }
        }

        public void Clear()
        {
            _delegateDict.Clear();
            _delegateLookupDict.Clear();
        }
    }

    public class EventBase
    {
        private int _errorCode;
        public int ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }
    }
}