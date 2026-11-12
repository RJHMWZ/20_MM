using UnityEngine;
using UnityEditor;
using Match3;
using System.Collections.Generic;

namespace Match3.EditorScripts
{
    public class LevelEditorWindow : EditorWindow
    {
        private LevelData currentLevel;
        private ElementDatabase elementDatabase;
        private Vector2 mainScrollPos;
        private Vector2 paletteScroll;
        private readonly Dictionary<int, bool> refillSequenceFoldouts = new Dictionary<int, bool>();
        private List<int> copiedRefillSequence;
        
        // Palette
        private enum PaintMode { TogglePlayable, PaintRandomNormal, PaintSpecificNormal, PaintSpecial, PaintCollectible, PaintObstacle, PaintGenerator, Erase, PaintMother, PaintChild, PaintConveyor }
        private PaintMode paintMode = PaintMode.TogglePlayable;
        
        private int selectedNormalId = -1;
        private SpecialElementData selectedSpecial;
        
        private CollectibleElementData selectedCollectible;
        private ObstacleData selectedObstacle;
        private GeneratorData selectedGenerator;
        private MotherData selectedMother;
        private ChildData selectedChild;
        private Direction collectibleDir = Direction.Up;
        private int collectibleLen = 3;
        private Direction conveyorDir = Direction.Up;

        private const float CELL_SIZE = 40f;

        [MenuItem("Tools/Match3/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnEnable()
        {
            string[] guids = AssetDatabase.FindAssets("t:ElementDatabase");
            if (guids.Length > 0)
            {
                elementDatabase = AssetDatabase.LoadAssetAtPath<ElementDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
        }

        private void OnGUI()
        {
            mainScrollPos = EditorGUILayout.BeginScrollView(mainScrollPos);

            GUILayout.Label("Match 3 Level Editor", EditorStyles.boldLabel);

            currentLevel = (LevelData)EditorGUILayout.ObjectField("Level Data", currentLevel, typeof(LevelData), false);
            elementDatabase = (ElementDatabase)EditorGUILayout.ObjectField("Element Database", elementDatabase, typeof(ElementDatabase), false);

            if (currentLevel == null)
            {
                EditorGUILayout.HelpBox("Please select or create a Level Data asset to begin.", MessageType.Info);
                if (GUILayout.Button("Create New Level Data")) CreateNewLevelData();
                EditorGUILayout.EndScrollView();
                return;
            }

            if (elementDatabase == null)
            {
                EditorGUILayout.HelpBox("Please create and assign an Element Database to paint elements.", MessageType.Warning);
                if (GUILayout.Button("Create Element Database")) CreateElementDatabase();
            }

            EditorGUILayout.Space();
            DrawGridSettings();
            DrawRefillSequenceSettings();
            DrawTutorialSettings();

            EditorGUILayout.Space();
            if (elementDatabase != null)
            {
                DrawPalette();
            }

            EditorGUILayout.Space();
            DrawGrid();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(currentLevel);
            }

            EditorGUILayout.EndScrollView();
        }

        private void CreateNewLevelData()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Level Data", "NewLevel", "asset", "Save Level Data Asset");
            if (!string.IsNullOrEmpty(path))
            {
                LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();
                newLevel.ValidateGrid();
                AssetDatabase.CreateAsset(newLevel, path);
                AssetDatabase.SaveAssets();
                currentLevel = newLevel;
            }
        }

        private void CreateElementDatabase()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Element Database", "ElementDatabase", "asset", "Save Element Database Asset");
            if (!string.IsNullOrEmpty(path))
            {
                ElementDatabase newDb = ScriptableObject.CreateInstance<ElementDatabase>();
                AssetDatabase.CreateAsset(newDb, path);
                AssetDatabase.SaveAssets();
                elementDatabase = newDb;
            }
        }

        private void DrawGridSettings()
        {
            GUILayout.Label("Level Rules", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            int newSteps = EditorGUILayout.IntField("Max Steps", currentLevel.maxSteps);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(currentLevel, "Change Steps");
                currentLevel.maxSteps = Mathf.Max(1, newSteps);
            }

            EditorGUILayout.Space();
            GUILayout.Label("Level Targets", EditorStyles.boldLabel);
            
            SerializedObject serializedObject = new SerializedObject(currentLevel);
            serializedObject.Update();
            SerializedProperty levelTargetsProp = serializedObject.FindProperty("levelTargets");
            EditorGUILayout.PropertyField(levelTargetsProp, true);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            int newWidth = EditorGUILayout.IntSlider("Width", currentLevel.width, 1, 20);
            int newHeight = EditorGUILayout.IntSlider("Height", currentLevel.height, 1, 20);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(currentLevel, "Change Grid Size");
                currentLevel.width = newWidth;
                currentLevel.height = newHeight;
                currentLevel.ValidateGrid();
                EnsureRefillSequenceColumns(false);
            }

            if (GUILayout.Button("Force Validate Grid"))
            {
                Undo.RecordObject(currentLevel, "Validate Grid");
                currentLevel.ValidateGrid();
            }
        }

        private void EnsureRefillSequenceColumns(bool recordUndo)
        {
            bool needsList = currentLevel.columnRefillSequences == null;
            int existingCount = needsList ? 0 : currentLevel.columnRefillSequences.Count;
            if (!needsList && existingCount >= currentLevel.width) return;

            if (recordUndo) Undo.RecordObject(currentLevel, "Add Refill Sequence Columns");

            if (needsList)
            {
                currentLevel.columnRefillSequences = new List<ColumnRefillSequence>();
            }

            while (currentLevel.columnRefillSequences.Count < currentLevel.width)
            {
                currentLevel.columnRefillSequences.Add(new ColumnRefillSequence());
            }

            EditorUtility.SetDirty(currentLevel);
        }

        private void DrawRefillSequenceSettings()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Column Refill Sequences", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Each list is consumed when a normal element is spawned from the top of that column. " +
                "After the list is exhausted, refill returns to random elements.",
                MessageType.Info);

            EnsureRefillSequenceColumns(true);

            if (elementDatabase == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign an Element Database to select and validate refill elements.",
                    MessageType.Warning);
            }
            else
            {
                DrawRefillDatabaseValidation();
            }

            for (int column = 0; column < currentLevel.width; column++)
            {
                ColumnRefillSequence sequence = currentLevel.columnRefillSequences[column];
                if (sequence == null)
                {
                    Undo.RecordObject(currentLevel, "Repair Refill Sequence");
                    sequence = new ColumnRefillSequence();
                    currentLevel.columnRefillSequences[column] = sequence;
                    EditorUtility.SetDirty(currentLevel);
                }
                if (sequence.normalElementIds == null)
                {
                    Undo.RecordObject(currentLevel, "Repair Refill Sequence");
                    sequence.normalElementIds = new List<int>();
                    EditorUtility.SetDirty(currentLevel);
                }

                bool expanded = refillSequenceFoldouts.TryGetValue(column, out bool savedExpanded) && savedExpanded;
                expanded = EditorGUILayout.Foldout(
                    expanded,
                    $"Column {column} ({sequence.normalElementIds.Count})",
                    true);
                refillSequenceFoldouts[column] = expanded;
                if (!expanded) continue;

                EditorGUI.indentLevel++;
                DrawRefillSequenceColumn(column, sequence);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(2f);
            }

            int hiddenCount = currentLevel.columnRefillSequences.Count - currentLevel.width;
            if (hiddenCount > 0)
            {
                EditorGUILayout.HelpBox(
                    $"{hiddenCount} saved column configuration(s) are outside the current grid width and are ignored at runtime.",
                    MessageType.Warning);
                if (GUILayout.Button("Delete Hidden Column Configurations"))
                {
                    Undo.RecordObject(currentLevel, "Delete Hidden Refill Columns");
                    currentLevel.columnRefillSequences.RemoveRange(currentLevel.width, hiddenCount);
                    EditorUtility.SetDirty(currentLevel);
                }
            }
        }

        private void DrawRefillDatabaseValidation()
        {
            if (elementDatabase.normalElements == null || elementDatabase.normalElements.Count == 0)
            {
                EditorGUILayout.HelpBox("Element Database has no normal elements.", MessageType.Error);
                return;
            }

            HashSet<int> ids = new HashSet<int>();
            foreach (NormalElementInfo info in elementDatabase.normalElements)
            {
                if (info == null)
                {
                    EditorGUILayout.HelpBox("Element Database contains a null normal element.", MessageType.Error);
                }
                else if (!ids.Add(info.id))
                {
                    EditorGUILayout.HelpBox(
                        $"Element Database contains duplicate normal element ID {info.id}.",
                        MessageType.Error);
                }
            }

            if (elementDatabase.unifiedNormalPrefab == null)
            {
                EditorGUILayout.HelpBox("Unified Normal Prefab is not assigned.", MessageType.Error);
            }
            else if (elementDatabase.unifiedNormalPrefab.GetComponent<NormalElementView>() == null)
            {
                EditorGUILayout.HelpBox(
                    "Unified Normal Prefab is missing NormalElementView.",
                    MessageType.Error);
            }
        }

        private void DrawRefillSequenceColumn(int column, ColumnRefillSequence sequence)
        {
            bool removeRequested = false;
            int removeIndex = -1;
            int moveFrom = -1;
            int moveTo = -1;

            for (int index = 0; index < sequence.normalElementIds.Count; index++)
            {
                int configuredId = sequence.normalElementIds[index];
                NormalElementInfo configuredInfo =
                    elementDatabase != null ? elementDatabase.GetNormalElement(configuredId) : null;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label((index + 1).ToString(), GUILayout.Width(24f));

                if (configuredInfo != null && configuredInfo.sprite != null)
                {
                    GUILayout.Label(
                        new GUIContent(configuredInfo.sprite.texture),
                        GUILayout.Width(28f),
                        GUILayout.Height(28f));
                }
                else
                {
                    GUILayout.Space(32f);
                }

                if (elementDatabase != null &&
                    elementDatabase.normalElements != null &&
                    elementDatabase.normalElements.Count > 0)
                {
                    List<NormalElementInfo> choices =
                        elementDatabase.normalElements.FindAll(info => info != null);
                    string[] labels = choices.ConvertAll(info =>
                        string.IsNullOrEmpty(info.spineSkinName)
                            ? $"ID {info.id}"
                            : $"ID {info.id} - {info.spineSkinName}").ToArray();
                    int selectedIndex = choices.FindIndex(info => info.id == configuredId);
                    int popupIndex = Mathf.Max(0, selectedIndex);

                    EditorGUI.BeginChangeCheck();
                    popupIndex = EditorGUILayout.Popup(popupIndex, labels);
                    if (EditorGUI.EndChangeCheck() && popupIndex >= 0 && popupIndex < choices.Count)
                    {
                        Undo.RecordObject(currentLevel, "Change Refill Element");
                        sequence.normalElementIds[index] = choices[popupIndex].id;
                        EditorUtility.SetDirty(currentLevel);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField($"ID {configuredId}");
                }

                GUI.enabled = index > 0;
                if (GUILayout.Button("▲", GUILayout.Width(28f)))
                {
                    moveFrom = index;
                    moveTo = index - 1;
                }
                GUI.enabled = index < sequence.normalElementIds.Count - 1;
                if (GUILayout.Button("▼", GUILayout.Width(28f)))
                {
                    moveFrom = index;
                    moveTo = index + 1;
                }
                GUI.enabled = true;

                if (GUILayout.Button("−", GUILayout.Width(28f)))
                {
                    removeRequested = true;
                    removeIndex = index;
                }
                EditorGUILayout.EndHorizontal();

                if (elementDatabase != null && configuredInfo == null)
                {
                    EditorGUILayout.HelpBox(
                        $"ID {configuredId} does not exist and will be skipped at runtime.",
                        MessageType.Error);
                }
            }

            if (moveFrom >= 0)
            {
                Undo.RecordObject(currentLevel, "Reorder Refill Sequence");
                int value = sequence.normalElementIds[moveFrom];
                sequence.normalElementIds.RemoveAt(moveFrom);
                sequence.normalElementIds.Insert(moveTo, value);
                EditorUtility.SetDirty(currentLevel);
            }
            else if (removeRequested)
            {
                Undo.RecordObject(currentLevel, "Remove Refill Element");
                sequence.normalElementIds.RemoveAt(removeIndex);
                EditorUtility.SetDirty(currentLevel);
            }

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = elementDatabase != null &&
                          elementDatabase.normalElements != null &&
                          elementDatabase.normalElements.Exists(info => info != null);
            if (GUILayout.Button("Add Element"))
            {
                NormalElementInfo first =
                    elementDatabase.normalElements.Find(info => info != null);
                Undo.RecordObject(currentLevel, "Add Refill Element");
                sequence.normalElementIds.Add(first.id);
                EditorUtility.SetDirty(currentLevel);
            }
            GUI.enabled = true;

            if (GUILayout.Button("Copy"))
            {
                copiedRefillSequence = new List<int>(sequence.normalElementIds);
            }

            GUI.enabled = copiedRefillSequence != null;
            if (GUILayout.Button("Paste"))
            {
                Undo.RecordObject(currentLevel, "Paste Refill Sequence");
                sequence.normalElementIds = new List<int>(copiedRefillSequence);
                EditorUtility.SetDirty(currentLevel);
            }
            GUI.enabled = sequence.normalElementIds.Count > 0;
            if (GUILayout.Button("Clear"))
            {
                Undo.RecordObject(currentLevel, "Clear Refill Sequence");
                sequence.normalElementIds.Clear();
                EditorUtility.SetDirty(currentLevel);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTutorialSettings()
        {
            GUILayout.Label("Tutorial Settings", EditorStyles.boldLabel);
            
            SerializedObject serializedObject = new SerializedObject(currentLevel);
            serializedObject.Update();
            
            SerializedProperty hasTutorialProp = serializedObject.FindProperty("hasTutorial");
            EditorGUILayout.PropertyField(hasTutorialProp);
            
            if (hasTutorialProp.boolValue)
            {
                SerializedProperty tutorialStepsProp = serializedObject.FindProperty("tutorialSteps");
                EditorGUILayout.PropertyField(tutorialStepsProp, true);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPalette()
        {
            GUILayout.Label("Paint Palette", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(paintMode == PaintMode.TogglePlayable, "Toggle Playable", "Button")) paintMode = PaintMode.TogglePlayable;
            if (GUILayout.Toggle(paintMode == PaintMode.Erase, "Erase Element", "Button")) paintMode = PaintMode.Erase;
            if (GUILayout.Toggle(paintMode == PaintMode.PaintRandomNormal, "Random Normal", "Button")) paintMode = PaintMode.PaintRandomNormal;
            if (GUILayout.Toggle(paintMode == PaintMode.PaintConveyor, "Paint Conveyor", "Button")) paintMode = PaintMode.PaintConveyor;
            GUILayout.EndHorizontal();

            if (paintMode == PaintMode.PaintConveyor)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Conveyor Settings", EditorStyles.boldLabel);
                conveyorDir = (Direction)EditorGUILayout.EnumPopup("Direction", conveyorDir);
                EditorGUILayout.HelpBox("Drag to place continuous conveyors. The direction will auto-update if you drag.", MessageType.Info);
            }

            EditorGUILayout.Space();
            GUILayout.Label("Normal Elements", EditorStyles.boldLabel);
            paletteScroll = EditorGUILayout.BeginScrollView(paletteScroll, GUILayout.Height(70));
            GUILayout.BeginHorizontal();
            foreach (var norm in elementDatabase.normalElements)
            {
                bool isSelected = (paintMode == PaintMode.PaintSpecificNormal && selectedNormalId == norm.id);
                GUI.backgroundColor = isSelected ? Color.green : Color.white;
                
                GUIContent content = new GUIContent(norm.sprite ? norm.sprite.texture : null, "ID: " + norm.id);
                if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    paintMode = PaintMode.PaintSpecificNormal;
                    selectedNormalId = norm.id;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            GUILayout.Label("Special Elements", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            foreach (var spec in elementDatabase.specialElements)
            {
                if (spec == null) continue;
                bool isSelected = (paintMode == PaintMode.PaintSpecial && selectedSpecial == spec);
                GUI.backgroundColor = isSelected ? Color.green : Color.white;
                
                GUIContent content = new GUIContent(spec.sprite ? spec.sprite.texture : null, spec.elementId);
                if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    paintMode = PaintMode.PaintSpecial;
                    selectedSpecial = spec;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            GUILayout.Label("Collectible Settings (Retracting)", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            collectibleDir = (Direction)EditorGUILayout.EnumPopup("Direction", collectibleDir);
            collectibleLen = EditorGUILayout.IntSlider("Length", collectibleLen, 2, 10);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            foreach (var coll in elementDatabase.collectibles)
            {
                if (coll == null) continue;
                bool isSelected = (paintMode == PaintMode.PaintCollectible && selectedCollectible == coll);
                GUI.backgroundColor = isSelected ? Color.green : Color.white;
                
                GUIContent content = new GUIContent(coll.headSprite ? coll.headSprite.texture : null, coll.collectibleId);
                if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    paintMode = PaintMode.PaintCollectible;
                    selectedCollectible = coll;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.Label("Obstacles", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            foreach (var obs in elementDatabase.obstacles)
            {
                if (obs == null) continue;
                bool isSelected = (paintMode == PaintMode.PaintObstacle && selectedObstacle == obs);
                GUI.backgroundColor = isSelected ? Color.green : Color.white;
                
                // Get texture from the sprite of the obstacle prefab if available
                Texture2D tex = null;
                if (obs.prefab != null)
                {
                    var sr = obs.prefab.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null && sr.sprite != null)
                    {
                        tex = sr.sprite.texture;
                    }
                }
                
                GUIContent content = new GUIContent(tex, obs.obstacleId);
                if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    paintMode = PaintMode.PaintObstacle;
                    selectedObstacle = obs;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.Label("Generators", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            foreach (var gen in elementDatabase.generators)
            {
                if (gen == null) continue;
                bool isSelected = (paintMode == PaintMode.PaintGenerator && selectedGenerator == gen);
                GUI.backgroundColor = isSelected ? Color.green : Color.white;
                
                GUIContent content = new GUIContent(gen.targetIcon ? gen.targetIcon.texture : null, gen.generatorId);
                if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    paintMode = PaintMode.PaintGenerator;
                    selectedGenerator = gen;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.Label("Mother & Child", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            foreach (var mother in elementDatabase.mothers)
            {
                if (mother == null) continue;
                bool isSelected = (paintMode == PaintMode.PaintMother && selectedMother == mother);
                GUI.backgroundColor = isSelected ? Color.green : Color.white;
                
                GUIContent content = new GUIContent(mother.targetIcon ? mother.targetIcon.texture : null, mother.motherId);
                if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    paintMode = PaintMode.PaintMother;
                    selectedMother = mother;
                }
                GUI.backgroundColor = Color.white;
            }
            foreach (var child in elementDatabase.children)
            {
                if (child == null) continue;
                bool isSelected = (paintMode == PaintMode.PaintChild && selectedChild == child);
                GUI.backgroundColor = isSelected ? Color.green : Color.white;
                
                Texture2D tex = null;
                if (child.prefab != null)
                {
                    var sr = child.prefab.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null && sr.sprite != null) tex = sr.sprite.texture;
                }
                
                GUIContent content = new GUIContent(tex, child.childId);
                if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    paintMode = PaintMode.PaintChild;
                    selectedChild = child;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawGrid()
        {
            if (currentLevel.cells == null || currentLevel.cells.Count == 0)
            {
                currentLevel.ValidateGrid();
            }

            GUILayout.Label("Grid (Click/Drag to Paint)", EditorStyles.boldLabel);

            Rect gridRect = GUILayoutUtility.GetRect(currentLevel.width * CELL_SIZE, currentLevel.height * CELL_SIZE);
            
            Event e = Event.current;
            bool isMouseAction = e.type == EventType.MouseDown || e.type == EventType.MouseDrag;
            
            // First pass: Base cells and normal/special items
            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    int drawY = currentLevel.height - 1 - y;
                    Rect cellRect = new Rect(gridRect.x + x * CELL_SIZE, gridRect.y + drawY * CELL_SIZE, CELL_SIZE, CELL_SIZE);

                    GridCellData cell = currentLevel.GetCell(x, y);
                    if (cell == null) continue;

                    if (isMouseAction && cellRect.Contains(e.mousePosition) && e.button == 0)
                    {
                        // To prevent drag-toggling from flipping rapidly back and forth, TogglePlayable only works on MouseDown
                        if (paintMode == PaintMode.TogglePlayable && e.type == EventType.MouseDrag)
                        {
                            // Do nothing for drag in TogglePlayable mode
                        }
                        else
                        {
                            ApplyPaint(cell);
                            e.Use();
                        }
                    }

                    DrawCellVisuals(cellRect, cell);
                }
            }

            // Draw mother elements overlay (Third pass)
            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    GridCellData cell = currentLevel.GetCell(x, y);
                    if (cell != null && cell.isPlayable && cell.itemType == CellItemType.Mother && cell.motherData != null)
                    {
                        int drawY = currentLevel.height - 1 - y; // Bottom-left cell Y
                        
                        int offsetX = cell.normalElementId % 100;
                        int offsetY = cell.normalElementId / 100;
                        
                        // We want to draw it spanning gridWidth and gridHeight.
                        // However, to make it perfectly centered around the exact cell the user clicked,
                        // we must calculate the top-left based on that clicked center cell.
                        int centerDrawY = currentLevel.height - 1 - (y + offsetY); // The actual Y cell clicked
                        int topDrawY = centerDrawY - (cell.motherData.gridHeight - 1) / 2;
                        int leftDrawX = x + offsetX - (cell.motherData.gridWidth - 1) / 2;
                        
                        Rect motherRect = new Rect(
                            gridRect.x + leftDrawX * CELL_SIZE, 
                            gridRect.y + topDrawY * CELL_SIZE, 
                            CELL_SIZE * cell.motherData.gridWidth, 
                            CELL_SIZE * cell.motherData.gridHeight
                        );
                        
                        // Draw semi-transparent background
                        EditorGUI.DrawRect(motherRect, new Color(0.2f, 0.8f, 0.2f, 0.5f));
                        
                        // Draw Border
                        GUI.color = Color.green;
                        GUI.DrawTexture(new Rect(motherRect.x, motherRect.y, motherRect.width, 2), EditorGUIUtility.whiteTexture);
                        GUI.DrawTexture(new Rect(motherRect.x, motherRect.y, 2, motherRect.height), EditorGUIUtility.whiteTexture);
                        GUI.DrawTexture(new Rect(motherRect.x, motherRect.yMax - 2, motherRect.width, 2), EditorGUIUtility.whiteTexture);
                        GUI.DrawTexture(new Rect(motherRect.xMax - 2, motherRect.y, 2, motherRect.height), EditorGUIUtility.whiteTexture);
                        GUI.color = Color.white;
                        
                        // Draw Mother Icon at the center
                        if (cell.motherData.targetIcon != null)
                        {
                            Texture2D tex = cell.motherData.targetIcon.texture;
                            Rect spriteRect = cell.motherData.targetIcon.textureRect;
                            Rect coords = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);
                            
                            // Center it perfectly in the motherRect and make it 1 CELL_SIZE wide/high
                            float iconSize = CELL_SIZE * 0.8f; // slightly smaller than a full cell
                            float centerX = motherRect.x + (motherRect.width - iconSize) / 2f;
                            float centerY = motherRect.y + (motherRect.height - iconSize) / 2f;
                            
                            Rect texRect = new Rect(centerX, centerY, iconSize, iconSize);
                            GUI.DrawTextureWithTexCoords(texRect, tex, coords);
                        }

                        // Draw Label over icon
                        GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
                        style.normal.textColor = Color.white;
                        
                        // Add shadow to text so it's visible over the icon
                        GUIStyle shadowStyle = new GUIStyle(style);
                        shadowStyle.normal.textColor = Color.black;
                        
                        Rect labelRect = motherRect;
                        GUI.Label(new Rect(labelRect.x + 1, labelRect.y + 1, labelRect.width, labelRect.height), cell.motherData.motherId + "\n(" + cell.motherData.gridWidth + "x" + cell.motherData.gridHeight + ")", shadowStyle);
                        GUI.Label(labelRect, cell.motherData.motherId + "\n(" + cell.motherData.gridWidth + "x" + cell.motherData.gridHeight + ")", style);
                    }
                }
            }
            // Fourth pass: Collectibles
            for (int x = 0; x < currentLevel.width; x++)
            {
                for (int y = 0; y < currentLevel.height; y++)
                {
                    GridCellData cell = currentLevel.GetCell(x, y);
                    if (cell != null && cell.isPlayable && cell.itemType == CellItemType.RetractingCollectible && cell.collectibleData != null)
                    {
                        int drawY = currentLevel.height - 1 - y;
                        Rect cellRect = new Rect(gridRect.x + x * CELL_SIZE, gridRect.y + drawY * CELL_SIZE, CELL_SIZE, CELL_SIZE);
                        DrawCollectibleOverlay(cellRect, cell);
                    }
                }
            }
        }

        private void ApplyPaint(GridCellData cell)
        {
            Undo.RecordObject(currentLevel, "Paint Cell");
            switch (paintMode)
            {
                case PaintMode.TogglePlayable:
                    // Only toggle on MouseDown, not drag, to prevent flickering
                    if (Event.current.type == EventType.MouseDown)
                    {
                        cell.isPlayable = !cell.isPlayable;
                    }
                    break;
                case PaintMode.PaintRandomNormal:
                    if (cell.isPlayable) cell.itemType = CellItemType.RandomNormal;
                    break;
                case PaintMode.PaintSpecificNormal:
                    if (cell.isPlayable)
                    {
                        cell.itemType = CellItemType.SpecificNormal;
                        cell.normalElementId = selectedNormalId;
                    }
                    break;
                case PaintMode.PaintSpecial:
                    if (cell.isPlayable && selectedSpecial != null)
                    {
                        cell.itemType = CellItemType.Special;
                        cell.specialElement = selectedSpecial;
                    }
                    break;
                case PaintMode.PaintCollectible:
                    if (cell.isPlayable && selectedCollectible != null)
                    {
                        cell.itemType = CellItemType.RetractingCollectible;
                        cell.collectibleData = selectedCollectible;
                        cell.collectibleDirection = collectibleDir;
                        cell.collectibleLength = collectibleLen;
                    }
                    break;
                case PaintMode.PaintObstacle:
                    if (cell.isPlayable && selectedObstacle != null)
                    {
                        cell.itemType = CellItemType.Obstacle;
                        cell.obstacleData = selectedObstacle;
                    }
                    break;
                case PaintMode.PaintGenerator:
                    if (cell.isPlayable && selectedGenerator != null)
                    {
                        cell.itemType = CellItemType.Generator;
                        cell.generatorData = selectedGenerator;
                    }
                    break;
                case PaintMode.PaintMother:
                    if (cell.isPlayable && selectedMother != null)
                    {
                        // x,y is the center cell. Calculate bottom-left based on center.
                        int bottomLeftX = cell.x - (selectedMother.gridWidth - 1) / 2;
                        int bottomLeftY = cell.y - (selectedMother.gridHeight - 1) / 2;

                        // Check if there's enough space
                        bool canPlace = true;
                        for (int i = 0; i < selectedMother.gridWidth; i++)
                        {
                            for (int j = 0; j < selectedMother.gridHeight; j++)
                            {
                                int cx = bottomLeftX + i;
                                int cy = bottomLeftY + j;
                                if (cx < 0 || cx >= currentLevel.width || cy < 0 || cy >= currentLevel.height)
                                {
                                    canPlace = false;
                                    break;
                                }
                            }
                            if (!canPlace) break;
                        }
                        
                        if (canPlace)
                        {
                            // Clear out the area first
                            for (int i = 0; i < selectedMother.gridWidth; i++)
                            {
                                for (int j = 0; j < selectedMother.gridHeight; j++)
                                {
                                    int cx = bottomLeftX + i;
                                    int cy = bottomLeftY + j;
                                    var targetCell = currentLevel.GetCell(cx, cy);
                                    if (targetCell != null)
                                    {
                                        targetCell.itemType = CellItemType.None;
                                        targetCell.motherData = null;
                                        targetCell.normalElementId = -1;
                                    }
                                }
                            }
                            
                            // Set the mother at the bottom-left cell
                            var rootCell = currentLevel.GetCell(bottomLeftX, bottomLeftY);
                            if (rootCell != null)
                            {
                                rootCell.itemType = CellItemType.Mother;
                                rootCell.motherData = selectedMother;
                                // We store the center offset so we can draw it perfectly centered in Editor
                                rootCell.normalElementId = (cell.x - bottomLeftX) + ((cell.y - bottomLeftY) * 100); 
                            }
                        }
                    }
                    break;
                case PaintMode.PaintChild:
                    if (cell.isPlayable && selectedChild != null)
                    {
                        cell.itemType = CellItemType.Child;
                        cell.childData = selectedChild;
                    }
                    break;
                case PaintMode.Erase:
                    if (cell.isPlayable)
                    {
                        cell.itemType = CellItemType.None;
                        cell.specialElement = null;
                        cell.collectibleData = null;
                        cell.obstacleData = null;
                        cell.generatorData = null;
                        cell.motherData = null;
                        cell.childData = null;
                        cell.normalElementId = -1;
                        cell.hasConveyor = false;
                    }
                    break;
                case PaintMode.PaintConveyor:
                    if (cell.isPlayable && cell.itemType != CellItemType.Obstacle && cell.itemType != CellItemType.Generator && cell.itemType != CellItemType.Mother && cell.itemType != CellItemType.Child && cell.itemType != CellItemType.RetractingCollectible && cell.itemType != CellItemType.Special)
                    {
                        cell.hasConveyor = true;
                        cell.conveyorDirection = conveyorDir;
                        
                        // Check if we are dragging from another cell to auto-orient the previous cell as a corner
                        if (Event.current.type == EventType.MouseDrag)
                        {
                            // This logic is simple: we just set the current cell's direction.
                            // The corner visuals can be handled at runtime or via an editor pass.
                        }
                    }
                    else if (cell.isPlayable)
                    {
                        Debug.LogWarning("Conveyor cannot overlap with Obstacles, Generators, Mothers, Children, or Specials.");
                    }
                    break;
            }
            GUI.changed = true;
        }

        private void DrawCellVisuals(Rect rect, GridCellData cell)
        {
            // Background
            Color bgColor = cell.isPlayable ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.1f, 0.1f, 0.1f);
            EditorGUI.DrawRect(rect, bgColor);
            
            // Border
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 1, rect.height), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 1, rect.width, 1), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 1, rect.y, 1, rect.height), EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;

            if (cell.isPlayable)
            {
                if (cell.hasConveyor)
                {
                    Texture2D conveyorTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Arts/block/chuansongdai1.png");
                    if (conveyorTex != null)
                    {
                        GUIUtility.RotateAroundPivot(GetConveyorRotation(cell.conveyorDirection), rect.center);
                        GUI.DrawTexture(rect, conveyorTex);
                        GUIUtility.RotateAroundPivot(-GetConveyorRotation(cell.conveyorDirection), rect.center); // Reset
                    }
                    
                    // Draw a simple arrow indicating direction
                    GUI.color = Color.yellow;
                    GUIStyle arrowStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 20 };
                    string arrowStr = "";
                    switch (cell.conveyorDirection) {
                        case Direction.Up: arrowStr = "↑"; break;
                        case Direction.Down: arrowStr = "↓"; break;
                        case Direction.Left: arrowStr = "←"; break;
                        case Direction.Right: arrowStr = "→"; break;
                    }
                    GUI.Label(rect, arrowStr, arrowStyle);
                    GUI.color = Color.white;
                }

                Texture2D tex = null;
                Rect coords = new Rect(0, 0, 1, 1);
                string label = "";

                if (cell.itemType == CellItemType.RandomNormal) label = "?";
                else if (cell.itemType == CellItemType.SpecificNormal && elementDatabase != null)
                {
                    var norm = elementDatabase.GetNormalElement(cell.normalElementId);
                    if (norm != null && norm.sprite != null)
                    {
                        tex = norm.sprite.texture;
                        Rect spriteRect = norm.sprite.textureRect;
                        coords = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);
                    }
                    else label = cell.normalElementId.ToString();
                }
                else if (cell.itemType == CellItemType.Special && cell.specialElement != null)
                {
                    if (cell.specialElement.sprite != null)
                    {
                        tex = cell.specialElement.sprite.texture;
                        Rect spriteRect = cell.specialElement.sprite.textureRect;
                        coords = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);
                    }
                    else label = cell.specialElement.elementId;
                }
                else if (cell.itemType == CellItemType.Obstacle && cell.obstacleData != null)
                {
                    if (cell.obstacleData.prefab != null)
                    {
                        var sr = cell.obstacleData.prefab.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null && sr.sprite != null)
                        {
                            tex = sr.sprite.texture;
                            Rect spriteRect = sr.sprite.textureRect;
                            coords = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);
                        }
                        else label = cell.obstacleData.obstacleId;
                    }
                    else label = cell.obstacleData.obstacleId;
                }
                else if (cell.itemType == CellItemType.Generator && cell.generatorData != null)
                {
                    if (cell.generatorData.targetIcon != null)
                    {
                        tex = cell.generatorData.targetIcon.texture;
                        Rect spriteRect = cell.generatorData.targetIcon.textureRect;
                        coords = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);
                    }
                    else label = cell.generatorData.generatorId;
                }
                else if (cell.itemType == CellItemType.Mother && cell.motherData != null)
                {
                    // For the base 1x1 cells covered by Mother, we just leave them blank visually
                    // The overlay logic in DrawGrid will render the giant image over them!
                }
                else if (cell.itemType == CellItemType.Child && cell.childData != null)
                {
                    if (cell.childData.prefab != null)
                    {
                        var sr = cell.childData.prefab.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null && sr.sprite != null)
                        {
                            tex = sr.sprite.texture;
                            Rect spriteRect = sr.sprite.textureRect;
                            coords = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);
                        }
                        else label = cell.childData.childId;
                    }
                    else label = cell.childData.childId;
                }
                // NOTE: Collectibles are drawn in the second pass!

                if (tex != null)
                {
                    Rect texRect = new Rect(rect.x + 4, rect.y + 4, rect.width - 8, rect.height - 8);
                    GUI.DrawTextureWithTexCoords(texRect, tex, coords);
                }
                else if (!string.IsNullOrEmpty(label))
                {
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.alignment = TextAnchor.MiddleCenter;
                    style.normal.textColor = Color.white;
                    GUI.Label(rect, label, style);
                }
            }
            else
            {
                GUI.color = Color.red;
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.red;
                GUI.Label(rect, "X", style);
                GUI.color = Color.white;
            }
        }

        private float GetConveyorRotation(Direction dir)
        {
            switch (dir)
            {
                case Direction.Right: return 0f;
                case Direction.Down: return 90f;
                case Direction.Left: return 180f;
                case Direction.Up: return 270f;
            }
            return 0f;
        }

        private void DrawCollectibleOverlay(Rect tailRect, GridCellData cell)
        {
            float width = CELL_SIZE;
            float height = CELL_SIZE;
            float x = tailRect.x;
            float y = tailRect.y;
            
            // In the new logic, the clicked cell is the TAIL.
            // The direction indicates where the HEAD is relative to the TAIL.
            // Editor GUI Y increases downwards. Game Logic Y increases upwards.

            if (cell.collectibleDirection == Direction.Up) {
                // Head is above Tail. In GUI, "above" means smaller Y.
                height = CELL_SIZE * cell.collectibleLength;
                y = tailRect.yMax - height; 
            }
            else if (cell.collectibleDirection == Direction.Down) {
                // Head is below Tail. In GUI, "below" means larger Y.
                height = CELL_SIZE * cell.collectibleLength;
                // y is already tailRect.y, it extends downwards naturally
            }
            else if (cell.collectibleDirection == Direction.Right) {
                // Head is right of Tail.
                width = CELL_SIZE * cell.collectibleLength;
                // x is already tailRect.x, extends right
            }
            else if (cell.collectibleDirection == Direction.Left) {
                // Head is left of Tail.
                width = CELL_SIZE * cell.collectibleLength;
                x = tailRect.xMax - width;
            }

            Rect fullRect = new Rect(x, y, width, height);

            // Draw semi-transparent background body
            EditorGUI.DrawRect(fullRect, new Color(0.8f, 0.5f, 0.2f, 0.6f));
            
            // Calculate where the head rect should be drawn visually
            Rect headVisualRect = tailRect; // Default fallback
            if (cell.collectibleDirection == Direction.Up) headVisualRect = new Rect(x, y, CELL_SIZE, CELL_SIZE);
            else if (cell.collectibleDirection == Direction.Down) headVisualRect = new Rect(x, fullRect.yMax - CELL_SIZE, CELL_SIZE, CELL_SIZE);
            else if (cell.collectibleDirection == Direction.Right) headVisualRect = new Rect(fullRect.xMax - CELL_SIZE, y, CELL_SIZE, CELL_SIZE);
            else if (cell.collectibleDirection == Direction.Left) headVisualRect = new Rect(x, y, CELL_SIZE, CELL_SIZE);

            // Draw head sprite
            if (cell.collectibleData.headSprite != null) {
                Texture2D tex = cell.collectibleData.headSprite.texture;
                Rect spriteRect = cell.collectibleData.headSprite.textureRect;
                Rect coords = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);
                GUI.DrawTextureWithTexCoords(new Rect(headVisualRect.x+4, headVisualRect.y+4, CELL_SIZE-8, CELL_SIZE-8), tex, coords);
            }
            else {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = Color.yellow;
                GUI.Label(headVisualRect, "Head", style);
            }

            // Draw border around the entire long shape
            GUI.color = Color.yellow;
            GUI.DrawTexture(new Rect(fullRect.x, fullRect.y, fullRect.width, 2), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(fullRect.x, fullRect.y, 2, fullRect.height), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(fullRect.x, fullRect.yMax - 2, fullRect.width, 2), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(fullRect.xMax - 2, fullRect.y, 2, fullRect.height), EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
            
            // Draw an arrow in the center of the tail pointing towards the head
            string arrowText = "";
            switch (cell.collectibleDirection)
            {
                case Direction.Up: arrowText = "↑"; break;
                case Direction.Down: arrowText = "↓"; break;
                case Direction.Left: arrowText = "←"; break;
                case Direction.Right: arrowText = "→"; break;
            }
            
            GUIStyle arrowStyle = new GUIStyle(EditorStyles.boldLabel);
            arrowStyle.alignment = TextAnchor.MiddleCenter;
            arrowStyle.normal.textColor = Color.red;
            arrowStyle.fontSize = 24;
            // Draw arrow in the middle of the whole body to clearly show Tail -> Head flow
            GUI.Label(fullRect, arrowText, arrowStyle);
        }
    }
}
