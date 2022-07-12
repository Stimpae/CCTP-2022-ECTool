using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECTool.Scripts.Scriptables;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Tools.MeshTools;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class Generator : MonoBehaviour
{
    // The seed to initialise the random generation
    public int seed = 1;

    // Collection of the spawned scriptable objects (these should correspond with the objects created?_
    [SerializeField, HideInInspector] protected List<ScriptableObject> 
        m_vegetationScriptables = new List<ScriptableObject>();

    // feel like this can be in its own object as well
    [HideInInspector] public SettingsData settingsData;
    
    // Materials
    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    private List<Material> m_materials = new List<Material>();
    
    public void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        
        // Creates a new instance of the settings data
        settingsData = ScriptableObject.CreateInstance<SettingsData>();
        
        m_vegetationScriptables = new List<ScriptableObject>();
        m_materials = new List<Material>();
    }

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
            var scriptable = m_vegetationScriptables[index - 1];

            // Get this card type
            var card = type as VegetationSo;

            // Set this card type parent to the scriptable GameObject
            if (scriptable is VegetationSo {} vegetationSo) card.parent = vegetationSo.containerObject.go;

            // Create a new container and then set the parent of the actual Game Object
            containerObject = new VegetationContainerObject(card.parent, "Container", "Container");

            // Set our parent scriptable to this.. bla bla
            card.parentSo = (VegetationSo) scriptable;

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
        if (m_vegetationScriptables[index] is VegetationSo { } objectSo)
        {
            objectGO = objectSo.containerObject.go;
        }

        // loop through all objects and check if this object is any other objects parents
        // if it is then we save a reference to that objects index to remove later.
        for (int i = 0; i < m_vegetationScriptables.Count; i++)
        {
            if (m_vegetationScriptables[i] is VegetationSo { } tempSo && tempSo.parent == objectGO && tempSo.parent != null)
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
        for (int i = 0; i < m_vegetationScriptables.Count; i++)
        {
            // gets this index scriptable
            if (m_vegetationScriptables[i] is VegetationSo { } so)
            {
                // there is a match 
                if (parent == so.parent)
                {
                    list.Add(i);

                    for (int j = i; j >= 0; j--)
                    {
                        if (m_vegetationScriptables[i] is VegetationSo { } childSo)
                        {
                            if (so.containerObject.go == childSo.parent)
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
            // Deletes the meshes
            if (child.gameObject.CompareTag($"Vegetation"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
    
    /// <summary>
    /// Calculates the available nodes over the entire vegetation mesh, nodes are calculated using
    /// segments which are generated as the mesh is constructed
    /// </summary>
    /// <param name="objectData"> The scriptable that we are calculating the available nodes for</param>
    /// <param name="density"> Determines how dense the amount of available nodes are between segments.
    /// Higher numbers mean greater density and lower means less. the figure is multiplied by distance </param>
    /// <returns></returns>
    protected List<PlacementNodes> CalculateAvailableNodes(VegetationSo objectData, float density, float deadZone)
    {
        // Creates new temporary lists for placements nodes
        List<PlacementNodes> segmentNodes = objectData.segmentNodes;
        List<PlacementNodes> actualAvailableNodes = new List<PlacementNodes>();
        
        
        // then in the loop get the closet vectors to that y position?
        for (int i = 0; i < segmentNodes.Count; i++)
        {
            int nodesPerSegment = Mathf.FloorToInt(density);// 4 * 18 = 72 nodes per segment?
            float distance = 1.0f / nodesPerSegment;
            float lerpValue = 0;
            
            // if we are on the last node then break, we don't need to do anything further 
            if (i == segmentNodes.Count - 1) break;
            
            // gets the first and second closet positions to our current y value position
            PlacementNodes posThis = segmentNodes[i];
            PlacementNodes posNext = segmentNodes[i + 1];
            
            // Edge case that place 
            float edgeDistance = Mathf.Abs(posThis.Position.y - posNext.Position.y);
            if (edgeDistance >= deadZone) continue;
            
            for (int x = 0; x < nodesPerSegment; x++)
            {
                lerpValue += distance;
                
                Vector3 newPosition = Vector3.Lerp(posThis.Position, posNext.Position, lerpValue);
                var data = newPosition;
                actualAvailableNodes.Add(new PlacementNodes(data, posThis.Radius, posThis.RelatedObject));

                
            }
        }

        // start from the first index, getting the next position 
        return actualAvailableNodes;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parentObjectData"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    protected List<PlacementNodes> CalculatePlacementNodes(VegetationSo parentObjectData, float start, float end,
        int count, EPlacementType placementType)
    {
        // Creates new temporary lists for placements nodes
        List<PlacementNodes> availableNodes = new List<PlacementNodes>();
        List<PlacementNodes> actualPlacementNodes = new List<PlacementNodes>();

        Dictionary<MeshObject, List<PlacementNodes>> tempObjects = new Dictionary<MeshObject, List<PlacementNodes>>();
        
        // Dependent on the start and end locations only make a certain number of the 
        // nodes available
        foreach (var node in parentObjectData.availableNodes)
        {
            // Inverse the node position from world space to local
            Vector3 localPos = node.RelatedObject.go.transform.InverseTransformPoint(node.Position);
            
            if (localPos.y >= start
                && localPos.y <= end)
            {
                // Only do if we are in range
                // Construct our own dictionary of related objects && the nodes that correspond to them
                MeshObject nodeObject = node.RelatedObject;
                
                if (tempObjects.ContainsKey((nodeObject)))
                {
                    // if we already have a reference to this object, then just add the node
                    tempObjects[nodeObject].Add(node);
                }
                else
                {
                    // if we don't then we need to add 
                    tempObjects.Add(nodeObject, new List<PlacementNodes>());
                }
                
                availableNodes.Add(node);
            }
        }

        switch (placementType)
        {
            case EPlacementType.RANDOM:

                int actualCount = count > availableNodes.Count ? availableNodes.Count : count;
                
                for (int i = 0; i < actualCount; i++)
                {
                    // Add random node to our final list.
                    actualPlacementNodes.Add(ChooseRandomNode(availableNodes));
                }
                
                break;
            case EPlacementType.EVEN:

                // once we're done just look through each object -> get the last and first node in the list to work out
                // the distance, then decide how we many we can evenly distribute across that based on a density amount??
                foreach (var tempObject in tempObjects)
                {
                    int nodeCount = tempObject.Value.Count;
                    int tempCount = count;
                    
                    PlacementNodes firstNode = tempObject.Value[0];
                    PlacementNodes lastNode = tempObject.Value[nodeCount - 1];

                    float distance = Vector3.Distance(firstNode.Position, lastNode.Position);
                    tempCount = count >= nodeCount ? nodeCount : count;
                    
                    // need a better way of doing this so it evenly spreads across the distance.
                    
                    // this might do for the moment, but we do need to evenly space out the nodes i think?
                    for (int i = 0; i < tempCount; i++)
                    {
                        actualPlacementNodes.Add(tempObject.Value[i]);
                    }
                }
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(placementType), placementType, null);
        }
        
        return actualPlacementNodes;
    }

    private PlacementNodes ChooseRandomNode(List<PlacementNodes> availableNodes)
    {
        int maxNodes = availableNodes.Count;
        int randomIndex = Random.Range(0, maxNodes);

        PlacementNodes chosenNode = availableNodes[randomIndex];
        availableNodes.RemoveAt(randomIndex);
        
        return chosenNode;
    }
    
    /// <summary>
    /// Combines all the created meshes into the one
    /// </summary>
    public void CompleteBuildingMesh()
    {
        // Outline new lists to contain the game objects that need to be combined and the instances to use
        // alongside the combine mesh function    
        List<GameObject> objectsToCombine = new List<GameObject>();
        List<CombineInstance> instancesToCombine = new List<CombineInstance>();

        // New mesh for the shared mesh
        meshFilter.sharedMesh = new Mesh();
        
        // Loop through each of the children and grab all of the vegetation objects - these are our meshes
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            // Deletes the meshes
            if (child.gameObject.CompareTag($"Vegetation"))
            {
                objectsToCombine.Add(child.gameObject);
            }
        }

        
        // Loop through each game object and get the mesh and transform and add this to our instances list.
        foreach (var vegetation in objectsToCombine)
        {
            if (!vegetation) continue;
            CombineInstance instance = new CombineInstance();
            instance.mesh = vegetation.GetComponent<MeshFilter>().sharedMesh;
            instance.transform = vegetation.GetComponent<MeshFilter>().transform.localToWorldMatrix;
            
            instancesToCombine.Add(instance);
        }
        
        Mesh sharedMesh;
        
        (sharedMesh = meshFilter.sharedMesh).CombineMeshes(instancesToCombine.ToArray(), false, true);

        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            // Deletes the meshes
            if (child.gameObject.CompareTag("Vegetation"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        // This works, need to now figure out a way to handle the parenting without gameobjects..?
        for (int i = 0; i < m_vegetationScriptables.Count; i++)
        {
            if (m_vegetationScriptables[i] is VegetationSo { } tempSo)
            {
                if (tempSo.containerObject.go != null || tempSo.containerObject != null)
                {
                    DestroyImmediate(tempSo.containerObject.go, true);
                }
            }
        }
    }


    public void SaveObjectAsPrefab()
    {
        
    }

    public void SaveObjectAsProceduralScriptable()
    {
        
    }
    
    /// <summary>
    /// To be able to combine meshes properly we need to store all the necessary materials within a
    /// parent mesh renderer, this functions gets the material from the mesh renderer, if its not already in
    /// the mesh renderer it will add it and return.
    /// </summary>
    /// <param name="material"> The material that we want to get from the mesh renderer</param>
    /// <returns></returns>
    protected Material GetMaterialFromRenderer(Material material)
    {
        // m_materials keeps a local copy of our mesh renderer
        if (!m_materials.Contains(material))
        {
            m_materials.Add(material);
            meshRenderer.sharedMaterials = m_materials.ToArray();
        }
        
        int indexOfMaterial = System.Array.IndexOf(meshRenderer.sharedMaterials, material);
        return meshRenderer.sharedMaterials[indexOfMaterial];
    }
}
