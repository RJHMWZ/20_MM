namespace CodeKits.Pool
{
    public interface IPool<T>
    {
        T Pop();
        bool Push(T obj);
    }
}