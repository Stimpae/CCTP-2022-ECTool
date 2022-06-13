using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using UnityEngine;

[Serializable]
public class MeshGrassSO : ScriptableObject
{
    public string Name { get; } = "Mesh";
    public EGrassOptions Type { get; } = EGrassOptions.E_MESH;
    public MeshObject Object { get; set; }

    [Header("Testing")]
    // this is just here for testing atm
    public GameObject parent;
    public MeshObject parentObject;
    
    [Header("Grass Cards")] [Range(1, 10)] 
    public int count = 1;
    public int meshGrass = 1;
    // card type
    // size
    // randomness
    
    [Header("Grass Material")]
    public Material grassMaterial;
    // material
    // uvs tiling amount
    // chosen position
    // randomise the uvs bool?
}

