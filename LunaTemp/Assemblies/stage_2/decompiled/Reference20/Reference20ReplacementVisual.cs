using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	public sealed class Reference20ReplacementVisual : MonoBehaviour
	{
		private SkeletonGraphic rig;

		private RectTransform contents;

		private int feedLength;

		private bool feeding;

		private float feedEnd;

		private readonly Queue<int> feeds = new Queue<int>();

		private readonly List<RectTransform> payloads = new List<RectTransform>();

		private static Material graphicMaterial;

		private static Sprite leafSprite;

		private static readonly Dictionary<string, Sprite> cabinetSprites = new Dictionary<string, Sprite>();

		public int Kind { get; private set; }

		public float MotionRemaining => Mathf.Max(0f, feedEnd - Time.time) + (float)feeds.Count * 0.55f;

		public static bool Supports(int id)
		{
			return id == 100016 || id == 100042 || id == 100059 || id == 100060 || id == 3 || id == 4;
		}

		private static RectTransform Rect(string name, Transform parent)
		{
			RectTransform r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
			r.SetParent(parent, false);
			Vector2 vector3 = (r.anchorMin = (r.anchorMax = Vector2.one * 0.5f));
			r.pivot = Vector2.one * 0.5f;
			return r;
		}

		private static Image Picture(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size)
		{
			RectTransform r = Rect(name, parent);
			r.anchoredPosition = position;
			r.sizeDelta = size;
			Image image = r.gameObject.AddComponent<Image>();
			image.sprite = sprite;
			image.raycastTarget = false;
			return image;
		}

		private static Sprite CabinetSprite(string name)
		{
			if (!cabinetSprites.TryGetValue(name, out var sprite))
			{
				sprite = Resources.Load<Sprite>("Reference20/Skins/Cabinet/" + name);
				cabinetSprites[name] = sprite;
			}
			return sprite;
		}

		public static void Prewarm()
		{
			CabinetSprite("elements_sp_chugui_55");
			CabinetSprite("elements_sp_chugui_2");
		}

		private static SkeletonGraphic Spine(string name, Transform parent, string skin)
		{
			RectTransform r = Rect(name, parent);
			r.sizeDelta = Vector2.zero;
			Reference20SkeletonGraphic sg = r.gameObject.AddComponent<Reference20SkeletonGraphic>();
			sg.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Reference20/Skins/" + name + "_SkeletonData");
			if (!graphicMaterial)
			{
				graphicMaterial = Resources.Load<Material>("Reference20/Native/SpineGraphic");
			}
			sg.material = graphicMaterial;
			sg.raycastTarget = false;
			sg.Initialize(true);
			sg.Skeleton.SetSkin(skin);
			sg.Skeleton.SetSlotsToSetupPose();
			return sg;
		}

		public static RectTransform Create(int id, string name, Transform parent)
		{
			RectTransform root = Rect(name, parent);
			root.sizeDelta = Vector2.zero;
			if (id == 3 || id == 4)
			{
				Sprite sprite = Reference20Native.Sprite((id == 3) ? "sprite_0" : "sprite_1");
				root.sizeDelta = sprite.rect.size;
				Image image = root.gameObject.AddComponent<Image>();
				image.sprite = sprite;
				image.raycastTarget = false;
				return root;
			}
			Reference20ReplacementVisual visual = root.gameObject.AddComponent<Reference20ReplacementVisual>();
			visual.Kind = id;
			switch (id)
			{
			case 100060:
				visual.rig = Spine("chandelier", root, "default");
				visual.rig.rectTransform.localScale = Vector3.one * 0.9834473f;
				visual.Play("still", false);
				break;
			case 100042:
				visual.rig = Spine("bush", root, "bush_5");
				visual.rig.rectTransform.localScale = Vector3.one * 0.95378894f;
				visual.Play("still", false);
				break;
			case 100059:
			{
				int column = int.Parse(name.Split('_')[1]) - 4;
				visual.feedLength = 9 - column * 2;
				visual.rig = Spine("rockchipper", root, "wood_1");
				visual.rig.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
				visual.rig.rectTransform.localScale = new Vector3(0.96024007f, 64f / 75f, 1f);
				visual.Play("still" + visual.feedLength, false);
				break;
			}
			case 100016:
			{
				visual.contents = Rect("Cabinet", root);
				visual.contents.localScale = Vector3.one * 0.95522386f;
				Sprite side = CabinetSprite("elements_sp_chugui_55");
				Picture("LeftFrame", visual.contents, side, new Vector2(-67f, 0f), new Vector2(134f, 270f));
				Image right = Picture("RightFrame", visual.contents, side, new Vector2(67f, 0f), new Vector2(134f, 270f));
				right.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
				int[] array = new int[2] { -1, 1 };
				foreach (int x in array)
				{
					int[] array2 = new int[2] { 1, -1 };
					foreach (int y in array2)
					{
						RectTransform group = CanStack(visual.contents);
						group.anchoredPosition = new Vector2(x * 55, y * 53 - 5);
						visual.payloads.Add(group);
					}
				}
				break;
			}
			}
			return root;
		}

		private static RectTransform CanStack(Transform parent)
		{
			RectTransform root = Rect("CanStack", parent);
			root.sizeDelta = new Vector2(81f, 113f);
			Sprite sprite = CabinetSprite("elements_sp_chugui_2");
			Picture("CanTop", root, sprite, new Vector2(0f, 25f), new Vector2(81f, 63f));
			Picture("CanBottom", root, sprite, new Vector2(0f, -25f), new Vector2(81f, 63f));
			return root;
		}

		public static RectTransform CreateCan(Transform stage)
		{
			RectTransform root = CanStack(stage);
			root.gameObject.AddComponent<Reference20ReplacementVisual>().Kind = 100015;
			return root;
		}

		public void Configure(int hp)
		{
			if (Kind == 100042)
			{
				SetBushSkin(hp);
			}
		}

		private void SetBushSkin(int hp)
		{
			int num = ((hp >= 3) ? 5 : ((hp == 2) ? 4 : 3));
			string skin = "bush_" + num;
			rig.Skeleton.SetSkin(skin);
			rig.Skeleton.SetSlotsToSetupPose();
			Play("still", false);
		}

		private void Play(string name, bool loop)
		{
			if ((bool)rig && rig.Skeleton.Data.FindAnimation(name) != null)
			{
				rig.AnimationState.SetAnimation(0, name, loop);
			}
		}

		public float DamageDuration(int before, int after)
		{
			return (Kind == 100042) ? (5f / 6f) : ((Kind == 100016) ? 0.3f : 0.4f);
		}

		public void Damage(int before, int after)
		{
			if (Kind == 100042)
			{
				RectTransform stage = base.transform.parent as RectTransform;
				Reference20Fragments.Burst(stage, ((RectTransform)base.transform).anchoredPosition, "FX_Grass_sui01", 16);
				if (before == 3)
				{
					Reference20Fragments.Burst(stage, ((RectTransform)base.transform).anchoredPosition, "FX_Bush_01", 7, 0.65f);
				}
				if (after == 0)
				{
					Reference20Fragments.Burst(stage, ((RectTransform)base.transform).anchoredPosition, "FX_Bush_guizi_01", 10);
				}
				SetBushSkin(before);
				TrackEntry entry = rig.AnimationState.SetAnimation(0, "click", false);
				entry.Complete += delegate(TrackEntry finished)
				{
					if ((bool)this && (bool)rig && after > 0 && rig.AnimationState.GetCurrent(0) == finished)
					{
						SetBushSkin(after);
					}
				};
				return;
			}
			if (Kind == 100016)
			{
				if ((bool)contents)
				{
					contents.DOKill();
					contents.DOShakeAnchorPos(0.45f, new Vector2(7f, 3f), 14, 70f);
				}
				{
					foreach (RectTransform p in payloads)
					{
						if ((bool)p)
						{
							p.DOKill();
							p.DOPunchRotation(new Vector3(0f, 0f, 7f), 0.5f, 8, 0.5f);
						}
					}
					return;
				}
			}
			if (Kind == 100015)
			{
				base.transform.DOPunchScale(Vector3.one * 0.13f, 0.3f, 7, 0.5f);
			}
		}

		public RectTransform DetachPayload(RectTransform stage)
		{
			payloads.RemoveAll((RectTransform p) => !p);
			if (payloads.Count == 0)
			{
				return null;
			}
			int index = Random.Range(0, payloads.Count);
			RectTransform p2 = payloads[index];
			payloads.RemoveAt(index);
			p2.DOKill();
			Reference20Native.Raise(p2, stage);
			p2.localRotation = Quaternion.identity;
			return p2;
		}

		public void Feed()
		{
			if (feedLength > 0)
			{
				feeds.Enqueue(feedLength--);
				if (!feeding && Application.isPlaying)
				{
					StartCoroutine(FeedLoop());
				}
			}
		}

		private IEnumerator FeedLoop()
		{
			feeding = true;
			while (feeds.Count > 0)
			{
				int count = feeds.Dequeue();
				rig.timeScale = 0.9090909f;
				Play("strike_" + count, false);
				feedEnd = Time.time + 0.55f;
				Reference20Fragments.Burst(base.transform.parent as RectTransform, ((RectTransform)base.transform).anchoredPosition + Vector2.up * 55f, "FX_RockChipper_suishi", 8, 0.75f);
				if (count == 1)
				{
					Reference20Fragments.Burst(base.transform.parent as RectTransform, ((RectTransform)base.transform).anchoredPosition, "FX_RockChipper_suikuai", 12, 0.8f);
				}
				yield return new WaitForSeconds(0.55f);
				if (count > 1)
				{
					rig.timeScale = 1f;
					Play("still" + (count - 1), false);
				}
			}
			feeding = false;
			feedEnd = Time.time;
		}

		public void BearFalling()
		{
			if (Kind == 100060)
			{
				Play("falling_d", false);
			}
		}

		public void BearLanded()
		{
			if (Kind == 100060)
			{
				Play("dropped_d", false);
				rig.AnimationState.AddAnimation(0, "still", false, 0f);
			}
		}

		public void BearArrived()
		{
			if (Kind == 100060)
			{
				Play("arrived", false);
			}
		}

		public void BearFlying()
		{
			if (Kind == 100060)
			{
				Play("move3_u", false);
				rig.AnimationState.AddAnimation(0, "falling_u", false, 0f);
			}
		}

		public void Collected()
		{
			if (Kind == 100015)
			{
				base.transform.DORotate(new Vector3(0f, 0f, 360f), 1.2f, DG.Tweening.RotateMode.FastBeyond360).SetEase(Ease.InQuad);
			}
		}

		public void BreakCabinet(RectTransform stage)
		{
			if (Kind == 100016)
			{
				Reference20Fragments.Burst(stage, ((RectTransform)base.transform).anchoredPosition, "FX_guizi_01", 18, 0.8f);
			}
		}

		public static RectTransform Leaf(RectTransform parent)
		{
			RectTransform root = Rect("BushLeaf", parent);
			root.sizeDelta = new Vector2(62f, 56f);
			if (!leafSprite)
			{
				Texture2D atlas = Resources.Load<Texture2D>("Reference20/Skins/bush");
				leafSprite = Sprite.Create(atlas, new Rect(1689f, atlas.height - 227 - 56, 62f, 56f), new Vector2(0.5f, 0.5f), 100f);
			}
			Image image = root.gameObject.AddComponent<Image>();
			image.sprite = leafSprite;
			image.raycastTarget = false;
			root.localScale = Vector3.one * 0.8875f;
			return root;
		}
	}
}
