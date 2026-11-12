using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Reference20
{
	public sealed class Reference20PropVisual : MonoBehaviour
	{
		public int Kind;

		public SkeletonGraphic Rig;

		private bool hinting;

		public void ResetPose(bool born)
		{
			hinting = false;
			Rig.timeScale = 1f;
			Rig.AnimationState.ClearTracks();
			Rig.Skeleton.SetToSetupPose();
			string skin = ((Kind == 13) ? "bomb_cross" : ((Kind == 14) ? "bomb_square" : ((Kind == 15) ? "brick_color" : ((Kind == 12) ? "brick0_shu" : "brick0_heng"))));
			Rig.Skeleton.SetSkin(skin);
			Rig.Skeleton.SetSlotsToSetupPose();
			string idle = ((Kind == 14) ? "Bomb_still" : "Bomb_dropped_d");
			if (born)
			{
				Play(Reference20Props.Creation(Kind));
				Rig.AnimationState.AddAnimation(0, idle, true, 0f);
			}
			else
			{
				Play(idle, true);
			}
			CanvasGroup fade = GetComponent<CanvasGroup>();
			if ((bool)fade)
			{
				fade.DOKill();
				fade.alpha = 1f;
			}
			Rig.Update(0f);
			Rig.LateUpdate();
		}

		private void Play(string name, bool loop = false)
		{
			Rig.AnimationState.SetAnimation(0, name, loop);
		}

		private void IdleAfter()
		{
			Rig.AnimationState.AddAnimation(0, (Kind == 14) ? "Bomb_still" : "Bomb_dropped_d", true, 0f);
		}

		public void Hint(string name)
		{
			hinting = true;
			string suffix = (name.EndsWith("right") ? "_r" : (name.EndsWith("left") ? "_l" : (name.EndsWith("up") ? "_u" : (name.EndsWith("down") ? "_d" : ""))));
			Play("Bomb_hint" + suffix, true);
		}

		public void StopHint()
		{
			if (hinting)
			{
				hinting = false;
				Play((Kind == 14) ? "Bomb_still" : "Bomb_dropped_d", true);
			}
		}

		public void Falling()
		{
			hinting = false;
			Play("falling_d");
		}

		public void Landed()
		{
			hinting = false;
			Play("Bomb_dropped_d");
			IdleAfter();
		}
	}
}
