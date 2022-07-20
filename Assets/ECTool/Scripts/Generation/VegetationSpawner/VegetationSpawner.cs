using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECTool.Scripts.Scriptables.Enviroment;
using ECTool.Scripts.Tools.SpawningTools;
using UnityEngine;
using Random = UnityEngine.Random;

public class VegetationSpawner : MonoBehaviour
{
    // Terrain properties
    public Terrain terrain;

    // Vegetation properties
    public List<SpeciesSO> species = new List<SpeciesSO>();
    
    private void OnEnable()
    {
        species = new List<SpeciesSO>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speciesToAdd"></param>
    public void AddNewMeshToTerrainCollection(SpeciesSO speciesToAdd)
    {
        species.Add(speciesToAdd);

        if (speciesToAdd.speciesType != ESpeciesType.GRASS)
        {
            // Create new tree prototype collection
            List<TreePrototype> treePrototypeCollection = new List<TreePrototype>();

            // Loop through
            foreach (var so in species)
            {
                foreach (var mesh in so.speciesMeshes)
                {
                    if (mesh.meshObject == null) continue;
                    TreePrototype treePrototype = new TreePrototype();
                    treePrototype.prefab = mesh.meshObject;
                    treePrototypeCollection.Add(treePrototype);
                    mesh.index = treePrototypeCollection.Count - 1;
                }
            }

            // only add the tree prototype if our species actually has a mesh applied
            //if (speciesToAdd.speciesMeshes.Count == 0) return;

            // Add the new prototypes to the terrain data
            if (!terrain) return;
            terrain.terrainData.treePrototypes = treePrototypeCollection.ToArray();
            terrain.terrainData.RefreshPrototypes();
        }
        else
        {
            
            // must be a grass prototype
            List<DetailPrototype> grassPrototypeCollection = new List<DetailPrototype>();
            
            // Loop through
            foreach (var so in species)
            {
                if(so.speciesType != ESpeciesType.GRASS) continue;
                
                foreach (var mesh in so.speciesMeshes)
                {
                    if (mesh.meshObject == null) continue;
                    
                    DetailPrototype detailPrototype = new DetailPrototype();
                    
                    detailPrototype.usePrototypeMesh = true;
                    detailPrototype.prototypeTexture = null;
                    detailPrototype.prototype = mesh.meshObject;
                    detailPrototype.renderMode = DetailRenderMode.Grass;
                    
                    grassPrototypeCollection.Add(detailPrototype);
                    mesh.index = grassPrototypeCollection.Count - 1;
                }
            }
            
            // Add the new prototypes to the terrain data
            if (!terrain) return;
            terrain.terrainData.detailPrototypes = grassPrototypeCollection.ToArray();
            terrain.terrainData.RefreshPrototypes();
        }
    }

    /// <summary>
    /// Happens everytime the inspector relating to this is updated.
    /// Takes all of the terrain instances, updates their data based on the scriptable object
    /// assigned to that index.
    /// </summary>
    public void RefreshCurrentTerrainInstances()
    {
        // to do
    }

    /// <summary>
    /// 
    /// </summary>
    private void GenerateGrassPlacement()
    {
        foreach (var species in this.species)
        {
            if (species.speciesType != ESpeciesType.GRASS) continue;
            if (species.speciesMeshes.Count == 0) return;
            
            ProceduralMeshSO meshSo = species.speciesMeshes[Random.Range(0, species.speciesMeshes.Count)];
            
            // Spawn positions are added to this map, this determines placement details for this grass type.
            int[,] placementMap = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];

            Random.InitState(species.seed);

            for (int x = 0; x < terrain.terrainData.detailWidth; x++)
            {
                for (int y = 0; y < terrain.terrainData.detailHeight; y++)
                {
                    int instanceCount = 1;

                    Vector3 worldPosition = new Vector3();
                    var terrainData = terrain.terrainData;
                    var position = terrain.GetPosition();
                
                    worldPosition.x = position.x + (float)x / (float)terrainData.detailWidth * terrainData.size.x;
                    worldPosition.z = position.z + (float)y / (float)terrainData.detailHeight * terrainData.size.z;
                    worldPosition.y = 0f;

                    Vector3 localPos = worldPosition - position;
                    Vector2 normalisedPos = new Vector2(localPos.x / terrainData.size.x, localPos.z / terrainData.size.z);
                
                    //Skip if failing probability check
                    if (((Random.value * 100f) <= species.spawnChance) == false)
                    {
                        instanceCount = 0;
                        continue;
                    }
                
                    // Slope
                    float slope = terrainData.GetSteepness(normalisedPos.x, normalisedPos.y);
                    if (meshSo.slopeRange.x > 0 || meshSo.slopeRange.y < 90f)
                    {
                        //Reject if slope check fails
                        if (!(slope >= (meshSo.slopeRange.x) && slope <= (meshSo.slopeRange.y)))
                        {
                            instanceCount = 0;
                            continue;
                        }
                    }
                    placementMap[x, y] = instanceCount;
                }
            }
            
            terrain.terrainData.SetDetailLayer(0, 0, meshSo.index, placementMap);
        }
    }
    
    
    private void GenerateTreePlacements()
    {
        float normalisedHeight;
        
        List<TreeInstance> treeInstanceCollection = new List<TreeInstance>(terrain.terrainData.treeInstances);
        treeInstanceCollection.Clear();
        
        foreach (var species in this.species)
        {
            if(species.speciesType == ESpeciesType.GRASS) continue;
            
            Random.InitState(species.seed);
            
            foreach (var spawnPoint in species.spawnPositions)
            {
                // Randomly selected a mesh based off of some kind of figure?
                // Just select the first one for now
                
                if (species.speciesMeshes.Count == 0) return;
                ProceduralMeshSO meshSo = species.speciesMeshes[Random.Range(0, species.speciesMeshes.Count)];
                
                Random.InitState(species.seed + (int)spawnPoint.x + (int)spawnPoint.z);
                
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
                    position = new Vector3(normalisedPos.x, 0, normalisedPos.y),
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
        }


        terrain.terrainData.SetTreeInstances(treeInstanceCollection.ToArray(), true);

        //Ensures prototypes are persistent
        terrain.terrainData.RefreshPrototypes();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(terrain);
        UnityEditor.EditorUtility.SetDirty(terrain.terrainData);
#endif
    }

    
    /// <summary>
    /// 
    /// </summary>
    public void GenerateVegetationPlacements()
    {
        if (!terrain) return;
        
        List<SpeciesSO> tempSpecies = new List<SpeciesSO>();

        // make sure every species has generated points to spawn at.
        foreach (var so in species)
        {
            so.spawnPositions = PoissonHelper.GeneratePoints(terrain, so.speciesRadius, so.seed);
            
            // add to a temporary list of species to avoid sorting the user interface list
            tempSpecies.Add(so);
        }
        
        // randomisation a bit to prune positions based on complete random chance
        foreach (var speciesSo in tempSpecies)
        {
            Random.InitState(speciesSo.seed);
            
            for (int i = 0; i < speciesSo.spawnPositions.Count; i++)
            {
                // Skip this spawn position if random probability check is false
                if (((Random.value * 100f) <= speciesSo.spawnChance) == false)
                {
                    speciesSo.spawnPositions.RemoveAt(i);
                }
            }
        }


        // layer the temporary species dependent on the highest viability rating assigned.
        tempSpecies = tempSpecies.OrderByDescending(so => so.viabilityRadius).ToList();

        // Starting from the top layer, make our way down removing species that with this radius
        for (int i = 0; i < tempSpecies.Count; i++)
        {
            SpeciesSO speciesSo = tempSpecies[i];
            // if the top level is grass then skip (grass positions aren't included in layering)
            if(speciesSo.speciesType == ESpeciesType.GRASS) continue;
            
            for(int k = 0; k < tempSpecies.Count; k++)
            {
                SpeciesSO speciesToCheckSo = tempSpecies[k];
                
                // stops searching our current species
                if (speciesSo == speciesToCheckSo) continue;
                
                // if the top level is grass then skip (grass positions aren't included in layering)
                if(speciesToCheckSo.speciesType == ESpeciesType.GRASS) continue;
                
                // Check each position in our against each position in theirs.
                foreach (var position in speciesSo.spawnPositions)
                {
                    for (int j = 0; j < speciesToCheckSo.spawnPositions.Count; j++)
                    {
                        Vector3 offset = speciesToCheckSo.spawnPositions[j] - position;
                        float sqrLen = offset.sqrMagnitude;

                        // square the distance we compare with
                        if (sqrLen < speciesSo.speciesRadius * speciesSo.speciesRadius)
                        {
                            // we are inside
                            speciesToCheckSo.spawnPositions.RemoveAt(j);
                        }
                    }
                }
            }
        }
        foreach (var speciesSo in species)
        {
            int index = tempSpecies.IndexOf(speciesSo);
            speciesSo.spawnPositions = tempSpecies[index].spawnPositions;
        }
        
        GenerateGrassPlacement();
        GenerateTreePlacements();
 
    }
    
    void OnDrawGizmos()
    {
        // Debug Colours
        Color[] colours = { Color.blue, Color.green, Color.red, 
            Color.yellow, Color.cyan, Color.white, Color.magenta, };
        
        if (species.Count > 0)
        {
            int index = 0;
            foreach (var speciesSo in species)
            {
                Gizmos.color = colours[index];
                
                if (speciesSo.spawnPositions != null) {
                    foreach (Vector3 point in speciesSo.spawnPositions) {
                        Gizmos.DrawWireSphere(point, speciesSo.speciesRadius);
                    }
                }

                index++;
            }
        }
        
    }
}