using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.Tools.MeshTools;
using UnityEngine;

public class VegetationSo : ScriptableObject
{
    [HideInInspector]
    public string name = "Default";
    public VegetationContainerObject containerObject;
    
    // Parent information
    [HideInInspector]
    public GameObject parent;
    
    [HideInInspector]
    public VegetationSo parentSo;
    
    // Segment information for placement
    public List<PlacementNodes> segmentNodes = new List<PlacementNodes>(); 
    public List<PlacementNodes> availableNodes = new List<PlacementNodes>();
    public List<PlacementNodes> placementNodes = new List<PlacementNodes>();
}
