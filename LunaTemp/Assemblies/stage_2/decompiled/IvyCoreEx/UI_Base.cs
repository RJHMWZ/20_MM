using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace IvyCoreEx
{
	[DisallowMultipleComponent]
	public class UI_Base : MonoBehaviour
	{
		public enum UIActionType
		{
			eEnter,
			eOut,
			eEnumCount
		}

		[HideInInspector]
		public Action OnDestroyCallBack;

		public bool generateMember_;

		public string componentName_;

		public bool generateStandaloneComponent_;

		public string componentDescription_;

		public bool notActive_ = false;

		[NonSerialized]
		private Tween enterAction_ = null;

		[NonSerialized]
		private Tween outAction_ = null;

		private Action mEnterActionPreCallBack_ = null;

		private Action mEnterActionEndCallBack_ = null;

		private Action mOutActionPreCallBack_ = null;

		private Action mOutActionEndCallBack_ = null;

		private Func<bool> mEnterActionCheckCallBack_ = null;

		private Func<bool> mOutActionCheckCallBack_ = null;

		public string enterActionDescription_ = string.Empty;

		public bool mIsEnterActionOverridable_ = false;

		public bool mRunEnterActionWhenEnable_;

		public bool mRunEnterActionWhenEnableRecursionChildren_;

		public string outActionDescription_ = string.Empty;

		public bool mIsOutActionOverridable_ = false;

		public bool mRunOutActionWhenEnable_;

		public bool mRunOutActionWhenEnableRecursionChildren_;

		[HideInInspector]
		public Vector3 m_initPosition;

		[HideInInspector]
		public Vector3 m_initLocalPosition;

		[HideInInspector]
		public Vector3 m_initAnchoredPosition3D;

		public void SetEnterActionPreCallBack(Action act)
		{
			mEnterActionPreCallBack_ = act;
		}

		public void SetEnterActionEndCallBack(Action act)
		{
			mEnterActionEndCallBack_ = act;
		}

		public void SetOutActionPreCallBack(Action act)
		{
			mOutActionPreCallBack_ = act;
		}

		public void SetOutActionEndCallBack(Action act)
		{
			mOutActionEndCallBack_ = act;
		}

		public void SetEnterActionCheckCallBack(Func<bool> checkCallBack)
		{
			mEnterActionCheckCallBack_ = checkCallBack;
		}

		public void SetOutActionCheckCallBack(Func<bool> checkCallBack)
		{
			mOutActionCheckCallBack_ = checkCallBack;
		}

		public bool IsEnterActionRunning()
		{
			return enterAction_ != null && enterAction_.IsPlaying();
		}

		public bool IsOutActionRunning()
		{
			return outAction_ != null && outAction_.IsPlaying();
		}

		private bool IsShowSelectComponent()
		{
			return generateMember_;
		}

		private bool IsShowGenerateStandaloneComponent()
		{
			return generateMember_;
		}

		public List<string> GetAllComponentsName()
		{
			List<string> names = new List<string>();
			Component[] list = GetComponents<Component>();
			names.Add("UnityEngine.GameObject");
			Component[] array = list;
			foreach (Component t in array)
			{
				names.Add(t.GetType().ToString());
			}
			return names;
		}

		private bool isEnterActionInvaild()
		{
			if (string.IsNullOrEmpty(enterActionDescription_))
			{
				return false;
			}
			return false;
		}

		private static List<string> GetActionsDescription()
		{
			return new List<string>();
		}

		public void OnEnable()
		{
			if (mRunEnterActionWhenEnable_)
			{
				RunEnterAction(mRunEnterActionWhenEnableRecursionChildren_);
			}
		}

		public void OnDisable()
		{
			if (mRunOutActionWhenEnable_)
			{
				RunOutAction(mRunOutActionWhenEnableRecursionChildren_);
			}
		}

		private bool isOutActionVaild()
		{
			if (string.IsNullOrEmpty(outActionDescription_))
			{
				return false;
			}
			return false;
		}

		public void RunEnterAction(bool recursionChildren = false)
		{
			if (mEnterActionCheckCallBack_ == null || mEnterActionCheckCallBack_())
			{
				RunAction(UIActionType.eEnter, recursionChildren, mEnterActionPreCallBack_, mEnterActionEndCallBack_);
			}
		}

		public void RunEnterActionWithoutOtherCallBack(bool recursionChildren = false)
		{
			if (mEnterActionCheckCallBack_ == null || mEnterActionCheckCallBack_())
			{
				RunAction(UIActionType.eEnter, recursionChildren);
			}
		}

		public void RunOutAction(bool recursionChildren = false)
		{
			if (mOutActionCheckCallBack_ == null || mOutActionCheckCallBack_())
			{
				RunAction(UIActionType.eOut, recursionChildren, mOutActionPreCallBack_, mOutActionEndCallBack_);
			}
		}

		public void RunOutActionWithoutOtherCallBack(bool recursionChildren = false)
		{
			if (mOutActionCheckCallBack_ == null || mOutActionCheckCallBack_())
			{
				RunAction(UIActionType.eOut, recursionChildren);
			}
		}

		protected void RunAction(UIActionType type, bool recursionChildren = false, Action preCallBack = null, Action endCallBack = null)
		{
			if (recursionChildren)
			{
				Transform baseTrans = base.transform;
				if (!(baseTrans != null))
				{
					return;
				}
				UI_Base[] childList = baseTrans.GetComponentsInChildren<UI_Base>();
				for (int i = 0; i < childList.Length; i++)
				{
					switch (type)
					{
					}
					bool flag = false;
					if (preCallBack != null || endCallBack != null)
					{
						preCallBack?.Invoke();
						endCallBack?.Invoke();
					}
				}
				return;
			}
			switch (type)
			{
			case UIActionType.eEnter:
				if (!mIsEnterActionOverridable_ && enterAction_ != null && enterAction_.IsPlaying() && !enterAction_.IsComplete())
				{
					return;
				}
				break;
			case UIActionType.eOut:
				if (!mIsOutActionOverridable_ && outAction_ != null && outAction_.IsPlaying() && !outAction_.IsComplete())
				{
					return;
				}
				break;
			}
			bool flag2 = false;
			if (preCallBack != null || endCallBack != null)
			{
				preCallBack?.Invoke();
				endCallBack?.Invoke();
			}
		}

		protected void ResetToInitPosition()
		{
			GetComponent<RectTransform>().anchoredPosition3D = new Vector3(m_initAnchoredPosition3D.x, m_initAnchoredPosition3D.y, m_initAnchoredPosition3D.z);
		}

		public void Awake()
		{
			m_initPosition = base.transform.position;
			m_initLocalPosition = base.transform.localPosition;
			RectTransform rectTR_ = GetComponent<RectTransform>();
			if (rectTR_ != null)
			{
				m_initAnchoredPosition3D = GetComponent<RectTransform>().anchoredPosition3D;
			}
			else
			{
				m_initAnchoredPosition3D = base.transform.position;
			}
			if (notActive_)
			{
				base.gameObject.SetActive(false);
			}
		}

		public void OnDestroy()
		{
			OnDestroyCallBack?.Invoke();
		}
	}
}
