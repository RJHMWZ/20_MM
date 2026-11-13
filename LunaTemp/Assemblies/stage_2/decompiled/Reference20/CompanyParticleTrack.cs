using System;

namespace Reference20
{
	[Serializable]
	public sealed class CompanyParticleTrack
	{
		public float start;

		public float[][] samples;

		[NonSerialized]
		private bool[] validSamples;

		public bool ValidSample(int index)
		{
			if (validSamples == null)
			{
				validSamples = new bool[samples.Length];
				for (int i = 0; i < samples.Length; i++)
				{
					float[] sample = samples[i];
					bool valid = sample != null && sample.Length >= 20;
					if (valid)
					{
						for (int j = 0; j < sample.Length; j++)
						{
							if (float.IsNaN(sample[j]) || float.IsInfinity(sample[j]))
							{
								valid = false;
								break;
							}
						}
					}
					validSamples[i] = valid;
				}
			}
			return validSamples[index];
		}
	}
}
