using UnityEngine;

namespace Match3
{
	public static class Match3EffectPlayer
	{
		public const float DefaultLifetime = 2f;

		public static GameObject Play(GameObject prefab, Vector3 worldPosition, Transform parent = null, float? scale = null, float lifetime = 2f, Quaternion? rotation = null, bool stopParticleLoops = true)
		{
			if (prefab == null)
			{
				return null;
			}
			GameObject effect = Object.Instantiate(prefab, worldPosition, rotation ?? Quaternion.identity, parent);
			if (scale.HasValue)
			{
				effect.transform.localScale = Vector3.one * scale.Value;
			}
			PlayParticles(effect, stopParticleLoops);
			if (lifetime > 0f)
			{
				Object.Destroy(effect, lifetime);
			}
			return effect;
		}

		public static void PlayParticles(GameObject effect, bool stopLoops = true)
		{
			if (effect == null)
			{
				return;
			}
			ParticleSystem[] componentsInChildren = effect.GetComponentsInChildren<ParticleSystem>(true);
			foreach (ParticleSystem system in componentsInChildren)
			{
				if (!(system == null))
				{
					system.gameObject.SetActive(true);
					if (stopLoops)
					{
						ParticleSystem.MainModule main = system.main;
						main.loop = false;
					}
					system.Play(true);
				}
			}
		}
	}
}
