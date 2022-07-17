using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLeafRingSO : VegetationSo
{
    PlantLeafRingSO()
    {
        name = "Leaf Ring";
    }
    
    [Header("Leaf Ring")]
    public Material material;
    [Range(4, 12)] public int count = 4;
    [Range(0.001f, 1f)] public float radius = 0.005f;
    [Range(0.01f, 1)] public float width = 0.01f;
    [Range(0.01f, 1)] public float length = 0.01f;
    
    [Header("Leaf Ring Distortion")]
    [Range(1, 180f)] public float bend = 15.0f;
    
    [Header("Leaf Ring Scale")]
    [Range(0, 0.25f)] public float scaleVariation = 0;

    [Header("Leaf Ring Rotation")]
    [Range(-180, 0)] public float pitch = -60;
    [Range(-40, 40)] public float pitchVariation = 0;
    [Range(-180, 0)] public float yaw = 0;
    [Range(-40, 40)] public float yawVariation = 0;
}
