namespace CodeKits.Event
{
	public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
	{
		private void OnDestroy()
		{
			UnRegister();
		}
	}
}
