using System;
using System.Collections.Generic;

namespace Spine
{
	public class AnimationStateData
	{
		public struct AnimationPair
		{
			public readonly Animation a1;

			public readonly Animation a2;

			public AnimationPair(Animation a1, Animation a2)
			{
				this.a1 = a1;
				this.a2 = a2;
			}

			public override string ToString()
			{
				return a1.name + "->" + a2.name;
			}
		}

		public class AnimationPairComparer : IEqualityComparer<AnimationPair>
		{
			public static readonly AnimationPairComparer Instance = new AnimationPairComparer();

			bool IEqualityComparer<AnimationPair>.Equals(AnimationPair x, AnimationPair y)
			{
				return x.a1 == y.a1 && x.a2 == y.a2;
			}

			int IEqualityComparer<AnimationPair>.GetHashCode(AnimationPair obj)
			{
				int h1 = obj.a1.GetHashCode();
				return ((h1 << 5) + h1) ^ obj.a2.GetHashCode();
			}
		}

		internal SkeletonData skeletonData;

		private readonly Dictionary<AnimationPair, float> animationToMixTime = new Dictionary<AnimationPair, float>(AnimationPairComparer.Instance);

		internal float defaultMix;

		public SkeletonData SkeletonData => skeletonData;

		public float DefaultMix
		{
			get
			{
				return defaultMix;
			}
			set
			{
				defaultMix = value;
			}
		}

		public AnimationStateData(SkeletonData skeletonData)
		{
			if (skeletonData == null)
			{
				throw new ArgumentException("skeletonData cannot be null.", "skeletonData");
			}
			this.skeletonData = skeletonData;
		}

		public void SetMix(string fromName, string toName, float duration)
		{
			Animation from = skeletonData.FindAnimation(fromName);
			if (from == null)
			{
				throw new ArgumentException("Animation not found: " + fromName, "fromName");
			}
			Animation to = skeletonData.FindAnimation(toName);
			if (to == null)
			{
				throw new ArgumentException("Animation not found: " + toName, "toName");
			}
			SetMix(from, to, duration);
		}

		public void SetMix(Animation from, Animation to, float duration)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from", "from cannot be null.");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to", "to cannot be null.");
			}
			AnimationPair key = new AnimationPair(from, to);
			animationToMixTime.Remove(key);
			animationToMixTime.Add(key, duration);
		}

		public float GetMix(Animation from, Animation to)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from", "from cannot be null.");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to", "to cannot be null.");
			}
			AnimationPair key = new AnimationPair(from, to);
			if (animationToMixTime.TryGetValue(key, out var duration))
			{
				return duration;
			}
			return defaultMix;
		}
	}
}
