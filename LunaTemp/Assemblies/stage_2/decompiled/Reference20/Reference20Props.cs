using System;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	public static class Reference20Props
	{
		public const float SourceCellSize = 0.52734375f;

		private static readonly Dictionary<string, SkeletonDataAsset> data = new Dictionary<string, SkeletonDataAsset>();

		private static readonly Dictionary<int, Stack<Reference20PropVisual>> visuals = new Dictionary<int, Stack<Reference20PropVisual>>();

		private static readonly Dictionary<string, Stack<Reference20EffectInstance>> effects = new Dictionary<string, Stack<Reference20EffectInstance>>();

		private static readonly List<Reference20PropVisual> hints = new List<Reference20PropVisual>();

		private static Material spineMaterial;

		public static float VisualScale(int id)
		{
			return 1.28f * ((id == 13) ? 0.75f : 0.8f);
		}

		public static float CreationDuration(int id)
		{
			SkeletonDataAsset d = Resources.Load<SkeletonDataAsset>("Reference20/Props/Bomb_SkeletonData");
			return d.GetSkeletonData(true).FindAnimation(Creation(id))?.Duration ?? 0f;
		}

		public static string Creation(int id)
		{
			object result;
			switch (id)
			{
			default:
				result = "Bomb_heng_creat_1";
				break;
			case 12:
				result = "Bomb_shu_creat_1";
				break;
			case 15:
				result = "Boom_color_creat_1";
				break;
			case 14:
				result = "Boom_square_creat";
				break;
			case 13:
				result = "Boom_cross_creat_1";
				break;
			}
			return (string)result;
		}

		public static RectTransform Rect(string name, Transform parent)
		{
			RectTransform r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
			r.SetParent(parent, false);
			Vector2 vector3 = (r.anchorMin = (r.anchorMax = Vector2.one * 0.5f));
			r.sizeDelta = Vector2.zero;
			return r;
		}

		public static SkeletonGraphic Rig(string asset, Transform parent, string skin, string animation)
		{
			if (!data.TryGetValue(asset, out var d))
			{
				d = Resources.Load<SkeletonDataAsset>("Reference20/Props/" + asset + "_SkeletonData");
				data[asset] = d;
			}
			RectTransform r = Rect(asset, parent);
			SkeletonGraphic sg = r.gameObject.AddComponent<SkeletonGraphic>();
			sg.skeletonDataAsset = d;
			if (!spineMaterial)
			{
				spineMaterial = Resources.Load<Material>("Reference20/Native/SpineGraphic");
			}
			sg.material = spineMaterial;
			sg.raycastTarget = false;
			sg.Initialize(true);
			sg.Skeleton.SetSkin(skin);
			sg.Skeleton.SetSlotsToSetupPose();
			if (animation != null)
			{
				sg.AnimationState.SetAnimation(0, animation, false);
			}
			sg.Update(0f);
			sg.LateUpdate();
			return sg;
		}

		public static int Id(string name)
		{
			int result;
			switch (name)
			{
			default:
				result = 0;
				break;
			case "SameColorBlock":
				result = 15;
				break;
			case "SquareBlock":
				result = 14;
				break;
			case "AreaBlock":
				result = 13;
				break;
			case "VerticalLineBlock":
				result = 12;
				break;
			case "LineBlock":
				result = 11;
				break;
			}
			return result;
		}

		private static Reference20PropVisual CreateVisual(int id, Transform parent)
		{
			RectTransform root = Rect("Prop_" + id, parent);
			RectTransform dynamic = Rect("Dynamic", Rect("ArtRoot", Rect("Root", root)));
			Reference20PropVisual v = root.gameObject.AddComponent<Reference20PropVisual>();
			v.Kind = id;
			object skin;
			switch (id)
			{
			default:
				skin = "brick0_heng";
				break;
			case 15:
				skin = "brick_color";
				break;
			case 14:
				skin = "bomb_square";
				break;
			case 13:
				skin = "bomb_cross";
				break;
			}
			v.Rig = Rig("Bomb", dynamic, (string)skin, "Bomb_still");
			return v;
		}

		public static RectTransform Visual(int id, Transform parent, bool born = true)
		{
			if (!visuals.TryGetValue(id, out var pool))
			{
				pool = new Stack<Reference20PropVisual>();
				visuals[id] = pool;
			}
			Reference20PropVisual v = null;
			while (pool.Count > 0 && !v)
			{
				v = pool.Pop();
			}
			if (!v)
			{
				v = CreateVisual(id, parent);
			}
			RectTransform root = v.transform as RectTransform;
			root.SetParent(parent, false);
			root.anchoredPosition = Vector2.zero;
			root.localScale = Vector3.one * VisualScale(id);
			root.localRotation = Quaternion.identity;
			root.Find("Root/ArtRoot/Dynamic").localRotation = Quaternion.identity;
			root.gameObject.SetActive(true);
			v.ResetPose(born);
			return root;
		}

		public static bool ReleaseVisual(Transform child, RectTransform stage)
		{
			Reference20PropVisual v = child.GetComponent<Reference20PropVisual>();
			if (!v)
			{
				return false;
			}
			v.StopHint();
			child.gameObject.SetActive(false);
			child.SetParent(Reference20Native.Overlay(stage), false);
			if (!visuals.TryGetValue(v.Kind, out var pool))
			{
				pool = new Stack<Reference20PropVisual>();
				visuals[v.Kind] = pool;
			}
			pool.Push(v);
			return true;
		}

		public static void Warm(RectTransform stage)
		{
			string[] array = new string[3] { "Bomb", "bomb_bomb", "ColorBomb_ColorBomb" };
			foreach (string asset in array)
			{
				Resources.Load<SkeletonDataAsset>("Reference20/Props/" + asset + "_SkeletonData").GetSkeletonData(true);
			}
			int[] array2 = new int[5] { 11, 12, 13, 14, 15 };
			foreach (int id in array2)
			{
				for (int i = 0; i < 8; i++)
				{
					Reference20PropVisual v = CreateVisual(id, Reference20Native.Overlay(stage));
					ReleaseVisual(v.transform, stage);
				}
			}
			Reference20CompanyParticles.Prewarm(stage);
		}

		public static bool Hint(RectTransform root, string name)
		{
			Reference20PropVisual v = root.GetComponentInChildren<Reference20PropVisual>();
			if (!v)
			{
				return false;
			}
			v.Hint(name);
			if (!hints.Contains(v))
			{
				hints.Add(v);
			}
			return true;
		}

		public static void StopHints()
		{
			foreach (Reference20PropVisual v in hints)
			{
				if ((bool)v)
				{
					v.StopHint();
				}
			}
			hints.Clear();
		}

		public static void Reset()
		{
			visuals.Clear();
			effects.Clear();
			hints.Clear();
		}

		private static RectTransform EffectRoot(string key, RectTransform stage, Vector2 position, float duration, Action<RectTransform> create, Action<RectTransform> play)
		{
			if (!effects.TryGetValue(key, out var pool))
			{
				pool = new Stack<Reference20EffectInstance>();
				effects[key] = pool;
			}
			Reference20EffectInstance instance = null;
			while (pool.Count > 0 && !instance)
			{
				instance = pool.Pop();
			}
			if (!instance)
			{
				RectTransform root = Rect(key, Reference20Native.Overlay(stage));
				create(root);
				instance = root.gameObject.AddComponent<Reference20EffectInstance>();
				instance.Capture();
			}
			else
			{
				instance.Restore();
			}
			RectTransform rt = instance.transform as RectTransform;
			rt.SetParent(Reference20Native.Overlay(stage), false);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.anchoredPosition = position;
			rt.localScale = Vector3.one * 0.8875f;
			rt.localRotation = Quaternion.identity;
			rt.SetAsLastSibling();
			play(rt);
			instance.Lease(duration, delegate(Reference20EffectInstance i)
			{
				pool.Push(i);
			});
			return rt;
		}

		public static Vector2 ColorComboCenter(Vector2 position)
		{
			return new Vector2(540f, 960f);
		}

		public static RectTransform SpineEffect(string key, RectTransform stage, Vector2 position, string asset, string skin, string animation, float life, float speed = 1f)
		{
			return EffectRoot(key, stage, position, life, delegate(RectTransform r)
			{
				Rig(asset, r, skin, null);
			}, delegate(RectTransform r)
			{
				SkeletonGraphic componentInChildren = r.GetComponentInChildren<SkeletonGraphic>(true);
				componentInChildren.timeScale = speed;
				componentInChildren.AnimationState.ClearTracks();
				componentInChildren.Skeleton.SetToSetupPose();
				componentInChildren.Skeleton.SetSkin(skin);
				componentInChildren.Skeleton.SetSlotsToSetupPose();
				componentInChildren.AnimationState.SetAnimation(0, animation, key == "SameColorEffect");
				componentInChildren.Update(0f);
				componentInChildren.LateUpdate();
			});
		}

		public static bool TryEffect(string name, RectTransform stage, Vector2 position, float scale, float life, out RectTransform root)
		{
			root = null;
			switch (name)
			{
			case "AreaAreaEffect":
				root = SpineEffect(name, stage, position, "bomb_bomb", "default", "Boom_bomb_1", 1.7f);
				break;
			case "SameColorChangeEffect":
				root = SpineEffect(name, stage, position, "ColorBomb_ColorBomb", "default", "huixuanzhuan", 2.5f);
				break;
			case "SameColorEffect":
				root = SpineEffect(name, stage, position, "Bomb", "brick_color", "Boom_color_start", (life > 0f) ? life : 3600f);
				break;
			case "AreaEffect":
				root = ParticleHalo(stage, position, false);
				return true;
			default:
				if (!(name == "SquareBigEffect"))
				{
					break;
				}
				goto case "SquareEffect";
			case "SquareEffect":
				root = Reference20CompanyEffects.Play("BeeImpact", stage, position);
				return true;
			}
			if (!root)
			{
				return false;
			}
			root.localScale = Vector3.one * ((name == "SameColorEffect") ? (VisualScale(15) * 0.8875f) : ((name == "SameColorChangeEffect") ? 0.68160003f : 1.136f));
			return true;
		}

		public static void LineBlast(RectTransform stage, Vector2 position, bool horizontal, float duration)
		{
			string skin = (horizontal ? "brick0_heng" : "brick0_shu");
			RectTransform root = EffectRoot(horizontal ? "CompanyLineH" : "CompanyLineV", stage, position, duration, delegate(RectTransform r)
			{
				Rig("Bomb", r, skin, null);
				Rig("Bomb", r, skin, null);
			}, delegate(RectTransform r)
			{
				r.localScale = Vector3.one;
				int num = -1;
				SkeletonGraphic[] componentsInChildren = r.GetComponentsInChildren<SkeletonGraphic>();
				foreach (SkeletonGraphic skeletonGraphic in componentsInChildren)
				{
					skeletonGraphic.Skeleton.SetToSetupPose();
					skeletonGraphic.Skeleton.SetSkin(skin);
					skeletonGraphic.Skeleton.SetSlotsToSetupPose();
					skeletonGraphic.AnimationState.ClearTracks();
					skeletonGraphic.AnimationState.SetAnimation(0, "Bomb_dropped_d", true);
					skeletonGraphic.Update(0f);
					skeletonGraphic.LateUpdate();
					RectTransform rectTransform = skeletonGraphic.rectTransform;
					rectTransform.anchoredPosition = Vector2.zero;
					rectTransform.localRotation = Quaternion.identity;
					rectTransform.localScale = Vector3.one * (VisualScale(11) * 0.8875f);
					Reference20CompanyParticles.MovingTrail(stage, position, (horizontal ? Vector2.right : Vector2.up) * num, duration);
					rectTransform.DOAnchorPos((horizontal ? Vector2.right : Vector2.up) * num * 3231.2888f, duration).SetEase(Ease.Linear);
					num = 1;
				}
			});
		}

		public static RectTransform BeeFlight(RectTransform stage, Vector2 position, float duration, int payload, bool reverse)
		{
			RectTransform root = SpineEffect("HtmlBee" + payload, stage, position, "Bomb", "bomb_square", "Bomb_fly1_1", duration + 0.32f);
			root.localScale = Vector3.one * (VisualScale(14) * 0.8875f);
			if (payload > 0 && !root.Find("Payload"))
			{
				RectTransform companion = Visual(payload, root, false);
				companion.name = "Payload";
				companion.localScale = Vector3.one * (VisualScale(payload) / VisualScale(14));
			}
			return root;
		}

		public static RectTransform ParticleHalo(RectTransform stage, Vector2 position, bool color, float scale = 1f)
		{
			return Reference20CompanyEffects.Play(color ? "ColorFinish" : "Bomb", stage, position, scale);
		}

		public static void ChangeNormal(RectTransform root, int id)
		{
			if (!root.Find("element"))
			{
				Image image = root.GetComponent<Image>();
				if ((bool)image)
				{
					image.enabled = false;
				}
				SkeletonGraphic sg = Rig("element", root, (id == 3) ? "brick2_0" : "brick4_0", "shake");
				sg.rectTransform.localScale = Vector3.one * 1.1115417f;
			}
		}
	}
}
