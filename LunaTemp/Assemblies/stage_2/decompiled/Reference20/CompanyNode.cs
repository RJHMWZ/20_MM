using System;
using UnityEngine;

namespace Reference20
{
	[Serializable]
	public sealed class CompanyNode
	{
		public string name;

		public Vector2 position;

		public Vector2 size;

		public Vector2 pivot;

		public Vector2 anchorMin = Vector2.one * 0.5f;

		public Vector2 anchorMax = Vector2.one * 0.5f;

		public Vector3 scale = Vector3.one;

		public Vector3 rotation;

		public CompanyImage image;

		public CompanyText text;

		public CompanyNode[] children;

		public Color renderedColor;
	}
}
