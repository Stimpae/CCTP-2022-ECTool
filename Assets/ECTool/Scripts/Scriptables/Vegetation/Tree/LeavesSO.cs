using UnityEngine;

namespace ECTool.Scripts.Scriptables.Vegetation.Tree
{
    public class LeavesSO : VegetationSo
    {
        LeavesSO()
        {
            name = "Leaves";
        }

        [Header("Leaves")] public EPlacementType placementType = EPlacementType.RANDOM;
        public Material material;
        [Range(0, 60)] public int count = 0;
        [Range(0, 20)] public float start = 1;
        [Range(0, 20)] public float end = 2;
        [Range(0, 3)] public float size = 1;
        [Range(0, 3)] public float scale = 0.2f;
        
        [Header("Branch Rotation")] 
        [Range(-360, 360)] public float pitch;
        [Range(-360, 360)] public float roll;
        [Range(-360, 360)] public float yaw;
    }
}
