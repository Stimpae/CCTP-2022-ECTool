using System;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.EditorTools.Rulesets;
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
        private GrassRuleset m_ruleset;
        
        private SerializedProperty m_grassCardSO;
        
        private void OnEnable()
        {
            m_ruleset = new GrassRuleset();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // find the serialized object data within our targeted object
            m_grassGenerator = (GrassGenerator) target;
            
            // create the settings editor and update the object
            var settingData = serializedObject.FindProperty("settingsData");
            CreateCachedEditor(settingData.objectReferenceValue, GetType(), ref settingEditor);
            settingEditor.serializedObject.Update();
            
            EditorGUILayout.Space();

            if (showWarnings)
            {
                EditorGUILayout.HelpBox(warningText, MessageType.Warning);
            }
        }

        /// <summary>CreateCachedEditor(settingData.objectReferenceValue, GetType(), ref m_settingEditor);
        /// Overriden creation tab. Outlines the drop down menu for each of the object creation options for this class.
        /// Handles the styling and GUI layout of these elements.
        /// </summary>
        protected override void CreationTab()
        {
            m_grassCardSO = serializedObject.FindProperty("m_vegetationScriptables");
            
            // Creates a drop down menu and adds all available options to this, linking the creation
            // function with the appropriate enum value for the option selected
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Grass Card"), false, CreateItemClicked, EGrassOptions.E_GRASSCARD);
            menu.AddItem(new GUIContent("Add Mesh"), false, CreateItemClicked, EGrassOptions.E_MESH);
            
            // Adds a drop down button which display the menu when clicked
            if (EditorGUILayout.DropdownButton(new GUIContent("Create"), FocusType.Keyboard))
            {
                menu.ShowAsContext();
            }
            
            // Loop through the current list and display the each card
            DrawObjectList(m_grassCardSO);

            EditorGUILayout.Space();
            
            // Draws a window style box that contains all of the tabs created from the options menu
            if (m_objectTabSelected >= 0)
            {
                GUILayout.BeginVertical("window");
                
               if (m_grassCardSO.GetArrayElementAtIndex(m_objectTabSelected) != null)
               {   
                   var data = m_grassCardSO.GetArrayElementAtIndex(m_objectTabSelected);
                   CreateCachedEditor(data.objectReferenceValue, GetType(), ref detailsEditor);
                   DrawFocusedObject();
               }
               
               GUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
            m_grassCardSO.serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// Handles converting the parameter into the enum option relating to this class
        /// </summary>
        /// <param name="parameter"> this is the enum value passed through as a generic object</param>
        private void CreateItemClicked(object parameter)
        {
            // Adds object to the targeted object
            AddNewObject(Enum.GetName(typeof(EGrassOptions) ,parameter));
        }
        
        /// <summary>
        /// Handles adding the appropriate scriptable object to the targeted class dependent on the
        /// options selected in the context menu.
        /// </summary>
        /// <param name="option"> the string value of the enumeration value that needs to be added as a tab option </param>
        private void AddNewObject(string option)
        {
            // Used for inserting objects at certain positions (prerequisite for parenting)
            int indexToAdd = m_objectTabSelected >= 0 ? m_objectTabSelected + 1 : 0;
            
            // Gets the parent of what we are adding things to do
            EGrassOptions optionParent = m_objectTabSelected >= 0
                ? m_ruleset.GetEnumValueFromProperty(m_grassCardSO.GetArrayElementAtIndex(m_objectTabSelected))
                : EGrassOptions.E_NONE;
            
            // index before this if its actually valid?
            if (m_ruleset.IsValidParent(m_ruleset.GetEnumValueFromString(option), optionParent))
            {
                showWarnings = false;
                
                // Switches between string options and adds the appropriate scriptable to the selected class
                switch (option)
                {
                    case "E_GRASSCARD":
                        GrassCardSO card = ScriptableObject.CreateInstance<GrassCardSO>();
                        m_grassGenerator.CreateObject(card, EGrassOptions.E_GRASSCARD, indexToAdd);
                        break;
                    case "E_MESH":
                        MeshGrassSO mesh = ScriptableObject.CreateInstance<MeshGrassSO>();
                        m_grassGenerator.CreateObject(mesh, EGrassOptions.E_MESH, indexToAdd);
                        break;
                }
                
                m_grassGenerator.RebuildGrassCards();
            }
            else
            {
                showWarnings = true;
                warningText = "You can not add this object type as child of this element.";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void DrawFocusedObject()
        {
            DrawPropertiesExcluding(detailsEditor.serializedObject, "m_Script");
            if (GUI.changed)
            {
                m_grassGenerator.RebuildGrassCards();
            }
            detailsEditor.serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawObjectList(SerializedProperty list)
        {
            if (list.arraySize < 0) return;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty property = list.GetArrayElementAtIndex(i);
                
                if (m_ruleset.GetGameObjectFromProperty(property) == null && property != null)
                {
                    ShowObject(i, property, list,0);
                }
            }
        }

        private void ShowObject(int index, SerializedProperty element, SerializedProperty list, float depth) {
            
            // I am sure i can simplify this some how 
            name = element.objectReferenceValue switch
            {
                MeshGrassSO meshGrassSo => meshGrassSo.name,
                GrassCardSO enemyObject => enemyObject.name,
                _ => name
            };
            
            EditorGUILayout.BeginHorizontal();

            // get the width - delete is 20% - rest is 80% (move 10% down for each parent type bla bla)
                var currentWidth = EditorGUIUtility.currentViewWidth;
                var deleteWidth = currentWidth / 100 * 25;
                var buttonDefaultWidth = (currentWidth / 100) * 65 - depth;
                
                if (index == m_objectTabSelected)
                {
                    if (GUILayout.Button(name.ToString(), GUI.skin.FindStyle("ECButtonSelected"), GUILayout.Height(23)))
                    {
                        // select the appropriate index of the list
                        m_objectTabSelected = -1;
                    }
                }
                else
                {
                    EditorGUILayout.Space(0);

                    if (GUILayout.Button(name.ToString(), "Button", GUILayout.Height(23),
                        GUILayout.Width(buttonDefaultWidth)))
                    {
                        // select the appropriate index of the list
                        m_objectTabSelected = index;
                    }

                    if (GUILayout.Button("Delete", GUILayout.Height(23), GUILayout.Width(deleteWidth)))
                    {
                        m_objectTabSelected = -1;
                        m_grassGenerator.DestroyObject(index, list);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2);

                var thisDepth = depth;
                
                // may need to check here to see if this list is bigger than our current scriptable object container
                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty property = list.GetArrayElementAtIndex(i);

                    if (element.serializedObject == null) return;
                    
                    if (element.objectReferenceValue is VegetationSo {} vegetationSo)
                    {
                        if (vegetationSo.containerObject.go != null && vegetationSo.containerObject.go 
                            == m_ruleset.GetGameObjectFromProperty(property))
                        {
                            for (int j = i; j >= 0; j--)
                            {
                                SerializedProperty prevProperty = list.GetArrayElementAtIndex(i);
                                if (vegetationSo.containerObject.go != null && vegetationSo.containerObject.go 
                                    == m_ruleset.GetGameObjectFromProperty(prevProperty))
                                {
                                    break;
                                }
                            }

                            ShowObject(i, property, list, thisDepth + 34);
                        }
                    }
                }
        }
    }
}
