using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using UnityEngine;

[Serializable]
public class GrassCardSO: VegetationSO
{
    GrassCardSO()
    {
        Name = "Grass";
    }
    
    public EGrassOptions Type { get; } = EGrassOptions.E_GRASSCARD;
    
    [Header("Defaults")]
    [Range(1, 30)] public int count = 1;
    public Material grassMaterial;
    public ECardTypes cardType = ECardTypes.E_CARD;
    
    [Header("Position")]
    [Range(0, 1.0f)] public float positionVariation = 0;
    
    [Header("Scale")]
    [Range(0.1f, 10)] public float height = 1;
    [Range(0.1f, 10)] public float width = 1;
    [Range(0, 0.25f)] public float scaleVariation = 0;
    
    [Header("Rotation")]
    [Range(-180, 180)] public float rotationVariationYaw = 0;
    [Range(-40, 40)] public float rotationVariationPitch= 0;
}

