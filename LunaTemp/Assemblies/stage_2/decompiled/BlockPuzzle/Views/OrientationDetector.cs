using System;
using UnityEngine;

namespace BlockPuzzle.Views
{
	public class OrientationDetector : MonoBehaviour
	{
		private int lastScreenWidth;

		private int lastScreenHeight;

		private ScreenOrientationType currentOrientation;

		[Header("Debounce Settings")]
		public float debounceTime = 0.1f;

		private float debounceTimer = 0f;

		private bool isDebouncing = false;

		public static OrientationDetector Instance { get; private set; }

		public ScreenOrientationType CurrentOrientation => currentOrientation;

		public event Action<ScreenOrientationType> OnOrientationChanged;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
				currentOrientation = ((Screen.width <= Screen.height) ? ScreenOrientationType.Portrait : ScreenOrientationType.Landscape);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void Update()
		{
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
			{
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
				isDebouncing = true;
				debounceTimer = debounceTime;
			}
			if (isDebouncing)
			{
				debounceTimer -= Time.deltaTime;
				if (debounceTimer <= 0f)
				{
					isDebouncing = false;
					CheckOrientation();
				}
			}
		}

		private void CheckOrientation()
		{
			ScreenOrientationType newOrientation = ((Screen.width <= Screen.height) ? ScreenOrientationType.Portrait : ScreenOrientationType.Landscape);
			if (newOrientation != currentOrientation)
			{
				currentOrientation = newOrientation;
				this.OnOrientationChanged?.Invoke(currentOrientation);
			}
		}
	}
}
