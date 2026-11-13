using UnityEngine;

namespace Reference20
{
	public sealed class Reference20ColorTrailLease : MonoBehaviour
	{
		public Reference20ColorTrail[] Layers;

		private float remaining;

		private bool leased;

		public void Begin()
		{
			remaining = 0.5f;
			leased = true;
			base.gameObject.SetActive(true);
		}

		public void Complete()
		{
			if (leased)
			{
				leased = false;
				Reference20ColorTrail.Release(this);
			}
		}

		private void Update()
		{
			if (leased)
			{
				remaining -= Time.deltaTime;
				if (remaining <= 0f)
				{
					Complete();
				}
			}
		}
	}
}
