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
        private string[] m_tabNames = { "Terrain", "Vegetation" };
        private int m_currentTabSelected = 0;
        
        // focused editor
        private Editor m_focusedSpeciesEditor;
        private int m_focusedSpeciesSelected = -1;
        
        private Editor m_focusedMeshEditor;
        private int m_focusedMeshSelected = -1;
        
        // Terrain properties
        private SerializedProperty m_terrain;

        // Vegetation properties
        private SerializedProperty m_speciesScriptables;
        
        int newSpeciesWindowID = -1;
        
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
            m_speciesScriptables = serializedObject.FindProperty("species");


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

            if (m_terrain != null && m_spawner.species.Count != 0)
            {
                // Button for actually saving the selected object.
                if (GUILayout.Button("Generate Vegetation", GUILayout.Height(40)))
                {
                    m_spawner.GenerateVegetationPlacements();
                }
            }
        }

        private void DrawVegetationGUI()
        {
            HandlePickedVegetation();
            
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical("window");

            EditorGUILayout.LabelField("Species", EditorStyles.boldLabel);
            if (m_speciesScriptables.arraySize == 0)
                EditorGUILayout.HelpBox("Add a Species Object", MessageType.Info);
            
            if (m_speciesScriptables.arraySize > 0)
            {
                for (int i = 0; i < m_speciesScriptables.arraySize; i++)
                {
                    SerializedProperty property = m_speciesScriptables.GetArrayElementAtIndex(i);
                    if (property != null)
                    {
                        DrawSpeciesPanel(i, property);
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
                    newSpeciesWindowID = EditorGUIUtility.GetControlID(FocusType.Passive) + 200; 
                    EditorGUIUtility.ShowObjectPicker<SpeciesSO>(null, false, "", newSpeciesWindowID);
                }
            }
            
            EditorGUILayout.Space();

            if (m_focusedSpeciesSelected >= 0)
            {
                GUILayout.BeginVertical("window");
                
                EditorGUILayout.Space();
                
                if (m_speciesScriptables.arraySize >= m_focusedSpeciesSelected)
                {
                    var data = m_speciesScriptables.GetArrayElementAtIndex(m_focusedSpeciesSelected);
                    CreateCachedEditor(data.objectReferenceValue, GetType(), ref m_focusedSpeciesEditor);
                    DrawFocusedSpecies();

                    GUILayout.EndVertical();
                
                    EditorGUILayout.Space();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawSpeciesPanel(int index, SerializedProperty property)
        {
            SpeciesSO speciesSo = property.objectReferenceValue as SpeciesSO;
            if(!speciesSo) return;

            string objectName = speciesSo.name;
            //Texture image = AssetPreview.GetAssetPreview(speciesSo.speciesMeshes[0].meshObject);
            GUIContent content = new GUIContent(objectName);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (index == m_focusedSpeciesSelected)
                {
                    if (GUILayout.Button(content, GUI.skin.FindStyle("ECButtonSelected"), GUILayout.Height(35)))
                    {
                        // select the appropriate index of the list
                        m_focusedSpeciesSelected = -1;
                        
                        // If we have selected a species
                    }
                }
                else
                {
                    if (GUILayout.Button(content,"Button",GUILayout.Height(35)))
                    {
                        // select the appropriate index of the list
                        m_focusedSpeciesSelected = index;
                    }
                }
                
                if (GUILayout.Button("Delete", GUILayout.Height(35)))
                {
                    m_focusedSpeciesSelected = -1;
                   if(m_spawner) m_spawner.species.RemoveAt(index);
                }
            }
            
            EditorGUILayout.Space();

            if (index != m_focusedSpeciesSelected) return;
            
            if (speciesSo.speciesMeshes.Count > 0)
            {
                for (int i = 0; i < speciesSo.speciesMeshes.Count; i++)
                {
                    ProceduralMeshSO meshSo = speciesSo.speciesMeshes[i];
                    if (meshSo != null)
                    {
                        DrawMeshObjectPanel(i, meshSo);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add a Species Mesh to this Object", MessageType.Info);
            }
            
            EditorGUILayout.Space();
        }

        private void DrawMeshObjectPanel(int index, ProceduralMeshSO property)
        {
            string objectName = property.name;
            Texture image = AssetPreview.GetAssetPreview(property.meshObject);
            GUIContent content = new GUIContent(objectName, image);
            
            var currentWidth = EditorGUIUtility.currentViewWidth;
            var buttonDefaultWidth = (currentWidth / 100) * 65;
            
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                if (index == m_focusedMeshSelected)
                {
                    if (GUILayout.Button(content, GUI.skin.FindStyle("ECButtonSelected"), 
                            GUILayout.Height(25), GUILayout.Width(buttonDefaultWidth)))
                    {
                        // select the appropriate index of the list
                        m_focusedMeshSelected = -1;
                    }
                }
                else
                {
                    if (GUILayout.Button(content,"Button",GUILayout.Height(25), 
                            GUILayout.Width(buttonDefaultWidth)))
                    {
                        // select the appropriate index of the list
                        m_focusedMeshSelected = index;
                    }
                }
            }
            
            

            if (index != m_focusedMeshSelected) return;
            if (m_focusedMeshSelected < 0) return;
            
            EditorGUILayout.Space();
            
            GUILayout.BeginVertical("window");
            EditorGUILayout.Space();
                
            CreateCachedEditor(property, GetType(), ref m_focusedMeshEditor);
            DrawFocusedMeshObject();
                
            GUILayout.EndVertical();
            
            EditorGUILayout.Space();

        }

        private void DrawFocusedMeshObject()
        {
            DrawPropertiesExcluding(m_focusedMeshEditor.serializedObject, "m_Script");
            m_focusedMeshEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawFocusedSpecies()
        {
            DrawPropertiesExcluding(m_focusedSpeciesEditor.serializedObject, "m_Script");
            if (GUI.changed)
            {
                // Rebuild the distribution?
            }
            m_focusedSpeciesEditor.serializedObject.ApplyModifiedProperties();
        }

        private void HandlePickedVegetation()
        {
            if (Event.current.commandName == "ObjectSelectorClosed" &&
                EditorGUIUtility.GetObjectPickerControlID() == newSpeciesWindowID)
            {
                // Get the procedural mesh scriptable 
                SpeciesSO pickedObject = (SpeciesSO)EditorGUIUtility.GetObjectPickerObject();
                newSpeciesWindowID = -1;
            
                // Add the picked object to our mesh scriptables list
                if(m_spawner && pickedObject) m_spawner.AddNewMeshToTerrainCollection(pickedObject);
                //if(m_spawner && pickedObject) m_spawner.terrain.terrainData.treePrototypes;
            }
        }

        private void DrawSettingsGUI()
        {
        }
    }
}
