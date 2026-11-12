using UnityEngine;

namespace BlockPuzzle.Views
{
	public class OrientationActiveSwitcher : MonoBehaviour
	{
		[Header("物体布局配置")]
		[SerializeField]
		private OrientationObjectLayout[] layouts;

		private void Start()
		{
			if (OrientationDetector.Instance != null)
			{
				OrientationDetector.Instance.OnOrientationChanged += ApplyOrientation;
			}
			ApplyOrientation(ScreenOrientationType.Portrait);
		}

		private void OnDestroy()
		{
			if (OrientationDetector.Instance != null)
			{
				OrientationDetector.Instance.OnOrientationChanged -= ApplyOrientation;
			}
		}

		private void ApplyOrientation(ScreenOrientationType orientation)
		{
			orientation = ScreenOrientationType.Portrait;
			if (layouts == null)
			{
				return;
			}
			bool isLandscape = orientation == ScreenOrientationType.Landscape;
			OrientationObjectLayout[] array = layouts;
			foreach (OrientationObjectLayout layout in array)
			{
				if (layout?.Target == null)
				{
					continue;
				}
				bool active = (isLandscape ? layout.LandscapeActive : layout.PortraitActive);
				layout.Target.SetActive(active);
				if (!active)
				{
					continue;
				}
				if (layout.IsUI)
				{
					RectTransform rectTransform = layout.Target.GetComponent<RectTransform>();
					if (rectTransform != null)
					{
						if (layout.SwitchAnchors)
						{
							rectTransform.anchorMin = (isLandscape ? layout.LandscapeAnchorMin : layout.PortraitAnchorMin);
							rectTransform.anchorMax = (isLandscape ? layout.LandscapeAnchorMax : layout.PortraitAnchorMax);
							rectTransform.pivot = (isLandscape ? layout.LandscapePivot : layout.PortraitPivot);
						}
						rectTransform.anchoredPosition = (isLandscape ? layout.LandscapeAnchoredPosition : layout.PortraitAnchoredPosition);
						if (layout.LandscapeSizeDelta != Vector2.zero || layout.PortraitSizeDelta != Vector2.zero)
						{
							rectTransform.sizeDelta = (isLandscape ? layout.LandscapeSizeDelta : layout.PortraitSizeDelta);
						}
						rectTransform.localScale = (isLandscape ? layout.LandscapeScale : layout.PortraitScale);
					}
				}
				else
				{
					layout.Target.transform.localPosition = (isLandscape ? layout.LandscapePosition : layout.PortraitPosition);
					layout.Target.transform.localScale = (isLandscape ? layout.LandscapeScale : layout.PortraitScale);
				}
			}
		}

		private void MarkDirty()
		{
		}

		public void RecordAsLandscape()
		{
			if (layouts == null)
			{
				return;
			}
			OrientationObjectLayout[] array = layouts;
			foreach (OrientationObjectLayout layout in array)
			{
				if (layout?.Target == null)
				{
					continue;
				}
				layout.LandscapeActive = layout.Target.activeSelf;
				layout.LandscapeScale = layout.Target.transform.localScale;
				if (layout.IsUI)
				{
					RectTransform rect = layout.Target.GetComponent<RectTransform>();
					if (rect != null)
					{
						layout.LandscapeAnchoredPosition = rect.anchoredPosition;
						layout.LandscapeSizeDelta = rect.sizeDelta;
						if (layout.SwitchAnchors)
						{
							layout.LandscapeAnchorMin = rect.anchorMin;
							layout.LandscapeAnchorMax = rect.anchorMax;
							layout.LandscapePivot = rect.pivot;
						}
					}
				}
				else
				{
					layout.LandscapePosition = layout.Target.transform.localPosition;
				}
			}
			MarkDirty();
			Debug.Log("已记录当前状态为横屏布局");
		}

		public void RecordAsPortrait()
		{
			if (layouts == null)
			{
				return;
			}
			OrientationObjectLayout[] array = layouts;
			foreach (OrientationObjectLayout layout in array)
			{
				if (layout?.Target == null)
				{
					continue;
				}
				layout.PortraitActive = layout.Target.activeSelf;
				layout.PortraitScale = layout.Target.transform.localScale;
				if (layout.IsUI)
				{
					RectTransform rect = layout.Target.GetComponent<RectTransform>();
					if (rect != null)
					{
						layout.PortraitAnchoredPosition = rect.anchoredPosition;
						layout.PortraitSizeDelta = rect.sizeDelta;
						if (layout.SwitchAnchors)
						{
							layout.PortraitAnchorMin = rect.anchorMin;
							layout.PortraitAnchorMax = rect.anchorMax;
							layout.PortraitPivot = rect.pivot;
						}
					}
				}
				else
				{
					layout.PortraitPosition = layout.Target.transform.localPosition;
				}
			}
			MarkDirty();
			Debug.Log("已记录当前状态为竖屏布局");
		}

		public void PreviewLandscape()
		{
			ApplyOrientation(ScreenOrientationType.Landscape);
		}

		public void PreviewPortrait()
		{
			ApplyOrientation(ScreenOrientationType.Portrait);
		}
	}
}
