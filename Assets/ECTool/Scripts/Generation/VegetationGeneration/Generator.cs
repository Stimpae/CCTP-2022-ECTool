using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.Scriptables;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // The seed to initialise the random generation
    public int seed = 1;
    
    // Collection of the spawned scriptable objects (these should correspond with the objects created?_
    [SerializeField, HideInInspector]
    protected List<ScriptableObject>  m_testing = new List<ScriptableObject>();

    // feel like this can be in its own object as well
    [HideInInspector] public SettingsData settingsData;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    protected void CreateObjectContainers<T>(T type, int index, string name) where T : ScriptableObject
    {
        // need to get the parent somewhere here as well
        VegetationContainerObject containerObject;
        
        // this can be put in a function in a parent class
        // Create the actual parent containers dependent on the targetted index
        if (index > 0)
        {
            // Get the scriptable of the parent (index before)
            var scriptable = m_testing[index - 1];
            
            // Get this card type
            var card = type as VegetationSO;
            
            // Set this card type parent to the scriptable GameObject
            if (scriptable is VegetationSO {} vegetationSo) card.Parent = vegetationSo.Object.go;
                    
            // Create a new container and then set the parent of the actual Game Object
            containerObject = new VegetationContainerObject(card.Parent,"Container", "Container");

            // Then set the container object of this object to the previously created.
            card.Object = containerObject;
        }
        else
        {
            // Create a new container object and set the parent to whatever the parent is of this.
            containerObject = new VegetationContainerObject(gameObject, "Container", "Container");
            
            // Set the container and parent to null (this is used to check indentations in the custom inspector.
            if (type is VegetationSO { } card)
            {
                card.Object = containerObject;
                card.Parent = null;
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="list"></param>
    public void DestroyObject(int index, SerializedProperty list)
    {
        // Temp list to hold the parent and all children.
        List<int> tempIndexList = new List<int>() {index};
        
        GameObject objectGO = null;
        
        // Get the scriptable at this index game object.
        if (m_testing[index] is VegetationSO { } objectSo)
        {
            objectGO = objectSo.Object.go;
        }
        
        // loop through all objects and check if this object is any other objects parents
        // if it is then we save a reference to that objects index to remove later.
        for (int i = 0; i < m_testing.Count; i++)
        {
            if (m_testing[i] is VegetationSO { } tempSo && tempSo.Parent == objectGO && tempSo.Parent != null)
            {
                CheckDestroyedObjectChildren(objectGO, tempIndexList);
            }
        }
        
        // Loops through all of the children and removes them from our scriptable list
        for (int i = tempIndexList.Count - 1; i >= 0; i--)
        {
            list.DeleteArrayElementAtIndex(tempIndexList[i]);
        }
        
        // Destroys the parents (should destroy all children to
        DestroyImmediate(objectGO);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="list"></param>
    public void CheckDestroyedObjectChildren(GameObject parent, List<int> list)
    {
        for (int i = 0; i < m_testing.Count; i++)
        {
            // gets this index scriptable
            if (m_testing[i] is VegetationSO { } so)
            {
                // there is a match 
                if (parent == so.Parent)
                {
                    list.Add(i);
                    
                    for (int j = i; j >= 0; j--)
                    {
                        if (m_testing[i] is VegetationSO { } childSo)
                        {
                            if(so.Object.go == childSo.Parent)
                            {
                                break;
                            }
                        }
                    }
                    CheckDestroyedObjectChildren(so.Object.go, list);
                }
            }
        }
    }
}
