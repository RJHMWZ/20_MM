using System.Collections;
using UnityEngine;

namespace Match3
{
	public class AudioManager : MonoBehaviour
	{
		public enum SoundPriority
		{
			None,
			NormalMatch,
			SpecialGenerate
		}

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

		private SoundPriority currentFramePriority = SoundPriority.None;

		private AudioClip clipToPlayThisFrame = null;

		public static AudioManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
				if (bgmSource == null)
				{
					bgmSource = base.gameObject.AddComponent<AudioSource>();
				}
				if (sfxContinuousSource == null)
				{
					sfxContinuousSource = base.gameObject.AddComponent<AudioSource>();
				}
				if (sfxPrioritySource == null)
				{
					sfxPrioritySource = base.gameObject.AddComponent<AudioSource>();
				}
			}
			else
			{
				Object.Destroy(base.gameObject);
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

		public void PlayBGM(AudioClip clip)
		{
			if (!(bgmSource == null) && !(clip == null))
			{
				bgmSource.clip = clip;
				bgmSource.loop = true;
				bgmSource.Play();
			}
		}

		public void StopBGM()
		{
			if (bgmSource != null)
			{
				bgmSource.Stop();
			}
		}

		public void PlayVictorySound()
		{
			if (!(victoryClip == null) && !(sfxContinuousSource == null))
			{
				StartCoroutine(PlaySoundWithDelay(victoryClip, 0.8f, true));
			}
		}

		public void PlayDefeatSound()
		{
			if (!(defeatClip == null) && !(sfxContinuousSource == null))
			{
				StartCoroutine(PlaySoundWithDelay(defeatClip, 0.8f, true));
			}
		}

		public void PlaySettlementSound()
		{
			if (!(settlementClip == null) && !(sfxContinuousSource == null))
			{
				sfxContinuousSource.PlayOneShot(settlementClip);
			}
		}

		private IEnumerator PlaySoundWithDelay(AudioClip clip, float delay, bool stopBgm)
		{
			yield return new WaitForSeconds(delay);
			if (stopBgm)
			{
				StopBGM();
			}
			if (sfxContinuousSource != null)
			{
				sfxContinuousSource.PlayOneShot(clip);
			}
		}

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
			if (!(clip == null) && !(sfxContinuousSource == null))
			{
				sfxContinuousSource.PlayOneShot(clip);
			}
		}

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
				PlayContinuous(clipToPlay);
			}
		}

		public void PlayBombBombMergeSound()
		{
			PlayContinuousSound(bombBombMergeClip);
		}

		public void PlayMagicBallSwapSound()
		{
			PlayContinuousSound(magicBallSwapClip);
		}

		public void PlayMagicBallGenerateSound()
		{
			PlayContinuousSound(magicBallGenerateClip);
		}

		private void RequestPrioritySound(AudioClip clip, SoundPriority priority)
		{
			if (!(clip == null) && priority >= currentFramePriority)
			{
				currentFramePriority = priority;
				clipToPlayThisFrame = clip;
			}
		}
	}
}
