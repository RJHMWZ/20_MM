using System;
using System.Collections.Generic;

namespace Reference20 {
// Fixed data shapes are supported by both Newtonsoft in Unity and Playworks' JSON runtime.
[Serializable] public sealed class NativeCurveKey {
 public float frame,number;
 public bool active;
 public float[] vector;
 public string sprite;
}
[Serializable] public sealed class NativeClipData {
 public Dictionary<string,NativeCurveKey[]> props;
 public Dictionary<string,Dictionary<string,NativeCurveKey[]>> comps;
 public Dictionary<string,NativeClipData> paths;
}
}
