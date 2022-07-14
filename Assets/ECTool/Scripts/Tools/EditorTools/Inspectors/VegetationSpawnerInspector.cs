using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.Tools.EditorTools.Inspectors
{
    [CustomEditor(typeof(VegetationSpawner))]
    public class VegetationSpawnerInspector : Editor
    {
        // This spawner
        private VegetationSpawner m_spawner;
        
        // Tabs
        private Texture[] m_tabIcons;
        private string[] m_tabNames = { "Terrain", "Vegetation", "Settings" };
        private int m_currentTabSelected = 0;
        
        // Terrain properties
        private SerializedProperty m_terrain;

        // Vegetation properties
        
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
            // Setting property's
        }

        public override void OnInspectorGUI()
        {
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
                ,GUILayout.MaxHeight(25.0f));
            
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
            
        }

        private void DrawSettingsGUI()
        {
            
        }
    }
}
