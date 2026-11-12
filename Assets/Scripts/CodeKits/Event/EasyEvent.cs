using System;

namespace CodeKits.Event
{
    public class EasyEvent<T> : IEasyEvent
    {
        private Action<T> _onEvent = e => { };
        
        public IUnRegister Register(Action<T> onEvent)
        {
            _onEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        private void UnRegister(Action<T> onEvent)
        {
            _onEvent -= onEvent;
        }

        public void Trigger(T t) => _onEvent?.Invoke(t);
        
        public IUnRegister Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _) => onEvent();
        }
    }
}