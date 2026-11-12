using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
	[Serializable]
	public class ColumnRefillSequence
	{
		[Tooltip("Normal element IDs spawned in order for this column")]
		public List<int> normalElementIds = new List<int>();
	}
}
