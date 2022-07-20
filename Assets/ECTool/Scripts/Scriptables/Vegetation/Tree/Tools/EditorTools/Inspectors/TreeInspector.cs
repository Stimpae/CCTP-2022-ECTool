using System;
using ECTool.Scripts.EditorTools;
using ECTool.Scripts.Generation.VegetationGeneration;
using ECTool.Scripts.Scriptables.Vegetation.Tree;
using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.Tools.EditorTools.Inspectors
{
    [CustomEditor(typeof(TreeGenerator))]
    public class TreeInspector : CustomInspector
    {
        // the object we are currently targeting
        private TreeGenerator m_treeGenerator;

        // list of the names of the tabs created 
        private int m_objectTabSelected = -1;
        private TreeRuleset m_ruleset;

        private SerializedProperty m_scriptablesContainer;

        private void OnEnable()
        {
            m_ruleset = new TreeRuleset();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // find the serialized object data within our targeted object
            m_treeGenerator = (TreeGenerator)target;

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
            menu.AddItem(new GUIContent("Add Trunk"), false, CreateItemClicked, ETreeOptions.E_TRUNK);
            menu.AddItem(new GUIContent("Add Branch"), false, CreateItemClicked, ETreeOptions.E_BRANCH);
            menu.AddItem(new GUIContent("Add Leaves"), false, CreateItemClicked, ETreeOptions.E_LEAVES);

            // Adds a drop down button which display the menu when clicked
            if (EditorGUILayout.DropdownButton(new GUIContent("Create"), FocusType.Keyboard))
            {
                menu.ShowAsContext();
            }

            EditorGUILayout.Space();

            // Loop through the current list and display the each card
            DrawComponentList(m_scriptablesContainer);

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
            AddNewObject(Enum.GetName(typeof(ETreeOptions), parameter));
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
            ETreeOptions optionParent = m_objectTabSelected >= 0
                ? m_ruleset.GetEnumValueFromProperty(m_scriptablesContainer.GetArrayElementAtIndex(m_objectTabSelected))
                : ETreeOptions.E_NONE;

            ETreeOptions childValue = (ETreeOptions)Enum.Parse(typeof(ETreeOptions), option);

            // index before this if its actually valid?
            if (m_ruleset.IsValidParent(childValue, optionParent))
            {
                showWarnings = false;

                // Switches between string options and adds the appropriate scriptable to the selected class
                switch (option)
                {
                    case "E_TRUNK":
                        TrunkSO trunk = ScriptableObject.CreateInstance<TrunkSO>();
                        m_treeGenerator.CreateObject(trunk, ETreeOptions.E_TRUNK, indexToAdd);
                        break;
                    case "E_BRANCH":
                        BranchSO branch = ScriptableObject.CreateInstance<BranchSO>();
                        m_treeGenerator.CreateObject(branch, ETreeOptions.E_BRANCH, indexToAdd);
                        break;
                    case "E_LEAVES":
                        LeavesSO leaves = ScriptableObject.CreateInstance<LeavesSO>();
                        m_treeGenerator.CreateObject(leaves, ETreeOptions.E_LEAVES, indexToAdd);
                        break;
                }

                m_treeGenerator.RebuildTree();
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
                m_treeGenerator.RebuildTree();
            }
            detailsEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawComponentList(SerializedProperty list)
        {
            if (list.arraySize < 0) return;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty property = list.GetArrayElementAtIndex(i);

                if (m_ruleset.GetParentFromProperty(property) == null && property != null)
                {
                    DrawComponentObject(i, property, list, 0);
                }
            }
        }

        // Can move this to the custom inspector
        void DrawComponentObject(int index, SerializedProperty element, SerializedProperty list, float depth)
        {
            // I am sure i can simplify this some how 
            name = element.objectReferenceValue switch
            {
                TrunkSO trunkSo => trunkSo.name,
                BranchSO branchSo => branchSo.name,
                LeavesSO leavesSo => leavesSo.name,
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

                // Just a hacky way for the root components to stop being deleted a causing a crash
                // Need a better way of handling this by checking children of components being deleted and clearing them first

                if (GUILayout.Button("Delete", GUILayout.Height(23), GUILayout.Width(deleteWidth)))
                {
                    m_objectTabSelected = -1;
                    m_treeGenerator.DestroyObject(index, list);
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
                
                if (element.objectReferenceValue is VegetationSo { } vegetationSo)
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

                        DrawComponentObject(i, property, list, thisDepth + 34);
                    }
                }
            }
        }
    }
}