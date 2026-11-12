using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

// Keep the company web preloader in project code, without changing the shared Playworks SDK.
[InitializeOnLoad]
public static class Reference20CompanyBuildBranding {
 const string StylePath="Assets/Editor/CompanyBranding/preloader.css";
 static readonly Dictionary<string,DateTime> stamps=new Dictionary<string,DateTime>();
 static double next;
 static Reference20CompanyBuildBranding(){EditorApplication.update+=Tick;}
 public static string ApplyHtml(string html){
  if(!html.Contains("application-preloader")||!File.Exists(StylePath))return html;
  string css=File.ReadAllText(StylePath);
  return Regex.Replace(html,@"<style\b[^>]*>([\s\S]*?)</style>",m=>m.Groups[1].Value.Contains(".preloader")?"<style id=\"company-preloader\">"+css+"</style>":m.Value,RegexOptions.IgnoreCase);
 }
 [MenuItem("Reference20/Apply Company HTML Branding")]
 public static void ApplyDevelop(){
  foreach(string path in new[]{"LunaTemp/stage4/develop/iframe.html","LunaTemp/stage4/develop/index.html"}){
   if(!File.Exists(path))continue;
   string html=File.ReadAllText(path),branded=ApplyHtml(html);
   if(html!=branded)File.WriteAllText(path,branded);
   stamps[path]=File.GetLastWriteTimeUtc(path);
  }
 }
 static void Tick(){
  if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+2;
  try{foreach(string path in new[]{"LunaTemp/stage4/develop/iframe.html","LunaTemp/stage4/develop/index.html"}){
   if(!File.Exists(path))continue;DateTime stamp;if(!stamps.TryGetValue(path,out stamp)||stamp!=File.GetLastWriteTimeUtc(path)){ApplyDevelop();break;}
  }}catch(IOException){/* Export may still be writing; retry on the next editor tick. */}
 }
}
