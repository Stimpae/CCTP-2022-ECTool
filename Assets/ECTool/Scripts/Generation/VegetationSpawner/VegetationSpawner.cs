using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.Scriptables.Enviroment;
using ECTool.Scripts.Tools.SpawningTools;
using UnityEngine;
using Random = UnityEngine.Random;

public class VegetationSpawner : MonoBehaviour
{
    
    // Terrain properties
    public Terrain terrain;
    
    // Vegetation properties
    public List<ProceduralMeshSO> meshScriptables = new List<ProceduralMeshSO>();

    public List<Vector3> m_testingPoints = new List<Vector3>();

    private void OnEnable()
    {
        meshScriptables = new List<ProceduralMeshSO>();
        m_testingPoints = new List<Vector3>();
    }

    public void AddNewMeshToTerrain(ProceduralMeshSO meshSo)
    {
        // Create new tree prototype collection
        List<TreePrototype> treePrototypeCollection = new List<TreePrototype>();
        
        // Add the new mesh to our scritpables
        meshScriptables.Add(meshSo);
        
        // Loop through
        foreach (var mesh in meshScriptables)
        {
            if (mesh.meshObject == null) continue;
            TreePrototype treePrototype = new TreePrototype();
            treePrototype.prefab = mesh.meshObject;
            treePrototypeCollection.Add(treePrototype);
            mesh.index = treePrototypeCollection.Count - 1;
        }

        // Add the new prototypes to the terrain data
        if (!terrain) return;
        terrain.terrainData.treePrototypes = treePrototypeCollection.ToArray();
        terrain.terrainData.RefreshPrototypes();
    }

    public void RefreshCurrentTerrainInstances()
    {
        
    }

    public void GenerateTerrainPlacements()
    {
        if (!terrain) return;
        
        float normalisedHeight;
        
        List<TreeInstance> treeInstanceCollection = new List<TreeInstance>(terrain.terrainData.treeInstances);
        treeInstanceCollection.Clear();

        ProceduralMeshSO meshSo = meshScriptables[0];
        
        Random.InitState(meshSo.seed);
        
        m_testingPoints = PoissonHelper.GeneratePoints(terrain, meshSo.collisionRadius, meshSo.seed);
        
        foreach (var spawnPoint in m_testingPoints)
        {
            Random.InitState(meshSo.seed + (int)spawnPoint.x + (int)spawnPoint.z);
            
            // Get a random tree from this scriptable object? run probability checks on them?
            // Can have multiple on one?
            
            // Skip this spawn position if random probability check is false
            if (((Random.value * 100f) <= meshSo.spawnChance) == false) continue;
            
            // Word out the local position then use that to determine the 0 - 1 normalised
            Vector3 localPos = spawnPoint - terrain.GetPosition();
            
            var terrainData = terrain.terrainData;
            Vector2 normalisedPos = new Vector2(localPos.x / terrainData.size.x, localPos.z / terrainData.size.z);
            
            // Slope
            float slope = terrainData.GetSteepness(normalisedPos.x, normalisedPos.y);
            
            // Normalised Height
            float height = terrain.terrainData.GetHeight(
                Mathf.CeilToInt(normalisedPos.x * terrain.terrainData.heightmapTexture.width),
                Mathf.CeilToInt(normalisedPos.y * terrain.terrainData.heightmapTexture.height)
            );

            normalisedHeight = height / terrainData.size.y;

            if (meshSo.slopeRange.x > 0 || meshSo.slopeRange.y < 90f)
            {
                //Reject if slope check fails
                if (!(slope >= (meshSo.slopeRange.x) && slope <= (meshSo.slopeRange.y)))
                {
                    continue;
                }
            }
            
            //Passed all conditions, add instance
            TreeInstance treeInstance = new TreeInstance
            {
                position = new Vector3(normalisedPos.x, 0 , normalisedPos.y),
                rotation = Random.Range(0f, 359f) * Mathf.Rad2Deg
            };

            float scale = Random.Range(meshSo.scaleRange.x, meshSo.scaleRange.y);
            treeInstance.heightScale = scale;
            treeInstance.widthScale = scale;
            
            treeInstance.color = Color.white;
            treeInstance.lightmapColor = Color.white;
            
            treeInstance.prototypeIndex = meshSo.index;
            
            treeInstanceCollection.Add(treeInstance);
        }
        
        terrain.terrainData.SetTreeInstances(treeInstanceCollection.ToArray(), true);
        
        //Ensures prototypes are persistent
        terrain.terrainData.RefreshPrototypes();
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(terrain);
        UnityEditor.EditorUtility.SetDirty(terrain.terrainData);
#endif
    }
    
}
