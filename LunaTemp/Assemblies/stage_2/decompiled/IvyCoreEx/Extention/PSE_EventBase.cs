using UnityEngine;

namespace IvyCoreEx.Extention
{
	public class PSE_EventBase
	{
		public virtual void Reset()
		{
		}

		public virtual void DoEvent(ParticleSystemExtend pse, float curTime, Vector3 worldPos, Vector3 locaPos)
		{
		}
	}
}
