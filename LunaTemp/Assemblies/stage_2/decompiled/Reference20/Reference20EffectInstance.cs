using System;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Reference20
{
	public sealed class Reference20EffectInstance : MonoBehaviour
	{
		private struct Pose
		{
			public Transform transform;

			public Vector3 position;

			public Vector3 scale;

			public Quaternion rotation;

			public bool active;

			public CanvasGroup alpha;

			public float opacity;
		}

		private struct SpinePose
		{
			public SkeletonGraphic skeleton;

			public string animation;

			public bool loop;
		}

		public string PoolKey;

		private Pose[] poses;

		private SpinePose[] skeletons;

		private Reference20CocosClip[] clips;

		private Reference20ParticleGraphic[] particles;

		private Reference20Particle3DGraphic[] particles3d;

		private float remaining;

		private float rootOpacity;

		private Action<Reference20EffectInstance> release;

		public void Capture()
		{
			CanvasGroup rootAlpha = GetComponent<CanvasGroup>();
			rootOpacity = (rootAlpha ? rootAlpha.alpha : 1f);
			Transform[] nodes = GetComponentsInChildren<Transform>(true);
			poses = new Pose[nodes.Length];
			for (int j = 0; j < nodes.Length; j++)
			{
				Transform t = nodes[j];
				CanvasGroup alpha = t.GetComponent<CanvasGroup>();
				poses[j] = new Pose
				{
					transform = t,
					position = t.localPosition,
					scale = t.localScale,
					rotation = t.localRotation,
					active = t.gameObject.activeSelf,
					alpha = alpha,
					opacity = (alpha ? alpha.alpha : 1f)
				};
			}
			clips = GetComponentsInChildren<Reference20CocosClip>(true);
			particles = GetComponentsInChildren<Reference20ParticleGraphic>(true);
			particles3d = GetComponentsInChildren<Reference20Particle3DGraphic>(true);
			SkeletonGraphic[] spine = GetComponentsInChildren<SkeletonGraphic>(true);
			skeletons = new SpinePose[spine.Length];
			for (int i = 0; i < spine.Length; i++)
			{
				TrackEntry track = spine[i].AnimationState.GetCurrent(0);
				skeletons[i] = new SpinePose
				{
					skeleton = spine[i],
					animation = track?.Animation.Name,
					loop = (track?.Loop ?? false)
				};
			}
		}

		public void Restore()
		{
			Reference20CocosClip[] array = clips;
			foreach (Reference20CocosClip clip in array)
			{
				clip.Stop(false);
			}
			Pose[] array2 = poses;
			for (int j = 0; j < array2.Length; j++)
			{
				Pose p = array2[j];
				if ((bool)p.transform)
				{
					p.transform.DOKill();
					p.transform.localPosition = p.position;
					p.transform.localScale = p.scale;
					p.transform.localRotation = p.rotation;
					p.transform.gameObject.SetActive(p.active);
					if ((bool)p.alpha)
					{
						p.alpha.alpha = p.opacity;
					}
				}
			}
			SpinePose[] array3 = skeletons;
			for (int k = 0; k < array3.Length; k++)
			{
				SpinePose pose = array3[k];
				SkeletonGraphic skeleton = pose.skeleton;
				skeleton.AnimationState.ClearTracks();
				skeleton.Skeleton.SetToSetupPose();
				if (!string.IsNullOrEmpty(pose.animation))
				{
					skeleton.AnimationState.SetAnimation(0, pose.animation, pose.loop);
				}
			}
			Reference20ParticleGraphic[] array4 = particles;
			foreach (Reference20ParticleGraphic emitter in array4)
			{
				emitter.Restart();
			}
			Reference20Particle3DGraphic[] array5 = particles3d;
			foreach (Reference20Particle3DGraphic emitter2 in array5)
			{
				emitter2.Restart();
			}
			CanvasGroup rootAlpha = GetComponent<CanvasGroup>();
			if ((bool)rootAlpha)
			{
				rootAlpha.alpha = rootOpacity;
			}
			remaining = 0f;
			release = null;
		}

		public void Lease(float seconds, Action<Reference20EffectInstance> complete)
		{
			remaining = seconds;
			release = complete;
			base.gameObject.SetActive(true);
		}

		private void Update()
		{
			if (release != null)
			{
				remaining -= Time.deltaTime;
				if (remaining <= 0f)
				{
					Complete();
				}
			}
		}

		public void Complete()
		{
			if (release == null)
			{
				return;
			}
			Action<Reference20EffectInstance> complete = release;
			release = null;
			Reference20CompanyParticles.StopChildren(base.transform);
			Pose[] array = poses;
			for (int i = 0; i < array.Length; i++)
			{
				Pose pose = array[i];
				if ((bool)pose.transform)
				{
					pose.transform.DOKill();
				}
			}
			base.gameObject.SetActive(false);
			complete(this);
		}
	}
}
