using System;
using UnityEngine;

namespace BlockPuzzle.Views
{
	[Serializable]
	public class OrientationObjectLayout
	{
		[Tooltip("目标物体")]
		public GameObject Target;

		[Header("横屏布局")]
		public bool LandscapeActive = true;

		public Vector3 LandscapePosition;

		public Vector3 LandscapeScale = Vector3.one;

		[Header("竖屏布局")]
		public bool PortraitActive = true;

		public Vector3 PortraitPosition;

		public Vector3 PortraitScale = Vector3.one;

		[Header("UI专用（RectTransform）")]
		public bool IsUI = false;

		public Vector2 LandscapeAnchoredPosition;

		public Vector2 LandscapeSizeDelta;

		public Vector2 PortraitAnchoredPosition;

		public Vector2 PortraitSizeDelta;

		[Header("UI锚点设置")]
		[Tooltip("是否切换锚点预设")]
		public bool SwitchAnchors = false;

		public Vector2 LandscapeAnchorMin = new Vector2(0.5f, 0.5f);

		public Vector2 LandscapeAnchorMax = new Vector2(0.5f, 0.5f);

		public Vector2 LandscapePivot = new Vector2(0.5f, 0.5f);

		public Vector2 PortraitAnchorMin = new Vector2(0.5f, 0.5f);

		public Vector2 PortraitAnchorMax = new Vector2(0.5f, 0.5f);

		public Vector2 PortraitPivot = new Vector2(0.5f, 0.5f);
	}
}
