namespace CodeKits.Singleton
{
    public interface ISingleton
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void OnSingletonInit();

        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
    }
}