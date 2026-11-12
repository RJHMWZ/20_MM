using System;
using System.Collections.Generic;
using UnityEngine;

namespace IvyCoreEx.Extention
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class ParticleSystemExtend : MonoBehaviour
	{
		public bool isDestoryWhenStop_ = false;

		public bool isInvisibleWhenStop_ = false;

		public bool isUsePoolMode_ = true;

		public bool isForceUseCallBackStopActionMode_ = true;

		public List<List<ParticleSystem>> mKeepSameRandSeedParticleList;

		public GameObject mPoolObject_;

		private ParticleSystem mMainPS_ = null;

		private bool mWasStopped_ = false;

		public List<PSE_EventBase> mEventList_;

		private Dictionary<string, Action> mTriggerActionSaver_;

		[HideInInspector]
		public Action OnParticleSystemStoppedCallback;

		private void OnEnable()
		{
		}

		private void CheckDestoryMode()
		{
			if (isDestoryWhenStop_)
			{
				isInvisibleWhenStop_ = false;
				isUsePoolMode_ = false;
			}
		}

		private void CheckPoolMode()
		{
			if (isUsePoolMode_)
			{
				isDestoryWhenStop_ = false;
				isInvisibleWhenStop_ = false;
			}
		}

		private void CheckInvisibleMode()
		{
			if (isInvisibleWhenStop_)
			{
				isDestoryWhenStop_ = false;
				isUsePoolMode_ = false;
			}
		}

		public void AddTriggerAction(string name, Action action)
		{
			if (mTriggerActionSaver_ == null)
			{
				mTriggerActionSaver_ = new Dictionary<string, Action>();
			}
			mTriggerActionSaver_[name] = action;
		}

		public void TriggerAtion(string name)
		{
			if (mTriggerActionSaver_ != null && mTriggerActionSaver_.ContainsKey(name))
			{
				mTriggerActionSaver_[name]();
			}
		}

		public void RemoveTriggerAction(string name)
		{
			if (mTriggerActionSaver_ != null && mTriggerActionSaver_.ContainsKey(name))
			{
				mTriggerActionSaver_.Remove(name);
			}
		}

		public void RemoveAllTriggerAction()
		{
			if (mTriggerActionSaver_ != null)
			{
				mTriggerActionSaver_.Clear();
			}
		}

		public void Start()
		{
			if (mMainPS_ == null)
			{
				mMainPS_ = GetComponent<ParticleSystem>();
			}
			if (mMainPS_ != null && isForceUseCallBackStopActionMode_)
			{
				ParticleSystem.MainModule main = mMainPS_.main;
				main.loop = false;
			}
		}

		public void ResetEvents()
		{
			if (mMainPS_ != null && mEventList_ != null && mEventList_.Count > 0)
			{
				for (int i = 0; i < mEventList_.Count; i++)
				{
					mEventList_[i].Reset();
				}
			}
		}

		public void Update()
		{
		}

		public void OnParticleSystemStopped()
		{
			OnParticleSystemStoppedCallback?.Invoke();
			if (isUsePoolMode_)
			{
				if (mPoolObject_ != null)
				{
					UnityEngine.Object.Destroy(mPoolObject_);
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				ResetEvents();
			}
			if (isInvisibleWhenStop_)
			{
				if (mMainPS_ != null)
				{
				}
				if (mPoolObject_ != null)
				{
					mPoolObject_.SetActive(false);
				}
				else
				{
					base.gameObject.SetActive(false);
				}
				ResetEvents();
			}
			if (isDestoryWhenStop_)
			{
				if (mPoolObject_ != null)
				{
					UnityEngine.Object.Destroy(mPoolObject_);
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
