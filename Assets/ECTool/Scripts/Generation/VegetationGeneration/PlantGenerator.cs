using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantGenerator : Generator
{
    /// <summary>
    /// Creates a scriptable object of the specified enum type and adds this object to the correct scriptable object
    /// container.
    /// </summary>
    /// <param name="type"> The type of scriptable object </param>
    /// <param name="option"> The enum option of what needs to be created </param>
    /// <param name="index"></param>
    /// <typeparam name="T"> The type of scriptable object to be created </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> default options type exception </exception>
    public void CreateObject<T>( T type, EPlantOptions option, int index) where T : ScriptableObject
    {
        // Call to parent to create all of the containers for this object
        CreateObjectContainers(type, index, "Plant Containers");
        
        switch (option)
        {
            case EPlantOptions.LEAF:

                if (index > 0)
                    // Inset this at a specific position in the List.
                    m_vegetationScriptables.Insert(index, type as PlantLeafSO);
                else
                    // adds at the end of the index
                    m_vegetationScriptables.Add(type as PlantLeafSO);
                
                break;
            case EPlantOptions.LEAFRING:
                
                if (index > 0)
                    // Insert this at a specific position in the List.
                    m_vegetationScriptables.Insert(index, type as PlantLeafRingSO);
                else
                    // adds at the end of the index
                    m_vegetationScriptables.Add(type as PlantLeafRingSO);
                break;
            
            case EPlantOptions.STEM:

                if (index > 0)
                    // Inset this at a specific position in the List.
                    m_vegetationScriptables.Insert(index, type as PlantStemSO);
                else
                    // adds at the end of the index
                    m_vegetationScriptables.Add(type as PlantStemSO);
                
                break;
            case EPlantOptions.HEAD:
                
                if (index > 0)
                    // Insert this at a specific position in the List.
                    m_vegetationScriptables.Insert(index, type as PlantHeadSO);
                else
                    // adds at the end of the index
                    m_vegetationScriptables.Add(type as PlantHeadSO);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void RebuildPlant()
    {
        Random.InitState(seed);
        
        foreach (var scriptable in m_vegetationScriptables)
        {
            switch (scriptable)
            {
                case PlantStemSO plantStemSo:
                    BuildStemScriptable(plantStemSo);
                    break;
                case PlantLeafSO plantLeafSo:
                    BuildLeafScriptable(plantLeafSo);
                    break;
                case PlantLeafRingSO plantLeafRingSo:
                    BuildLeafRingScriptable(plantLeafRingSo);
                    break;
                case PlantHeadSO plantHeadSo:
                    BuildHeadScriptable(plantHeadSo);
                    break;
            }
        }
        
        CompleteBuildingMesh();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stem"></param>
    private void BuildStemScriptable(PlantStemSO stem)
    {
        // each ring segment that builds up the stem
        stem.segmentNodes = new List<PlacementNodes>();
            
        // available nodes are the multitude of nodes spread along the 
        // area of the mesh object and where children can positions themselves
        stem.availableNodes = new List<PlacementNodes>();
            
        // placement nodes are the actual node positions that we are going to place objects
        stem.placementNodes = new List<PlacementNodes>();
        
        // creates a new mesh object (game object, mesh, mesh filter, mesh renderer)
        MeshObject meshObject = new MeshObject(stem.containerObject.go, stem.material, "Stem", "Vegetation");
        MeshBuilder meshBuilder = new MeshBuilder();
        
        meshObject.go.transform.Rotate(stem.yaw, stem.roll, stem.pitch);

        float randomAngle = Random.Range(-stem.bend, stem.bend);
        float bendAngleRadians = randomAngle * Mathf.Deg2Rad;
        float bendRadius = stem.length / bendAngleRadians;
        float angleInc = bendAngleRadians / stem.segments;
        
        Vector3 startOffset = new Vector3(bendRadius, 0, 0);

        for (int i = 0; i <= stem.segments; i++)
        {
            float heightNormalised = (float)i / stem.segments;
            
            Vector3 currentOffset = Vector3.zero;
            currentOffset.x = Mathf.Cos(angleInc * i);
            currentOffset.y = Mathf.Sin(angleInc * i);

            float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
            Quaternion currentRotation = Quaternion.Euler(0.0f, 0.0f, zAngleDegrees);

            currentOffset *= bendRadius;    
            currentOffset -= startOffset;
            
            // create each segment and assigns this to our mesh builder
            meshObject.MeshFilter.sharedMesh =
                MeshHelper.BuildRingSegment(meshBuilder, stem.sides, currentOffset, stem.radius, heightNormalised,
                    i > 0, currentRotation, 0, 0);

            Vector3 worldPosition = meshObject.go.transform.TransformPoint(currentOffset);
            
            stem.segmentNodes.Add(new PlacementNodes(worldPosition, stem.radius, meshObject));
        }
        
        stem.availableNodes = CalculateAvailableNodes(stem, 5, 10.0f);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="leaf"></param>
    private void BuildLeafScriptable(PlantLeafSO leaf)
    {
        // placement nodes are the actual node positions that we are going to place objects
        leaf.placementNodes = new List<PlacementNodes>();

        if (leaf.count > 0)
        {
            if (leaf.parent != null && leaf.parentSo)
            {
                leaf.placementNodes =
                    CalculatePlacementNodes(leaf.parentSo, leaf.start, leaf.end, leaf.count);
            }

            // Loop through each node position and create leaf mesh
            foreach (var node in leaf.placementNodes)
            {
                // Construct the mesh object
                MeshObject meshObject = new MeshObject(leaf.containerObject.go, leaf.material, "Leaf", "Vegetation");
                
                // Set the position to our nodes position
                meshObject.go.transform.position = node.Position;
                
                // Calculates the angle based off count of placement nodes
                int currentIndex = leaf.placementNodes.IndexOf(node);
                float yawAngle = 360.0f * currentIndex / leaf.count;
              
                float randPitch = Random.Range(-leaf.pitchVariation, leaf.pitchVariation);
                float randYaw = yawAngle + Random.Range(-leaf.yawVariation, leaf.yawVariation);
                float randRoll = Random.Range(-leaf.rollVariation, leaf.rollVariation);
                
                meshObject.go.transform.Rotate(randRoll, randYaw, randPitch);
                
                // Determine scale/size of each leaf with variation
                float randScale = Random.Range(-leaf.scaleVariation, leaf.scaleVariation);
                
                // Builds the leaf segment (just a quad with bottom pivot)
                meshObject.MeshFilter.sharedMesh = BuildLeafSection(Vector3.zero, Quaternion.identity, 
                    2, 3, leaf.length + randScale, leaf.width + randScale, leaf.bend);
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="leafRing"></param>
    private void BuildLeafRingScriptable(PlantLeafRingSO leafRing)
    {
        if (leafRing.count > 0)
        {
            // Loop through each node position and create leaf mesh
            for (int i = 0; i < leafRing.count; i++)
            {
                // Construct the mesh object
                MeshObject meshObject = new MeshObject(leafRing.containerObject.go, 
                    leafRing.material, "Leaf", "Vegetation");
                
                // Rotate the actual object to get the correct pitch
                float yAngle = 360.0f * i / leafRing.count;
                Quaternion radialRotation = Quaternion.Euler(0.0f, yAngle, 0.0f);
                Vector3 position = radialRotation * Vector3.forward * leafRing.radius;
                
                float randPitch = Random.Range(-leafRing.pitchVariation, leafRing.pitchVariation);
                float randYaw = Random.Range(-leafRing.yawVariation, leafRing.yawVariation);

                meshObject.go.transform.Rotate(randPitch + leafRing.pitch, yAngle + randYaw + leafRing.yaw, 0);
                meshObject.go.transform.position = position;
 
                // Builds the leaf segment (just a quad with bottom pivot)
                meshObject.MeshFilter.sharedMesh = BuildLeafSection(Vector3.zero, Quaternion.identity, 
                    2, 3, leafRing.length, leafRing.width, leafRing.bend);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="head"></param>
    private void BuildHeadScriptable(PlantHeadSO head)
    {
        // placement nodes are the actual node positions that we are going to place objects
        head.placementNodes = new List<PlacementNodes>();
        Vector3 headPosition = Vector3.zero;
        
        if (head.parent != null && head.parentSo)
        {
            int count = head.parentSo.availableNodes.Count;
            headPosition = head.parentSo.availableNodes[count - 1].Position;
        }
        
        // Construct the mesh object
        MeshObject meshObject = new MeshObject(head.containerObject.go, 
            head.material, "Head", "Vegetation");

        meshObject.go.transform.position = headPosition;
        
        // might need to rotate it to face up?
        meshObject.go.transform.Rotate(0,head.yaw,180 + head.pitch);
        
        // need to calculate the centre of the mesh to place as the offset
        Vector3 offset = new Vector3(-head.size * 0.5f, -head.dip, -head.size * 0.5f);
        
        meshObject.MeshFilter.mesh = MeshHelper.BuildQuadCanopy(offset, head.size, head.size, 0f, head.dip);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="rotation"></param>
    /// <param name="widthCount"></param>
    /// <param name="heightCount"></param>
    /// <param name="segmentHeight"></param>
    /// <param name="segmentWidth"></param>
    /// <returns></returns>
    private Mesh BuildLeafSection(Vector3 offset, Quaternion rotation, 
        int widthCount, int heightCount, float segmentHeight, float segmentWidth, float bend)
    {
        MeshBuilder meshBuilder = new MeshBuilder();
        
        float bendAngleRadians = bend * Mathf.Deg2Rad;
        float angleInc = bendAngleRadians / heightCount;

        float bendRadius = segmentHeight / bendAngleRadians;

        Vector3 startOffset = new Vector3(0.0f, bendRadius, 0.0f);
        
        // Collate it into something and pass it through?
        // Do i need to pass the data through somewhere? 
        for (int i = 0; i <= heightCount; i++)
        {
            // Add uv scaling?
            var vPos = (1.0f / heightCount) * i;
            
            float xOffset = segmentWidth * 0.5f;
            
            Vector3 centrePos = Vector3.zero;
            centrePos.y = Mathf.Cos(angleInc * i);
            centrePos.z = Mathf.Sin(angleInc * i);

            float bendAngleDegrees = (angleInc * i) * Mathf.Rad2Deg;
            Quaternion bendRotation = Quaternion.Euler(bendAngleDegrees, 0.0f, 0.0f);

            centrePos *= bendRadius;
            centrePos -= startOffset;

            Vector3 normal = rotation * (bendRotation * Vector3.up);

            for (int j = 0; j <= widthCount; j++)
            {
                float xPos = (segmentWidth / widthCount) * j;
                
                // Calculating the U axis for the UVs
                float uPos = (1.0f / widthCount) * j;

                Vector3 position = offset + rotation * new Vector3(xPos - xOffset, centrePos.y, centrePos.z);

                Vector2 uv = new Vector2(uPos, vPos);
                
                bool buildTriangles = i > 0 && j > 0;

                MeshHelper.BuildQuadGrid(meshBuilder, position, uv, buildTriangles,
                    widthCount + 1, normal);
            }
        }
        
        return meshBuilder.CreateMesh();
    }
}
