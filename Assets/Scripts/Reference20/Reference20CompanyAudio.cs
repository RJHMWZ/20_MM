using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Reference20 {
public static class Reference20CompanyAudio {
 static Dictionary<string,string> aliases;
 public static string Name(string eventName){
  // The reference has no separate charge loop; do not substitute a looping explosion.
  if(eventName=="Audio_Match_SameColorCrusherStandby")return null;
  if(aliases==null)aliases=JsonConvert.DeserializeObject<Dictionary<string,string>>(Resources.Load<TextAsset>("Reference20/Company/audio-aliases").text);
  string name;return aliases.TryGetValue(eventName,out name)?name:eventName;
 }
}
}
