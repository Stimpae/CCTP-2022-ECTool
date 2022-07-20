using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.Scriptables.Enviroment
{
    [CreateAssetMenu(fileName = "Terrain Mesh Object", menuName = "Assets/Vegetation/Terrain Mesh Object")]
    public class ProceduralMeshSO : ScriptableObject
    {
        [HideInInspector] 
        public int index; // index within the terrain data
        [Header("Mesh")] 
        public GameObject meshObject;

        [Header("Ranges")]
        [RangeSlider(0, 90)] public Vector2 slopeRange;
        [RangeSlider(0.1f, 2)] public Vector2 scaleRange = new Vector2(1.0f, 1.2f);
        
        
    }
}
