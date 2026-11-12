using System;
using UnityEngine;

namespace Reference20
{
	[Serializable]
	public sealed class CompanyImage
	{
		public int type;

		public bool preserveAspect;

		public Color color = Color.white;

		public CompanySprite sprite;
	}
}
