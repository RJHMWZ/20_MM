using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	public class Reference20CocosClip : MonoBehaviour
	{
		public NativeClip[] clips;

		private NativeClip current;

		private float time;

		public void Play(int index)
		{
			if (clips != null && clips.Length != 0)
			{
				current = clips[Mathf.Clamp(index, 0, clips.Length - 1)];
				time = 0f;
				Apply(current.data, base.transform, 0f);
			}
		}

		public bool Play(string name)
		{
			int index = Array.FindIndex(clips, (NativeClip c) => c.name == name);
			if (index < 0)
			{
				return false;
			}
			Play(index);
			return true;
		}

		public void Stop(bool reset = true)
		{
			if (reset && current != null)
			{
				Apply(current.data, base.transform, 0f);
			}
			current = null;
			time = 0f;
		}

		private void Update()
		{
			if (current != null)
			{
				time += Time.deltaTime;
				float t = time;
				if (t > current.duration)
				{
					t = ((((uint)current.wrap & 0x10u) != 0) ? Mathf.PingPong(t, Mathf.Max(0.01f, current.duration)) : ((current.wrap < 2) ? current.duration : (t % Mathf.Max(0.01f, current.duration))));
				}
				Apply(current.data, base.transform, t);
				if (time > current.duration && current.wrap < 2)
				{
					current = null;
				}
			}
		}

		private void Apply(NativeClipData data, Transform root, float t)
		{
			if (data == null)
			{
				return;
			}
			Properties(data.props, root, t);
			if (data.comps != null)
			{
				foreach (KeyValuePair<string, Dictionary<string, NativeCurveKey[]>> comp in data.comps)
				{
					Properties(comp.Value, root, t);
				}
			}
			if (data.paths == null)
			{
				return;
			}
			foreach (KeyValuePair<string, NativeClipData> p in data.paths)
			{
				Transform target = root.Find(p.Key);
				if ((bool)target)
				{
					Apply(p.Value, target, t);
				}
			}
		}

		private void Properties(Dictionary<string, NativeCurveKey[]> props, Transform tr, float t)
		{
			if (props == null)
			{
				return;
			}
			foreach (KeyValuePair<string, NativeCurveKey[]> prop in props)
			{
				NativeCurveKey[] keys = prop.Value;
				if (keys == null || keys.Length == 0)
				{
					continue;
				}
				NativeCurveKey a = keys[0];
				NativeCurveKey b = a;
				for (int i = 0; i < keys.Length; i++)
				{
					if (keys[i].frame <= t)
					{
						a = keys[i];
						b = a;
						continue;
					}
					b = keys[i];
					break;
				}
				float f = ((b.frame > a.frame) ? Mathf.Clamp01((t - a.frame) / (b.frame - a.frame)) : 0f);
				RectTransform rt = tr as RectTransform;
				if (prop.Key == "spriteFrame")
				{
					Image im = tr.GetComponent<Image>();
					if ((bool)im && a.sprite != null)
					{
						Reference20Native.ApplySpriteFrame(im, Reference20Native.Sprite(a.sprite));
					}
				}
				else if (prop.Key == "position" && a.vector != null)
				{
					Vector3 v2 = Vector3.Lerp(Vec(a.vector), Vec(b.vector), f);
					if ((bool)rt)
					{
						rt.anchoredPosition = v2;
					}
					else
					{
						tr.localPosition = v2;
					}
				}
				else if (prop.Key == "scale" && a.vector != null)
				{
					Vector3 scale = Vector3.Lerp(Vec(a.vector), Vec(b.vector), f);
					scale.z = 1f;
					tr.localScale = scale;
				}
				else if (prop.Key == "angle")
				{
					tr.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(a.number, b.number, f));
				}
				else if (prop.Key == "scaleX" || prop.Key == "scaleY")
				{
					Vector3 v = tr.localScale;
					float z = Mathf.Lerp(a.number, b.number, f);
					if (prop.Key == "scaleX")
					{
						v.x = z;
					}
					else
					{
						v.y = z;
					}
					tr.localScale = v;
				}
				else if (prop.Key == "opacity")
				{
					CanvasGroup group = tr.GetComponent<CanvasGroup>();
					if (!group)
					{
						group = tr.gameObject.AddComponent<CanvasGroup>();
					}
					group.alpha = Mathf.Lerp(a.number, b.number, f) / 255f;
				}
				else if (prop.Key == "active")
				{
					tr.gameObject.SetActive(a.active);
				}
			}
		}

		private Vector3 Vec(float[] v)
		{
			return new Vector3(v[0], v[1], (v.Length > 2) ? v[2] : 1f);
		}
	}
}
