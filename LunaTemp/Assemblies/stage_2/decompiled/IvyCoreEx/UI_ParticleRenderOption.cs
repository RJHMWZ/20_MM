using System.Collections;
using UnityEngine;

namespace IvyCoreEx
{
	public class UI_ParticleRenderOption : MonoBehaviour
	{
		public Canvas mDestCanvas_ = null;

		public bool mFindingClosestCanvas_ = false;

		public ParticleSystem mDestPS_;

		private ParticleSystem mPS_;

		public bool mDelayDoLogic_ = false;

		public int mDelayDoLogicFrames_ = 1;

		public void DoOptionLogic()
		{
			Canvas canvas = mDestCanvas_;
			if (!(mPS_ == null) && canvas != null)
			{
				ParticleSystemRenderer[] renderList = mPS_.GetComponentsInChildren<ParticleSystemRenderer>(true);
				for (int i = 0; i < renderList.Length; i++)
				{
					int srcOrder = renderList[i].sortingOrder;
					renderList[i].gameObject.layer = canvas.gameObject.layer;
					renderList[i].sortingOrder = canvas.sortingOrder;
					renderList[i].sortingLayerID = canvas.sortingLayerID;
					renderList[i].sortingFudge = (0f - (float)srcOrder) * 0.1f + renderList[i].sortingFudge;
				}
			}
		}

		private IEnumerator DoOptionCoroutineLogic()
		{
			int val = mDelayDoLogicFrames_;
			while (val > 0)
			{
				int num = val - 1;
				val = num;
				yield return null;
			}
			DoOptionLogic();
		}

		private void Start()
		{
			CheckCavansAndRoot();
		}

		public void CheckCavansAndRoot()
		{
			if (mFindingClosestCanvas_)
			{
				mDestCanvas_ = GetComponentInParent<Canvas>();
			}
			if (mDestCanvas_ == null)
			{
			}
			if (mDestPS_ != null)
			{
				mPS_ = mDestPS_;
			}
			else
			{
				mPS_ = GetComponent<ParticleSystem>();
			}
			if (!(mDestCanvas_ != null))
			{
				return;
			}
			if (Application.isPlaying)
			{
				if (mDelayDoLogic_)
				{
					StartCoroutine(DoOptionCoroutineLogic());
				}
				else
				{
					DoOptionLogic();
				}
			}
			else
			{
				DoOptionLogic();
			}
		}
	}
}
