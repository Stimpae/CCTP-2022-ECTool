using System;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
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
        private List<string> m_objectTabs = new List<string>();
        private int m_objectTabSelected = 0;
        
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
        
        /// <summary>
        /// Overriden creation tab. Outlines the drop down menu for each of the object creation options for this class.
        /// Handles the styling and GUI layout of these elements.
        /// </summary>
        protected override void CreationTab()
        {
            // Creates a drop down menu and adds all available options to this, linking the creation
            // function with the appropriate enum value for the option selected
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Grass Card"), false, CreateItemClicked, EGrassOptions.GRASSCARD);
            menu.AddItem(new GUIContent("Add Custom Grass Card"), false, CreateItemClicked, EGrassOptions.CUSTOMCARD);
            menu.AddItem(new GUIContent("Add Mesh"), false, CreateItemClicked, EGrassOptions.MESH);
            
            // Adds a drop down button which display the menu when clicked
            if (EditorGUILayout.DropdownButton(new GUIContent("Create"), FocusType.Keyboard))
            {
                menu.ShowAsContext();
            }
            
            // Draws a window style box that contains all of the tabs created from the options menu
            GUILayout.BeginVertical("window");
            if (m_objectTabs.Count > 0)
            {
                m_objectTabSelected = GUILayout.SelectionGrid(m_objectTabSelected, m_objectTabs.ToArray(),1);
            }
            GUILayout.EndVertical();
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
            // not sure what this is doing yet
            int index = m_objectTabs.Count + 1;
        
            // Switches between string options and adds the appropriate scriptable to the selected class
            switch (option)
            {
                case "GRASSCARD":
                    
                    /*
                    GrassCardSO card = ScriptableObject.CreateInstance<GrassCardSO>();
                    // add this as a new class that's holds the type and editor so we can loop through these etc etc
                    
                    m_grassGenerator.CreateObject(card, EGrassOptions.GRASSCARD);
                    */
                    
                    m_objectTabs.Add("Grass Card");
                    break;
                case "CUSTOMCARD":

                    EditorGUI.indentLevel++;
                    m_objectTabs.Add("Custom Grass Card");
                    break;
                case "MESH":
                    m_objectTabs.Add("Mesh");
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
    }
}
