using UnityEngine;

namespace ECTool.Scripts.Scriptables.Vegetation.Tree
{
    public class BranchSO : VegetationSo
    {
        BranchSO()
        {
            name = "Branch";
        }

        [Header("Branch")]
        public Material material;
        [Range(0, 30)] public int count = 0;
        [RangeSlider(0, 20)] public Vector2 positionRange = new Vector2(0, 5f);
        [Range(0, 30)] public int segments = 4;
        [Range(0, 30)] public int sides = 4;
        
        [Header("Branch Scale")]
        [Range(0, 10)] public float length = 1.5f;
        public AnimationCurve lengthShape = AnimationCurve.Linear(0.1f, 0.03f, 0.97f, 0.8f);
        [Range(0, 50)] public float lengthVariationPercentage = 0.0f;
        [Range(0, 50)] public float radiusVariationPercentage = 0.0f;
        [Range(10, 100)] public int radiusPercentage = 25;
        public AnimationCurve shape = AnimationCurve.Linear(0.1f, 0.03f, 0.97f, 0.8f);
        
        [Header("Branch Distortion")]
        [Range(1, 180f)] public float bend = 15.0f;
        [Range(-0.4f, 0.4f)] public float sinStrength = 0.0f;
        [Range(-2.0f, 2.0f)] public float sinFrequency = 0.0f;
        [Range(0.0f, 100.0f)] public float randomness = 0.0f;
        [Range(-20.0f, 20.0f)] public float upAttraction = 0.01f;
        [Range(0.0f, 1.0f)] public float noise = 0.0f;

        [Header("Branch Rotation")] 
        [RangeSlider(0, 40)] public Vector2 yawVariation = new Vector2(0, 40);
        [Range(0, 180f)] public float pitch = 90;
        [Range(0, 180f)]public float roll;
    }
}
