using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeKits.Singleton
{
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = SingletonCreator.CreateSingleton<T>();
                }

                return _instance;
            }
        }

        public virtual void OnSingletonInit()
        {
        }

        public virtual void Dispose()
        {
            _instance = null;
        }
    }
}