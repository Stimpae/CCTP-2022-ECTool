using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.EditorTools
{
    /// <summary>
    /// Parent class to handle inheritance for sub vegetation / terrain editors.
    /// Handles the removal of the script tag from the  base inspector view.
    /// </summary>
    public class CustomInspector : Editor
    {
        // Define tab tags
        protected string[] tabs = {"Create","Save"};
        protected int tabSelected = 0;
        
        // list of the editors displaying
        protected Editor settingEditor;
        protected Editor detailsEditor;
        
        // custom GUI skin
        public GUISkin skin;
        
        // Editor warnings 
        protected bool showWarnings = false;
        protected string warningText = "";
        
        /// <summary>
        /// Override the default inspector GUI and disable the script section of the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // set the system skin to default
            GUI.skin = skin;

            // Draw the inspector modified properties (this is for the default inspector settings)
            DrawPropertiesExcluding(serializedObject, "m_Script");
            
            // Start of the GUI layout vertical box
            EditorGUILayout.BeginVertical();
        
            tabSelected = GUILayout.Toolbar(tabSelected, tabs);
        
            EditorGUILayout.EndVertical();
            
            HandleTabs(tabs);
        }
        
        /// <summary>
        /// Handles the visuals of switching between each case, can be overriden if need but most classes
        /// will only need to base functionality
        /// </summary>
        /// <param name="tabs"> an array of all the available tabs for this editor </param>
        protected virtual void HandleTabs(string[] tabs)
        {
            if (tabSelected >= 0)
            {
                switch (tabs[tabSelected])
                {
                    case "Create":
                        CreationTab();
                        break;
                    case "Save":
                        SaveTab();
                        break;
                }
            }
        }
        
        /// <summary>
        /// Holds all of the GUI elements for the custom inspector that are related to creating
        /// and alternating how each group object is created and handled. 
        /// </summary>
        protected virtual void CreationTab()
        {
        }
        
        /// <summary>
        /// Holds all of the GUI elements for the custom inspector that are related to saving the mesh as
        /// either a prefab or as a standard unity mesh.
        /// </summary>
        protected virtual void SaveTab()
        {
        }
    }
}
