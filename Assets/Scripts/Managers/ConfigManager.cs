using UnityEngine;
using Luna.Unity;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

namespace Manager
{
    public enum AdTriggerType
    {
        InvisibleOverlay = 0,
        DownloadPanel = 1
    }

    public enum LevelTransitionType
    {
        Popup = 0,
        Fade = 1
    }

    public class ConfigManager : MonoBehaviour
    {
        private static ConfigManager instance;

        public static ConfigManager Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                // 在场景中查找
                instance = (ConfigManager)FindObjectOfType(typeof(ConfigManager));

                if (FindObjectsOfType(typeof(ConfigManager)).Length > 1)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarning("[Singleton] " + typeof(ConfigManager).Name +
                                         " should never be more than 1 in scene!");
                    }

                    return instance;
                }

                if (instance != null)
                {
                    return instance;
                }

                // 如果场景中没有则创建一个新实例
                var gameObject = new GameObject(typeof(ConfigManager).Name);
                DontDestroyOnLoad(gameObject);
                instance = gameObject.AddComponent<ConfigManager>();
                return instance;
            }
        }

        [LunaPlaygroundAsset("Logo", 1, "Game Settings")]
        public Texture2D LogPic;

        [LunaPlaygroundAsset("Icon", 2, "Game Settings")]
        public Texture2D IconPic;

        [LunaPlaygroundAsset("背景", 3, "Game Settings")]
        public Texture2D BGPic;
        
        #region 音频配置
        [Header("音频配置")]
        [LunaPlaygroundAsset("背景音乐", 1, "Audio Settings")]
        [Tooltip("游戏背景音乐")]
        public AudioClip BackgroundMusic;
        #endregion
        
        #region 棋盘背景图配置
        [Header("棋盘背景图配置")]
        [Tooltip("是否使用配置的背景图替换棋盘默认背景")]
        public bool useCustomGridBackground = true;
        
        [LunaPlaygroundAsset("棋盘背景", 1, "Board Settings")]
        [Tooltip("棋盘背景图（将替换所有棋盘的背景，自动适配棋盘大小）")]
        public Texture2D GridBackgroundPic;
        #endregion

        #region 广告触发配置
        [Header("广告触发配置")]
        [LunaPlaygroundField("广告触发步数", 1, "Ad Settings")]
        [Tooltip("触发全局广告的具体关卡与步数（格式：关卡_步数）。例如'1_3'代表第1关的第3步触发，'2_1'代表第2关的第1步触发。为空表示不触发。")]
        public string adTriggerStep = "";
        
        [LunaPlaygroundField("广告触发类型", 2, "Ad Settings")]
        [Tooltip("触发广告时的表现形式。InvisibleOverlay：全屏不可见遮罩，点击跳转；DownloadPanel：弹出指定的下载UI面板。")]
        public AdTriggerType adTriggerType = AdTriggerType.InvisibleOverlay;
        #endregion

        #region 关卡切换配置
        [Header("关卡切换配置")]
        [LunaPlaygroundField("下一关切换方式", 1, "Level Settings")]
        [Tooltip("通关后进入下一关的过渡方式。Popup: 弹出NextLevelUI面板; Fade: 无弹窗淡入淡出。")]
        public LevelTransitionType levelTransitionType = LevelTransitionType.Popup;
        #endregion

    }
}