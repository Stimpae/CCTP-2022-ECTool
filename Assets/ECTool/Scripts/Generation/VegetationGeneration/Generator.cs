using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECTool.Scripts.Scriptables;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables.Enviroment;
using ECTool.Scripts.Tools.MeshTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
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

    public void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        m_vegetationScriptables = new List<ScriptableObject>();
        
        // Creates a new instance of the settings data
        settingsData = ScriptableObject.CreateInstance<SettingsData>();
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
            m_vegetationScriptables.RemoveAt(tempIndexList[i]);
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
            int nodesPerSegment = Mathf.FloorToInt(density);
            float distance = 1.0f / nodesPerSegment;
            float lerpValue = 0;
            
            // if we are on the last node then break, we don't need to do anything further 
            if (i == segmentNodes.Count - 1) break;
            
            // gets the first and second closet positions to our current y value position
            PlacementNodes posThis = segmentNodes[i];
            PlacementNodes posNext = segmentNodes[i + 1];
            
            // Edge case that place available nodes between multiple objects?
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
        int count)
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

            if (!(localPos.y >= start) || !(localPos.y <= end)) continue;
            
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
        
        // once we're done just look through each object -> get the last and first node in the list to work out
        // the distance, then decide how we many we can evenly distribute across that based on the count
        foreach (var tempObject in tempObjects)
        {
            // the amount of available nodes on this object
            int nodeCount = tempObject.Value.Count; 
            
            // the count we want to distribute across the object
            int tempCount = count >= nodeCount ? nodeCount : count;

            if (nodeCount == 0 || tempCount == 0) continue;
            
            // How many nodes to increment based on the current node count and our tempCount
            int nodeIncrementCount = nodeCount / tempCount;
            
            // loop through the amount of nodes that we need to distribute 
            for (int i = 0; i < tempCount; i++)
            {
                actualPlacementNodes.Add(tempObject.Value[i * nodeIncrementCount]);
            }
        }

        return actualPlacementNodes;
    }
    
    /// <summary>
    /// Combines all the created meshes into the one
    /// </summary>
    public void CompleteBuildingMesh()
    { 
        // Need to rewrite this to use lists etc and can probably be handled better
        meshFilter.sharedMesh = new Mesh();
        
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        
         foreach (MeshFilter filter in meshFilters)
         {
             MeshRenderer component = filter.GetComponent<MeshRenderer>();
             
             if (!component ||
                 !filter.sharedMesh ||
                 component.sharedMaterials.Length != filter.sharedMesh.subMeshCount)
             {
                 continue;
             }
             
             for (int s = 0; s < filter.sharedMesh.subMeshCount; s++)
             {
                 int materialArrayIndex = Contains(materials, component.sharedMaterials[s].name);
                 if (materialArrayIndex == -1)
                 {
                     materials.Add(component.sharedMaterials[s]);
                     materialArrayIndex = materials.Count - 1;
                 }
                 combineInstanceArrays.Add(new ArrayList());
 
                 CombineInstance combineInstance = new CombineInstance();
                 combineInstance.transform = component.transform.localToWorldMatrix;
                 combineInstance.subMeshIndex = s;
                 combineInstance.mesh = filter.sharedMesh;
                 (combineInstanceArrays[materialArrayIndex] as ArrayList)?.Add(combineInstance);
             }
         }
         
         // Combine by material index into per-material meshes
         // also, Create CombineInstance array for next step
         Mesh[] meshes = new Mesh[materials.Count];
         CombineInstance[] combineInstances = new CombineInstance[materials.Count];
 
         for (int m = 0; m < materials.Count; m++)
         {
             CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList)?
                 .ToArray(typeof(CombineInstance)) as CombineInstance[];
             meshes[m] = new Mesh();
             meshes[m].CombineMeshes(combineInstanceArray, true, true);
 
             combineInstances[m] = new CombineInstance();
             combineInstances[m].mesh = meshes[m];
             combineInstances[m].subMeshIndex = 0;
         }
 
         // Combine into one
         meshFilter.sharedMesh = new Mesh();
         meshFilter.sharedMesh.CombineMeshes(combineInstances, false, false);
 
         // Destroy other meshes
         foreach (Mesh oldMesh in meshes)
         {
             oldMesh.Clear();
             DestroyImmediate(oldMesh);
         }
 
         // Assign materials
         Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
         meshRenderer.materials = materialsArray;
 
         ResetChildren();
    }
    
    private int Contains(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }
    
    public void SaveObjectAsPrefab()
    {
        // this is the combine mesh, now take this, create a new game object
        // attach it to this and add the required components then save it
        GameObject tempGo = new GameObject();
        tempGo.transform.SetParent(transform);

        MeshFilter tempMeshFilter = tempGo.AddComponent<MeshFilter>();
        MeshRenderer tempMeshRenderer = tempGo.AddComponent<MeshRenderer>();
        LODGroup lodGroup = tempGo.AddComponent<LODGroup>();
        
        LOD lod = default;
        Renderer[] renderers = { tempMeshRenderer };
        lod.renderers = renderers;
        lod.screenRelativeTransitionHeight = 0.1f;

        LOD[] lods = { lod };
        lodGroup.SetLODs(lods);

        tempMeshRenderer.sharedMaterials = meshRenderer.sharedMaterials;
        tempMeshFilter.sharedMesh = meshFilter.sharedMesh;
        tempGo.name = settingsData.objectName;
        
        AssetDatabase.CreateAsset(tempMeshFilter.sharedMesh, settingsData.prefabPath + "/" + tempGo.name + ".asset");
        PrefabUtility.SaveAsPrefabAsset(tempGo, settingsData.prefabPath + "/" + tempGo.name + ".prefab");
        AssetDatabase.SaveAssets();
        
        DestroyImmediate(tempGo);
    }

    public void SaveObjectAsProceduralScriptable()
    {
        GameObject tempGo = new GameObject();
        tempGo.transform.SetParent(transform);

        MeshFilter tempMeshFilter = tempGo.AddComponent<MeshFilter>();
        MeshRenderer tempMeshRenderer = tempGo.AddComponent<MeshRenderer>();
        LODGroup lodGroup = tempGo.AddComponent<LODGroup>();

        LOD lod = default;
        Renderer[] renderers = { tempMeshRenderer };
        lod.renderers = renderers;

        LOD[] lods = { lod };
        lodGroup.SetLODs(lods);

        tempMeshRenderer.sharedMaterials = meshRenderer.sharedMaterials;
        tempMeshFilter.sharedMesh = meshFilter.sharedMesh;
        tempGo.name = settingsData.objectName;
        
        AssetDatabase.CreateAsset(meshFilter.sharedMesh, settingsData.proceduralObjectPath + "/" + tempGo.name + ".asset");
        PrefabUtility.SaveAsPrefabAsset(tempGo, settingsData.proceduralObjectPath + "/" + tempGo.name + ".prefab");
        AssetDatabase.SaveAssets();
        
        ProceduralMeshSO asset = ScriptableObject.CreateInstance<ProceduralMeshSO>();
        asset.meshObject = tempGo;
        
        AssetDatabase.CreateAsset(asset, settingsData.proceduralObjectPath + "/" + tempGo.name + ".asset");
        AssetDatabase.SaveAssets();
        
        DestroyImmediate(tempGo);
    }
}
