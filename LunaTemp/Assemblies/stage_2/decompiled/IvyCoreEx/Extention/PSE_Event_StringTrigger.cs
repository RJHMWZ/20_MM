using UnityEngine;

namespace IvyCoreEx.Extention
{
	public class PSE_Event_StringTrigger : PSE_EventBase
	{
		public string mTriggerName_;

		public float mTriggerTime_;

		private bool mHadTriggered_ = false;

		public override void Reset()
		{
			mHadTriggered_ = false;
		}

		public override void DoEvent(ParticleSystemExtend pse, float curTime, Vector3 worldPos, Vector3 locaPos)
		{
			if (!mHadTriggered_ && curTime >= mTriggerTime_)
			{
				mHadTriggered_ = true;
				pse.TriggerAtion(mTriggerName_);
			}
		}
	}
}
