using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLeafSO : VegetationSo
{
    PlantLeafSO()
    {
        name = "Leaf";
    }
    
    [Header("Leaf")] 
    public EPlacementType placementType = EPlacementType.RANDOM;
    public Material material;
    [Range(0, 60)] public int count = 0;
    [Range(0.001f, 1f)] public float start = 0.001f;
    [Range(0.001f, 1f)] public float end = 1;
    [Range(0.01f, 1)] public float width = 0.01f;
    [Range(0.01f, 1)] public float length = 0.01f;
    
    [Header("Leaf Distortion")]
    [Range(1, 180f)] public float bend = 15.0f;
    
    [Header("Leaf Scale")]
    [Range(0, 0.25f)] public float scaleVariation = 0;

    [Header("Leaf Rotation")]
    [Range(-180, 180)] public float yawVariation = 0;
    [Range(-40, 40)] public float rollVariation = 0;
    [Range(-40, 40)] public float pitchVariation = 0;
}
