
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Unity.EditorCoroutines.Editor;
namespace EditorTool
{
    public class ApiResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public string[] data { get; set; }
    }
    public class PlayableTools : EditorWindow
    {
        private List<FileInfo> zipFiles = new List<FileInfo>();
        private string folderPath = "LunaTemp/stage4/create-hub/";
        private string selectedZipPath;

        public string zipPath = "";
        public string extractPath = "";

        private string gameName;
        private string version;
        private string applicationName;
        private string iosLink;
        private string androidLink;
        private Producer selectedProducer;
        private Platform selectedPlatform;
        private List<MonoScript> scriptList = new List<MonoScript>();
        private string maxVersion;
        private Texture2D draggedImage;

        private string imagePath= "Assets/Resources/Screenshot/preview.jpg";          // 图片的文件路径
        private string imgFolderPath;
       [MenuItem("ExportTools/Open Window %t")]
        private static void OpenWindow()
        {
            // 打开窗口
            var window = GetWindow<PlayableTools>();
            // 设置窗口标题
            window.titleContent = new GUIContent("Playable Tools");
            window.Show();
        }

        private void OnEnable()
        {
            gameName = EditorPrefs.GetString("PlayableTools_gameName", string.Empty);
            version = EditorPrefs.GetString("PlayableTools_version", string.Empty);
            applicationName = EditorPrefs.GetString("PlayableTools_ApplicationName", string.Empty);
            iosLink = EditorPrefs.GetString("PlayableTools_IosLink", string.Empty);
            androidLink = EditorPrefs.GetString("PlayableTools_AndroidLink", string.Empty);
            maxVersion = EditorPrefs.GetString("PlayableTools_maxVersion", string.Empty);
            selectedPlatform = Platform.Unity;
            // Deserialize script paths and load scripts
            scriptList.Clear();
            string[] scriptPaths = EditorPrefs.GetString("PlayableTools_ScriptPaths", "").Split(';');
            foreach (var path in scriptPaths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(path);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                        if (script != null && !scriptList.Contains(script))
                        {
                            scriptList.Add(script);
                        }
                    }
                }
            }

            ScanZipFiles();
            if (!string.IsNullOrEmpty(gameName))
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(PostRequest(gameName));
            }
        }

        private void SavePreferences()
        {
            EditorPrefs.SetString("PlayableTools_gameName", gameName);
            EditorPrefs.SetString("PlayableTools_version", version);
            EditorPrefs.SetString("PlayableTools_maxVersion", maxVersion);
            EditorPrefs.SetString("PlayableTools_ApplicationName", applicationName);
            EditorPrefs.SetString("PlayableTools_IosLink", iosLink);
            EditorPrefs.SetString("PlayableTools_AndroidLink", androidLink);

            // Serialize script paths
            List<string> scriptGUIDs = new List<string>();
            foreach (var script in scriptList)
            {
                string path = AssetDatabase.GetAssetPath(script);
                if (!string.IsNullOrEmpty(path))
                {
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    scriptGUIDs.Add(guid);
                }
            }

            EditorPrefs.SetString("PlayableTools_ScriptPaths", string.Join(";", scriptGUIDs));
        }


        private void OnGUI()
        {
            gameName = EditorGUILayout.TextField("gameName", gameName);
            version = EditorGUILayout.TextField("version", version);
            EditorGUILayout.LabelField("maxVersion", maxVersion);
            GUILayout.Label("导出的文件列表", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("文件名", GUILayout.Width(250));
            GUILayout.Label("日期", GUILayout.Width(150));
            GUILayout.Label("选择", GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();


            if (zipFiles.Count == 0)
            {
                GUILayout.Label("No ZIP files found in the specified directory.");
            }
            else
            {
                foreach (var fileInfo in zipFiles)
                {
                    EditorGUILayout.BeginHorizontal();

                    // 高亮显示选中的 ZIP 文件
                    if (fileInfo.FullName == selectedZipPath)
                    {
                        GUI.backgroundColor = Color.cyan;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.white;
                    }

                    GUILayout.Label(fileInfo.Name, GUILayout.Width(250));
                    GUILayout.Label(fileInfo.LastWriteTime.ToString(), GUILayout.Width(150));
                    // 禁用已选中的按钮
                    GUI.enabled = fileInfo.FullName != selectedZipPath;
                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        selectedZipPath = fileInfo.FullName;
                        zipPath = fileInfo.FullName;
                    }

                    GUI.enabled = true;
                    GUI.backgroundColor = Color.white; // 重置背景颜色

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Refresh ZIP Files", GUILayout.Width(160)))
            {
                ScanZipFiles();
                EditorCoroutineUtility.StartCoroutineOwnerless(PostRequest(gameName));
            }

            GUILayout.Label("参数配置", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();


            //applicationName = EditorGUILayout.TextField("Application Name", applicationName);
            //iosLink = EditorGUILayout.TextField("iOS Link", iosLink);
            //androidLink = EditorGUILayout.TextField("Android Link", androidLink);
            //selectedPlatform = (Platform)EditorGUILayout.EnumPopup("Platforms", selectedPlatform);
            iosLink = "https://apps.apple.com/us/app/colorblock-combo-blast/id6478063606?mt=8";
                androidLink = "https://apps.apple.com/us/app/colorblock-combo-blast/id6478063606?mt=8";
            if (EditorGUI.EndChangeCheck())
            {
                SavePreferences();
            }

            GUILayout.Label("C# Scripts");

            GUILayout.Label("Drag and Drop C# scripts here:");
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drop Scripts Here");

            HandleDragAndDrop(dropArea);

            // Display and manage script list
            for (int i = 0; i < scriptList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var script = scriptList[i];
                EditorGUILayout.ObjectField(script, typeof(MonoScript), false);
                if (GUILayout.Button("Remove"))
                {
                    scriptList.RemoveAt(i);
                    SavePreferences(); // Save immediately when script is removed
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
            //制作人名单
            selectedProducer = (Producer)EditorGUILayout.EnumPopup("producer", selectedProducer);
            GUILayout.Label("导出", EditorStyles.boldLabel);
            // 输入库
            GUI.enabled = !string.IsNullOrEmpty(zipPath) && zipFiles.Count > 0;


            if (GUILayout.Button("导出"))
            {
                if (!CheckGitAndAdSkip())
                {
                    return;
                }

                extractPath = zipPath.Replace(".zip", "");

                var unzipResult = UnzipFile();
                Debug.Log($"解压结果:{unzipResult}");

                // 读取 extractPath路径下 index.html 进行处理
                string htmlFilePath = Path.Combine(extractPath, "index.html");

                string html = HtmlReader.ReadHtmlFromFile(htmlFilePath);

                var parser = new HtmlParser(html);
                HtmlNode rootNode = parser.Parse();

                // var converter = new HtmlToDictionaryConverter();
                // Dictionary<string, object> dictionary = converter.Convert(rootNode);
                // PrintDictionary(dictionary, 0);

                // 修改
                // 1.lunaPlaygroundNode
                var lunaPlaygroundNode = rootNode.FindNode(rootNode, "script", "window.LUNA_PLAYGROUND_CREATIVE_ID");
                var lunaPlaygroundText = lunaPlaygroundNode.InnerText;
                // var LUCID = "LPG-6F9L-58D";
                // var playgroundOverrides = "";
                //
                // lunaPlaygroundText =
                //     $"window.LUCID = \"{LUCID}\",window.playgroundOverrides = " +
                //     $"{{\"Manager.ConfigManager\":{{\"endCardTime\":[\"float\",5],\"gameName\":[\"string\",\"Name\"],\"gameNameColor\":[\"color\",1,1,1,1]," +
                //     $"\"gameDescription\":[\"string\",\"Description\"],\"gameDescriptionColor\":[\"color\",1,1,1,1],\"gameButtonName\":[\"string\",\"Play Now!\"]," +
                //     $"\"gameButtonNameColor\":[\"color\",1,1,1,1]}}}}; window.playgroundAssetOverrides = {{}}," +
                //     $"window.LUNA_PLAYGROUND_BUNDLES";

                var playgroundOverrides = "{}";
                if (scriptList.Count > 0)
                {
                    Debug.LogError(scriptList.Count);
                    JObject jsonResult = new JObject();
                    foreach (var script in scriptList)
                    {
                        Type configManagerType = script.GetClass();
                        if (configManagerType == null)
                        {
                            Debug.LogError("Failed to get class from script: " + script.name);
                            continue;
                        }

                        var classData = ProcessConfigManager(configManagerType);

                        jsonResult.Add(configManagerType.FullName, classData);
                    }

                    playgroundOverrides = jsonResult.ToString(Formatting.None);
                }

                var playgroundAssetOverrides = "{}";
                // 占位符替换
                // "window.LUNA_PLAYGROUND_CREATIVE_ID" 替换 "window.LUCID = "LPG-6F9L-58D""
                lunaPlaygroundText = lunaPlaygroundText.Replace("window.LUNA_PLAYGROUND_CREATIVE_ID", "window.LUCID = \"LPG-6F9L-58D\"");
                // "window.LUNA_PLAYGROUND_OVERRIDES" 替换 为 "{"Manager.ConfigManager":{"endCardTime":["float",5],"gameName":["string","Name"],"gameNameColor":["color",1,1,1,1],"gameDescription":["string","Description"],"gameDescriptionColor":["color",1,1,1,1],"gameButtonName":["string","Play Now!"],"gameButtonNameColor":["color",1,1,1,1]}}; window.playgroundAssetOverrides = {}"
                lunaPlaygroundText = lunaPlaygroundText.Replace("window.LUNA_PLAYGROUND_OVERRIDES",
                    $"window.playgroundOverrides = {playgroundOverrides}; window.playgroundAssetOverrides = {playgroundAssetOverrides}");
                // "window.LUNA_PLAYGROUND_BUNDLES" 暂不处理
                lunaPlaygroundNode.InnerText = lunaPlaygroundText;

                // 2.environment
                var environmentNode = rootNode.FindNode(rootNode, "script", "window.$environmen");
                // string applicationName = "";
                // string iosLink = "";
                // string androidLink = "https://play.google.com/store/apps/details?id=com.merge.Seatopia";
                string targetPlatform = selectedPlatform switch
                {
                    Platform.Unity => "unityads",
                    _ => throw new ArgumentOutOfRangeException()
                };

                var environmentText =
                    $"window.$environment={{baseUrl:\"./\",resourceConfig:{{json:\"inline\",image:\"inline\",video:\"inline\",sound:\"compress\",blob:\"compress\"}}," +
                    $"packageConfig:Object.assign({{inlineScripts:!0,inlineJsons:!0,inlineBlobs:!0,inlineImages:!0,inlineSounds:!0,inlineVideos:!0,applicationName:\"{applicationName}\"," +
                    $"iosLink:\"{iosLink}\",androidLink:\"{androidLink}\",orientation:\"unspecified\",languages:[\"en\",\"en\"]}}," +
                    $"window.LUNA_PLAYGROUND_PACKAGE_CONFIG||{{}}),playerPrefs:!0,usingWebWorkers:\"off\",targetPlatform:\"{targetPlatform}\"," +
                    $"runtimeAnalysisModules:[\"physics3d\",\"physics2d\",\"particle_system\",\"reflection\",\"prefabs\",\"mecanim-wasm\"]}}";
                environmentNode.InnerText = environmentText;

                // 3.LUNA_PI_SETTINGS
                var lunaPISettingsNode = rootNode.FindNode(rootNode, "script", "window.pi.apply");
                var lunaPISettingsText =
                    @"[""unityads"",160190,7450333,""8447b4c1f753abb8ed97b094e23d1fa64badd5df2ee64e65e9cb22a4d5413ada"","""",null,3000,500,null,""""]";
                lunaPISettingsText = $"window.pi.apply(window,{lunaPISettingsText}||[])";
                lunaPISettingsNode.InnerText = lunaPISettingsText;




                // 补充
                // 1.Brotli解压缩的算法
                rootNode.InsertNode(rootNode, "body", HtmlNode.Create("script",
                    new Dictionary<string, string> { ["data-startup-only"] = null }, BrotliScriptsJs()));
                // 2.Base64解压缩的算法
                rootNode.InsertNode(rootNode, "body", HtmlNode.Create("script",
                    new Dictionary<string, string> { ["data-startup-only"] = null }, Base64ScriptsJs()));

                // 3.集成js代码
                var allJsNodes = GetAllJsNodes();
                foreach (var htmlNode in allJsNodes)
                {
                    rootNode.InsertNode(rootNode, "body", htmlNode);
                }

                // 4.集成图片
                var allImgNodes = GetAllImgNodes();
                foreach (var htmlNode in allImgNodes)
                {
                    rootNode.InsertNode(rootNode, "body", htmlNode);
                }

                // 5.Event
                rootNode.InsertNode(rootNode, "body", HtmlNode.Create("script", null, LifeJs()));

                // 6.Base64解压缩的算法
                rootNode.InsertNode(rootNode, "body", HtmlNode.Create("script", null, EventJs()));

                // 7.Debug
                rootNode.InsertNode(rootNode, "body", HtmlNode.Create("script", null, DebugJs()));

                // 删除
                var optResult = false;
                optResult = rootNode.DeleteNode(rootNode, "style", ".preloader{display:none}");
                Debug.Log($"DeleteNode optResult : {optResult}");
                optResult = rootNode.DeleteNode(rootNode, "script", @"function replaceElementText(e,t)");
                Debug.Log($"DeleteNode optResult : {optResult}");
                optResult = rootNode.DeleteNode(rootNode, "script", @"window.addEventListener(""luna:ready""");
                Debug.Log($"DeleteNode optResult : {optResult}");

                optResult = rootNode.DeleteNodeWithAttribute(rootNode, "script", new Dictionary<string, string> { ["src"] = "engine/scripts.js" });
                Debug.Log($"DeleteNode optResult : {optResult}");

                // optResult = rootNode.DeleteNodeWithAttribute(rootNode, "canvas", new Dictionary<string, string> { ["id"] = "application-canvas" });
                // Debug.Log($"DeleteNode optResult : {optResult}");
                // optResult = rootNode.DeleteNode(rootNode, "script", @"(()=>{function e(){const e=navigator.userAgent.match(/Chrom(e|ium)\/([0-9]+)\./);");
                // Debug.Log($"DeleteNode optResult : {optResult}");
                optResult = rootNode.DeleteNode(rootNode, "script", @"window.addEventListener(""luna:ready"",(()=>{const e={GameStarted:""Game Started""");
                Debug.Log($"DeleteNode optResult : {optResult}");
                // optResult = rootNode.DeleteNode(rootNode, "script", @"window._bridgeReady=!1,window._domReady=!1,window._readyEventEmitted=!1,window.addEventListener");
                // Debug.Log($"DeleteNode optResult : {optResult}");
                // optResult = rootNode.DeleteNode(rootNode, "script", @"function startGame(){return new Promise((function(n,e){pc.TextGenerator.fontRatio=2,");
                // Debug.Log($"DeleteNode optResult : {optResult}");
                // optResult = rootNode.DeleteNode(rootNode, "script", @"!function(){var n=!1,e=!1,i=[],t=function(){if(!n){n=!0;var e=function(){(window.app.app.events||window.app.app)");
                // Debug.Log($"DeleteNode optResult : {optResult}");
                // optResult = rootNode.DeleteNode(rootNode, "script", @"!function(){var n=!1;window.audioVolumeToggle=function(e){e!==n&&(n=e,e?(Luna.Unity.LifeCycle.OnMute(),");
                // Debug.Log($"DeleteNode optResult : {optResult}");

                // 导出
                var generateHtmlWithDoctype = rootNode.GenerateHtmlWithDoctype();

                var htmlFileExportPath = Path.Combine(folderPath, selectedPlatform.ToString());
                generateHtmlWithDoctype = Reference20CompanyBuildBranding.ApplyHtml(generateHtmlWithDoctype);
                HtmlReader.WriteHtmlToFile(htmlFileExportPath, generateHtmlWithDoctype);




                //// 创建 ZIP 文件的完整路径
                //string zipFilePath = Path.Combine(folderPath, gameName + "_" + version + ".zip");

                //// 压缩文件夹
                //ZipFile.CreateFromDirectory(folderPath, zipFilePath);



                DirectoryInfo dir = new DirectoryInfo(selectedZipPath.Replace(".zip", ""));
                DirectoryInfo[] dirs = dir.GetDirectories();
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (!file.Name.EndsWith(".meta") && file.Name == "playground.json")
                    {
                        string tempPath = Path.Combine(htmlFileExportPath, file.Name);
                        file.CopyTo(tempPath, true);
                    }
                }
                string zipFilePath = Path.Combine(folderPath, $"{gameName + "_" + version}.zip");

                //string newTargetPath = Path.Combine(htmlFileExportPath, "gamePic");
                ////Directory.Move(imgFolderPath, newTargetPath);
                //if (!Directory.Exists(newTargetPath))
                //{
                //    Directory.CreateDirectory(newTargetPath);
                //}
                //CopyDirectory(imgFolderPath, newTargetPath);

                WriteImage(Path.Combine(htmlFileExportPath));

                // 压缩目标文件夹
                if (Directory.Exists(htmlFileExportPath))
                {
                    // 删除现有的 ZIP 文件（如果存在）
                    if (File.Exists(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                    }

                    // 创建新的 ZIP 文件
                    ZipFile.CreateFromDirectory(htmlFileExportPath, zipFilePath);
                }
                UploadFileAsync(zipFilePath, selectedProducer.ToString(), selectedProducer.ToString());
            }




            GUI.enabled = true;

            if (GUILayout.Button("Open 文件目录", GUILayout.Width(150)))
            {
                if (!string.IsNullOrEmpty(folderPath))
                {
                    EditorUtility.RevealInFinder(folderPath);
                }
            }
            if (GUILayout.Button("跳转后台", GUILayout.Width(150)))
            {
                Application.OpenURL("http://10.80.1.5/footage/game_original/index");
            }
        }
        private void WriteImage(string targetPath)
        {

            if(!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            if(File.Exists(imagePath))
            {
                string selectedImagePath = Path.Combine(targetPath, "preview.jpg");
                File.WriteAllBytes(selectedImagePath, File.ReadAllBytes(imagePath));
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("失败成功", "未能找到preview图片，操作中止", "确定");
                throw new System.Exception("未能找到preview图片，操作中止");
            }
        }

        private bool CheckGitAndAdSkip()
        {
            // 1. 检查总文件夹中是否有git
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string gitPath = Path.Combine(projectPath, ".git");
            if (!Directory.Exists(gitPath))
            {
                Debug.LogWarning("Git 检查失败：未找到 .git 文件夹");
                if (!EditorUtility.DisplayDialog("提示", "总文件夹中还未添加git，是否继续打包上传？", "继续", "取消"))
                {
                    return false;
                }
            }
            else
            {
                Debug.Log("Git 检查成功：已包含 .git 文件夹");
            }

            // 2. 检查场景中是否有绑定/调用了 AdSkip 方法
            bool hasAdSkip = false;

            UnityEngine.UI.Button[] buttons = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Button>();
            foreach (var btn in buttons)
            {
                if (btn.gameObject.scene.name == null) continue; // 排除预制体
                for (int i = 0; i < btn.onClick.GetPersistentEventCount(); i++)
                {
                    if (btn.onClick.GetPersistentMethodName(i) == "AdSkip")
                    {
                        hasAdSkip = true;
                        break;
                    }
                }
                if (hasAdSkip) break;
            }

            // 补充检查当前场景文件内容
            if (!hasAdSkip)
            {
                string scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
                if (!string.IsNullOrEmpty(scenePath))
                {
                    string fullPath = Path.Combine(projectPath, scenePath);
                    if (File.Exists(fullPath))
                    {
                        string sceneContent = File.ReadAllText(fullPath);
                        if (sceneContent.Contains("m_MethodName: AdSkip"))
                        {
                            hasAdSkip = true;
                        }
                    }
                }
            }

            if (!hasAdSkip)
            {
                Debug.LogWarning("AdSkip 检查失败：场景中未检测到绑定或调用 AdSkip 方法");
                if (!EditorUtility.DisplayDialog("提示", "场景中未检测到绑定或调用 AdSkip 方法，是否继续打包上传？", "继续", "取消"))
                {
                    return false;
                }
            }
            else
            {
                Debug.Log("AdSkip 检查成功：场景中已绑定或调用 AdSkip 方法");
            }

            return true;
        }

        private void UploadFileAsync(string path, string developer, string uploader)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(UploadCoroutine(path, developer, uploader));
        }
        //获取最大版本
        private IEnumerator PostRequest(string projectName)
        {
            // 检查 projectName 是否为空
            if (string.IsNullOrEmpty(projectName))
            {
                Debug.LogWarning("Project name is empty, skipping version check");
                yield break;
            }
            
            // 创建请求体
            string jsonBody = "{\"project_name\": \"" + projectName + "\"}";

            // 创建UnityWebRequest
            using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://10.80.1.5/oapi/htmlOriginVer", jsonBody))
            {
                // 设置请求头
                www.method = UnityWebRequest.kHttpVerbPOST;

                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Accept", "*/*");
                www.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
                www.SetRequestHeader("User-Agent", "PostmanRuntime-ApipostRuntime/1.1.0");
                www.SetRequestHeader("Cache-Control", "no-cache");

                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
                www.downloadHandler = new DownloadHandlerBuffer();

                // 发送请求并等待响应
                yield return www.SendWebRequest();

                // 检查网络错误
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + www.error);
                }
                else
                {
                    // 处理成功的响应
                    string jsonResponse = www.downloadHandler.text;
                    Debug.Log("Response: " + jsonResponse);
                    
                    // 检查响应是否为空
                    if (string.IsNullOrEmpty(jsonResponse))
                    {
                        Debug.LogWarning("Response is empty");
                        yield break;
                    }
                    
                    try
                    {
                        // 解析返回的JSON数据
                        ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                        // 访问 data 数组的第一个元素
                        if (response != null && response.data != null && response.data.Length > 0)
                        {
                            string version = response.data[0];
                            maxVersion = version;
                            SavePreferences();
                            Debug.Log("Version: " + version);
                        }
                        else
                        {
                            Debug.LogWarning("Data array is empty or response is null");
                            Debug.LogWarning("Response status: " + (response?.status ?? "null"));
                            Debug.LogWarning("Response message: " + (response?.message ?? "null"));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to parse JSON response: " + e.Message);
                    }

                    // 解析返回的JSON数据
                    //ProcessResponse(jsonResponse);
                }
            }
        }
        private IEnumerator UploadCoroutine(string path, string developer, string uploader)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            WWWForm form = new WWWForm();
            form.AddField("developer", developer);
            form.AddField("uploader", uploader);
            form.AddBinaryData("files", fileBytes, $"{gameName + "_" + version}.zip", "application/zip");

            using (UnityWebRequest www = UnityWebRequest.Post("http://10.80.1.5/oapi/htmlOrigin", form))
            {
                www.SetRequestHeader("Accept", "*/*");
                www.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
                www.SetRequestHeader("User-Agent", "PostmanRuntime-ApipostRuntime/1.1.0");
                www.SetRequestHeader("Cache-Control", "no-cache");
                yield return www.SendWebRequest();


                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error: {www.error}");
                }
                else
                {
                    EditorUtility.DisplayDialog("上传成功", "您已经上传成功！", "确定");
                    //刷新最新版本
                    EditorCoroutineUtility.StartCoroutineOwnerless(PostRequest(gameName));
                    Debug.Log("File uploaded successfully!");
                }
            }
        }


        /// <summary>
        /// 将https链接置空
        /// </summary>
        /// <param name="rootNode"></param>
        private void ReplaceHttpsAndSetEmpty(HtmlNode rootNode)
        {
            var TTAndPangleSettingsNode = rootNode.FindNode(rootNode, "script", "document.location.search.toLowerCase().indexOf(\"spector\")");
            var TTAndPangleSettingsText = TTAndPangleSettingsNode.InnerText;
            TTAndPangleSettingsText = TTAndPangleSettingsText.Replace("https://spectorcdn.babylonjs.com/spector.bundle.js", "");
            TTAndPangleSettingsText = TTAndPangleSettingsText.Replace("https://mrdoob.github.io/stats.js/build/stats.min.js", "");
            TTAndPangleSettingsNode.InnerText = TTAndPangleSettingsText;

            var TTAndPangleSettingsNode02 = rootNode.FindNode(rootNode, "script", "window.pi.apply");
            var TTAndPangleSettingsText02 = TTAndPangleSettingsNode02.InnerText;
            TTAndPangleSettingsText02 = TTAndPangleSettingsText02.Replace("\"https://collector.lunalabs.io/api/v1/stats/collect\"", "");
            TTAndPangleSettingsText02 = TTAndPangleSettingsText02.Replace("\"https://collector.lunalabs.io/api/v1/stats/errors/collect\"", "");
            TTAndPangleSettingsNode02.InnerText = TTAndPangleSettingsText02;
        }
        private void CopyDirectory(string sourceDir, string destDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.Name.EndsWith(".meta"))
                {
                    string tempPath = Path.Combine(destDir, file.Name);
                    file.CopyTo(tempPath, true);
                }
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDir, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }
        private void DeleteDirectory(string dirPath)
        {
            // 检查目录是否存在
            if (Directory.Exists(dirPath))
            {
                // 删除所有文件
                foreach (FileInfo file in new DirectoryInfo(dirPath).GetFiles())
                {
                    file.Delete();
                }

                // 递归删除所有子目录
                foreach (DirectoryInfo subDir in new DirectoryInfo(dirPath).GetDirectories())
                {
                    DeleteDirectory(subDir.FullName);
                }

                // 删除空目录
                Directory.Delete(dirPath);
            }
            else
            {
                Console.WriteLine($"目录 {dirPath} 不存在!");
            }
        }
        private void ScanZipFiles()
        {
            zipFiles.Clear();
            if (Directory.Exists(folderPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                zipFiles.AddRange(dirInfo.GetFiles("*.zip"));
                if (zipFiles.Count > 0)
                {
                    selectedZipPath = zipFiles[0].FullName;
                    zipPath = selectedZipPath;
                }
            }
        }

        private void HandleDragAndDrop(Rect dropArea)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            MonoScript script = draggedObject as MonoScript;
                            if (script != null && !scriptList.Contains(script))
                            {
                                scriptList.Add(script);
                                SavePreferences(); // Save immediately when script is added
                            }
                        }
                    }

                    Event.current.Use();
                    break;
            }
        }

        private JObject ProcessConfigManager(Type configManagerType)
        {
            // Create an instance of ConfigManager
            MonoBehaviour instance = new GameObject().AddComponent(configManagerType) as MonoBehaviour;

            JObject classData = new JObject();
            FieldInfo[] fields = configManagerType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                object[] attributes = field.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    // if (attribute is LunaPlaygroundFieldAttribute || attribute is LunaPlaygroundAssetAttribute)
                    if (attribute is LunaPlaygroundFieldAttribute)
                    {
                        string fieldType = GetFieldType(field);
                        JArray fieldValue = GetFieldValue(field, instance);

                        JArray values = new JArray { fieldType };
                        values.AddRange(fieldValue);
                        classData.Add(field.Name, values);
                    }
                }
            }

            // jsonResult.Add(configManagerType.FullName, classData);
            // string compactJson = jsonResult.ToString(Formatting.None);
            // Debug.Log(compactJson);

            // Destroy the temporary GameObject
            if (instance != null) DestroyImmediate(instance.gameObject);

            return classData;
        }

        private string GetFieldType(FieldInfo field)
        {
            if (field.FieldType == typeof(bool))
            {
                return "boolean";
            }
            if (field.FieldType == typeof(float))
            {
                return "float";
            }

            if (field.FieldType == typeof(int))
            {
                return "int";
            }

            if (field.FieldType == typeof(string))
            {
                return "string";
            }

            if (field.FieldType == typeof(Color))
            {
                return "color";
            }

            if (field.FieldType == typeof(Texture2D))
            {
                return "texture2D";
            }

            Debug.LogError("无效类型");
            return "unknown";
        }

        private JArray GetFieldValue(FieldInfo field, MonoBehaviour instance)
        {
            object value = field.GetValue(instance);

            if (field.FieldType == typeof(bool))
            {
                return new JArray(((bool)value) ? 1 : 0);
            }

            if (field.FieldType == typeof(float))
            {
                return new JArray((float)value);
            }

            if (field.FieldType == typeof(int))
            {
                return new JArray((int)value);
            }

            if (field.FieldType == typeof(string))
            {
                return new JArray((string)value);
            }

            if (field.FieldType == typeof(Color))
            {
                Color color = (Color)value;
                return new JArray(color.r, color.g, color.b, color.a);
            }

            if (field.FieldType == typeof(Texture2D))
            {
                return new JArray(); // Just placeholder for the texture, as we can't serialize it directly into JSON
            }
            Debug.LogError("无效类型");
            return new JArray();
        }

        static void PrintDictionary(Dictionary<string, object> dictionary, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 2);
            foreach (var kvp in dictionary)
            {
                if (kvp.Value is Dictionary<string, object> nestedDictionary)
                {
                    Debug.Log($"{indent}{kvp.Key}:");
                    PrintDictionary(nestedDictionary, indentLevel + 1);
                }
                else if (kvp.Value is List<Dictionary<string, object>> nestedList)
                {
                    Debug.Log($"{indent}{kvp.Key}:");
                    foreach (var item in nestedList)
                    {
                        PrintDictionary(item, indentLevel + 1);
                    }
                }
                else
                {
                    Debug.Log($"{indent}{kvp.Key}: {kvp.Value}");
                }
            }
        }

        private bool UnzipFile()
        {
            // 检查文件是否存在
            if (File.Exists(zipPath))
            {
                // 如果解压目录存在则先删除
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }

                // 创建解压目录（如果未删除成功则报错）
                Directory.CreateDirectory(extractPath);

                // 解压 .zip 文件
                ZipFile.ExtractToDirectory(zipPath, extractPath);

                // 刷新 Unity Asset 数据库，以便显示新解压的文件
                AssetDatabase.Refresh();

                Debug.Log("解压完成！");
                return true;
            }
            else
            {
                Debug.LogWarning("指定的 .zip 文件不存在！");
            }

            return false;
        }

        public List<HtmlNode> GetAllJsNodes()
        {
            string cachePath = Path.Combine(extractPath, "cache");
            // 遍历cachePath文件夹下面的所有js文件
            var jsFiles = Directory.GetFiles(cachePath, "*.js", SearchOption.AllDirectories).ToList();

            List<HtmlNode> htmlNodes = new List<HtmlNode>();
            foreach (var jsFile in jsFiles)
            {
                // jsFile文件的内容
                string jsFileContent = File.ReadAllText(jsFile);
                HtmlNode htmlNode = HtmlNode.Create("script", new Dictionary<string, string> { ["data-startup-onlyyy"] = null }, jsFileContent);
                htmlNodes.Add(htmlNode);
            }

            return htmlNodes;
        }

        public List<HtmlNode> GetAllImgNodes()
        {
            string cachePath = Path.Combine(extractPath, "assets");
            List<HtmlNode> htmlNodes = new List<HtmlNode>();
            // 遍历cachePath文件夹下面的所有js文件

            var pngFiles = Directory.GetFiles(cachePath, "*.png", SearchOption.AllDirectories).ToList();
            foreach (var filePath in pngFiles)
            {
                // 读取PNG文件的内容
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // 将字节数组编码为Base64字符串
                string base64Data = Convert.ToBase64String(fileBytes);

                // 生成Data URI
                string dataUri = $"data:image/png;base64,{base64Data}";

                // filePath 是jsFile的全路径去掉extractPath
                var fileName = filePath.Replace(extractPath, "").Substring(1);
                fileName = fileName.Replace("\\", "/");

                var attributes = new Dictionary<string, string>();
                attributes["id"] = fileName;
                attributes["src"] = dataUri;
                attributes["style"] = "display:none";
                attributes["crossorigin"] = "";
                HtmlNode htmlNode = HtmlNode.Create("img", attributes);
                htmlNodes.Add(htmlNode);
            }

            var mp4Files = Directory.GetFiles(cachePath, "*.mp4", SearchOption.AllDirectories).ToList();
            foreach (var filePath in mp4Files)
            {
                // 读取PNG文件的内容
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // 将字节数组编码为Base64字符串
                string base64Data = Convert.ToBase64String(fileBytes);

                // 生成Data URI
                string dataUri = $"data:video/mp4;base64,{base64Data}";

                // filePath 是jsFile的全路径去掉extractPath
                var fileName = filePath.Replace(extractPath, "").Substring(1);
                fileName = fileName.Replace("\\", "/");

                var attributes = new Dictionary<string, string>();
                attributes["id"] = fileName;
                attributes["src"] = dataUri;
                attributes["style"] = "display:none";

                attributes["preload"] = "auto";
                attributes["autoload"] = "";
                attributes["muted"] = "";
                attributes["playsinline"] = "";

                attributes["crossorigin"] = "";
                HtmlNode htmlNode = HtmlNode.Create("video", attributes);
                htmlNodes.Add(htmlNode);
            }




            var jpgFiles = Directory.GetFiles(cachePath, "*.jpeg", SearchOption.AllDirectories).ToList();
            foreach (var filePath in jpgFiles)
            {
                // 读取PNG文件的内容
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // 将字节数组编码为Base64字符串
                string base64Data = Convert.ToBase64String(fileBytes);

                // 生成Data URI
                string dataUri = $"data:image/jpeg;base64,{base64Data}";

                // filePath 是jsFile的全路径去掉extractPath
                var fileName = filePath.Replace(extractPath, "").Substring(1);
                fileName = fileName.Replace("\\", "/");

                var attributes = new Dictionary<string, string>();
                attributes["id"] = fileName;
                attributes["src"] = dataUri;
                attributes["style"] = "display:none";
                attributes["crossorigin"] = "";
                HtmlNode htmlNode = HtmlNode.Create("img", attributes);
                htmlNodes.Add(htmlNode);
            }

            return htmlNodes;
        }

        private string BrotliScriptsJs()
        {
            return
                @"window.makeBrotliDecodeStr='()=>{function tb(b){this.data=b;this.g=0}function Pa(b){let d=-1,c=16;for(;0<c;)0!=b>>>c&&(d+=c,b>>>=c),c>>=1;return d+b}function Qa(b,d,c){b=(b-c>>d)+4;const g=Pa(b)-1;return((g-1<<1|b>>g&1)-1<<d)+(1<<d)+c+16}function ub(b){const d=b.ia;b.ia=0;16<=b.g&&(b.h=b.m[b.i++]<<16|b.h>>>16,b.g-=16);if(0==p(b,1))return 16;let c=p(b,3);if(0!=c)return 17+c;c=p(b,3);if(0!=c){if(1==c){if(0==d)return-1;b.ia=1;if(1==p(b,1))return-1;c=p(b,6);return 10>c||30<c?-1:c}return 8+c}return 17}function ja(b){16<=\nb.g&&(b.h=b.m[b.i++]<<16|b.h>>>16,b.g-=16);if(0!=p(b,1)){const d=p(b,3);return 0==d?1:p(b,d)+(1<<d)}return 0}function T(b,d,c){d=b[d];const g=c.h>>>c.g;d+=g&255;const e=b[d]>>16,m=b[d]&65535;if(8>=e)return c.g+=e,m;d=d+m+((g&(1<<e)-1)>>>8);c.g+=(b[d]>>16)+8;return b[d]&65535}function Ra(b,d,c){16<=c.g&&(c.h=c.m[c.i++]<<16|c.h>>>16,c.g-=16);b=T(b,d,c);d=vb[b];16<=c.g&&(c.h=c.m[c.i++]<<16|c.h>>>16,c.g-=16);return wb[b]+(16>=d?p(c,d):ka(c,d))}function la(b,d,c,g,e){2030<e.i&&H(e);16<=e.g&&(e.h=e.m[e.i++]<<\n16|e.h>>>16,e.g-=16);var m=p(e,2);if(1==m){var a=new Int32Array(d),h=new Int32Array(4),k=1+Pa(b-1);b=p(e,2)+1;for(m=0;m<b;m++){16<=e.g&&(e.h=e.m[e.i++]<<16|e.h>>>16,e.g-=16);var A=p(e,k);h[m]=A}k=b;4==b&&(k+=p(e,1));switch(k){case 1:a[h[0]]=1;break;case 2:a[h[0]]=1;a[h[1]]=1;break;case 3:a[h[0]]=1;a[h[1]]=2;a[h[2]]=2;break;case 4:a[h[0]]=2;a[h[1]]=2;a[h[2]]=2;a[h[3]]=2;break;case 5:a[h[0]]=1,a[h[1]]=2,a[h[2]]=3,a[h[3]]=3}d=ta(c,g,8,a,d)}else{a=new Int32Array(d);h=new Int32Array(18);b=32;for(k=0;18>\nm&&0<b;m++){A=xb[m];16<=e.g&&(e.h=e.m[e.i++]<<16|e.h>>>16,e.g-=16);var r=e.h>>>e.g&15;e.g+=Sa[r]>>16;r=Sa[r]&65535;h[A]=r;0!=r&&(b-=32>>r,k++)}var v=h;h=a;b=0;k=8;A=m=0;r=32768;const L=new Int32Array(33);for(ta(L,L.length-1,5,v,18);b<d&&0<r;){2030<e.i&&H(e);16<=e.g&&(e.h=e.m[e.i++]<<16|e.h>>>16,e.g-=16);v=e.h>>>e.g&31;e.g+=L[v]>>16;var f=L[v]&65535;if(16>f)m=0,h[b++]=f,0!=f&&(k=f,r-=32768>>f);else{v=f-14;let J=0;16==f&&(J=k);A!=J&&(m=0,A=J);f=m;0<m&&(m-=2,m<<=v);16<=e.g&&(e.h=e.m[e.i++]<<16|e.h>>>\n16,e.g-=16);m+=p(e,v)+3;v=m-f;for(f=0;f<v;f++)h[b++]=A;0!=A&&(r-=v<<15-A)}}h.fill(0,b,d);d=ta(c,g,8,a,d)}return d}function Ta(b,d,c){2030<c.i&&H(c);const g=ja(c)+1;if(1==g)return d.fill(0,0,b),g;16<=c.g&&(c.h=c.m[c.i++]<<16|c.h>>>16,c.g-=16);var e=0;0!=p(c,1)&&(e=p(c,4)+1);var m=g+e,a=new Int32Array(Ua[m+31>>5]+1),h=a.length-1;la(m,m,a,h,c);for(m=0;m<b;){2030<c.i&&H(c);16<=c.g&&(c.h=c.m[c.i++]<<16|c.h>>>16,c.g-=16);var k=T(a,h,c);if(0==k)d[m]=0,m++;else if(k<=e)for(16<=c.g&&(c.h=c.m[c.i++]<<16|c.h>>>\n16,c.g-=16),k=(1<<k)+p(c,k);0!=k;)d[m]=0,m++,k--;else d[m]=k-e,m++}16<=c.g&&(c.h=c.m[c.i++]<<16|c.h>>>16,c.g-=16);if(1==p(c,1)){c=new Int32Array(256);for(e=0;256>e;e++)c[e]=e;for(a=0;a<b;a++)if(h=d[a]&255,d[a]=c[h],0!=h){e=c;for(m=e[h];0<h;h--)e[h]=e[h-1];e[0]=m}}return g}function ua(b,d,c){const g=b.s,e=4+2*d;16<=b.g&&(b.h=b.m[b.i++]<<16|b.h>>>16,b.g-=16);let m=T(b.v,2*d,b);b=Ra(b.v,2*d+1,b);m=1==m?g[e+1]+1:0==m?g[e]:m-2;m>=c&&(m-=c);g[e]=g[e+1];g[e+1]=m;return b}function Va(b){b.M=ua(b,0,b.F);const d=\nb.s[5];b.da=d<<6;b.xa=b.Y[b.da]&255;b.X=b.ea[d]<<9;b.qa=b.X+256}function va(b,d,c){let g=b.v[2*d];if(1>=c)return b.v[2*d+1]=g,b.v[2*d+2]=g,268435456;c+=2;g+=la(c,c,b.v,2*d,b);b.v[2*d+1]=g;g+=la(26,26,b.v,2*d+1,b);b.v[2*d+2]=g;return Ra(b.v,2*d+1,b)}function wa(b,d,c,g){const e=new Int32Array(c+c*Ua[d+31>>5]);let m=c;for(let a=0;a<c;++a)e[a]=m,m+=la(b,d,e,a,g);return e}function Wa(b,d){for(d=1<<d-1;0!=(b&d);)d>>=1;return(b&d-1)+d}function ta(b,d,c,g,e){d=b[d];const m=new Int32Array(e),a=new Int32Array(16);\nvar h=new Int32Array(16);var k;for(k=0;k<e;k++)a[g[k]]++;h[1]=0;for(k=1;15>k;k++)h[k+1]=h[k]+a[k];for(k=0;k<e;k++)0!=g[k]&&(m[h[g[k]]++]=k);g=e=1<<c;if(1==h[15]){for(h=0;h<g;h++)b[d+h]=m[0];return g}k=h=0;for(let E=1,u=2;E<=c;E++,u<<=1)for(;0<a[E];a[E]--){var A=b,r=d+h,v=u;var f=e;var L=E<<16|m[k++];do f-=v,A[r+f]=L;while(0<f);h=Wa(h,E)}A=g-1;v=-1;r=d;for(let E=c+1,u=2;15>=E;E++,u<<=1)for(;0<a[E];a[E]--){if((h&A)!=v){r+=e;e=E;for(v=1<<e-c;15>e;){v-=a[e];if(0>=v)break;e++;v<<=1}f=e-c;e=1<<f;g+=e;v=\nh&A;b[d+v]=f+c<<16|r-d-v}f=b;L=r+(h>>c);var J=u,N=e,C=E-c<<16|m[k++];do N-=J,f[L+N]=C;while(0<N);h=Wa(h,E)}return g}function H(b){if(!(0!=b.ga&&-2<=xa(b))){var d=b.i<<1,c=4096-d;b.T.copyWithin(0,d,4096);for(b.i=0;4096>c;){d=Xa(b.R,b.T,c,4096-c);if(0>=d){b.ga=1;b.Ea=c;c+=1;break}c+=d}d=b.T;c>>=1;b=b.m;for(let g=0;g<c;++g)b[g]=d[2*g]&255|(d[2*g+1]&255)<<8}}function p(b,d){const c=b.h>>>b.g&(1<<d)-1;b.g+=d;return c}function ka(b,d){const c=p(b,16);b.h=b.m[b.i++]<<16|b.h>>>16;b.g-=16;return c|p(b,d-16)<<\n16}function ya(b){2030<b.i&&H(b);b.h=b.m[b.i++]<<16|b.h>>>16;b.g-=16;b.h=b.m[b.i++]<<16|b.h>>>16;b.g-=16}function Ya(b){const d=32-b.g&7;0!=d&&p(b,d)}function xa(b){let d=2048;0!=b.ga&&(d=b.Ea+1>>1);return d-b.i}function yb(){this.B=new Int8Array(0);this.ea=new Int8Array(0);this.Y=new Int8Array(0);this.ra=new Int8Array(0);this.ta=new Int8Array(0);this.Ba=new Int8Array(0);this.T=new Int8Array(0);this.m=new Int16Array(0);new Int32Array(0);this.s=new Int32Array(0);this.v=new Int32Array(0);this.ja=new Int32Array(0);\nthis.oa=new Int32Array(0);this.va=new Int32Array(0);this.ua=new Int32Array(0);this.U=this.na=this.I=this.V=this.ia=this.Da=this.O=this.H=this.ya=this.Ca=this.K=this.u=this.P=this.N=this.Z=this.D=this.J=this.aa=this.Aa=this.qa=this.X=this.sa=this.da=this.ha=this.A=this.pa=this.xa=this.za=this.C=this.S=this.j=this.ba=this.fa=this.ka=this.ca=this.F=this.M=this.L=this.wa=this.$=this.o=this.ga=this.Ea=this.i=this.g=this.h=this.G=this.l=0;this.Fa=[];this.W=new Int32Array(0);this.la=0;this.ma=new Int8Array(0);\nthis.R=null;this.B=new Int8Array(0);this.s=new Int32Array(10);this.s[0]=16;this.s[1]=15;this.s[2]=11;this.s[3]=4}function U(b,d){return b<=d?b:d}function Xa(b,d,c,g){if(null==b)return-1;g=U(b.g+g,b.data.length);const e=g-b.g;d.set(b.data.subarray(b.g,g),c);b.g+=e;return e}const Ua=Int32Array.from([256,402,436,468,500,534,566,598,630,662,694,726,758,790,822,854,886,920,952,984,1016,1048,1080]),xb=Int32Array.from([1,2,3,4,0,5,17,6,16,7,8,9,10,11,12,13,14,15]),zb=Int32Array.from([0,3,2,1,0,0,0,0,0,0,\n3,3,3,3,3,3]),Ab=Int32Array.from([0,0,0,0,-1,1,-2,2,-3,3,-1,1,-2,2,-3,3]),Sa=Int32Array.from([131072,131076,131075,196610,131072,131076,131075,262145,131072,131076,131075,196610,131072,131076,131075,262149]),wb=Int32Array.from([1,5,9,13,17,25,33,41,49,65,81,97,113,145,177,209,241,305,369,497,753,1265,2289,4337,8433,16625]),vb=Int32Array.from([2,2,2,2,3,3,3,3,4,4,4,4,5,5,5,5,6,6,7,8,9,10,11,12,13,24]),Za=Int16Array.from([0,0,0,0,0,0,1,1,2,2,3,3,4,4,5,5,6,7,8,9,10,12,14,24]),$a=Int16Array.from([0,0,\n0,0,0,0,0,0,1,1,2,2,3,3,4,4,5,5,6,7,8,9,10,24]),fa=new Int16Array(2816);(function(b){const d=new Int16Array(24),c=new Int16Array(24);c[0]=2;for(var g=0;23>g;++g)d[g+1]=d[g]+(1<<Za[g]),c[g+1]=c[g]+(1<<$a[g]);for(g=0;704>g;++g){var e=g>>>6,m=-4;2<=e&&(e-=2,m=0);const h=(170064>>>2*e&3)<<3|g>>>3&7;e=(156228>>>2*e&3)<<3|g&7;var a=c[e];m+=4<a?3:a-2;a=4*g;b[a]=Za[h]|$a[e]<<8;b[a+1]=d[h];b[a+2]=c[e];b[a+3]=m}})(fa);let ma=new function(b,d,c){this.i=new Int32Array(0);this.h=new Int8Array(0);this.g=new Int32Array(0);\nthis.j=new Int16Array(0);this.i=new Int32Array(3*b);this.j=new Int16Array(b);this.h=new Int8Array(d);this.g=new Int32Array(c+1)}(121,167,50);(function(b,d,c,g,e){const m=g.length;let a=1,h=0;for(let k=0;k<m;++k){const A=g.charCodeAt(k);35==A?d[a++]=h:b[h++]=A}for(b=0;363>b;++b)c[b]=e.charCodeAt(b)-32})(ma.h,ma.g,ma.i,\'# #s #, #e #.# the #.com/#\\u00c2\\u00a0# of # and # in # to #""#"">#\\n#]# for # a # that #. # with #\\\'# from # by #. The # on # as # is #ing #\\n\\t#:#ed #(# at #ly #=""# of the #. This #,# not #er #al #=\\\'#ful #ive #less #est #ize #ous #\',\n""     !! ! ,  *!  &!  \\"" !  ) *   * -  ! # !  #!*!  +  ,$ !  -  %  .  / #   0  1 .  \\""   2  3!*   4%  ! # /   5  6  7  8 0  1 &   $   9 +   :  ;  < \'  !=  >  ?! 4  @ 4  2  &   A *# (   B  C& ) %  ) !*# *-% A +! *.  D! %\'  & E *6  F  G% ! *A *%  H! D  I!+!  J!+   K +- *4! A  L!*4  M  N +6  O!*% +.! K *G  P +%(  ! G *D +D  Q +# *K!*G!+D!+# +G +A +4!+% +K!+4!*D!+K!*K"");let za=new Int32Array(2048);(function(b,d,c){for(var g=0;256>g;++g)b[g]=g&63,b[512+g]=g>>2,b[1792+g]=2+(g>>6);for(g=0;128>g;++g)b[1024+\ng]=4*(d.charCodeAt(g)-32);for(d=0;64>d;++d)b[1152+d]=d&1,b[1216+d]=2+(d&1);d=1280;for(g=0;19>g;++g){const e=g&3,m=c.charCodeAt(g)-32;for(let a=0;a<m;++a)b[d++]=e}for(c=0;16>c;++c)b[1792+c]=1,b[2032+c]=6;b[1792]=0;b[2047]=7;for(c=0;256>c;++c)b[1536+c]=b[1792+c]<<3})(za,""         !!  !                  \\""#$##%#$&\'##(#)#++++++++++((&*\'##,---,---,-----,-----,-----&#\'###.///.///./////./////./////&#\'# "",""A/*  \':  & : $  \\u0081 @"");let ab=new Int32Array(32),Aa=new Int32Array(32);const bb=new Int32Array(25);\nfor(let b=0;25>b;++b)bb[b]=""AAAAKKLLKKKKKJJIHHIHHGGFF"".charCodeAt(b)-65;(function(b){Aa.set(b.subarray(0,b.length),0);let d=0;for(let c=0;c<b.length;++c){ab[c]=d;const g=Aa[c];0!=g&&(d+=c<<g)}for(b=b.length;32>b;++b)ab[b]=d})(bb);return function(b){const d=new yb;var c=new tb(b);d.v=new Int32Array(3091);d.v[0]=7;d.C=3;const g=Qa(2147483644,3,120);d.ta=new Int8Array(g);d.ua=new Int32Array(g);d.R=c;d.T=new Int8Array(4160);d.h=0;d.m=new Int16Array(2080);d.g=32;d.i=2048;d.ga=0;ya(d);d.l=1;let e=0;const m=\n[];for(;;){const aa=new Int8Array(16384);m.push(aa);d.Ba=aa;d.Ca=0;d.ya=16384;d.H=0;a:{var a=d;if(1==a.l){const Ba=ub(a);a.P=1<<Ba;a.N=a.P-16;a.l=2}let M=a.u,I=a.u-1,D=a.B;for(;10!=a.l;)switch(a.l){case 2:var h=a;if(0!=h.$)h.G=10,h.l=12;else{h.ja=new Int32Array(0);h.oa=new Int32Array(0);h.va=new Int32Array(0);2030<h.i&&H(h);b:{var k=h;16<=k.g&&(k.h=k.m[k.i++]<<16|k.h>>>16,k.g-=16);k.$=p(k,1);k.o=0;k.wa=0;k.L=0;if(0==k.$||0==p(k,1)){var A=p(k,2)+4;if(7==A){k.L=1;p(k,1);const l=p(k,2);if(0==l)break b;\nfor(let n=0;n<l;n++){16<=k.g&&(k.h=k.m[k.i++]<<16|k.h>>>16,k.g-=16);const x=p(k,8);k.o|=x<<8*n}}else for(let l=0;l<A;l++){16<=k.g&&(k.h=k.m[k.i++]<<16|k.h>>>16,k.g-=16);const n=p(k,4);k.o|=n<<4*l}k.o++;0==k.$&&(k.wa=p(k,1))}}if(0!=h.o||0!=h.L)if(0!=h.wa||0!=h.L?(Ya(h),h.l=0!=h.L?5:6):h.l=3,0==h.L&&(h.K+=h.o,1073741824<h.K&&(h.K=1073741824),h.u<h.P)){var r=h;let l=r.P;if(l>r.K){const n=r.K;for(;l>>1>n;)l>>=1;0==r.$&&16384>l&&16384<=r.P&&(l=16384)}if(!(l<=r.u)){var v=new Int8Array(l+37);0!=r.B.length&&\nv.set(r.B.subarray(0,r.u),0);r.B=v;r.u=l}}}M=a.u;I=a.u-1;D=a.B;continue;case 3:var f=a;f.F=ja(f)+1;f.M=va(f,0,f.F);f.ka=ja(f)+1;f.ca=va(f,1,f.ka);f.ba=ja(f)+1;f.fa=va(f,2,f.ba);2030<f.i&&H(f);16<=f.g&&(f.h=f.m[f.i++]<<16|f.h>>>16,f.g-=16);f.J=p(f,2);f.aa=p(f,4)<<f.J;f.ea=new Int8Array(f.F);for(let l=0;l<f.F;){const n=U(l+96,f.F);for(;l<n;++l)16<=f.g&&(f.h=f.m[f.i++]<<16|f.h>>>16,f.g-=16),f.ea[l]=p(f,2);2030<f.i&&H(f)}f.Y=new Int8Array(f.F<<6);const Ba=Ta(f.F<<6,f.Y,f);f.za=1;for(let l=0;l<f.F<<6;l++)if(f.Y[l]!=\nl>>6){f.za=0;break}f.ra=new Int8Array(f.ba<<2);const Bb=Ta(f.ba<<2,f.ra,f);f.ja=wa(256,256,Ba,f);f.oa=wa(704,704,f.ka,f);let Ca=16+f.aa+2*(24<<f.J),Da=Ca;1==f.ia&&(Ca=16+f.aa+2*(62<<f.J),Da=Qa(2147483644,f.J,f.aa));f.va=wa(Ca,Da,Bb,f);var L=Da;const cb=f.ta,db=f.ua,eb=f.J,fb=f.aa,Cb=1<<eb;let Ea=1,Fa=0,V=16;for(let l=0;l<fb;++l)cb[V]=0,db[V]=l+1,++V;for(;V<L;){const l=fb+((2+Fa<<Ea)-4<<eb)+1;for(let n=0;n<Cb;++n)cb[V]=Ea,db[V]=l+n,++V;Ea+=Fa;Fa^=1}f.da=0;f.sa=0;f.X=512*f.ea[0];f.qa=f.X+256;f.xa=0;\nf.pa=0;f.s[4]=1;f.s[5]=0;f.s[6]=1;f.s[7]=0;f.s[8]=1;f.s[9]=0;a.l=4;case 4:if(0>=a.o){a.l=2;continue}2030<a.i&&H(a);if(0==a.ca){var J=a;J.ca=ua(J,1,J.ka);J.pa=J.s[7]}a.ca--;16<=a.g&&(a.h=a.m[a.i++]<<16|a.h>>>16,a.g-=16);const na=T(a.oa,a.pa,a)<<2,gb=fa[na],Db=fa[na+1],Eb=fa[na+2];a.Aa=fa[na+3];16<=a.g&&(a.h=a.m[a.i++]<<16|a.h>>>16,a.g-=16);const Ga=gb&255;a.ha=Db+(16>=Ga?p(a,Ga):ka(a,Ga));16<=a.g&&(a.h=a.m[a.i++]<<16|a.h>>>16,a.g-=16);const Ha=gb>>8;a.Z=Eb+(16>=Ha?p(a,Ha):ka(a,Ha));a.A=0;a.l=7;case 7:if(0!=\na.za)for(;a.A<a.ha;){if(2030<a.i&&H(a),0==a.M&&Va(a),a.M--,16<=a.g&&(a.h=a.m[a.i++]<<16|a.h>>>16,a.g-=16),D[a.j]=T(a.ja,a.xa,a),a.j++,a.A++,a.j>=M){a.G=7;a.l=12;break}}else{let l=D[a.j-1&I]&255,n=D[a.j-2&I]&255;for(;a.A<a.ha;){2030<a.i&&H(a);0==a.M&&Va(a);const x=a.Y[a.da+(za[a.X+l]|za[a.qa+n])]&255;a.M--;n=l;16<=a.g&&(a.h=a.m[a.i++]<<16|a.h>>>16,a.g-=16);l=T(a.ja,x,a);D[a.j]=l;a.j++;a.A++;if(a.j>=M){a.G=7;a.l=12;break}}}if(7!=a.l)continue;a.o-=a.ha;if(0>=a.o){a.l=4;continue}let O=a.Aa;if(0>O)a.D=\na.s[a.C];else{2030<a.i&&H(a);if(0==a.fa){var N=a;N.fa=ua(N,2,N.ba);N.sa=N.s[9]<<2}a.fa--;16<=a.g&&(a.h=a.m[a.i++]<<16|a.h>>>16,a.g-=16);O=T(a.va,a.ra[a.sa+O]&255,a);if(16>O)a.D=a.s[a.C+zb[O]&3]+Ab[O];else{const l=a.ta[O];let n;32>=a.g+l?n=p(a,l):(16<=a.g&&(a.h=a.m[a.i++]<<16|a.h>>>16,a.g-=16),n=16>=l?p(a,l):ka(a,l));a.D=a.ua[O]+(n<<a.J)}}a.S=a.S!=a.N&&a.j<a.N?a.j:a.N;if(a.D>a.S){a.l=9;continue}0<O&&(a.C=a.C+1&3,a.s[a.C]=a.D);a.A=0;a.l=8;case 8:let W=a.j-a.D&I,X=a.j;const Y=a.Z-a.A,Ia=W+Y,hb=X+Y;if(Ia<\nI&&hb<I){if(12>Y||Ia>X&&hb>W)for(let l=0;l<Y;l+=4)D[X++]=D[W++],D[X++]=D[W++],D[X++]=D[W++],D[X++]=D[W++];else D.copyWithin(X,W,Ia);a.A+=Y;a.o-=Y;a.j+=Y}else for(;a.A<a.Z;)if(D[a.j]=D[a.j-a.D&I],a.o--,a.j++,a.A++,a.j>=M){a.G=8;a.l=12;break}8==a.l&&(a.l=4);continue;case 9:var C=a,E=M;const Ja=C.D-C.S-1;if(0>Ja){var u=C,Ka=-Ja-1,ib=C.Z;if(-1==u.la){u.ma=new Int8Array(256);let n=8;for(;0!=-1>>>n;)n++;n-=8;u.la=n;let x=0,ba=0;for(;0>x;){for(;u.W[ba+1]<x;)ba++;u.ma[x>>>n]=ba;x+=1<<n}}let l=u.ma[Ka>>>u.la];\nfor(;Ka>=u.W[l+1];)l++;u.C=u.C+1&3;u.s[u.C]=u.D;u.o-=ib;u.V=l;u.I=Ka-u.W[l];u.na=ib;u.U=0;C.l=14}else{const l=C.Z;var t=C.B,jb=C.j,B=l,oa=ma,kb=Ja>>>Aa[l];let n=jb;const x=oa.i,ba=oa.h,pa=oa.g,La=3*kb,lb=x[La],Q=x[La+1],mb=x[La+2];let nb=pa[lb];const Fb=pa[lb+1];let ob=pa[mb];const Gb=pa[mb+1];let ca=Q-11,qa=Q;if(1>ca||9<ca)ca=0;if(1>qa||9<qa)qa=0;for(;nb!=Fb;)t[n++]=ba[nb++];ca>B&&(ca=B);B-=ca;let pb=B-=qa;for(;0<pb;)t[n++]=0,pb--;if(10==Q||11==Q){let q=n-B;for(10==Q&&(B=1);0<B;){const Z=t[q]&255;\n192>Z?(97<=Z&&122>=Z&&(t[q]^=32),q+=1,--B):224>Z?(t[q+1]^=32,q+=2,B-=2):(t[q+2]^=5,q+=3,B-=3)}}else if(21==Q||22==Q){let q=n-B;const Z=oa.j[kb];let G=(Z&32767)+(16777216-(Z&32768));for(;0<B;){let R=1;const P=t[q]&255;if(128>P)G+=P,t[q]=G&127;else if(!(192>P))if(224>P)if(2<=B){const S=t[q+1];G+=S&63|(P&31)<<6;t[q]=192|G>>6&31;t[q+1]=S&192|G&63;R=2}else R=B;else if(240>P)if(3<=B){const S=t[q+1],ha=t[q+2];G+=ha&63|(S&63)<<6|(P&15)<<12;t[q]=224|G>>12&15;t[q+1]=S&192|G>>6&63;t[q+2]=ha&192|G&63;R=3}else R=\nB;else if(248>P)if(4<=B){const S=t[q+1],ha=t[q+2],qb=t[q+3];G+=qb&63|(ha&63)<<6|(S&63)<<12|(P&7)<<18;t[q]=240|G>>18&7;t[q+1]=S&192|G>>12&63;t[q+2]=ha&192|G>>6&63;t[q+3]=qb&192|G&63;R=4}else R=B;q+=R;B-=R;21==Q&&(B=0)}}for(;ob!=Gb;)t[n++]=ba[ob++];const rb=n-jb;C.j+=rb;C.o-=rb;C.j>=E?(C.G=4,C.l=12):C.l=4}continue;case 14:var Hb=a,Ib=a.j,y=a,sb=M;let da=y.j;const Jb=da;for(;y.na!=y.U;){const l=sb-da,n=y.W[y.V+1]-y.W[y.V]-y.I;let x=y.na-y.U;x>n&&(x=n);x>l&&(x=l);y.B.set(y.Fa[y.V].slice(y.I,y.I+x),da);\nda+=x;y.I+=x;y.U+=x;x==n&&(y.V++,y.I=0);if(da>=sb)break}Hb.j=Ib+(da-Jb);if(a.j>=M){a.G=14;a.l=12;break a}a.l=4;continue;case 5:for(;0<a.o;)2030<a.i&&H(a),16<=a.g&&(a.h=a.m[a.i++]<<16|a.h>>>16,a.g-=16),p(a,8),a.o--;a.l=2;continue;case 6:var z=a;const Kb=z.B;if(0>=z.o)32==z.g&&ya(z),z.l=2;else{for(var Ma=U(z.u-z.j,z.o),w=z,ra=Kb,ea=z.j,K=Ma;32!=w.g&&0!=K;)ra[ea++]=w.h>>>w.g,w.g+=8,K--;if(0!=K){var Na=U(xa(w),K>>1);if(0<Na){const l=w.i<<1,n=Na<<1;ra.set(w.T.subarray(l,l+n),ea);ea+=n;K-=n;w.i+=Na}if(0!=\nK)if(0<xa(w))for(16<=w.g&&(w.h=w.m[w.i++]<<16|w.h>>>16,w.g-=16);0!=K;)ra[ea++]=w.h>>>w.g,w.g+=8,K--;else for(;0<K;){const l=Xa(w.R,ra,ea,K);ea+=l;K-=l}}z.o-=Ma;z.j+=Ma;z.j==z.u?(z.G=6,z.l=12):(32==z.g&&ya(z),z.l=2)}continue;case 12:a.Da=U(a.j,a.u),a.l=13;case 13:var F=a;const sa=U(F.ya-F.H,F.Da-F.O);0!=sa&&(F.Ba.set(F.B.subarray(F.O,F.O+sa),F.Ca+F.H),F.H+=sa,F.O+=sa);if(0==(F.H<F.ya?1:0))break a;a.j>=a.N&&(a.S=a.N);a.j>=a.u&&(a.j>a.u&&D.copyWithin(0,a.u,a.j),a.j&=I,a.O=0);a.l=a.G;continue;default:throw""Unexpected state ""+\na.l;}10==a.l&&Ya(a)}e+=d.H;if(16384>d.H)break}11!=d.l&&(d.l=11,null!=d.R&&(d.R=null));const Oa=new Int8Array(e);let ia=0;for(let aa=0;aa<m.length;++aa){const M=m[aa],I=U(e,ia+16384)-ia;16384>I?Oa.set(M.subarray(0,I),ia):Oa.set(M,ia);ia+=I}return Oa}}'";
        }

        private string Base64ScriptsJs()
        {
            return
                @"function _base64ToArrayBuffer(e){for(var r=window.atob(e),n=r.length,o=new Uint8Array(n),t=0;t<n;t++)o[t]=r.charCodeAt(t);return o}function _initParallelDecompression(){window._usingWebWorkers=!0;const e=`\n              const unbrotli = (${window.makeBrotliDecodeStr})();\n              self.onmessage = async ( e ) => {\n                  const { buffer, length } = e.data;\n                  const result = await unbrotli( new Uint8Array( buffer, 0, length ) );\n      \n                  postMessage( { result: result.buffer }, [ result.buffer ] );\n              }\n          `,r=new Blob([e],{type:""application/javascript""});window._workerBlobUrl=URL.createObjectURL(r);const n=Math.max(Math.min(navigator.hardwareConcurrency||2,4),2);for(let e=0;e<n;e++)window._freeWorkers.push(new Worker(window._workerBlobUrl))}function _initSyncDecompression(){window._usingWebWorkers=!1,Function(`""use strict""; window.unbrotli = (${window.makeBrotliDecodeStr})()`)()}function _fetchFreeWorker(){return new Promise((e=>{0!==window._freeWorkers.length?e(window._freeWorkers.pop()):window._workerWaitingList.push(e)}))}function _freeWorker(e){0!==window._workerWaitingList.length?window._workerWaitingList.pop()(e):window._freeWorkers.push(e)}window.sharedBase122Buffer=null,(()=>{let e=0,r=0,n=0;const o=[0,10,13,34,38,92];function t(o,t){e|=(o<<=1)>>>r,r+=7,r>=8&&(t[n++]=e,r-=8,e=o<<7-r&255)}window._base122ToArrayBuffer=(s,i=!1)=>{const w=1.75*s.length|0;let a=null;i?((!window.sharedBase122Buffer||window.sharedBase122Buffer.length<w)&&(window.sharedBase122Buffer=new Uint8Array(w)),a=window.sharedBase122Buffer):a=new Uint8Array(w),e=0,r=0,n=0;for(let e=0;e<s.length;e++){const r=s.charCodeAt(e);if(r>127){const e=r>>>8&7;7!==e&&t(o[e],a),t(127&r,a)}else t(r,a)}return new Uint8Array(a.buffer,0,n)}})(),window._usingWebWorkers=!1,window._freeWorkers=[],window._workerWaitingList=[],window._workerBlobUrl=null,(()=>{const e=window.$environment&&window.$environment.usingWebWorkers||""off"";let r=!0;""off""===e?r=!1:""test""===e&&(r=murmurhash3_32_gc(window.pi.env.sessionId)%3==0);try{r?_initParallelDecompression():_initSyncDecompression()}catch(e){console.error(e),_initSyncDecompression()}finally{delete window.makeBrotliDecodeStr}})();const decompressUsingWebWorkers=function(e,r){return new Promise((n=>{_fetchFreeWorker().then((o=>{const t=r?_base122ToArrayBuffer(e):_base64ToArrayBuffer(e),s=t.buffer;o.postMessage({buffer:s,length:t.length},[s]);const i=e=>{o.removeEventListener(""message"",i),_freeWorker(o),n(new Uint8Array(e.data.result))};o.addEventListener(""message"",i)}))}))},assetToArrayBuffer=function(e,r){return r?_base122ToArrayBuffer(e):_base64ToArrayBuffer(e)},decompressInTheSameThread=function(e,r){return new Promise((async n=>{n(await unbrotli(assetToArrayBuffer(e,r)))}))},decompress=function(e,r){return window.lunaStartup&&(window.lunaStartup.timestamps.decompressionStarted=window.lunaStartup.timestamps.decompressionStarted||performance.now()),window._usingWebWorkers?decompressUsingWebWorkers(e,r):decompressInTheSameThread(e,r)},decompressArrayBuffer=function(e,r){return new Promise((n=>{decompress(e,r).then((({buffer:e})=>{n(e)})).catch((e=>console.error(e)))}))},decompressString=function(e,r){return new Promise((n=>{decompress(e,r).then((e=>{const r=new TextDecoder(""utf-8"").decode(e);n(r)})).catch((e=>console.error(e)))}))};window.addEventListener(""luna:ready"",(function(){Promise.all(window._compressedAssets).then((function(){window.lunaStartup&&(window.lunaStartup.timestamps.decompressionFinished=performance.now()),window._compressedAssets=null,window.unbrotli=null;for(const e of window._freeWorkers)e.terminate();window._freeWorkers=null,window._workerWaitingList=null,URL.revokeObjectURL(window._workerBlobUrl),window._workerBlobUrl=null,window.sharedBase122Buffer=null,window.dispatchEvent(new Event(""luna:build""))})).catch((e=>console.error(e)))}))";
        }

        private string LifeJs()
        {


            return
            @"!function(){var n=!1,e=!1;function t(){return mraid.isViewable()&&""hidden""!==mraid.getState()}function a(){n?t()&&e?(window.dispatchEvent(new Event(""luna:resume"")),e=!1):t()||e||(window.dispatchEvent(new Event(""luna:pause"")),e=!0):t()&&(window.dispatchEvent(new Event(""luna:start"")),n=!0)}function i(){}function d(n){window.dispatchEvent(new Event(n?""luna:unsafe:unmute"":""luna:unsafe:mute""))}var r=function(){""undefined""!=typeof mraid?(mraid.removeEventListener(""ready"",r),mraid.addEventListener(""viewableChange"",a),mraid.addEventListener(""stateChange"",a),mraid.addEventListener(""orientationchange"",i),mraid.addEventListener(""audioVolumeChange"",d),a()):window.dispatchEvent(new Event(""luna:start""))};window.addEventListener(""luna:build"",(function(){window.pi.logLoaded(),""undefined""!=typeof mraid?""loading""===mraid.getState()?mraid.addEventListener(""ready"",r):r():window.dispatchEvent(new Event(""luna:start""))}))}()";


        }

        private string EventJs()
        {

            return
                @"window.addEventListener(""luna:build"",(function(){Bridge.ready((function(){Luna.Unity.Playable.InstallFullGame=function(n,i){window.pi.logCta(),n=n||window.$environment.packageConfig.iosLink,i=i||window.$environment.packageConfig.androidLink;const o=/iphone|ipad|ipod|macintosh/i.test(window.navigator.userAgent.toLowerCase())?n:i;""undefined""!=typeof mraid?mraid.open(o):(console.warn(""Mraid is not defined""),window.open(o,""_blank""))}}))}))";

        }

        private string DebugJs()
        {
            return
                 //@"(()=>{let e=window.insertYourRemoteDebuggingTokenHere="""";if(e){const o=document.createElement(""script"");o.setAttribute(""src"",""./connector.js""),o.setAttribute(""data-channel"",e),o.setAttribute(""id"",""consolerescript""),document.head.appendChild(o)}window.devToolsAreOpen=!1;let o=0,t=()=>{if(e)return`Remote debugging is enabled, use the following link to access remote console: ./console.re/${e}`;return(o?`Logs are not displayed when dev tools are hidden. Please reload the game with dev tools opened to see ${o} hidden messages.\n`:"""")+""Learn how to debug your playable remotely at""};console.warn(Object.defineProperties(new Error,{toString:{value:()=>((new Error).stack.includes(""String@"")&&(window.devToolsAreOpen=!0),t())},message:{get:()=>(window.devToolsAreOpen=!0,t())},name:{get:()=>""[DEBUGGING]""}}));let n=new Promise((e=>{const o=()=>{console.re?e(console.re):setTimeout(o,100)};o()}));const s=[""debug"",""trace"",""info"",""log"",""warn"",""error""];for(const t of s){const s=console[t];console[t]=(...r)=>{r&&r[0]&&""string""==typeof r[0]&&r[0].startsWith(""consoleRe"")||(e&&n.then((e=>e[t].apply(console.re,r))),window.devToolsAreOpen?s.apply(console,r):o++)}}})()";
                 @"(()=>{let e=window.insertYourRemoteDebuggingTokenHere="""";if(e){const o=document.createElement(""script"");o.setAttribute(""src"",""""),o.setAttribute(""data-channel"",e),o.setAttribute(""id"",""consolerescript""),document.head.appendChild(o)}window.devToolsAreOpen=!1;let o=0,t=()=>{if(e)return`Remote debugging is enabled, use the following link to access remote console: http://console.re/${e}`;return(o?`Logs are not displayed when dev tools are hidden. Please reload the game with dev tools opened to see ${o} hidden messages.\n`:"""")+""Learn how to debug your playable remotely at https://docs.lunalabs.io/docs/playable/ad-networks/remote-debugging""};console.warn(Object.defineProperties(new Error,{toString:{value:()=>((new Error).stack.includes(""String@"")&&(window.devToolsAreOpen=!0),t())},message:{get:()=>(window.devToolsAreOpen=!0,t())},name:{get:()=>""[DEBUGGING]""}}));let n=new Promise((e=>{const o=()=>{console.re?e(console.re):setTimeout(o,100)};o()}));})()";

        }
        public enum Producer
        {
            黄霆伟,
            
            
        }
        public enum Platform
        {
            Unity,
            Applovin,
            TikTok,
            Pangle,
            FaceBook,
            Mintegral,
            Adwords,
        }

        // 工具类
        private void ProcessBase64String(string base64Input)
        {
            Debug.Log("Starting the decompression process...");

            try
            {
                // Convert Base64 string to byte array
                byte[] compressedData = Convert.FromBase64String(base64Input);
                Debug.Log("Decoded Base64 data length: " + compressedData.Length);

                byte[] decompressedData = DecompressBrotli(compressedData);
                if (decompressedData != null && decompressedData.Length > 0)
                {
                    string result = Encoding.UTF8.GetString(decompressedData);
                    Debug.Log("Decompressed Data: " + result);
                }
                else
                {
                    Debug.LogWarning("Decompression returned no data.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception during decompression: " + ex.Message);
            }
        }

        private byte[] DecompressBrotli(byte[] input)
        {
            try
            {
                using (var inputStream = new MemoryStream(input))
                using (var decompressionStream = new BrotliStream(inputStream, CompressionMode.Decompress))
                using (var outputStream = new MemoryStream())
                {
                    decompressionStream.CopyTo(outputStream);

                    // Convert the decompressed data to a byte array
                    byte[] decompressedData = outputStream.ToArray();
                    Debug.Log("Decompressed data length: " + decompressedData.Length);

                    // Log the raw decompressed data as a hex string for debugging
                    string hexDump = BitConverter.ToString(decompressedData).Replace("-", " ");
                    Debug.Log("Decompressed data (hex): " + hexDump);

                    return decompressedData;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Decompression failed: " + ex.Message);
                return null;
            }
        }
    }
}
