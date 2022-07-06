using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHeadSO : VegetationSo
{
    PlantHeadSO()
    {
        name = "Head";
    }
    
    [Header("Head")]
    public Material material;
}
