using UnityEngine;

namespace Reference20
{
	[ExecuteAlways]
	public class Reference20ResponsiveLayout : MonoBehaviour
	{
		public RectTransform board;

		public RectTransform background;

		public RectTransform badge;

		public RectTransform download;

		public RectTransform endFrame;

		private RectTransform canvasRect;

		private void OnEnable()
		{
			canvasRect = base.transform as RectTransform;
		}

		private void LateUpdate()
		{
			Refresh();
		}

		public void Refresh()
		{
			if (!canvasRect)
			{
				canvasRect = base.transform as RectTransform;
			}
			if (!canvasRect || !board)
			{
				return;
			}
			Vector2 size = canvasRect.rect.size;
			if (!(size.x <= 0f) && !(size.y <= 0f))
			{
				Rect safe = Screen.safeArea;
				if (Screen.width <= 0 || Screen.height <= 0)
				{
					safe = new Rect(0f, 0f, size.x, size.y);
				}
				float rx = size.x / (float)Mathf.Max(1, Screen.width);
				float ry = size.y / (float)Mathf.Max(1, Screen.height);
				Rect area = new Rect(safe.x * rx, safe.y * ry, safe.width * rx, safe.height * ry);
				if (area.width <= 0f || area.height <= 0f)
				{
					area = new Rect(0f, 0f, size.x, size.y);
				}
				bool landscape = size.x > size.y;
				float mapScale = Mathf.Max(0.01f, Mathf.Min(area.width / 1152f, area.height / 1536f) - 0.05f);
				float scale = mapScale / 0.8875f;
				Vector2 center = area.center;
				Vector2 vector3 = (board.anchorMin = (board.anchorMax = new Vector2(0.5f, 0.5f)));
				board.pivot = new Vector2(0.5f, 0.5f);
				board.sizeDelta = new Vector2(1080f, 1920f);
				board.localScale = Vector3.one * scale;
				board.anchoredPosition = center - size * 0.5f;
				if ((bool)background)
				{
					vector3 = (background.anchorMin = (background.anchorMax = new Vector2(0.5f, 0.5f)));
					background.anchoredPosition = Vector2.zero;
					background.localScale = Vector3.one;
					float i = Mathf.Max(size.x / 1079f, size.y / 1920f);
					background.sizeDelta = new Vector2(1079f, 1920f) * i;
				}
				if ((bool)badge)
				{
					vector3 = (badge.anchorMin = (badge.anchorMax = Vector2.zero));
					badge.pivot = new Vector2(0f, 1f);
					badge.anchoredPosition = new Vector2(area.x, area.yMax);
					badge.sizeDelta = new Vector2(350f, 350f);
					badge.localScale = Vector3.one;
				}
				if ((bool)download)
				{
					download.gameObject.SetActive(!landscape);
					vector3 = (download.anchorMin = (download.anchorMax = Vector2.zero));
					download.pivot = new Vector2(0.5f, 0.5f);
					download.localScale = Vector3.one;
					download.sizeDelta = new Vector2(423f, 159f);
					download.anchoredPosition = new Vector2(area.center.x, area.y + 139.776f);
				}
				if ((bool)endFrame)
				{
					endFrame.anchorMin = Vector2.zero;
					endFrame.anchorMax = Vector2.one;
					endFrame.pivot = new Vector2(0.5f, 0.5f);
					endFrame.anchoredPosition = Vector2.zero;
					endFrame.sizeDelta = Vector2.zero;
					endFrame.localScale = Vector3.one;
				}
			}
		}
	}
}
