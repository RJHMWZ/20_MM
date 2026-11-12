using UnityEngine;

namespace Reference20
{
	public sealed class Reference20CompanyParticleHandle : MonoBehaviour
	{
		public CompanyParticleEffect Definition;

		public float Elapsed;

		public float SampleRate = 60f;

		public bool Combo;

		public Vector2 Velocity;

		private bool loop;

		private bool leased;

		public void Play(CompanyParticleEffect effect, bool repeat, bool combo)
		{
			base.enabled = true;
			Definition = effect;
			Elapsed = 0f;
			SampleRate = 60f;
			Velocity = Vector2.zero;
			loop = repeat;
			Combo = combo;
			leased = true;
			base.gameObject.SetActive(true);
		}

		private void Update()
		{
			if (!leased)
			{
				return;
			}
			Elapsed += Time.deltaTime;
			if (!(Elapsed < Definition.duration))
			{
				if (loop)
				{
					Elapsed %= Definition.duration;
				}
				else
				{
					Stop();
				}
			}
		}

		public void Stop()
		{
			if (leased)
			{
				leased = false;
				base.gameObject.SetActive(false);
				Reference20CompanyParticles.Release(this);
			}
		}
	}
}
