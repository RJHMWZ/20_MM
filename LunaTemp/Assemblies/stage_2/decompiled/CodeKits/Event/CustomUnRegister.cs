using System;

namespace CodeKits.Event
{
	public struct CustomUnRegister : IUnRegister
	{
		private Action _onUnRegister;

		public CustomUnRegister(Action onUnRegister)
		{
			_onUnRegister = onUnRegister;
		}

		public void UnRegister()
		{
			_onUnRegister();
			_onUnRegister = null;
		}
	}
}
