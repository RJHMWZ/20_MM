using System;
using Spine.Unity;
using UnityEngine;

namespace Match3
{
	[Serializable]
	public class SwapAnimationBinding
	{
		[Tooltip("与哪种特殊元素交换时播放此动画")]
		public SpecialBehaviorType partnerBehaviorType;

		[Tooltip("交换动画专用 Spine 层（子物体，独立骨骼 Data）。留空则使用主 spineAnimation")]
		public SkeletonAnimation swapSpine;

		[SpineAnimation("", "swapSpine", true, true)]
		[Tooltip("交换动画名（拖入 swapSpine 后自动列出该骨骼的动画）")]
		public string animationName;

		[Tooltip("交换动画缩放倍率（1 = 骨骼原始大小）")]
		public float scaleMultiplier = 1f;
	}
}
