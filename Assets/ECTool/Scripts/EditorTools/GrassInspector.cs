using System;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.EditorTools
{
    /// <summary>
    /// Child class of the custom inspector, handles the display of the grass generation process
    /// </summary>
    [CustomEditor(typeof(GrassGenerator))]
    public class GrassInspector : CustomInspector
    {
        // the object we are currently targeting
        private GrassGenerator m_grassGenerator;
        
        // list of the names of the tabs created 
        private int m_objectTabSelected = -1;
        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // find the serialized object data within our targeted object
            m_grassGenerator = (GrassGenerator) target;
            
            // create the settings editor and update the object
            var settingData = serializedObject.FindProperty("settingsData");
            CreateCachedEditor(settingData.objectReferenceValue, GetType(), ref m_settingEditor);
            m_settingEditor.serializedObject.Update();
        }
        
        /// <summary>CreateCachedEditor(settingData.objectReferenceValue, GetType(), ref m_settingEditor);
        /// Overriden creation tab. Outlines the drop down menu for each of the object creation options for this class.
        /// Handles the styling and GUI layout of these elements.
        /// </summary>
        protected override void CreationTab()
        {
            var grassCardSO = serializedObject.FindProperty("m_testing");
            
            // Creates a drop down menu and adds all available options to this, linking the creation
            // function with the appropriate enum value for the option selected
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Grass Card"), false, CreateItemClicked, EGrassOptions.GRASSCARD);
            menu.AddItem(new GUIContent("Add Mesh"), false, CreateItemClicked, EGrassOptions.MESH);
            
            // Adds a drop down button which display the menu when clicked
            if (EditorGUILayout.DropdownButton(new GUIContent("Create"), FocusType.Keyboard))
            {
                menu.ShowAsContext();
            }
            
            // Loop through the current list and display the each card
            DrawObjectList(grassCardSO);

            EditorGUILayout.Space();
            
            // Draws a window style box that contains all of the tabs created from the options menu
            if (m_objectTabSelected >= 0)
            {
                GUILayout.BeginVertical("window");
                
               var data = grassCardSO.GetArrayElementAtIndex(m_objectTabSelected);
                CreateCachedEditor(data.objectReferenceValue, GetType(), ref m_detailsEditor);
                DrawFocusedObject();
                
                GUILayout.EndVertical();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// Handles converting the parameter into the enum option relating to this class
        /// </summary>
        /// <param name="parameter"> this is the enum value passed through as a generic object</param>
        private void CreateItemClicked(object parameter)
        {
            // Adds object to the targeted object
            AddNewObject(Enum.GetName(typeof(EGrassOptions), parameter));
        }
        
        /// <summary>
        /// Handles adding the appropriate scriptable object to the targeted class dependent on the
        /// options selected in the context menu.
        /// </summary>
        /// <param name="option"> the string value of the enumeration value that needs to be added as a tab option </param>
        private void AddNewObject(string option)
        {
            // Used for inserting objects at certain positions (prerequisite for parenting)
            int indexToAdd = m_objectTabSelected >= 0 ? m_objectTabSelected + 1 : -1;
            
            // Switches between string options and adds the appropriate scriptable to the selected class
            switch (option)
            { 
                    
                case "GRASSCARD":
                    
                    GrassCardSO card = ScriptableObject.CreateInstance<GrassCardSO>();
                    m_grassGenerator.CreateObject(card, EGrassOptions.GRASSCARD, indexToAdd);
                    
                    break;
                case "MESH":
                    
                    MeshGrassSO mesh = ScriptableObject.CreateInstance<MeshGrassSO>();
                    m_grassGenerator.CreateObject(mesh, EGrassOptions.MESH, indexToAdd);

                    break;
            }
        }
        
        protected override void SaveTab()
        {
            // Draws the settings tab without the scriptable options
            DrawPropertiesExcluding(m_settingEditor.serializedObject, "m_Script");
            m_settingEditor.serializedObject.ApplyModifiedProperties();
        
            GUILayout.Space(5.0f);
            
            // Button for actually saving the selected object.
            if (GUILayout.Button("Save as Prefab"))
            {
                // need to put our save function here-> need a save object we can
                // create at the beginning of each thing and then combine it with this
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawFocusedObject()
        {
            DrawPropertiesExcluding(m_detailsEditor.serializedObject, "m_Script");
            if (GUI.changed)
            {
                
            }
            m_detailsEditor.serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawObjectList(SerializedProperty list)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                SerializedProperty property = list.GetArrayElementAtIndex(i);

                switch (property.objectReferenceValue)
                {
                    case MeshGrassSO meshGrassSo:
                        name = meshGrassSo.Name;
                        break;
                    case GrassCardSO enemyObject:
                        name = enemyObject.Name;
                        break;
                }

                if (i == m_objectTabSelected)
                {
                    if (GUILayout.Button(name.ToString(), GUI.skin.FindStyle("ECButtonSelected"),  GUILayout.Height(18)))
                    {
                        // select the appropriate index of the list
                        m_objectTabSelected = -1;
                    }  
                }
                else
                {
                    if (GUILayout.Button(name.ToString(), "ECButton", GUILayout.Height(18)))
                    {
                        // select the appropriate index of the list
                        m_objectTabSelected = i;
                    }
                    
                    if (GUILayout.Button("Delete", GUILayout.Height(18), GUILayout.Width(70)))
                    {
                        list.DeleteArrayElementAtIndex(i);
                        i -= 1;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(0);
            }
        }
    }
}
