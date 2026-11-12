using System;

namespace CodeKits.Event
{
    public interface IEasyEvent
    {
        IUnRegister Register(Action onEvent);
    }
}