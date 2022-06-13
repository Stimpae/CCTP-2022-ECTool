using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using UnityEngine;

[Serializable]
public class GrassCardSO: ScriptableObject
{
    public string Name { get; } = "Grass Card";
    public EGrassOptions Type { get; } = EGrassOptions.E_GRASSCARD;
    public MeshObject Object { get; set; }
    public float Depth { get; set; }
    
    [Header("Testing")]
    // this is just here for testing atm
    public GameObject parent;
    public MeshObject parentObject;
    
    [Header("Grass Material")]
    public Material grassMaterial;
    
    [Header("Grass Cards")] [Range(1, 10)] 
    public int count = 1;
    // card type
    // size
    // randomness
    
    
    // material
    // uvs tiling amount
    // chosen position
    // randomise the uvs bool?
}

