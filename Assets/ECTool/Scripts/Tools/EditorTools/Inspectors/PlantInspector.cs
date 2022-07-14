using System;
using ECTool.Scripts.EditorTools;
using ECTool.Scripts.Generation.VegetationGeneration;
using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.Tools.EditorTools.Inspectors
{
    [CustomEditor(typeof(PlantGenerator))]
    public class PlantInspector : CustomInspector
    {
        // the object we are currently targeting
        private PlantGenerator m_plantGenerator;
        
        // list of the names of the tabs created 
        private int m_objectTabSelected = -1;
        private PlantRuleset m_ruleset;
        
        private SerializedProperty m_scriptablesContainer;
        
        private void OnEnable()
        {
            m_ruleset = new PlantRuleset();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // find the serialized object data within our targeted object
            m_plantGenerator = (PlantGenerator) target;
            
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
    
        /// <summary>
        /// Overriden creation tab. Outlines the drop down menu for each of the object creation options for this class.
        /// Handles the styling and GUI layout of these elements.
        /// </summary>
        protected override void CreationTab()
        {
            m_scriptablesContainer = serializedObject.FindProperty("m_vegetationScriptables");
            
            // Creates a drop down menu and adds all available options to this, linking the creation
            // function with the appropriate enum value for the option selected
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Leaf"), false, CreateItemClicked, EPlantOptions.LEAF);
            menu.AddItem(new GUIContent("Add Leaf Ring"), false, CreateItemClicked, EPlantOptions.LEAFRING);
            menu.AddItem(new GUIContent("Add Stem"), false, CreateItemClicked, EPlantOptions.STEM);
            menu.AddItem(new GUIContent("Add Plant Head"), false, CreateItemClicked, EPlantOptions.HEAD);
            
            // Adds a drop down button which display the menu when clicked
            if (EditorGUILayout.DropdownButton(new GUIContent("Create"), FocusType.Keyboard))
            {
                menu.ShowAsContext();
            }
            
            EditorGUILayout.Space();
            
            DrawObjectList(m_scriptablesContainer);

            EditorGUILayout.Space();
            
            // Draws a window style box that contains all of the tabs created from the options menu
            if (m_objectTabSelected >= 0)
            {
                GUILayout.BeginVertical("window");
                
                if (m_scriptablesContainer.GetArrayElementAtIndex(m_objectTabSelected) != null)
                {
                    var data = m_scriptablesContainer.GetArrayElementAtIndex(m_objectTabSelected);
                    CreateCachedEditor(data.objectReferenceValue, GetType(), ref detailsEditor);
                    DrawFocusedObject();
                }
                
                GUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
            m_scriptablesContainer.serializedObject.ApplyModifiedProperties();
        }
    
        /// <summary>
        /// Handles converting the parameter into the enum option relating to this class
        /// </summary>
        /// <param name="parameter"> this is the enum value passed through as a generic object</param>
        private void CreateItemClicked(object parameter)
        {
            // Adds object to the targeted object
            AddNewObject(Enum.GetName(typeof(EPlantOptions) ,parameter));
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
            EPlantOptions optionParent = m_objectTabSelected >= 0
                ? m_ruleset.GetEnumValueFromProperty(m_scriptablesContainer.GetArrayElementAtIndex(m_objectTabSelected))
                : EPlantOptions.NONE;
            
            EPlantOptions childValue = (EPlantOptions)Enum.Parse(typeof(EPlantOptions), option);
            
            // index before this if its actually valid?
            if (m_ruleset.IsValidParent(childValue, optionParent))
            {
                showWarnings = false;
                
                // Switches between string options and adds the appropriate scriptable to the selected class
                switch (option)
                {
                    case "LEAF":
                        PlantLeafSO leafSo = ScriptableObject.CreateInstance<PlantLeafSO>();
                        m_plantGenerator.CreateObject(leafSo, EPlantOptions.LEAF, indexToAdd);
                        break;
                    case "LEAFRING":
                        PlantLeafRingSO leafRingSo = ScriptableObject.CreateInstance<PlantLeafRingSO>();
                        m_plantGenerator.CreateObject(leafRingSo, EPlantOptions.LEAFRING, indexToAdd);
                        break;
                    case "STEM":
                        PlantStemSO stemSo = ScriptableObject.CreateInstance<PlantStemSO>();
                        m_plantGenerator.CreateObject(stemSo, EPlantOptions.STEM, indexToAdd);
                        break;
                    case "HEAD":
                        PlantHeadSO headSo = ScriptableObject.CreateInstance<PlantHeadSO>();
                        m_plantGenerator.CreateObject(headSo, EPlantOptions.HEAD, indexToAdd);
                        break;
                }
                
                m_plantGenerator.RebuildPlant();
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
                m_plantGenerator.RebuildPlant();
            }
            detailsEditor.serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawObjectList(SerializedProperty list)
        {
            if (list.arraySize < 0) return;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty property = list.GetArrayElementAtIndex(i);
                
                if (m_ruleset.GetParentFromProperty(property) == null && property != null)
                {
                    ShowObject(i, property, list,0);
                }
            }
        }

        private void ShowObject(int index, SerializedProperty element, SerializedProperty list, float depth) 
        {
            // I am sure i can simplify this some how 
            name = element.objectReferenceValue switch
            {
                PlantLeafSO leafSo => leafSo.name,
                PlantLeafRingSO leafRingSo => leafRingSo.name,
                PlantStemSO plantStemSo => plantStemSo.name,
                PlantHeadSO plantHeadSo => plantHeadSo.name,
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
                        m_plantGenerator.DestroyObject(index, list);
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
                        == m_ruleset.GetParentFromProperty(property))
                    {
                        for (int j = i; j >= 0; j--)
                        {
                            SerializedProperty prevProperty = list.GetArrayElementAtIndex(i);
                            if (vegetationSo.containerObject.go != null && vegetationSo.containerObject.go 
                                == m_ruleset.GetParentFromProperty(prevProperty))
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
