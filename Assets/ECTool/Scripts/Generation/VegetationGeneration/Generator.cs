using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECTool.Scripts.Scriptables;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables;
using ECTool.Scripts.Scriptables.Vegetation.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
            var card = type as VegetationSo;
            
            // Set this card type parent to the scriptable GameObject
            if (scriptable is VegetationSo {} vegetationSo) card.parent = vegetationSo.containerObject.go;
                    
            // Create a new container and then set the parent of the actual Game Object
            containerObject = new VegetationContainerObject(card.parent,"Container", "Container");

            // Then set the container object of this object to the previously created.
            card.containerObject = containerObject;
        }
        else
        {
            // Create a new container object and set the parent to whatever the parent is of this.
            containerObject = new VegetationContainerObject(gameObject, "Container", "Container");
            
            // Set the container and parent to null (this is used to check indentations in the custom inspector.
            if (type is VegetationSo { } card)
            {
                card.containerObject = containerObject;
                card.parent = null;
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
        if (m_testing[index] is VegetationSo { } objectSo)
        {
            objectGO = objectSo.containerObject.go;
        }
        
        // loop through all objects and check if this object is any other objects parents
        // if it is then we save a reference to that objects index to remove later.
        for (int i = 0; i < m_testing.Count; i++)
        {
            if (m_testing[i] is VegetationSo { } tempSo && tempSo.parent == objectGO && tempSo.parent != null)
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
            if (m_testing[i] is VegetationSo { } so)
            {
                // there is a match 
                if (parent == so.parent)
                {
                    list.Add(i);
                    
                    for (int j = i; j >= 0; j--)
                    {
                        if (m_testing[i] is VegetationSo { } childSo)
                        {
                            if(so.containerObject.go == childSo.parent)
                            {
                                break;
                            }
                        }
                    }
                    CheckDestroyedObjectChildren(so.containerObject.go, list);
                }
            }
        }
    }
    
    /// <summary>
    /// Gets the current number of children attached to this object and permanently deletes them.
    /// </summary>
    protected void ResetChildren()
    {
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.CompareTag($"Vegetation"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
    
    /// <summary>
    /// Calculates the positions that children objects can parent // attach themselves to.
    /// </summary>
    /// <returns></returns>
    protected List<PlacementNodes> CalculatePlacementNodes(List<PlacementNodes> parentNodes, float start, float end, int count)
    {
        // Creates new temporary lists for placements nodes (one for our final list
        // and one to declare all the nodes available in range
        List<PlacementNodes> tempNodes = new List<PlacementNodes>();
        List<PlacementNodes> actualAvailableNodes = new List<PlacementNodes>();

        // Dependent on the start and end locations only make a certain number of the 
        // nodes available
        foreach (var node in parentNodes)
        {
            // need to get the last height - 1?
            if (node.Position.y >= start
                && node.Position.y <= end)
            {
                actualAvailableNodes.Add(node);
            }
        }
        
        // work out the start and end nodes and find the distance between them
        Vector3 startNode = actualAvailableNodes[0].Position;
        Vector3 endNode = actualAvailableNodes[parentNodes.Count - 1].Position;
        float distance = Vector3.Distance(startNode, endNode);
        
        // calculate the average distance to cover that amount
        float averageDistance = distance / count;
        
        float startY = start;
 
        // then in the loop get the closet vectors to that y position?
        for (int i = 0; i < count; i++)
        {
            // sets the original branch position (not final)
            Vector3 branchPosition = new Vector3(0, startY, 0);

            // gets the first and second closet positions to our current y value position
            PlacementNodes positionOne = actualAvailableNodes.OrderBy(o => Vector3.Distance(o.Position, branchPosition)).ToArray()[0];
            PlacementNodes positionTwo = actualAvailableNodes.OrderBy(o => Vector3.Distance(o.Position, branchPosition)).ToArray()[1];

            // the t is the value at the same level as our y -> need to somehow calculate this
            Vector3 normalised = (branchPosition - positionOne.Position).normalized;
            Vector3 newPosition = Vector3.Lerp(positionOne.Position, positionTwo.Position, normalised.y);

            branchPosition = newPosition;
            tempNodes.Add(new PlacementNodes(branchPosition, positionOne.Radius));
            startY += averageDistance;
        }

        return tempNodes;
    }
}
