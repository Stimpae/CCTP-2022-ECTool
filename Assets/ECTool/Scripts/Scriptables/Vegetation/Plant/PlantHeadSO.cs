using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHeadSO : VegetationSo
{
    PlantHeadSO()
    {
        name = "Head";
    }
    
    [Header("Head")]
    public Material material;
    
    [Header("Head Scale")]
    [Range(0.01f, 1)] public float size = 0.01f;
    [Range(0.01f, 1)] public float dip = 0.01f;
    
    [Header("Head Rotation")]
    [Range(-360, 360)] public float pitch;
    [Range(-360, 360)] public float yaw;
}
