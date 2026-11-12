namespace Spine.Unity
{
	public class DoubleBuffered<T> where T : new()
	{
		private readonly T a = new T();

		private readonly T b = new T();

		private bool usingA;

		public T GetCurrent()
		{
			return usingA ? a : b;
		}

		public T GetNext()
		{
			usingA = !usingA;
			return usingA ? a : b;
		}
	}
}
