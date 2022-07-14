using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.Scriptables.Enviroment
{
    [CreateAssetMenu(fileName = "Terrain Mesh Object", menuName = "Assets/Vegetation/Terrain Mesh Object")]
    public class ProceduralMeshSO : ScriptableObject
    {
        [Header("Mesh")] 
        public GameObject meshObject;

        [Header("Placement")] 
        public bool alignToNormal;
        public bool randomYawAngle;

        [Header("Distribution")] 
        
        [Header("Collisions")]
        public float collisionRadius = 0;
        public float shadeRadius = 0;
        
        [Header("Canopys")]
        public int numberOfSteps;
        
        [Header("Growth")]
        public float growth;
    }
}
