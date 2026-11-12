using UnityEngine;
using System.Collections.Generic;

namespace Match3
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [Tooltip("Source for background music")]
        public AudioSource bgmSource;
        [Tooltip("Source for sounds that can overlap/play multiple times quickly")]
        public AudioSource sfxContinuousSource;
        [Tooltip("Source for priority based sounds (only the highest priority plays per frame)")]
        public AudioSource sfxPrioritySource;

        [Header("BGM")]
        public AudioClip bgmClip;

        [Header("Game State Sounds")]
        public AudioClip victoryClip;
        public AudioClip defeatClip;
        [Tooltip("结算界面弹出时的音效")]
        public AudioClip settlementClip;

        [Header("Continuous Sounds (Always Play)")]
        [Tooltip("交换时的声音")]
        public AudioClip swapClip;
        [Tooltip("收集到元素的声音")]
        public AudioClip collectClip;
        [Tooltip("长条物体后退的声音")]
        public AudioClip longObjectBackClip;

        [Header("Priority Sounds")]
        [Tooltip("普通元素消除时的声音 (Priority: Low)")]
        public AudioClip normalMatchClip;
        [Tooltip("特殊元素生成的声音(统一) (Priority: Medium)")]
        public AudioClip specialGenerateClip;

        [Header("Special Trigger Sounds (Priority: High)")]
        public AudioClip lineClearHorizontalClip;
        public AudioClip lineClearVerticalClip;
        public AudioClip bombExplosionClip;
        public AudioClip targetSeekerClip;
        public AudioClip colorClearClip;

        [Header("Special Combo Sounds (组合交换音效)")]
        [Tooltip("炸弹+炸弹交换时的合并爆炸音效（交换动画一开始就播放，与动画同步）")]
        public AudioClip bombBombMergeClip;
        [Tooltip("魔力球+魔力球交换时（两球在棋盘中心相撞瞬间）的交换音效")]
        public AudioClip magicBallSwapClip;
        [Tooltip("魔力球+特效交换时，普通元素变身成特效（生成特效）的音效")]
        public AudioClip magicBallGenerateClip;

        public enum SoundPriority
        {
            None = 0,
            NormalMatch = 1,      // 最低优先级：普通消除
            SpecialGenerate = 2   // 中等优先级：生成特殊元素
            // 特殊元素触发不再参与帧级优先级过滤，改为直接播放
        }

        private SoundPriority currentFramePriority = SoundPriority.None;
        private AudioClip clipToPlayThisFrame = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // 如果没有手动分配AudioSource，则自动添加
                if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
                if (sfxContinuousSource == null) sfxContinuousSource = gameObject.AddComponent<AudioSource>();
                if (sfxPrioritySource == null) sfxPrioritySource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (bgmClip != null)
            {
                PlayBGM(bgmClip);
            }
        }

        private void LateUpdate()
        {
            // 在一帧结束时，只播放当前帧收集到的最高优先级的音效
            if (clipToPlayThisFrame != null)
            {
                if (sfxPrioritySource != null)
                {
                    sfxPrioritySource.PlayOneShot(clipToPlayThisFrame);
                }
                clipToPlayThisFrame = null;
                currentFramePriority = SoundPriority.None;
            }
        }

        #region BGM Control
        
        public void PlayBGM(AudioClip clip)
        {
            if (bgmSource == null || clip == null) return;
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        public void StopBGM()
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }
        }
        
        #endregion

        #region Game State Sounds

        public void PlayVictorySound()
        {
            if (victoryClip == null || sfxContinuousSource == null) return;
            StartCoroutine(PlaySoundWithDelay(victoryClip, 0.8f, true));
        }

        public void PlayDefeatSound()
        {
            if (defeatClip == null || sfxContinuousSource == null) return;
            StartCoroutine(PlaySoundWithDelay(defeatClip, 0.8f, true));
        }

        /// <summary>
        /// 结算界面（胜利/失败/下一关面板）弹出时播放。
        /// 与胜负判定音效独立，用于面板弹出的“叮”一声。
        /// </summary>
        public void PlaySettlementSound()
        {
            if (settlementClip == null || sfxContinuousSource == null) return;
            sfxContinuousSource.PlayOneShot(settlementClip);
        }

        private System.Collections.IEnumerator PlaySoundWithDelay(AudioClip clip, float delay, bool stopBgm)
        {
            yield return new WaitForSeconds(delay);
            if (stopBgm) StopBGM();
            if (sfxContinuousSource != null) sfxContinuousSource.PlayOneShot(clip);
        }

        #endregion

        #region Continuous Sounds (一直触发)
        
        public void PlaySwapSound()
        {
            PlayContinuousSound(swapClip);
        }

        public void PlayCollectSound()
        {
            PlayContinuousSound(collectClip);
        }

        public void PlayLongObjectBackSound()
        {
            PlayContinuousSound(longObjectBackClip);
        }

        public void PlayObstacleDamageSound(AudioClip clip)
        {
            PlayContinuousSound(clip);
        }

        public void PlayObstacleDestroySound(AudioClip clip)
        {
            PlayContinuousSound(clip);
        }

        public void PlayGeneratorSound(AudioClip clip)
        {
            PlayContinuousSound(clip);
        }

        public void PlayContinuousSound(AudioClip clip)
        {
            if (clip != null && sfxContinuousSource != null)
            {
                sfxContinuousSource.PlayOneShot(clip);
            }
        }

        private void PlayContinuous(AudioClip clip)
        {
            if (clip == null || sfxContinuousSource == null) return;
            sfxContinuousSource.PlayOneShot(clip);
        }
        
        #endregion

        #region Priority Sounds (按优先级顺序触发)

        public void PlayNormalMatchSound()
        {
            RequestPrioritySound(normalMatchClip, SoundPriority.NormalMatch);
        }

        public void PlaySpecialGenerateSound()
        {
            RequestPrioritySound(specialGenerateClip, SoundPriority.SpecialGenerate);
        }

        public void PlaySpecialTriggerSound(SpecialBehaviorType type)
        {
            AudioClip clipToPlay = null;
            switch (type)
            {
                case SpecialBehaviorType.LineClearHorizontal:
                    clipToPlay = lineClearHorizontalClip;
                    break;
                case SpecialBehaviorType.LineClearVertical:
                    clipToPlay = lineClearVerticalClip;
                    break;
                case SpecialBehaviorType.BombExplosion:
                    clipToPlay = bombExplosionClip;
                    break;
                case SpecialBehaviorType.TargetSeeker:
                    clipToPlay = targetSeekerClip;
                    break;
                case SpecialBehaviorType.ColorClear:
                    clipToPlay = colorClearClip;
                    break;
            }

            if (clipToPlay != null)
            {
                // 特效触发声音改为“一直触发”，允许重叠播放
                PlayContinuous(clipToPlay);
            }
        }

        /// <summary>
        /// 炸弹+炸弹交换时的合并爆炸音效。
        /// 由 BoardManager 在交换动画开始（交换表演刚启动）时调用，与动画同步。
        /// </summary>
        public void PlayBombBombMergeSound()
        {
            PlayContinuousSound(bombBombMergeClip);
        }

        /// <summary>
        /// 魔力球+魔力球交换时的交换音效。
        /// 由 BoardManager 在两个魔力球飞到棋盘中心相撞的瞬间调用。
        /// </summary>
        public void PlayMagicBallSwapSound()
        {
            PlayContinuousSound(magicBallSwapClip);
        }

        /// <summary>
        /// 魔力球+特效交换时生成特效的音效。
        /// 由 BoardManager 在首个普通元素变身成特效的瞬间调用（每波只播一次）。
        /// </summary>
        public void PlayMagicBallGenerateSound()
        {
            PlayContinuousSound(magicBallGenerateClip);
        }

        private void RequestPrioritySound(AudioClip clip, SoundPriority priority)
        {
            if (clip == null) return;

            // 如果请求的优先级大于等于当前记录的最高优先级，则替换本帧要播放的音效
            if (priority >= currentFramePriority)
            {
                currentFramePriority = priority;
                clipToPlayThisFrame = clip;
            }
        }
        
        #endregion
    }
}
