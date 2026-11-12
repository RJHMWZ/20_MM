using System;
using System.Collections.Generic;

namespace CodeKits.Pool
{
    public class Pool<T> : IPool<T>
    {
        public int CurCount => _cacheStack.Count;

        private readonly Func<T> _creatorFunc;
        private readonly Action<T> _pushAction;
        private readonly Stack<T> _cacheStack = new();

        public Pool(Func<T> creatorFunc, Action<T> pushAction = null, int initCount = 0)
        {
            _creatorFunc = creatorFunc;
            _pushAction = pushAction;
            for (var i = 0; i < initCount; i++)
            {
                _cacheStack.Push(creatorFunc());
            }
        }

        public virtual T Pop()
        {
            if (_cacheStack.Count == 0)
                return _creatorFunc();
            return _cacheStack.Pop();
        }

        public virtual bool Push(T obj)
        {
            _pushAction?.Invoke(obj);
            _cacheStack.Push(obj);
            return true;
        }
    }
}