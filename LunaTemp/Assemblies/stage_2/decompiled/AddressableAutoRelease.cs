using UnityEngine;

public class AddressableAutoRelease : MonoBehaviour
{
	public string mKey_;

	public string Key
	{
		get
		{
			return mKey_;
		}
		set
		{
			mKey_ = value;
		}
	}
}
