using System;

namespace CodeKits.Event
{
	public class EasyEvent<T> : IEasyEvent
	{
		private Action<T> _onEvent = delegate
		{
		};

		public IUnRegister Register(Action<T> onEvent)
		{
			_onEvent = (Action<T>)Delegate.Combine(_onEvent, onEvent);
			return new CustomUnRegister(delegate
			{
				UnRegister(onEvent);
			});
		}

		private void UnRegister(Action<T> onEvent)
		{
			_onEvent = (Action<T>)Delegate.Remove(_onEvent, onEvent);
		}

		public void Trigger(T t)
		{
			_onEvent?.Invoke(t);
		}

		public IUnRegister Register(Action onEvent)
		{
			return Register(Action);
			void Action(T _)
			{
				onEvent();
			}
		}
	}
}
