using UnityEngine;

namespace Match3
{
    /// <summary>Shared runtime rules for short-lived board effects.</summary>
    public static class Match3EffectPlayer
    {
        public const float DefaultLifetime = 2f;

        public static GameObject Play(GameObject prefab, Vector3 worldPosition,
            Transform parent = null, float? scale = null,
            float lifetime = DefaultLifetime, Quaternion? rotation = null,
            bool stopParticleLoops = true)
        {
            if (prefab == null) return null;

            GameObject effect = Object.Instantiate(prefab, worldPosition,
                rotation ?? Quaternion.identity, parent);
            if (scale.HasValue) effect.transform.localScale = Vector3.one * scale.Value;
            PlayParticles(effect, stopParticleLoops);

            if (lifetime > 0f) Object.Destroy(effect, lifetime);
            return effect;
        }

        public static void PlayParticles(GameObject effect, bool stopLoops = true)
        {
            if (effect == null) return;

            foreach (ParticleSystem system in effect.GetComponentsInChildren<ParticleSystem>(true))
            {
                if (system == null) continue;
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
