using System;
using UnityEngine;

namespace BlockPuzzle.Views
{
    public class OrientationDetector : MonoBehaviour
    {
        public static OrientationDetector Instance { get; private set; }

        public event Action<ScreenOrientationType> OnOrientationChanged;

        private int lastScreenWidth;
        private int lastScreenHeight;
        private ScreenOrientationType currentOrientation;

        public ScreenOrientationType CurrentOrientation => currentOrientation;

        [Header("Debounce Settings")]
        public float debounceTime = 0.1f;
        private float debounceTimer = 0f;
        private bool isDebouncing = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            currentOrientation = Screen.width > Screen.height ? ScreenOrientationType.Landscape : ScreenOrientationType.Portrait;
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
            ScreenOrientationType newOrientation = Screen.width > Screen.height ? ScreenOrientationType.Landscape : ScreenOrientationType.Portrait;

            if (newOrientation != currentOrientation)
            {
                currentOrientation = newOrientation;
                OnOrientationChanged?.Invoke(currentOrientation);
            }
        }
    }
}