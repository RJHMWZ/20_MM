using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[RequireComponent(typeof(Animator))]
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonMecanim-Component")]
	public class SkeletonMecanim : SkeletonRenderer, ISkeletonAnimation
	{
		[Serializable]
		public class MecanimTranslator
		{
			public delegate void OnClipAppliedDelegate(Animation clip, int layerIndex, float weight, float time, float lastTime, bool playsBackward);

			public enum MixMode
			{
				AlwaysMix,
				MixNext,
				Hard
			}

			protected class ClipInfos
			{
				public bool isInterruptionActive = false;

				public bool isLastFrameOfInterruption = false;

				public int clipInfoCount = 0;

				public int nextClipInfoCount = 0;

				public int interruptingClipInfoCount = 0;

				public readonly List<AnimatorClipInfo> clipInfos = new List<AnimatorClipInfo>();

				public readonly List<AnimatorClipInfo> nextClipInfos = new List<AnimatorClipInfo>();

				public readonly List<AnimatorClipInfo> interruptingClipInfos = new List<AnimatorClipInfo>();

				public AnimatorStateInfo stateInfo;

				public AnimatorStateInfo nextStateInfo;

				public AnimatorStateInfo interruptingStateInfo;

				public float interruptingClipTimeAddition = 0f;
			}

			private class AnimationClipEqualityComparer : IEqualityComparer<AnimationClip>
			{
				internal static readonly IEqualityComparer<AnimationClip> Instance = new AnimationClipEqualityComparer();

				public bool Equals(AnimationClip x, AnimationClip y)
				{
					return x.GetInstanceID() == y.GetInstanceID();
				}

				public int GetHashCode(AnimationClip o)
				{
					return o.GetInstanceID();
				}
			}

			private class IntEqualityComparer : IEqualityComparer<int>
			{
				internal static readonly IEqualityComparer<int> Instance = new IntEqualityComparer();

				public bool Equals(int x, int y)
				{
					return x == y;
				}

				public int GetHashCode(int o)
				{
					return o;
				}
			}

			private const float WeightEpsilon = 0.0001f;

			public bool autoReset = true;

			public bool useCustomMixMode = true;

			public MixMode[] layerMixModes = new MixMode[0];

			public MixBlend[] layerBlendModes = new MixBlend[0];

			private readonly Dictionary<int, Animation> animationTable = new Dictionary<int, Animation>(IntEqualityComparer.Instance);

			private readonly Dictionary<AnimationClip, int> clipNameHashCodeTable = new Dictionary<AnimationClip, int>(AnimationClipEqualityComparer.Instance);

			private readonly List<Animation> previousAnimations = new List<Animation>();

			protected ClipInfos[] layerClipInfos = new ClipInfos[0];

			private Animator animator;

			public Animator Animator => animator;

			public int MecanimLayerCount
			{
				get
				{
					if (!animator)
					{
						return 0;
					}
					return animator.layerCount;
				}
			}

			public string[] MecanimLayerNames
			{
				get
				{
					if (!animator)
					{
						return new string[0];
					}
					string[] layerNames = new string[animator.layerCount];
					for (int i = 0; i < animator.layerCount; i++)
					{
						layerNames[i] = animator.GetLayerName(i);
					}
					return layerNames;
				}
			}

			protected event OnClipAppliedDelegate _OnClipApplied;

			public event OnClipAppliedDelegate OnClipApplied
			{
				add
				{
					_OnClipApplied += value;
				}
				remove
				{
					_OnClipApplied -= value;
				}
			}

			public void Initialize(Animator animator, SkeletonDataAsset skeletonDataAsset)
			{
				this.animator = animator;
				previousAnimations.Clear();
				animationTable.Clear();
				SkeletonData data = skeletonDataAsset.GetSkeletonData(true);
				foreach (Animation a in data.Animations)
				{
					animationTable.Add(a.Name.GetHashCode(), a);
				}
				clipNameHashCodeTable.Clear();
				ClearClipInfosForLayers();
			}

			private bool ApplyAnimation(Skeleton skeleton, AnimatorClipInfo info, AnimatorStateInfo stateInfo, int layerIndex, float layerWeight, MixBlend layerBlendMode, bool useClipWeight1 = false)
			{
				float weight = info.weight * layerWeight;
				if (weight < 0.0001f)
				{
					return false;
				}
				Animation clip = GetAnimation(info.clip);
				if (clip == null)
				{
					return false;
				}
				float time = AnimationTime(stateInfo.normalizedTime, info.clip.length, info.clip.isLooping, stateInfo.speed < 0f);
				weight = (useClipWeight1 ? layerWeight : weight);
				clip.Apply(skeleton, 0f, time, info.clip.isLooping, null, weight, layerBlendMode, MixDirection.In);
				if (this._OnClipApplied != null)
				{
					OnClipAppliedCallback(clip, stateInfo, layerIndex, time, info.clip.isLooping, weight);
				}
				return true;
			}

			private bool ApplyInterruptionAnimation(Skeleton skeleton, bool interpolateWeightTo1, AnimatorClipInfo info, AnimatorStateInfo stateInfo, int layerIndex, float layerWeight, MixBlend layerBlendMode, float interruptingClipTimeAddition, bool useClipWeight1 = false)
			{
				float clipWeight = (interpolateWeightTo1 ? ((info.weight + 1f) * 0.5f) : info.weight);
				float weight = clipWeight * layerWeight;
				if (weight < 0.0001f)
				{
					return false;
				}
				Animation clip = GetAnimation(info.clip);
				if (clip == null)
				{
					return false;
				}
				float time = AnimationTime(stateInfo.normalizedTime + interruptingClipTimeAddition, info.clip.length, stateInfo.speed < 0f);
				weight = (useClipWeight1 ? layerWeight : weight);
				clip.Apply(skeleton, 0f, time, info.clip.isLooping, null, weight, layerBlendMode, MixDirection.In);
				if (this._OnClipApplied != null)
				{
					OnClipAppliedCallback(clip, stateInfo, layerIndex, time, info.clip.isLooping, weight);
				}
				return true;
			}

			private void OnClipAppliedCallback(Animation clip, AnimatorStateInfo stateInfo, int layerIndex, float time, bool isLooping, float weight)
			{
				float speedFactor = stateInfo.speedMultiplier * stateInfo.speed;
				float lastTime = time - Time.deltaTime * speedFactor;
				if (isLooping && clip.duration != 0f)
				{
					time %= clip.duration;
					lastTime %= clip.duration;
				}
				this._OnClipApplied(clip, layerIndex, weight, time, lastTime, speedFactor < 0f);
			}

			public void Apply(Skeleton skeleton)
			{
				if (layerMixModes.Length < animator.layerCount)
				{
					int oldSize = layerMixModes.Length;
					Array.Resize(ref layerMixModes, animator.layerCount);
					for (int layer = oldSize; layer < animator.layerCount; layer++)
					{
						bool isAdditiveLayer = false;
						if (layer < layerBlendModes.Length)
						{
							isAdditiveLayer = layerBlendModes[layer] == MixBlend.Add;
						}
						layerMixModes[layer] = ((!isAdditiveLayer) ? MixMode.MixNext : MixMode.AlwaysMix);
					}
				}
				InitClipInfosForLayers();
				int layer2 = 0;
				for (int j = animator.layerCount; layer2 < j; layer2++)
				{
					GetStateUpdatesFromAnimator(layer2);
				}
				if (autoReset)
				{
					List<Animation> previousAnimations = this.previousAnimations;
					int i = 0;
					for (int l = previousAnimations.Count; i < l; i++)
					{
						previousAnimations[i].SetKeyedItemsToSetupPose(skeleton);
					}
					previousAnimations.Clear();
					int layer4 = 0;
					for (int m = animator.layerCount; layer4 < m; layer4++)
					{
						float layerWeight2 = ((layer4 == 0) ? 1f : animator.GetLayerWeight(layer4));
						if (layerWeight2 <= 0f)
						{
							continue;
						}
						bool hasNext2 = animator.GetNextAnimatorStateInfo(layer4).fullPathHash != 0;
						GetAnimatorClipInfos(layer4, out var isInterruptionActive2, out var clipInfoCount2, out var nextClipInfoCount2, out var interruptingClipInfoCount2, out var clipInfo2, out var nextClipInfo2, out var interruptingClipInfo2, out var shallInterpolateWeightTo1);
						for (int c7 = 0; c7 < clipInfoCount2; c7++)
						{
							AnimatorClipInfo info = clipInfo2[c7];
							float weight = info.weight * layerWeight2;
							if (!(weight < 0.0001f))
							{
								Animation clip = GetAnimation(info.clip);
								if (clip != null)
								{
									previousAnimations.Add(clip);
								}
							}
						}
						if (hasNext2)
						{
							for (int c6 = 0; c6 < nextClipInfoCount2; c6++)
							{
								AnimatorClipInfo info3 = nextClipInfo2[c6];
								float weight3 = info3.weight * layerWeight2;
								if (!(weight3 < 0.0001f))
								{
									Animation clip3 = GetAnimation(info3.clip);
									if (clip3 != null)
									{
										previousAnimations.Add(clip3);
									}
								}
							}
						}
						if (!isInterruptionActive2)
						{
							continue;
						}
						for (int c5 = 0; c5 < interruptingClipInfoCount2; c5++)
						{
							AnimatorClipInfo info2 = interruptingClipInfo2[c5];
							float clipWeight = (shallInterpolateWeightTo1 ? ((info2.weight + 1f) * 0.5f) : info2.weight);
							float weight2 = clipWeight * layerWeight2;
							if (!(weight2 < 0.0001f))
							{
								Animation clip2 = GetAnimation(info2.clip);
								if (clip2 != null)
								{
									previousAnimations.Add(clip2);
								}
							}
						}
					}
				}
				int layer3 = 0;
				for (int k = animator.layerCount; layer3 < k; layer3++)
				{
					float layerWeight = ((layer3 == 0) ? 1f : animator.GetLayerWeight(layer3));
					GetAnimatorStateInfos(layer3, out var isInterruptionActive, out var stateInfo, out var nextStateInfo, out var interruptingStateInfo, out var interruptingClipTimeAddition);
					bool hasNext = nextStateInfo.fullPathHash != 0;
					GetAnimatorClipInfos(layer3, out isInterruptionActive, out var clipInfoCount, out var nextClipInfoCount, out var interruptingClipInfoCount, out var clipInfo, out var nextClipInfo, out var interruptingClipInfo, out var interpolateWeightTo1);
					MixBlend layerBlendMode = ((layer3 < layerBlendModes.Length) ? layerBlendModes[layer3] : MixBlend.Replace);
					MixMode mode = GetMixMode(layer3, layerBlendMode);
					if (mode == MixMode.AlwaysMix)
					{
						for (int c4 = 0; c4 < clipInfoCount; c4++)
						{
							ApplyAnimation(skeleton, clipInfo[c4], stateInfo, layer3, layerWeight, layerBlendMode);
						}
						if (hasNext)
						{
							for (int c3 = 0; c3 < nextClipInfoCount; c3++)
							{
								ApplyAnimation(skeleton, nextClipInfo[c3], nextStateInfo, layer3, layerWeight, layerBlendMode);
							}
						}
						if (isInterruptionActive)
						{
							for (int c2 = 0; c2 < interruptingClipInfoCount; c2++)
							{
								ApplyInterruptionAnimation(skeleton, interpolateWeightTo1, interruptingClipInfo[c2], interruptingStateInfo, layer3, layerWeight, layerBlendMode, interruptingClipTimeAddition);
							}
						}
						continue;
					}
					int c;
					for (c = 0; c < clipInfoCount; c++)
					{
						if (ApplyAnimation(skeleton, clipInfo[c], stateInfo, layer3, layerWeight, layerBlendMode, true))
						{
							c++;
							break;
						}
					}
					for (; c < clipInfoCount; c++)
					{
						ApplyAnimation(skeleton, clipInfo[c], stateInfo, layer3, layerWeight, layerBlendMode);
					}
					c = 0;
					if (hasNext)
					{
						if (mode == MixMode.Hard)
						{
							for (; c < nextClipInfoCount; c++)
							{
								if (ApplyAnimation(skeleton, nextClipInfo[c], nextStateInfo, layer3, layerWeight, layerBlendMode, true))
								{
									c++;
									break;
								}
							}
						}
						for (; c < nextClipInfoCount; c++)
						{
							if (!ApplyAnimation(skeleton, nextClipInfo[c], nextStateInfo, layer3, layerWeight, layerBlendMode))
							{
							}
						}
					}
					c = 0;
					if (!isInterruptionActive)
					{
						continue;
					}
					if (mode == MixMode.Hard)
					{
						for (; c < interruptingClipInfoCount; c++)
						{
							if (ApplyInterruptionAnimation(skeleton, interpolateWeightTo1, interruptingClipInfo[c], interruptingStateInfo, layer3, layerWeight, layerBlendMode, interruptingClipTimeAddition, true))
							{
								c++;
								break;
							}
						}
					}
					for (; c < interruptingClipInfoCount; c++)
					{
						ApplyInterruptionAnimation(skeleton, interpolateWeightTo1, interruptingClipInfo[c], interruptingStateInfo, layer3, layerWeight, layerBlendMode, interruptingClipTimeAddition);
					}
				}
			}

			public KeyValuePair<Animation, float> GetActiveAnimationAndTime(int layer)
			{
				if (layer >= layerClipInfos.Length)
				{
					return new KeyValuePair<Animation, float>(null, 0f);
				}
				ClipInfos layerInfos = layerClipInfos[layer];
				bool isInterruptionActive = layerInfos.isInterruptionActive;
				AnimationClip clip = null;
				Animation animation = null;
				AnimatorStateInfo stateInfo;
				if (isInterruptionActive && layerInfos.interruptingClipInfoCount > 0)
				{
					clip = layerInfos.interruptingClipInfos[0].clip;
					stateInfo = layerInfos.interruptingStateInfo;
				}
				else
				{
					clip = layerInfos.clipInfos[0].clip;
					stateInfo = layerInfos.stateInfo;
				}
				animation = GetAnimation(clip);
				float time = AnimationTime(stateInfo.normalizedTime, clip.length, clip.isLooping, stateInfo.speed < 0f);
				return new KeyValuePair<Animation, float>(animation, time);
			}

			private static float AnimationTime(float normalizedTime, float clipLength, bool loop, bool reversed)
			{
				float time = AnimationTime(normalizedTime, clipLength, reversed);
				if (loop)
				{
					return time;
				}
				return (clipLength - time < 1f / 30f) ? clipLength : time;
			}

			private static float AnimationTime(float normalizedTime, float clipLength, bool reversed)
			{
				if (reversed)
				{
					normalizedTime = 1f - normalizedTime;
				}
				if (normalizedTime < 0f)
				{
					normalizedTime = normalizedTime % 1f + 1f;
				}
				return normalizedTime * clipLength;
			}

			private void InitClipInfosForLayers()
			{
				if (layerClipInfos.Length >= animator.layerCount)
				{
					return;
				}
				Array.Resize(ref layerClipInfos, animator.layerCount);
				int layer = 0;
				for (int i = animator.layerCount; layer < i; layer++)
				{
					if (layerClipInfos[layer] == null)
					{
						layerClipInfos[layer] = new ClipInfos();
					}
				}
			}

			private void ClearClipInfosForLayers()
			{
				int layer = 0;
				for (int i = layerClipInfos.Length; layer < i; layer++)
				{
					if (layerClipInfos[layer] == null)
					{
						layerClipInfos[layer] = new ClipInfos();
						continue;
					}
					layerClipInfos[layer].isInterruptionActive = false;
					layerClipInfos[layer].isLastFrameOfInterruption = false;
					layerClipInfos[layer].clipInfos.Clear();
					layerClipInfos[layer].nextClipInfos.Clear();
					layerClipInfos[layer].interruptingClipInfos.Clear();
				}
			}

			private MixMode GetMixMode(int layer, MixBlend layerBlendMode)
			{
				if (useCustomMixMode)
				{
					MixMode mode = layerMixModes[layer];
					if (layerBlendMode == MixBlend.Add && mode == MixMode.MixNext)
					{
						mode = MixMode.AlwaysMix;
						layerMixModes[layer] = mode;
					}
					return mode;
				}
				return (layerBlendMode != MixBlend.Add) ? MixMode.MixNext : MixMode.AlwaysMix;
			}

			private void GetStateUpdatesFromAnimator(int layer)
			{
				ClipInfos layerInfos = layerClipInfos[layer];
				int clipInfoCount = animator.GetCurrentAnimatorClipInfoCount(layer);
				int nextClipInfoCount = animator.GetNextAnimatorClipInfoCount(layer);
				List<AnimatorClipInfo> clipInfos = layerInfos.clipInfos;
				List<AnimatorClipInfo> nextClipInfos = layerInfos.nextClipInfos;
				List<AnimatorClipInfo> interruptingClipInfos = layerInfos.interruptingClipInfos;
				layerInfos.isInterruptionActive = clipInfoCount == 0 && clipInfos.Count != 0 && nextClipInfoCount == 0 && nextClipInfos.Count != 0;
				if (layerInfos.isInterruptionActive)
				{
					AnimatorStateInfo interruptingStateInfo = animator.GetNextAnimatorStateInfo(layer);
					layerInfos.isLastFrameOfInterruption = interruptingStateInfo.fullPathHash == 0;
					if (!layerInfos.isLastFrameOfInterruption)
					{
						animator.GetNextAnimatorClipInfo(layer, interruptingClipInfos);
						layerInfos.interruptingClipInfoCount = interruptingClipInfos.Count;
						float oldTime = layerInfos.interruptingStateInfo.normalizedTime;
						float newTime = interruptingStateInfo.normalizedTime;
						layerInfos.interruptingClipTimeAddition = newTime - oldTime;
						layerInfos.interruptingStateInfo = interruptingStateInfo;
					}
					return;
				}
				layerInfos.clipInfoCount = clipInfoCount;
				layerInfos.nextClipInfoCount = nextClipInfoCount;
				layerInfos.interruptingClipInfoCount = 0;
				layerInfos.isLastFrameOfInterruption = false;
				if (clipInfos.Capacity < clipInfoCount)
				{
					clipInfos.Capacity = clipInfoCount;
				}
				if (nextClipInfos.Capacity < nextClipInfoCount)
				{
					nextClipInfos.Capacity = nextClipInfoCount;
				}
				animator.GetCurrentAnimatorClipInfo(layer, clipInfos);
				animator.GetNextAnimatorClipInfo(layer, nextClipInfos);
				layerInfos.stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
				layerInfos.nextStateInfo = animator.GetNextAnimatorStateInfo(layer);
			}

			private void GetAnimatorClipInfos(int layer, out bool isInterruptionActive, out int clipInfoCount, out int nextClipInfoCount, out int interruptingClipInfoCount, out IList<AnimatorClipInfo> clipInfo, out IList<AnimatorClipInfo> nextClipInfo, out IList<AnimatorClipInfo> interruptingClipInfo, out bool shallInterpolateWeightTo1)
			{
				ClipInfos layerInfos = layerClipInfos[layer];
				isInterruptionActive = layerInfos.isInterruptionActive;
				clipInfoCount = layerInfos.clipInfoCount;
				nextClipInfoCount = layerInfos.nextClipInfoCount;
				interruptingClipInfoCount = layerInfos.interruptingClipInfoCount;
				clipInfo = layerInfos.clipInfos;
				nextClipInfo = layerInfos.nextClipInfos;
				interruptingClipInfo = (isInterruptionActive ? layerInfos.interruptingClipInfos : null);
				shallInterpolateWeightTo1 = layerInfos.isLastFrameOfInterruption;
			}

			private void GetAnimatorStateInfos(int layer, out bool isInterruptionActive, out AnimatorStateInfo stateInfo, out AnimatorStateInfo nextStateInfo, out AnimatorStateInfo interruptingStateInfo, out float interruptingClipTimeAddition)
			{
				ClipInfos layerInfos = layerClipInfos[layer];
				isInterruptionActive = layerInfos.isInterruptionActive;
				stateInfo = layerInfos.stateInfo;
				nextStateInfo = layerInfos.nextStateInfo;
				interruptingStateInfo = layerInfos.interruptingStateInfo;
				interruptingClipTimeAddition = (layerInfos.isLastFrameOfInterruption ? layerInfos.interruptingClipTimeAddition : 0f);
			}

			private Animation GetAnimation(AnimationClip clip)
			{
				if (!clipNameHashCodeTable.TryGetValue(clip, out var clipNameHashCode))
				{
					clipNameHashCode = clip.name.GetHashCode();
					clipNameHashCodeTable.Add(clip, clipNameHashCode);
				}
				animationTable.TryGetValue(clipNameHashCode, out var animation);
				return animation;
			}
		}

		[SerializeField]
		protected MecanimTranslator translator;

		private bool wasUpdatedAfterInit = true;

		public MecanimTranslator Translator => translator;

		protected event UpdateBonesDelegate _BeforeApply;

		protected event UpdateBonesDelegate _UpdateLocal;

		protected event UpdateBonesDelegate _UpdateWorld;

		protected event UpdateBonesDelegate _UpdateComplete;

		public event UpdateBonesDelegate BeforeApply
		{
			add
			{
				_BeforeApply += value;
			}
			remove
			{
				_BeforeApply -= value;
			}
		}

		public event UpdateBonesDelegate UpdateLocal
		{
			add
			{
				_UpdateLocal += value;
			}
			remove
			{
				_UpdateLocal -= value;
			}
		}

		public event UpdateBonesDelegate UpdateWorld
		{
			add
			{
				_UpdateWorld += value;
			}
			remove
			{
				_UpdateWorld -= value;
			}
		}

		public event UpdateBonesDelegate UpdateComplete
		{
			add
			{
				_UpdateComplete += value;
			}
			remove
			{
				_UpdateComplete -= value;
			}
		}

		public override void Initialize(bool overwrite, bool quiet = false)
		{
			if (valid && !overwrite)
			{
				return;
			}
			base.Initialize(overwrite, quiet);
			if (valid)
			{
				if (translator == null)
				{
					translator = new MecanimTranslator();
				}
				translator.Initialize(GetComponent<Animator>(), skeletonDataAsset);
				wasUpdatedAfterInit = false;
			}
		}

		public void Update()
		{
			if (valid)
			{
				wasUpdatedAfterInit = true;
				if (updateMode > UpdateMode.OnlyAnimationStatus)
				{
					ApplyAnimation();
				}
			}
		}

		protected void ApplyAnimation()
		{
			if (this._BeforeApply != null)
			{
				this._BeforeApply(this);
			}
			translator.Apply(skeleton);
			if (this._UpdateLocal != null)
			{
				this._UpdateLocal(this);
			}
			skeleton.UpdateWorldTransform();
			if (this._UpdateWorld != null)
			{
				this._UpdateWorld(this);
				skeleton.UpdateWorldTransform();
			}
			if (this._UpdateComplete != null)
			{
				this._UpdateComplete(this);
			}
		}

		public override void LateUpdate()
		{
			if (!wasUpdatedAfterInit)
			{
				Update();
			}
			base.LateUpdate();
		}
	}
}
