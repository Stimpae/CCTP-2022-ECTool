using UnityEngine;

namespace ECTool.Scripts.Scriptables.Vegetation.Tree
{
    public class LeavesSO : VegetationSo
    {
        LeavesSO()
        {
            name = "Leaves";
        }

        [Header("Leaves")]
        public Material material;
        [Range(0, 60)] public int count = 0;
        [RangeSlider(0, 20)] public Vector2 positionRange = new Vector2(0, 1f);
        
        [Header("Leaves Scale")] 
        public AnimationCurve lengthShape = AnimationCurve.Linear(0.1f, 0.03f, 0.97f, 0.8f);
        [Range(0, 3)] public float size = 1;
        [Range(0, 3)] public float scale = 0.2f;
        
        [Header("Leaves Rotation")]
        [Range(-360, 360)] public float pitch;
        [Range(-360, 360)] public float roll;
        [Range(-360, 360)] public float yaw;
    }
}
