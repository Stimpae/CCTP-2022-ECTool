using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.MeshTools;
using UnityEngine;

public class PlacementNodes
{
    public Vector3 Position;
    public float Radius;
    public MeshObject RelatedObject;

    public PlacementNodes(Vector3 pos, float rad, MeshObject relatedObject)
    {
        Position = pos;
        Radius = rad;
        RelatedObject = relatedObject;
    }
}
