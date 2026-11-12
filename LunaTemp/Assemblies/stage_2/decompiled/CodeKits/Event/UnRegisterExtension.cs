using UnityEngine;

namespace CodeKits.Event
{
	public static class UnRegisterExtension
	{
		private static T GetOrAddComponent<T>(GameObject obj) where T : Component
		{
			T trigger = obj.GetComponent<T>();
			if (!(Object)trigger)
			{
				trigger = obj.AddComponent<T>();
			}
			return trigger;
		}

		public static IUnRegister UnRegisterWhenObjDestroyed(this IUnRegister unRegister, GameObject obj)
		{
			return GetOrAddComponent<UnRegisterOnDestroyTrigger>(obj).AddUnRegister(unRegister);
		}
	}
}
