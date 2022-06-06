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
        protected string[] m_tabs = {"Create","Save"};
        protected int m_tabSelected = 0;
        
        // list of the editors displaying
        protected Editor m_settingEditor;
        protected Editor m_detailsEditor;
        
        // custom GUI skin
        public GUISkin m_skin;
        
        /// <summary>
        /// Override the default inspector GUI and disable the script section of the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // set the system skin to default
            GUI.skin = m_skin;

            // Draw the inspector modified properties (this is for the default inspector settings)
            DrawPropertiesExcluding(serializedObject, "m_Script");
            
            // Start of the GUI layout vertical box
            EditorGUILayout.BeginVertical();
        
            m_tabSelected = GUILayout.Toolbar(m_tabSelected, m_tabs);
        
            EditorGUILayout.EndVertical();
            
            HandleTabs(m_tabs);
        }
        
        /// <summary>
        /// Handles the visuals of switching between each case, can be overriden if need but most classes
        /// will only need to base functionality
        /// </summary>
        /// <param name="tabs"> an array of all the available tabs for this editor </param>
        protected virtual void HandleTabs(string[] tabs)
        {
            if (m_tabSelected >= 0)
            {
                switch (tabs[m_tabSelected])
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
