using ECTool.Scripts.Scriptables.Enviroment;
using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.Tools.EditorTools.Inspectors
{
    [CustomEditor(typeof(VegetationSpawner))]
    public class VegetationSpawnerInspector : Editor
    {
        // This spawner
        private VegetationSpawner m_spawner;
        public GUISkin skin;
        
        // Tabs
        private Texture[] m_tabIcons;
        private string[] m_tabNames = { "Terrain", "Vegetation", "Settings" };
        private int m_currentTabSelected = 0;
        
        // focused editor
        private Editor m_focusedEditor;
        private int m_focusedSelected = -1;
        
        // Terrain properties
        private SerializedProperty m_terrain;

        // Vegetation properties
        private SerializedProperty m_meshScriptables;
        
        int newMeshWindowID = -1;
        
        private void OnEnable()
        {
            // Load resources
            m_tabIcons = new Texture[3];
            m_tabIcons[0] = EditorGUIUtility.IconContent("Terrain Icon").image;
            m_tabIcons[1] = (Texture)AssetDatabase.LoadAssetAtPath("Assets/ECTool/Editor/Textures/T_TreeIcon.png", typeof(Texture));
            m_tabIcons[2] = EditorGUIUtility.IconContent("GameManager Icon").image;
            
            // Get property's related to each type of content
            
            // Terrain property's
            
            m_terrain = serializedObject.FindProperty("terrain");

            // Vegetation property's
            m_meshScriptables = serializedObject.FindProperty("meshScriptables");


            // Setting property's
        }

        public override void OnInspectorGUI()
        {
            // set the system skin to default
            GUI.skin = skin;

            m_spawner = (VegetationSpawner)target;

            // Draw the inspector modified properties (this is for the default inspector settings)
            //DrawPropertiesExcluding(serializedObject, "m_Script");

            m_currentTabSelected = GUILayout.Toolbar(m_currentTabSelected,
                new GUIContent[]
                {
                    new GUIContent("  Terrain", m_tabIcons[0]),
                    new GUIContent("  Vegetation", m_tabIcons[1]),
                    new GUIContent("  Settings", m_tabIcons[2]),
                }
                , GUILayout.MaxHeight(25.0f));

            
            
            EditorGUILayout.Space();

            if (m_currentTabSelected < 0) return;

            switch (m_currentTabSelected)
            {
                case 0:
                    DrawTerrainGUI();
                    break;
                case 1:
                    DrawVegetationGUI();
                    break;
                case 2:
                    DrawSettingsGUI();
                    break;
            }
        }

        private void DrawTerrainGUI()
        {
            GUILayout.BeginVertical("window");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Terrain", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_terrain);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawVegetationGUI()
        {
            HandlePickedVegetation();
            
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical("window");

            EditorGUILayout.LabelField("Vegetation", EditorStyles.boldLabel);
            if (m_meshScriptables.arraySize <= 0)
                EditorGUILayout.HelpBox("Add a Procedural Mesh Object", MessageType.Warning);
            
            if (m_meshScriptables.arraySize > 0)
            {
                for (int i = 0; i < m_meshScriptables.arraySize; i++)
                {
                    SerializedProperty property = m_meshScriptables.GetArrayElementAtIndex(i);
                    if (property != null)
                    {
                        DrawVegetationPanel(i, property);
                    }
                }
            }

            GUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button(
                        new GUIContent("Add", EditorGUIUtility.IconContent("Toolbar Plus").image, "Add new item"),
                        EditorStyles.miniButtonLeft, GUILayout.Width(60f)))
                {
                    newMeshWindowID = EditorGUIUtility.GetControlID(FocusType.Passive) + 200; 
                    EditorGUIUtility.ShowObjectPicker<ProceduralMeshSO>(null, false, "", newMeshWindowID);
                }
            }
            
            EditorGUILayout.Space();

            if (m_focusedSelected >= 0)
            {
                GUILayout.BeginVertical("window");
                
                EditorGUILayout.Space();
                
                if (m_meshScriptables.arraySize >= m_focusedSelected)
                {
                    var data = m_meshScriptables.GetArrayElementAtIndex(m_focusedSelected);
                    CreateCachedEditor(data.objectReferenceValue, GetType(), ref m_focusedEditor);
                    DrawFocusedVegetation();

                    GUILayout.EndVertical();
                
                    EditorGUILayout.Space();
                
                    // Button for actually saving the selected object.
                    if (GUILayout.Button("BUILD TEST"))
                    {
                        m_spawner.GenerateTerrainPlacements();
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawVegetationPanel(int index, SerializedProperty property)
        {
            ProceduralMeshSO meshSo = property.objectReferenceValue as ProceduralMeshSO;
            if(!meshSo) return;

            string objectName = meshSo.meshObject.name;
            Texture image = AssetPreview.GetAssetPreview(meshSo.meshObject);
            GUIContent content = new GUIContent(objectName, image);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (index == m_focusedSelected)
                {
                    if (GUILayout.Button(content, GUI.skin.FindStyle("ECButtonSelected"), GUILayout.Height(35)))
                    {
                        // select the appropriate index of the list
                        m_focusedSelected = -1;
                    }
                }
                else
                {
                    if (GUILayout.Button(content,"Button",GUILayout.Height(35)))
                    {
                        // select the appropriate index of the list
                        m_focusedSelected = index;
                    }
                }
                
                if (GUILayout.Button("Delete", GUILayout.Height(35)))
                {
                    m_focusedSelected = -1;
                   if(m_spawner) m_spawner.meshScriptables.RemoveAt(index);
                }
            }
        }

        private void DrawFocusedVegetation()
        {
            DrawPropertiesExcluding(m_focusedEditor.serializedObject, "m_Script");
            if (GUI.changed)
            {
                // Rebuild the distribution?
            }
            m_focusedEditor.serializedObject.ApplyModifiedProperties();
        }

        private void HandlePickedVegetation()
        {
            if (Event.current.commandName == "ObjectSelectorClosed" &&
                EditorGUIUtility.GetObjectPickerControlID() == newMeshWindowID)
            {
                // Get the procedural mesh scriptable 
                ProceduralMeshSO pickedObject = (ProceduralMeshSO)EditorGUIUtility.GetObjectPickerObject();
                newMeshWindowID = -1;
            
                // Add the picked object to our mesh scriptables list
                if(m_spawner && pickedObject) m_spawner.AddNewMeshToTerrain(pickedObject);
                //if(m_spawner && pickedObject) m_spawner.terrain.terrainData.treePrototypes;
            }
        }

        private void DrawSettingsGUI()
        {
        }
    }
}
