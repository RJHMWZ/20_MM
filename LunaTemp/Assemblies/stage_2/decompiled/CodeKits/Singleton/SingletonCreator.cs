using System;
using System.Reflection;
using UnityEngine;

namespace CodeKits.Singleton
{
	internal static class SingletonCreator
	{
		public static T CreateSingleton<T>() where T : class, ISingleton
		{
			Type type = typeof(T);
			Type monoType = typeof(MonoBehaviour);
			if (monoType.IsAssignableFrom(type))
			{
				return CreateMonoSingleton<T>();
			}
			return CreateGeneralSingleton<T>();
		}

		public static T CreateMonoSingleton<T>() where T : class, ISingleton
		{
			T instance = null;
			Type type = typeof(T);
			if (!Application.isPlaying)
			{
				return instance;
			}
			instance = UnityEngine.Object.FindObjectOfType(type) as T;
			if (instance == null)
			{
				GameObject obj = new GameObject(typeof(T).Name);
				UnityEngine.Object.DontDestroyOnLoad(obj);
				instance = obj.AddComponent(typeof(T)) as T;
			}
			instance.OnSingletonInit();
			return instance;
		}

		private static T CreateGeneralSingleton<T>() where T : class
		{
			Type type = typeof(T);
			ConstructorInfo[] constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
			ConstructorInfo ctor = Array.Find(constructorInfos, (ConstructorInfo x) => x.GetParameters().Length == 0);
			if (ctor == null)
			{
				throw new Exception("Constructor not found! in" + type);
			}
			return ctor.Invoke(null) as T;
		}
	}
}
