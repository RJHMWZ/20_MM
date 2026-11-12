using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	public class Reference20Native : MonoBehaviour
	{
		private static NativeGraph graph;

		private static Dictionary<string, SkeletonDataAsset> skeletons = new Dictionary<string, SkeletonDataAsset>();

		private static Material graphicMaterial;

		private static readonly HashSet<int> trimmedSprites = new HashSet<int>();

		private static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

		private static readonly Dictionary<int, Sprite[]> basicFragments = new Dictionary<int, Sprite[]>();

		private static Dictionary<string, float> dataScales;

		private static readonly Dictionary<string, Stack<Reference20EffectInstance>> effectPools = new Dictionary<string, Stack<Reference20EffectInstance>>();

		private static NativeClip[] basicHintClips;

		private static RectTransform overlayRoot;

		private static readonly Dictionary<int, Material> spriteMaterials = new Dictionary<int, Material>();

		private static readonly Dictionary<string, Material> particleMaterials = new Dictionary<string, Material>();

		public int id;

		private int health;

		private int animationRevision;

		private Reference20ReplacementVisual replacement;

		private Coroutine idleRoutine;

		private readonly Dictionary<RectTransform, float> cornTargets = new Dictionary<RectTransform, float>();

		private float cornMotionEnd;

		private Transform cornEffect;

		private Reference20CocosClip cornEffectClip;

		private bool cornEffectActive;

		public static NativeGraph Graph
		{
			get
			{
				if (graph == null)
				{
					graph = JsonConvert.DeserializeObject<NativeGraph>(Resources.Load<TextAsset>("Reference20/Native/graph").text);
				}
				return graph;
			}
		}

		public float CornMotionRemaining => replacement ? replacement.MotionRemaining : Mathf.Max(0f, cornMotionEnd - Time.time);

		private static int Blend(int value)
		{
			switch (value)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 768:
				return 3;
			case 769:
				return 6;
			case 770:
				return 5;
			case 771:
				return 10;
			case 772:
				return 7;
			case 773:
				return 8;
			case 774:
				return 2;
			case 775:
				return 4;
			case 776:
				return 9;
			default:
				return 10;
			}
		}

		private static Material SpriteMaterial(int src, int dst)
		{
			if (src == 770 && dst == 771)
			{
				return null;
			}
			int key = src * 1024 + dst;
			if (spriteMaterials.TryGetValue(key, out var mat))
			{
				return mat;
			}
			mat = new Material(Resources.Load<Shader>("Reference20/Native/CocosSprite"));
			mat.name = "CocosSprite_" + src + "_" + dst;
			mat.SetFloat("_SrcBlend", Blend(src));
			mat.SetFloat("_DstBlend", Blend(dst));
			spriteMaterials[key] = mat;
			return mat;
		}

		private static Material ParticleMaterial(CocosParticle3DData data)
		{
			float[] tint = data.tint;
			string key = data.src + "_" + data.dst + "_" + ((tint == null) ? "white" : string.Join(",", tint));
			if (particleMaterials.TryGetValue(key, out var material))
			{
				return material;
			}
			material = new Material(Resources.Load<Shader>("Reference20/Native/CocosSprite"));
			material.name = "CocosParticle_" + key;
			material.SetFloat("_SrcBlend", Blend(data.src));
			material.SetFloat("_DstBlend", Blend(data.dst));
			material.SetColor("_Color", (tint != null && tint.Length == 4) ? new Color(tint[0] * 2f, tint[1] * 2f, tint[2] * 2f, tint[3] * 2f) : Color.white);
			particleMaterials[key] = material;
			return material;
		}

		public static Reference20CocosClip Hint(RectTransform root, string name)
		{
			if (Reference20Props.Hint(root, name))
			{
				return null;
			}
			Reference20CocosClip[] componentsInChildren = root.GetComponentsInChildren<Reference20CocosClip>(true);
			foreach (Reference20CocosClip clip in componentsInChildren)
			{
				if (clip.Play(name))
				{
					return clip;
				}
			}
			Image image = root.GetComponent<Image>();
			if (!image || !image.sprite)
			{
				return null;
			}
			if (basicHintClips == null)
			{
				basicHintClips = Graph.basicHintClips;
			}
			RectTransform art = new GameObject("Presentation", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
			art.SetParent(root, false);
			art.sizeDelta = root.sizeDelta;
			Image graphic = art.GetComponent<Image>();
			graphic.sprite = image.sprite;
			graphic.color = image.color;
			graphic.raycastTarget = false;
			image.enabled = false;
			Reference20CocosClip animation = art.gameObject.AddComponent<Reference20CocosClip>();
			animation.clips = basicHintClips;
			animation.Play(name);
			return animation;
		}

		public static float CocosScale(string name)
		{
			if (dataScales == null)
			{
				TextAsset asset = Resources.Load<TextAsset>("Reference20/Native/spine-scales");
				dataScales = (asset ? JsonConvert.DeserializeObject<Dictionary<string, float>>(asset.text) : new Dictionary<string, float>());
			}
			float scale;
			return dataScales.TryGetValue(name, out scale) ? scale : 1f;
		}

		internal static Sprite Sprite(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			if (!sprites.TryGetValue(name, out var result))
			{
				result = Resources.Load<Sprite>((name == "sprite_0") ? "Reference20/Company/Elements/brick2_0" : ((name == "sprite_1") ? "Reference20/Company/Elements/brick4_0" : ("Reference20/Native/" + name)));
				sprites[name] = result;
			}
			return result;
		}

		public static void Prewarm()
		{
			Reference20ReplacementVisual.Prewarm();
			Reference20Fragments.Prewarm();
		}

		public static void WarmEffects(RectTransform stage)
		{
		}

		public static RectTransform Overlay(RectTransform stage)
		{
			if ((bool)overlayRoot && overlayRoot.parent == stage && overlayRoot.gameObject.activeSelf)
			{
				return overlayRoot;
			}
			for (int i = stage.childCount - 1; i >= 0; i--)
			{
				RectTransform child = stage.GetChild(i) as RectTransform;
				if ((bool)child && child.name == "TransientEffects" && child.gameObject.activeSelf)
				{
					overlayRoot = child;
					return child;
				}
			}
			RectTransform rt = new GameObject("TransientEffects", typeof(RectTransform), typeof(Canvas)).GetComponent<RectTransform>();
			rt.SetParent(stage, false);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.pivot = Vector2.zero;
			rt.sizeDelta = Vector2.zero;
			Canvas canvas = rt.GetComponent<Canvas>();
			canvas.overrideSorting = true;
			canvas.sortingOrder = 200;
			overlayRoot = rt;
			return rt;
		}

		private static RectTransform CollectionLayer(RectTransform stage)
		{
			for (int i = stage.childCount - 1; i >= 0; i--)
			{
				RectTransform child = stage.GetChild(i) as RectTransform;
				if ((bool)child && child.name == "CollectedBirds" && child.gameObject.activeSelf)
				{
					return child;
				}
			}
			RectTransform rt = new GameObject("CollectedBirds", typeof(RectTransform), typeof(Canvas)).GetComponent<RectTransform>();
			rt.SetParent(stage, false);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.pivot = Vector2.zero;
			rt.sizeDelta = Vector2.zero;
			Canvas canvas = rt.GetComponent<Canvas>();
			canvas.overrideSorting = true;
			canvas.sortingOrder = 300;
			return rt;
		}

		public static void Raise(RectTransform rt, RectTransform stage)
		{
			Vector3 position = rt.position;
			rt.SetParent(CollectionLayer(stage), true);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.anchoredPosition = (Vector2)stage.InverseTransformPoint(position) + stage.sizeDelta * 0.5f;
			Canvas canvas = rt.GetComponent<Canvas>();
			if ((bool)canvas)
			{
				canvas.overrideSorting = false;
			}
			rt.SetAsLastSibling();
		}

		public static void Fly(RectTransform rt, Vector2 from, Vector2 to, float duration)
		{
			Vector2 controlPoint = new Vector2((from.x + to.x) * 0.5f, Mathf.Max(from.y, to.y) + 177.5f);
			CubicFlight(rt, from, controlPoint, controlPoint, to, duration);
		}

		private static void CubicFlight(RectTransform rt, Vector2 from, Vector2 startHandle, Vector2 endHandle, Vector2 to, float duration)
		{
			rt.DOKill();
			DOTween.To(() => 0f, delegate(float t)
			{
				if ((bool)rt)
				{
					float num = 1f - t;
					rt.anchoredPosition = num * num * num * from + 3f * num * num * t * startHandle + 3f * num * t * t * endHandle + t * t * t * to;
				}
			}, 1f, duration).SetEase(Ease.Linear).SetTarget(rt);
		}

		public static void ResetCaches()
		{
			Reference20Props.Reset();
			Reference20Fragments.Reset();
			Reference20CompanyEffects.Reset();
			trimmedSprites.Clear();
			graph = null;
			overlayRoot = null;
			skeletons.Clear();
			sprites.Clear();
			effectPools.Clear();
		}

		private static RectTransform LeaseEffect(string name, RectTransform stage, Vector2 position, float duration, float scale = 0.8875f)
		{
			if (!Graph.effects.TryGetValue(name, out var node))
			{
				return null;
			}
			if (!effectPools.TryGetValue(name, out var pool))
			{
				pool = new Stack<Reference20EffectInstance>();
				effectPools[name] = pool;
			}
			Reference20EffectInstance instance = null;
			while (pool.Count > 0 && !instance)
			{
				instance = pool.Pop();
			}
			if (!instance)
			{
				RectTransform root = Node(node, Overlay(stage));
				instance = root.gameObject.AddComponent<Reference20EffectInstance>();
				instance.Capture();
			}
			else
			{
				instance.Restore();
			}
			RectTransform rt = instance.transform as RectTransform;
			rt.SetParent(Overlay(stage), false);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.anchoredPosition = position;
			rt.localScale = new Vector3(node.sx, node.sy, 1f) * scale;
			rt.SetAsLastSibling();
			instance.Lease(duration, delegate(Reference20EffectInstance i)
			{
				pool.Push(i);
			});
			return rt;
		}

		public static RectTransform Piece(string name, RectTransform stage)
		{
			NativeNode i = Array.Find(Graph.pieces, (NativeNode x) => x.name == name);
			if (i == null)
			{
				return null;
			}
			int id = int.Parse(name.Substring(name.LastIndexOf('_') + 1));
			RectTransform rt = ((id >= 11 && id <= 15) ? Reference20Props.Rect(name, stage) : (Reference20ReplacementVisual.Supports(id) ? Reference20ReplacementVisual.Create(id, name, stage) : Node(i, stage)));
			if (id >= 11 && id <= 15)
			{
				RectTransform prop = Reference20Props.Visual(id, rt, false);
			}
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.anchoredPosition = new Vector2(540f + (i.x - 1024f) * 0.8875f, 960f + (1088f + i.y) * 0.8875f);
			rt.localScale = new Vector3(i.sx, i.sy, 1f) * 0.8875f;
			if (id >= 100)
			{
				rt.gameObject.AddComponent<Reference20Native>().id = id;
			}
			return rt;
		}

		public static void WarmVisuals(RectTransform stage)
		{
			Reference20Props.Warm(stage);
		}

		public static bool ReleaseVisual(Transform child, RectTransform stage)
		{
			return Reference20Props.ReleaseVisual(child, stage);
		}

		public static RectTransform Visual(string name, Transform parent)
		{
			int id = Reference20Props.Id(name);
			if (id > 0)
			{
				return Reference20Props.Visual(id, parent);
			}
			if (!Graph.effects.TryGetValue(name, out var i))
			{
				return null;
			}
			RectTransform rt = Node(i, parent);
			rt.localScale = new Vector3(i.sx, i.sy, 1f);
			rt.localRotation = Quaternion.Euler(0f, 0f, i.angle);
			rt.anchoredPosition = Vector2.zero;
			rt.gameObject.SetActive(true);
			return rt;
		}

		public static RectTransform GuideHand(RectTransform parent)
		{
			NativeNode i = Graph.guideHand;
			if (i == null)
			{
				return null;
			}
			RectTransform rt = Node(i, parent);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.anchoredPosition = new Vector2(i.x, i.y);
			Reference20CocosClip[] componentsInChildren = rt.GetComponentsInChildren<Reference20CocosClip>(true);
			foreach (Reference20CocosClip clip in componentsInChildren)
			{
				int index = Array.FindIndex(clip.clips, (NativeClip c) => c.name == "common_hand");
				if (index >= 0)
				{
					clip.Play(index);
				}
			}
			return rt;
		}

		public static RectTransform DownloadArtwork(RectTransform parent)
		{
			NativeNode i = Graph.button;
			if (i == null)
			{
				return null;
			}
			RectTransform rt = Node(i, parent);
			Vector2 vector3 = (rt.anchorMin = (rt.anchorMax = new Vector2(0.5f, 0.5f)));
			rt.anchoredPosition = Vector2.zero;
			Reference20CocosClip[] componentsInChildren = rt.GetComponentsInChildren<Reference20CocosClip>(true);
			foreach (Reference20CocosClip clip in componentsInChildren)
			{
				clip.Play(0);
			}
			return rt;
		}

		private static SkeletonDataAsset Skeleton(string name)
		{
			if (skeletons.TryGetValue(name, out var result))
			{
				return result;
			}
			result = Resources.Load<SkeletonDataAsset>("Reference20/Native/" + name + "_data");
			if ((bool)result)
			{
				skeletons[name] = result;
				return result;
			}
			TextAsset text = Resources.Load<TextAsset>("Reference20/Native/" + name + ".atlas");
			List<Material> mats = new List<Material>();
			for (int i = 0; i < 8; i++)
			{
				Texture2D tex = Resources.Load<Texture2D>("Reference20/Native/" + name + "_tex" + i);
				if (tex == null)
				{
					break;
				}
				Material mat = new Material(Shader.Find("Spine/Skeleton"));
				mat.name = name + "_tex" + i;
				mat.mainTexture = tex;
				mats.Add(mat);
			}
			SpineAtlasAsset atlas = SpineAtlasAsset.CreateRuntimeInstance(text, mats.ToArray(), true);
			result = SkeletonDataAsset.CreateRuntimeInstance(Resources.Load<TextAsset>("Reference20/Native/" + name), atlas, true, 0.01f * CocosScale(name));
			skeletons[name] = result;
			return result;
		}

		private static RectTransform Node(NativeNode n, Transform parent)
		{
			GameObject go = new GameObject(n.name, typeof(RectTransform));
			RectTransform rt = go.GetComponent<RectTransform>();
			rt.SetParent(parent, false);
			RectTransform pr = parent as RectTransform;
			Vector2 vector3 = (rt.anchorMin = (rt.anchorMax = (pr ? pr.pivot : new Vector2(0.5f, 0.5f))));
			rt.pivot = new Vector2(n.ax, n.ay);
			rt.sizeDelta = new Vector2(n.w, n.h);
			rt.anchoredPosition = new Vector2(n.x, n.y);
			rt.localScale = new Vector3(n.sx, n.sy, 1f);
			rt.localRotation = Quaternion.Euler(0f, 0f, n.angle);
			if (n.particle != null)
			{
				Reference20ParticleGraphic emitter = go.AddComponent<Reference20ParticleGraphic>();
				emitter.Configure(n.particle, Sprite(n.particle.sprite));
				emitter.material = SpriteMaterial(n.src, n.dst);
			}
			if (n.particle3d != null)
			{
				Reference20Particle3DGraphic emitter2 = go.AddComponent<Reference20Particle3DGraphic>();
				emitter2.Configure(n.particle3d, Sprite(n.particle3d.sprite));
				emitter2.material = ParticleMaterial(n.particle3d);
				rt.localRotation = Quaternion.Euler(n.particle3d.rotation);
			}
			Color color = ((n.color != null && n.color.Length >= 3) ? new Color(n.color[0], n.color[1], n.color[2], 1f) : Color.white);
			if (!string.IsNullOrEmpty(n.sprite) || n.mask)
			{
				Image im = go.AddComponent<Image>();
				im.sprite = Sprite(n.sprite);
				if (n.spriteSizeMode == 1)
				{
					trimmedSprites.Add(im.GetInstanceID());
				}
				if (n.mask && !im.sprite)
				{
					im.sprite = Resources.Load<Sprite>("Reference20/white");
				}
				im.color = color;
				im.raycastTarget = false;
				im.material = SpriteMaterial(n.src, n.dst);
				if (n.mask)
				{
					Mask mask = go.AddComponent<Mask>();
					mask.showMaskGraphic = false;
				}
			}
			if (!string.IsNullOrEmpty(n.skeleton))
			{
				if (!graphicMaterial)
				{
					graphicMaterial = Resources.Load<Material>("Reference20/Native/SpineGraphic");
					if (!graphicMaterial)
					{
						graphicMaterial = new Material(Shader.Find("Spine/SkeletonGraphic"));
						graphicMaterial.EnableKeyword("_STRAIGHT_ALPHA_INPUT");
					}
				}
				SkeletonGraphic sg = go.AddComponent<SkeletonGraphic>();
				sg.skeletonDataAsset = Skeleton(n.skeleton);
				sg.material = graphicMaterial;
				sg.raycastTarget = false;
				sg.Initialize(true);
				sg.color = color;
				if (!string.IsNullOrEmpty(n.skin) && sg.Skeleton.Data.FindSkin(n.skin) != null)
				{
					sg.Skeleton.SetSkin(n.skin);
					sg.Skeleton.SetSlotsToSetupPose();
				}
				if (n.slots != null)
				{
					NativeSlot[] slots = n.slots;
					foreach (NativeSlot state in slots)
					{
						Slot slot = sg.Skeleton.FindSlot(state.name);
						if (slot != null)
						{
							slot.R = state.r;
							slot.G = state.g;
							slot.B = state.b;
							slot.A = state.a;
						}
					}
				}
				if (!string.IsNullOrEmpty(n.anim) && sg.Skeleton.Data.FindAnimation(n.anim) != null)
				{
					sg.AnimationState.SetAnimation(0, n.anim, n.loop || n.anim.Contains("idle") || n.anim.Contains("待机"));
				}
				else if (string.IsNullOrEmpty(n.anim))
				{
					if (sg.Skeleton.Data.FindAnimation("normal") != null)
					{
						sg.AnimationState.SetAnimation(0, "normal", true);
					}
					else if (sg.Skeleton.Data.FindAnimation("idle_1") != null)
					{
						sg.AnimationState.SetAnimation(0, "idle_1", true);
					}
				}
				sg.timeScale = n.timeScale;
			}
			if (n.opacity < 255f)
			{
				CanvasGroup group = go.GetComponent<CanvasGroup>();
				if (!group)
				{
					group = go.AddComponent<CanvasGroup>();
				}
				group.alpha = n.opacity / 255f;
			}
			if (n.mask && n.inverted)
			{
				rt.anchoredPosition += new Vector2(n.w, 0f);
			}
			if (n.children != null)
			{
				NativeNode[] children = n.children;
				foreach (NativeNode c in children)
				{
					RectTransform child = Node(c, rt);
					if (n.mask && n.inverted)
					{
						child.anchoredPosition -= new Vector2(n.w, 0f);
					}
				}
			}
			if (n.clips != null && n.clips.Length != 0)
			{
				Reference20CocosClip animation = go.AddComponent<Reference20CocosClip>();
				animation.clips = n.clips;
				if (n.autoplay && n.clips[0].name.StartsWith("ele_anim_"))
				{
					animation.Play(0);
				}
			}
			go.SetActive(n.active);
			return rt;
		}

		public static RectTransform Effect(string name, RectTransform stage, Vector2 position, float scale = 0.8875f, float lifetime = -1f)
		{
			if (name == "CommonBornPropsEffect")
			{
				return Reference20CompanyEffects.Play("Wave", stage, position, 0.55f);
			}
			if (Reference20Props.TryEffect(name, stage, position, scale, lifetime, out var own))
			{
				return own;
			}
			RectTransform rt = LeaseEffect(name, stage, position, 2f, scale);
			if (!rt)
			{
				return null;
			}
			float duration = ((name == "AreaAreaEffect" || name == "SameColorChangeEffect") ? 2f : ((name == "SquareBigEffect") ? 1.5f : ((name == "SquareEffect" || name == "HenHouseDestroy") ? 1f : 0.6f)));
			Reference20CocosClip[] componentsInChildren = rt.GetComponentsInChildren<Reference20CocosClip>(true);
			foreach (Reference20CocosClip clip in componentsInChildren)
			{
				clip.Play(0);
				duration = Mathf.Max(duration, clip.clips[0].duration);
			}
			rt.GetComponent<Reference20EffectInstance>().Lease((lifetime > 0f) ? lifetime : (duration + 0.02f), delegate(Reference20EffectInstance i)
			{
				effectPools[name].Push(i);
			});
			return rt;
		}

		public static void FinishEffect(RectTransform rt)
		{
			if ((bool)rt)
			{
				Reference20CompanyParticles.StopChildren(rt);
				Reference20EffectInstance instance = rt.GetComponent<Reference20EffectInstance>();
				if ((bool)instance)
				{
					instance.Complete();
				}
			}
		}

		public static void HideEffect(RectTransform rt)
		{
			if ((bool)rt)
			{
				CanvasGroup alpha = rt.GetComponent<CanvasGroup>();
				if (!alpha)
				{
					alpha = rt.gameObject.AddComponent<CanvasGroup>();
				}
				alpha.alpha = 0f;
			}
		}

		private static Sprite[] BasicFragments(int color)
		{
			if (basicFragments.TryGetValue(color, out var result))
			{
				return result;
			}
			Sprite source = Sprite((color == 4) ? "sprite_1" : "sprite_0");
			Rect rect = source.rect;
			result = new Sprite[4];
			for (int i = 0; i < 4; i++)
			{
				Rect part = new Rect(rect.x + (float)(i % 2) * rect.width * 0.5f, rect.y + (float)(i / 2) * rect.height * 0.5f, rect.width * 0.5f, rect.height * 0.5f);
				result[i] = UnityEngine.Sprite.Create(source.texture, part, Vector2.one * 0.5f, source.pixelsPerUnit);
				result[i].name = source.name + "_fragment_" + i;
			}
			basicFragments[color] = result;
			return result;
		}

		public static void BaseBurst(RectTransform stage, Vector2 position, int color)
		{
			Reference20CompanyEffects.Play((color == 4) ? "NormalGold" : "NormalBlue", stage, position);
		}

		public static void ApplySpriteFrame(Image image, Sprite sprite)
		{
			if ((bool)sprite && !(image.sprite == sprite))
			{
				image.sprite = sprite;
				if (trimmedSprites.Contains(image.GetInstanceID()))
				{
					image.SetNativeSize();
				}
			}
		}

		public static void LineBlast(RectTransform stage, Vector2 position, bool horizontal, float duration)
		{
			Reference20Props.LineBlast(stage, position, horizontal, duration);
		}

		public static void ColorBeam(RectTransform stage, Vector2 from, Vector2 to, float delay)
		{
			DOTween.Sequence().SetTarget(stage).AppendInterval(delay)
				.AppendCallback(delegate
				{
					Reference20CompanyEffects.Beam(stage, from, to);
				});
		}

		private static void PlayClips(RectTransform rt, string name)
		{
			Reference20CocosClip[] componentsInChildren = rt.GetComponentsInChildren<Reference20CocosClip>(true);
			foreach (Reference20CocosClip clip in componentsInChildren)
			{
				clip.Play(name);
			}
		}

		private static void PlayFlightSpine(RectTransform rt, string name, bool loop)
		{
			SkeletonGraphic[] componentsInChildren = rt.GetComponentsInChildren<SkeletonGraphic>(true);
			foreach (SkeletonGraphic skeleton in componentsInChildren)
			{
				if (skeleton.Skeleton.Data.FindAnimation(name) != null)
				{
					skeleton.AnimationState.SetAnimation(0, name, loop);
				}
			}
		}

		public static void BeeFlight(RectTransform stage, Vector2 from, Vector2 to, float duration, int payload = 0, bool reverse = false, Vector2? payloadOffset = null)
		{
			bool combo = payload > 0;
			RectTransform rt = Reference20Props.BeeFlight(stage, from, duration, payload, reverse);
			if ((bool)rt)
			{
				float launchDelay = Mathf.Max(0f, duration - 0.5f);
				DOTween.Sequence().SetTarget(rt).AppendInterval(launchDelay)
					.Append(rt.DOAnchorPos(to, 0.5f).SetEase((payload <= 0) ? Ease.Linear : Ease.InQuad));
				RectTransform payloadRoot = rt.Find("Payload") as RectTransform;
				if ((bool)payloadRoot)
				{
					payloadRoot.anchoredPosition = (payloadOffset ?? Vector2.zero) / rt.localScale.x;
					DOTween.Sequence().SetTarget(rt).AppendInterval(launchDelay)
						.Append(payloadRoot.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.InQuad));
				}
				CanvasGroup fade = rt.GetComponent<CanvasGroup>();
				if (!fade)
				{
					fade = rt.gameObject.AddComponent<CanvasGroup>();
				}
				DOTween.Sequence().SetTarget(rt).AppendInterval(duration)
					.Append(rt.DOScale(Vector3.zero, 0.3f))
					.Join(fade.DOFade(0f, 0.3f));
			}
		}

		public void PlayAnimation(string name, bool loop = false)
		{
			SkeletonGraphic[] componentsInChildren = GetComponentsInChildren<SkeletonGraphic>(true);
			foreach (SkeletonGraphic s in componentsInChildren)
			{
				if (s.Skeleton.Data.FindAnimation(name) != null)
				{
					s.gameObject.SetActive(true);
					s.AnimationState.SetAnimation(0, name, loop);
				}
			}
		}

		public void Configure(int type, int hp)
		{
			id = type;
			health = hp;
			animationRevision++;
			replacement = GetComponent<Reference20ReplacementVisual>();
			if ((bool)replacement)
			{
				replacement.Configure(hp);
				return;
			}
			if (id == 100059)
			{
				cornTargets.Clear();
				cornMotionEnd = Time.time;
				cornEffect = base.transform.Find("Root/ArtRoot/Dynamic/headNode/Effect/Root");
				if ((bool)cornEffect)
				{
					cornEffectClip = cornEffect.GetComponentInChildren<Reference20CocosClip>(true);
					if ((bool)cornEffectClip)
					{
						cornEffectClip.Play("ele_anim_bread_effect");
						cornEffectClip.Stop();
					}
					cornEffect.gameObject.SetActive(false);
					cornEffectActive = false;
				}
			}
			ApplyIdlePose();
			if (Application.isPlaying)
			{
				if (idleRoutine != null)
				{
					StopCoroutine(idleRoutine);
				}
				idleRoutine = StartCoroutine(IdleLoop(animationRevision));
			}
		}

		private void ApplyIdlePose()
		{
			string pose = ((id != 100016) ? ((id != 100015) ? ((id != 100042) ? ((id == 100032) ? "左右看" : "idle") : ((health >= 3) ? "动效1-待机" : ((health == 2) ? "动效3-待机" : "动效4-开屏后待机"))) : ((health >= 3) ? "egg_idle" : ((health == 2) ? "egg2_idle" : "idle"))) : ((health >= 3) ? "egg_idle" : "idle"));
			PlayAnimation(pose, true);
		}

		private IEnumerator IdleLoop(int revision)
		{
			int index = 0;
			while (revision == animationRevision && health > 0)
			{
				string animation;
				float duration;
				if (id == 100042)
				{
					animation = ((health >= 3) ? "动效1-待机" : ((health == 2) ? "动效3-待机" : "动效4-开屏后待机"));
					duration = 8f;
				}
				else if (id == 100032)
				{
					animation = ((index % 2 == 0) ? "左右看" : "原地挣扎");
					duration = ((index % 2 == 0) ? 7.0667f : 1.83f);
				}
				else if (id == 100016)
				{
					animation = ((health >= 3) ? "egg_idle" : "idle");
					duration = 3f;
				}
				else if (id == 100015 && health > 1)
				{
					animation = ((health == 3) ? "egg_idle" : "egg2_idle");
					duration = 3f;
				}
				else
				{
					animation = ((index % 3 == 0) ? "idle" : ((index % 3 == 1) ? "idle2" : "idle3"));
					duration = ((index % 3 == 0) ? 2f : 3.2f);
				}
				PlayAnimation(animation);
				yield return new WaitForSeconds(duration);
				index++;
			}
		}

		public void NestFeedback()
		{
			Transform art = base.transform.Find("Root/ArtRoot");
			Reference20CocosClip clip = (art ? art.GetComponent<Reference20CocosClip>() : null);
			if ((bool)clip)
			{
				clip.Play(0);
			}
		}

		public float DamageDuration(int before, int after)
		{
			float result;
			if (!replacement)
			{
				if (id == 100042)
				{
					result = ((after > 0) ? 1f : 0.33f);
				}
				else if (id == 100016)
				{
					result = ((before == 3) ? 1.7f : 0.25f);
				}
				else if (id == 100032)
				{
					result = ((before % 2 == 0) ? 0.8f : 0.867f);
				}
				else
				{
					switch (before)
					{
					default:
						result = 0.4f;
						break;
					case 2:
						result = 0.5f;
						break;
					case 3:
						result = 0.8f;
						break;
					}
				}
			}
			else
			{
				result = replacement.DamageDuration(before, after);
			}
			return result;
		}

		public float AnimateDamage(int before, int after)
		{
			health = after;
			if ((bool)replacement)
			{
				replacement.Damage(before, after);
				return replacement.DamageDuration(before, after);
			}
			if (id == 100016)
			{
				NestFeedback();
			}
			int revision = ++animationRevision;
			if (idleRoutine != null)
			{
				StopCoroutine(idleRoutine);
				idleRoutine = null;
			}
			float duration = DamageDuration(before, after);
			if (Application.isPlaying)
			{
				StartCoroutine(DamageSequence(before, after, duration, revision));
			}
			return duration;
		}

		private IEnumerator DamageSequence(int before, int after, float duration, int revision)
		{
			if (id == 100042)
			{
				Reference20Native reference20Native = this;
				object obj;
				switch (before)
				{
				default:
					obj = "动效5-消除";
					break;
				case 2:
					obj = "动效4-消除";
					break;
				case 3:
					obj = "动效2-消除";
					break;
				}
				reference20Native.PlayAnimation((string)obj);
			}
			else if (id == 100016 && before == 3)
			{
				SkeletonGraphic[] eggs = GetComponentsInChildren<SkeletonGraphic>(true);
				for (int i = 0; i < eggs.Length; i++)
				{
					StartCoroutine(Hatch(eggs[i], (float)i * 0.2f, revision));
				}
			}
			else if (id == 100015)
			{
				Reference20Native reference20Native2 = this;
				object obj2;
				switch (before)
				{
				default:
					obj2 = "collect";
					break;
				case 2:
					obj2 = "egg_open2";
					break;
				case 3:
					obj2 = "egg_open";
					break;
				}
				reference20Native2.PlayAnimation((string)obj2);
			}
			else if (id == 100032)
			{
				PlayAnimation((before % 2 == 0) ? "收缩绳子，公鸡挤压" : "绳子松绑");
			}
			yield return new WaitForSeconds(duration);
			if (after > 0 && revision == animationRevision)
			{
				ApplyIdlePose();
				idleRoutine = StartCoroutine(IdleLoop(revision));
			}
		}

		private IEnumerator Hatch(SkeletonGraphic egg, float delay, int revision)
		{
			yield return new WaitForSeconds(delay);
			if (!egg || revision != animationRevision)
			{
				yield break;
			}
			Animate(egg, "egg_open", false);
			yield return new WaitForSeconds(0.6f);
			if (!egg || revision != animationRevision)
			{
				yield break;
			}
			Animate(egg, "egg_open2", false);
			yield return new WaitForSeconds(Mathf.Max(0f, 0.6f - delay));
			if ((bool)egg && revision == animationRevision)
			{
				Animate(egg, "collect", false);
				yield return new WaitForSeconds(0.5f);
				if ((bool)egg && revision == animationRevision)
				{
					Animate(egg, "idle", true);
				}
			}
		}

		private void Animate(SkeletonGraphic skeleton, string animation, bool loop)
		{
			if (skeleton.Skeleton.Data.FindAnimation(animation) != null)
			{
				skeleton.AnimationState.SetAnimation(0, animation, loop);
			}
		}

		public void Hit(int remaining)
		{
			AnimateDamage(health, remaining);
		}

		public void PlayCollected()
		{
			animationRevision++;
			if ((bool)replacement)
			{
				replacement.Collected();
			}
			else
			{
				PlayAnimation((id == 100042) ? "动效6-消除" : ((id == 100032) ? "收集" : "collect_2"), true);
			}
		}

		public void UseChickData(SkeletonDataAsset data, string skin)
		{
			if (!data)
			{
				return;
			}
			SkeletonGraphic[] componentsInChildren = GetComponentsInChildren<SkeletonGraphic>(true);
			foreach (SkeletonGraphic skeleton in componentsInChildren)
			{
				skeleton.skeletonDataAsset = data;
				skeleton.Initialize(true);
				if (!string.IsNullOrEmpty(skin) && skeleton.Skeleton.Data.FindSkin(skin) != null)
				{
					skeleton.Skeleton.SetSkin(skin);
					skeleton.Skeleton.SetSlotsToSetupPose();
				}
			}
		}

		public RectTransform DetachNestChick(RectTransform stage)
		{
			if ((bool)replacement)
			{
				return replacement.DetachPayload(stage);
			}
			SkeletonGraphic[] eggs = GetComponentsInChildren<SkeletonGraphic>(false);
			if (eggs.Length == 0)
			{
				return null;
			}
			return Detach(eggs[UnityEngine.Random.Range(0, eggs.Length)], stage, null);
		}

		public RectTransform DetachCornHead(RectTransform stage)
		{
			if ((bool)replacement)
			{
				return null;
			}
			SkeletonGraphic[] componentsInChildren = GetComponentsInChildren<SkeletonGraphic>(false);
			foreach (SkeletonGraphic head in componentsInChildren)
			{
				if (head.name == "head")
				{
					return Detach(head, stage, "collect_2");
				}
			}
			return null;
		}

		private RectTransform Detach(SkeletonGraphic skeleton, RectTransform stage, string animation)
		{
			RectTransform rect = skeleton.rectTransform;
			Raise(rect, stage);
			rect.localRotation = Quaternion.identity;
			if (!string.IsNullOrEmpty(animation))
			{
				Animate(skeleton, animation, true);
			}
			return rect;
		}

		private float MoveCorn(RectTransform rect, float delta)
		{
			if (!cornTargets.TryGetValue(rect, out var target))
			{
				target = rect.anchoredPosition.x;
			}
			target += delta;
			cornTargets[rect] = target;
			float duration = Mathf.Abs(target - rect.anchoredPosition.x) / 232.72726f;
			rect.DOKill();
			rect.DOAnchorPosX(target, duration).SetEase(Ease.Linear);
			return duration;
		}

		private void CornEffect(bool active)
		{
			if (!cornEffect)
			{
				return;
			}
			if (active && !cornEffectActive)
			{
				cornEffect.gameObject.SetActive(true);
				if ((bool)cornEffectClip)
				{
					cornEffectClip.Play("ele_anim_bread_effect");
				}
			}
			else if (!active)
			{
				if ((bool)cornEffectClip)
				{
					cornEffectClip.Stop();
				}
				cornEffect.gameObject.SetActive(false);
			}
			cornEffectActive = active;
		}

		public void Eat(int remaining)
		{
			if ((bool)replacement)
			{
				replacement.Feed();
				return;
			}
			health = remaining;
			int revision = ++animationRevision;
			float duration = 0.55f;
			RectTransform[] componentsInChildren = GetComponentsInChildren<RectTransform>(true);
			foreach (RectTransform rect in componentsInChildren)
			{
				if (rect.name == "headNode")
				{
					duration = Mathf.Max(duration, MoveCorn(rect, 128f));
				}
				if (!(rect.name == "Mask"))
				{
					continue;
				}
				duration = Mathf.Max(duration, MoveCorn(rect, 128f));
				for (int i = 0; i < rect.childCount; i++)
				{
					RectTransform child = rect.GetChild(i) as RectTransform;
					if ((bool)child)
					{
						duration = Mathf.Max(duration, MoveCorn(child, -128f));
					}
				}
			}
			cornMotionEnd = Time.time + duration;
			CornEffect(true);
			if (Application.isPlaying)
			{
				StartCoroutine(EatSequence(revision, duration));
			}
		}

		private IEnumerator EatSequence(int revision, float duration)
		{
			PlayAnimation("eat", true);
			yield return new WaitForSeconds(duration);
			if (revision == animationRevision)
			{
				CornEffect(false);
				PlayAnimation("eat2");
				yield return new WaitForSeconds(0.4f);
				if (revision == animationRevision)
				{
					ApplyIdlePose();
					idleRoutine = StartCoroutine(IdleLoop(revision));
				}
			}
		}
	}
}
