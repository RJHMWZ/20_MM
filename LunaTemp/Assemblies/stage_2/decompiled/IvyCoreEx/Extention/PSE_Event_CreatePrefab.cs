using UnityEngine;

namespace IvyCoreEx.Extention
{
	public class PSE_Event_CreatePrefab : PSE_EventBase
	{
		private bool mPlayed_ = false;

		public float mCreateTime_;

		public Vector3 mRandomPosition_;

		public GameObject mPrefabObject;

		public override void Reset()
		{
			mPlayed_ = false;
		}

		public override void DoEvent(ParticleSystemExtend pse, float curTime, Vector3 worldPos, Vector3 locaPos)
		{
			if (!mPlayed_ && curTime >= mCreateTime_)
			{
				mPlayed_ = true;
				GameObject obj = ((mPrefabObject != null) ? Object.Instantiate(mPrefabObject) : null);
				if (obj != null)
				{
					obj.transform.position = worldPos + mRandomPosition_;
				}
			}
		}
	}
}
