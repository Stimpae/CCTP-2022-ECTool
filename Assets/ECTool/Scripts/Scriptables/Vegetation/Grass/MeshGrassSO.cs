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
    
    [Header("Mesh")]
    public GameObject mesh;
    [Range(0, 30)] public int count = 0;

    [Header("Mesh Transforms"), Space(5.0f)]
    [Header("Mesh Positions")]
    [Range(0,0.5f)] public float positionVariation = 1;
    
    [Header("Mesh Scale")]
    [Range(0, 1)] public float scaleVariation = 1;
    
    [Header("Mesh Rotation")]
    [Range(-180, 180)] public float rotationVariationYaw = 0;
}

