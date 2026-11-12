using System;
using UnityEngine;

namespace BlockPuzzle.Views
{
    public enum ScreenOrientationType
    {
        Landscape,
        Portrait
    }

    /// <summary>
    /// 横竖屏物体布局数据
    /// </summary>
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

    /// <summary>
    /// 横竖屏物体切换器
    /// 根据屏幕方向自动切换物体的激活状态、位置和缩放
    /// </summary>
    public class OrientationActiveSwitcher : MonoBehaviour
    {
        [Header("物体布局配置")]
        [SerializeField] private OrientationObjectLayout[] layouts;

        private void Start()
        {
            if (OrientationDetector.Instance != null)
            {
                OrientationDetector.Instance.OnOrientationChanged += ApplyOrientation;
            }
            
            // 无论当前是横屏还是竖屏，始终强制使用竖屏布局参数，避免布局在横屏时变乱
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
            // 我们通过 GameUIManager 等比缩放了整个 Canvas，
            // 为了维持原布局，强制覆盖这里的 orientation，始终使用 Portrait 布局的数据。
            orientation = ScreenOrientationType.Portrait;

            if (layouts == null) return;

            bool isLandscape = orientation == ScreenOrientationType.Landscape;

            foreach (var layout in layouts)
            {
                if (layout?.Target == null) continue;

                // 设置激活状态
                bool active = isLandscape ? layout.LandscapeActive : layout.PortraitActive;
                layout.Target.SetActive(active);

                if (!active) continue;

                if (layout.IsUI)
                {
                    // UI 物体使用 RectTransform
                    var rectTransform = layout.Target.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        // 先设置锚点（如果启用了锚点切换）
                        if (layout.SwitchAnchors)
                        {
                            rectTransform.anchorMin = isLandscape
                                ? layout.LandscapeAnchorMin
                                : layout.PortraitAnchorMin;
                            rectTransform.anchorMax = isLandscape
                                ? layout.LandscapeAnchorMax
                                : layout.PortraitAnchorMax;
                            rectTransform.pivot = isLandscape
                                ? layout.LandscapePivot
                                : layout.PortraitPivot;
                        }

                        rectTransform.anchoredPosition = isLandscape
                            ? layout.LandscapeAnchoredPosition
                            : layout.PortraitAnchoredPosition;

                        if (layout.LandscapeSizeDelta != Vector2.zero || layout.PortraitSizeDelta != Vector2.zero)
                        {
                            rectTransform.sizeDelta = isLandscape
                                ? layout.LandscapeSizeDelta
                                : layout.PortraitSizeDelta;
                        }

                        rectTransform.localScale = isLandscape
                            ? layout.LandscapeScale
                            : layout.PortraitScale;
                    }
                }
                else
                {
                    // 普通物体使用 Transform
                    layout.Target.transform.localPosition = isLandscape
                        ? layout.LandscapePosition
                        : layout.PortraitPosition;

                    layout.Target.transform.localScale = isLandscape
                        ? layout.LandscapeScale
                        : layout.PortraitScale;
                }
            }
        }

        /// <summary>
        /// 强制将当前类标记为Dirty，使编辑器能够保存更改
        /// </summary>
        private void MarkDirty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }

        /// <summary>
        /// 从当前状态记录横屏布局
        /// </summary>
        public void RecordAsLandscape()
        {
            if (layouts == null) return;

            // 为了在编辑器中支持Undo，并且能在嵌套数组中生效，必须触发序列化修改
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Record Landscape Layout");
#endif

            foreach (var layout in layouts)
            {
                if (layout?.Target == null) continue;

                layout.LandscapeActive = layout.Target.activeSelf;
                layout.LandscapeScale = layout.Target.transform.localScale;

                if (layout.IsUI)
                {
                    var rect = layout.Target.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        layout.LandscapeAnchoredPosition = rect.anchoredPosition;
                        layout.LandscapeSizeDelta = rect.sizeDelta;

                        // 记录锚点信息
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

        /// <summary>
        /// 从当前状态记录竖屏布局
        /// </summary>
        public void RecordAsPortrait()
        {
            if (layouts == null) return;

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Record Portrait Layout");
#endif

            foreach (var layout in layouts)
            {
                if (layout?.Target == null) continue;

                layout.PortraitActive = layout.Target.activeSelf;
                layout.PortraitScale = layout.Target.transform.localScale;

                if (layout.IsUI)
                {
                    var rect = layout.Target.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        layout.PortraitAnchoredPosition = rect.anchoredPosition;
                        layout.PortraitSizeDelta = rect.sizeDelta;

                        // 记录锚点信息
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

        /// <summary>
        /// 预览横屏布局
        /// </summary>
        public void PreviewLandscape()
        {
            ApplyOrientation(ScreenOrientationType.Landscape);
        }

        /// <summary>
        /// 预览竖屏布局
        /// </summary>
        public void PreviewPortrait()
        {
            ApplyOrientation(ScreenOrientationType.Portrait);
        }
    }
}
