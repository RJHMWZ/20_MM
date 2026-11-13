using UnityEngine;
using UnityEngine.UI;

namespace Match3.UI
{
	public sealed class RuntimeLoadingScreen
	{
		private const string LogoResourcePath = "Reference20/Company/UI/Loading_Logo_en";

		private readonly GameObject root;

		private readonly RectTransform progressFillRect;

		private RuntimeLoadingScreen(GameObject rootObject, RectTransform fillRect)
		{
			root = rootObject;
			progressFillRect = fillRect;
		}

		public static RuntimeLoadingScreen Create()
		{
			GameObject root = new GameObject("RuntimeLoadingScreen");
			Canvas canvas = root.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.referencePixelsPerUnit = 100f;
			canvas.sortingOrder = 32000;
			float shortSide = Mathf.Max(320f, Mathf.Min(Screen.width, Screen.height));
			Image background = CreateImage("PurpleBackground", root.transform);
			StretchToParent(background.rectTransform);
			background.color = new Color32(72, 34, 122, byte.MaxValue);
			Image logo = CreateImage("Loading_Logo_en", root.transform);
			RectTransform logoRect = logo.rectTransform;
			logoRect.anchorMin = new Vector2(0.5f, 0.5f);
			logoRect.anchorMax = new Vector2(0.5f, 0.5f);
			logoRect.pivot = new Vector2(0.5f, 0.5f);
			float logoWidth = Mathf.Clamp(shortSide * 0.58f, 230f, 590f);
			logoWidth = Mathf.Min(logoWidth, (float)Screen.width * 0.78f, (float)Screen.height * 0.55f);
			float logoHeight = logoWidth * 524f / 700f;
			float gap = Mathf.Clamp(shortSide * 0.08f, 24f, 48f);
			float trackHeight = Mathf.Clamp(shortSide * 0.024f, 12f, 24f);
			logoRect.sizeDelta = new Vector2(logoWidth, logoHeight);
			logoRect.anchoredPosition = new Vector2(0f, (gap + trackHeight) * 0.5f);
			logo.sprite = Resources.Load<Sprite>("Reference20/Company/UI/Loading_Logo_en");
			logo.preserveAspect = true;
			logo.color = Color.white;
			Image track = CreateImage("ProgressTrack", root.transform);
			RectTransform trackRect = track.rectTransform;
			trackRect.anchorMin = new Vector2(0.5f, 0.5f);
			trackRect.anchorMax = new Vector2(0.5f, 0.5f);
			trackRect.pivot = new Vector2(0.5f, 0.5f);
			trackRect.sizeDelta = new Vector2(logoWidth * 0.9f, trackHeight);
			trackRect.anchoredPosition = new Vector2(0f, (0f - (logoHeight + gap)) * 0.5f);
			track.color = new Color32(39, 18, 76, 190);
			Image fill = CreateImage("ProgressFill", track.transform);
			RectTransform fillRect = fill.rectTransform;
			fillRect.anchorMin = new Vector2(0f, 0f);
			fillRect.anchorMax = new Vector2(0f, 1f);
			fillRect.offsetMin = new Vector2(5f, 5f);
			fillRect.offsetMax = new Vector2(-5f, -5f);
			fill.color = new Color32(byte.MaxValue, 220, 105, byte.MaxValue);
			return new RuntimeLoadingScreen(root, fillRect);
		}

		public void SetProgress(float value)
		{
			if (progressFillRect != null)
			{
				progressFillRect.anchorMax = new Vector2(Mathf.Clamp01(value), 1f);
			}
		}

		public void Close()
		{
			if (root != null)
			{
				Object.Destroy(root);
			}
		}

		private static Image CreateImage(string objectName, Transform parent)
		{
			GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			imageObject.transform.SetParent(parent, false);
			return imageObject.GetComponent<Image>();
		}

		private static void StretchToParent(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
		}
	}
}
