using UnityEditor;
using MCPForUnity.Editor.Services;
using MCPForUnity.Editor.Services.Transport;
[InitializeOnLoad]
public static class Reference20McpConnect {
 static Reference20McpConnect() { EditorApplication.delayCall += Connect; }
 static async void Connect() { if (!MCPServiceLocator.TransportManager.IsRunning(TransportMode.Http)) await MCPServiceLocator.TransportManager.StartAsync(TransportMode.Http); }
}

