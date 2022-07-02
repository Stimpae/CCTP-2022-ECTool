using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationSo : ScriptableObject
{
    public string name = "Default";
    public VegetationContainerObject containerObject;
    public VegetationContainerObject parentObject;
    public List<PlacementNodes> defaultNodes = new List<PlacementNodes>(); 
    public List<PlacementNodes> placementNodes = new List<PlacementNodes>();
}
