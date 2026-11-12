using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeKits.Singleton
{
    internal static class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class, ISingleton
        {
            var type = typeof(T);
            var monoType = typeof(MonoBehaviour);
            if (monoType.IsAssignableFrom(type))
                return CreateMonoSingleton<T>();
            else
            {
                var instance = CreateGeneralSingleton<T>();
                return instance;
            }
        }

        public static T CreateMonoSingleton<T>() where T : class, ISingleton
        {
            T instance = null;
            var type = typeof(T);
            if (!Application.isPlaying)
                return instance;

            instance = Object.FindObjectOfType(type) as T;
            if (instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                Object.DontDestroyOnLoad(obj);
                instance = obj.AddComponent(typeof(T)) as T;
            }

            instance.OnSingletonInit();
            return instance;
        }

        private static T CreateGeneralSingleton<T>() where T : class
        {
            var type = typeof(T);
            // 获得私有构造函数
            var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            // 取到没有参数的构造函数
            var ctor = Array.Find(constructorInfos, x => x.GetParameters().Length == 0);
            if (ctor == null)
                throw new Exception("Constructor not found! in" + type);
            return ctor.Invoke(null) as T;
        }
    }
}