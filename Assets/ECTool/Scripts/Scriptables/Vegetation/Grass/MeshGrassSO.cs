using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using UnityEngine;

[Serializable]
public class MeshGrassSO : VegetationSo
{
    MeshGrassSO()
    {
        name = "                Mesh                ";
    }
    
    public EGrassOptions Type { get; } = EGrassOptions.E_MESH;
    
    [Header("Defaults")]
    [Range(1, 30)] public int count = 1;
    public Material material;
    public Mesh mesh;
    
    [Header("Position")]
    [Range(0,0.5f)] public float positionVariation = 1;
    
    [Header("Scale")]
    [Range(0, 10)] public float scaleVariation = 1;
    
    [Header("Rotation")]
    [Range(0, 10)] public float rotationVariation = 1;
}

