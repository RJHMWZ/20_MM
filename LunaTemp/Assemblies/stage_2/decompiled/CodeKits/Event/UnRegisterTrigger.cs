using System.Collections.Generic;
using UnityEngine;

namespace CodeKits.Event
{
	public abstract class UnRegisterTrigger : MonoBehaviour
	{
		private readonly HashSet<IUnRegister> _unRegisters = new HashSet<IUnRegister>();

		public IUnRegister AddUnRegister(IUnRegister unRegister)
		{
			_unRegisters.Add(unRegister);
			return unRegister;
		}

		protected void UnRegister()
		{
			foreach (IUnRegister unRegister in _unRegisters)
			{
				unRegister.UnRegister();
			}
			_unRegisters.Clear();
		}
	}
}
