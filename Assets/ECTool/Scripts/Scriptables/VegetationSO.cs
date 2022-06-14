using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationSO : ScriptableObject
{
    public string Name { get; set; } = "Default";
    public VegetationContainerObject Object { get; set; }
    public GameObject Parent { get; set; }
}
