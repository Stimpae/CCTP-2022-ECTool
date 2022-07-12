using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantStemSO : VegetationSo
{
    PlantStemSO()
    {
        name = "Stem";
    }
    
    [Header("Stem"),Space(5f)]
    public Material material;
    [Range(4, 8)] public int segments = 4;
    [Range(4, 8)] public int sides = 4;
    
    [Header("Stem Scale")]
    [Range(0.001f, 1f)] public float length = 0.5f;
    [Range(0.001f, 0.1f)] public float radius = 0.005f;
    
    [Header("Stem Distortion")]
    [Range(1, 180f)] public float bend = 15.0f;
    
    [Header("Stem Rotation")]
    [Range(-360, 360)] public float pitch;
    [Range(-360, 360)] public float roll;
    [Range(-360, 360)] public float yaw;
}
