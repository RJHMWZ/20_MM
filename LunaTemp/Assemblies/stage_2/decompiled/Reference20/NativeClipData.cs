using System;
using System.Collections.Generic;

namespace Reference20
{
	[Serializable]
	public sealed class NativeClipData
	{
		public Dictionary<string, NativeCurveKey[]> props;

		public Dictionary<string, Dictionary<string, NativeCurveKey[]>> comps;

		public Dictionary<string, NativeClipData> paths;
	}
}
