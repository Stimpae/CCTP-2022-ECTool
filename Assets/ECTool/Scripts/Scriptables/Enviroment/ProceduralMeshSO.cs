using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.Scriptables.Enviroment
{
    [CreateAssetMenu(fileName = "Terrain Mesh Object", menuName = "Assets/Vegetation/Terrain Mesh Object")]
    public class ProceduralMeshSO : ScriptableObject
    {
        [HideInInspector] 
        public int index; // index within the terrain data
        
        [Range(1, 100000)] public int seed;
        
        [Header("Mesh")] 
        public GameObject meshObject;

        [Header("Placement")] 
        public bool alignToNormal;
        public bool randomYawAngle;

        [Header("Distribution")] 
        [Range(1, 100)] public int spawnChance = 50;
        [Range(0.2f, 50)] public float collisionRadius = 5f;
        
        [Header("Canopys")]
        public int numberOfSteps;

        [Header("Ranges")] 
        [RangeSlider(0, 1500)] public Vector2 heightRange;
        [RangeSlider(0, 90)] public Vector2 slopeRange;
        [RangeSlider(0.1f, 2)] public Vector2 scaleRange = new Vector2(1.0f, 1.2f);
    }
}
