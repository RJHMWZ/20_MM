using System.Collections.Generic;
using UnityEngine;

namespace IvyCoreEx.Extention
{
	public class PSE_Event_PlaySound : PSE_EventBase
	{
		private bool mPlayed_ = false;

		public float mPlayTime_;

		public AudioClip mAudioClip_;

		public bool mUseRandomAudioClip_;

		public List<AudioClip> mAudioClipRandom_;

		public bool mNotEffectByTimeScale_ = false;

		public override void Reset()
		{
			mPlayed_ = false;
		}

		public override void DoEvent(ParticleSystemExtend pse, float curTime, Vector3 worldPos, Vector3 locaPos)
		{
			if (!mPlayed_ && curTime >= mPlayTime_)
			{
				mPlayed_ = true;
			}
		}
	}
}
