using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshGrassSO : ScriptableObject
{
    public string Name { get; } = "Mesh";
    
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

