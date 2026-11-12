using UnityEngine;

namespace CodeKits.Singleton
{
	public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if ((Object)_instance == (Object)null)
				{
					_instance = SingletonCreator.CreateMonoSingleton<T>();
				}
				return _instance;
			}
		}

		public void OnSingletonInit()
		{
		}

		public void Dispose()
		{
			Object.Destroy(base.gameObject);
		}

		protected virtual void OnApplicationQuit()
		{
			if (!((Object)_instance == (Object)null))
			{
				Object.Destroy(_instance.gameObject);
				_instance = null;
			}
		}

		protected virtual void OnDestroy()
		{
			_instance = null;
		}
	}
}
