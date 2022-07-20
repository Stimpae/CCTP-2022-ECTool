using System.Collections.Generic;
using UnityEngine;

namespace ECTool.Scripts.Scriptables.Enviroment
{
    [CreateAssetMenu(fileName = "Terrain Species Object", menuName = "Assets/Vegetation/Terrain Species Object")]
    public class SpeciesSO : ScriptableObject
    {
        [Range(1, 100000)] public int seed = 1;
    
        [Header("Species")] 
        public List<ProceduralMeshSO> speciesMeshes = new List<ProceduralMeshSO>();

        [Header("Type")] 
        public ESpeciesType speciesType = ESpeciesType.TREE;
    
        [Header("Distribution")] 
        [Range(1, 100)] public int spawnChance = 50;
        [Range(0.2f, 50)] public float speciesRadius = 5f; // this handles the local for your species
        [Range(0.2f, 50)] public float viabilityRadius = 5f; // this handles the layering with multiclass
        
        [HideInInspector] 
        public List<Vector3> spawnPositions = new List<Vector3>();
    }
    
}
