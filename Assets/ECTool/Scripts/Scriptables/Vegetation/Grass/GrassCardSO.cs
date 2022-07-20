using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using UnityEngine;

[Serializable]
public class GrassCardSO: VegetationSo
{
    GrassCardSO()
    {
        name = "Grass";
    }
    
    public ECardTypes cardType = ECardTypes.SINGLE;
    
    [Header("Grass")]
    public Material grassMaterial;
    [Range(1, 30)] public int count = 1;
    [Range(0.1f, 10)] public float height = 1;
    [Range(0.1f, 10)] public float width = 1;
    [Range(1, 6)] public int widthSegments = 1;
    [Range(1, 6)] public int heightSegments = 1;
    [Range(0.001f, 1f)] public float radius = 0.005f;
    
    [Header("Grass Distortion")]
    [Range(1, 180f)] public float bend = 15.0f;
    [Range(1, 180f)] public float curve = 1.0f;
    
    [Header("Grass Position")]
    [Range(0, 1.0f)] public float positionOffset = 0;
    
    [Header("Grass Scale")]
    [Range(0, 1f)] public float scaleVariation = 0;

    [Header("Rotation")]
    [Range(-180, 180)] public float yawVariation = 0;
    [Range(-40, 40)] public float pitchVariation= 0;
}

