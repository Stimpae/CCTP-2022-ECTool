using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationSo : ScriptableObject
{
    public string name = "Default";
    public VegetationContainerObject containerObject;
    
    // Parent information
    public GameObject parent;
    public VegetationSo parentSo;
    
    // Segment information for placement
    public List<PlacementNodes> segmentNodes = new List<PlacementNodes>(); 
    public List<PlacementNodes> availableNodes = new List<PlacementNodes>();
    public List<PlacementNodes> placementNodes = new List<PlacementNodes>();
}
