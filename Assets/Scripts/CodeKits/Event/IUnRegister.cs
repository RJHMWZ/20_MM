using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeKits.Event
{
    public interface IUnRegister
    {
        void UnRegister();
    }

    public static class UnRegisterExtension
    {
        private static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            var trigger = obj.GetComponent<T>();
            if (!trigger)
                trigger = obj.AddComponent<T>();
            return trigger;
        }
        
        public static IUnRegister UnRegisterWhenObjDestroyed(this IUnRegister unRegister, GameObject obj)
        {
            return GetOrAddComponent<UnRegisterOnDestroyTrigger>(obj).AddUnRegister(unRegister);
        }
    }

    public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
    {
        private void OnDestroy()
        {
            UnRegister();
        }
    }

    public abstract class UnRegisterTrigger : MonoBehaviour
    {
        private readonly HashSet<IUnRegister> _unRegisters = new();

        public IUnRegister AddUnRegister(IUnRegister unRegister)
        {
            _unRegisters.Add(unRegister);
            return unRegister;
        }

        protected void UnRegister()
        {
            foreach (var unRegister in _unRegisters)
                unRegister.UnRegister();
            
            _unRegisters.Clear();
        }
    }
}