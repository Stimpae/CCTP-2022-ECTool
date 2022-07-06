using UnityEngine;

namespace ECTool.Scripts.Scriptables.Vegetation.Tree
{
    public class TrunkSO : VegetationSo
    {
        TrunkSO()
        {
            name = "Trunk";
        }
        
        [Header("Trunk"),Space(5f)]
        public Material material;
        [Range(4, 40)] public int segments = 12;
        [Range(4, 16)] public int sides = 12;
    
        [Header("Trunk Scale")]
        [Range(1, 20)] public int length = 6;
        [Range(0.1f, 2f)] public float radius = 0.18f;
        public AnimationCurve shape = AnimationCurve.Linear(0.1f, 0.03f, 0.97f, 0.8f);
    
        [Header("Trunk Distortion")]
        [Range(1, 180f)] public float bend = 15.0f;
        [Range(0.0f, 90f)] public float twist = 44.0f;
        [Range(0.0f, 0.4f)] public float sinStrength = 0.3f;
        [Range(0.0f, 2.0f)] public float sinFrequency = 1f;
        [Range(0.0f, 1.0f)] public float noise = 0.0f;

        [Header("Trunk Root"), Space(5f)]
        [Range(0.1f, 2f)] public float rootRadius = 0.33f;
        [Range(0.5f,2.0f)]public float rootHeight = 2;
        [Range(0.0f, 1.0f)] public float rootCurvature = 0.18f;
        [Range(0, 10)] public int rootFrequency = 4;
    }
}
