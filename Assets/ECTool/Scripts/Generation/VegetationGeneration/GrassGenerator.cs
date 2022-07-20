using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles holding all of the data and mesh objects relating to constructing a grass plane object.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GrassGenerator : Generator
{
    /// <summary>
    /// Creates a scriptable object of the specified enum type and adds this object to the correct scriptable object
    /// container.
    /// </summary>
    /// <param name="type"> The type of scriptable object </param>
    /// <param name="option"> The enum option of what needs to be created </param>
    /// <typeparam name="T"> The type of scriptable object to be created </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> default options type exception </exception>
    public void CreateObject<T>( T type, EGrassOptions option, int index) where T : ScriptableObject
    {
        // Call to parent to create all of the containers for this object
        CreateObjectContainers(type, index, "Grass Containers");
        
        switch (option)
        {
            case EGrassOptions.E_GRASSCARD:

                if (index > 0)
                    // Inset this at a specific position in the List.
                    m_vegetationScriptables.Insert(index, type as GrassCardSO);
                else
                    // adds at the end of the index
                    m_vegetationScriptables.Add(type as GrassCardSO);
                
                break;
            case EGrassOptions.E_MESH:
                
                if (index > 0)
                    // Insert this at a specific position in the List.
                    m_vegetationScriptables.Insert(index, type as MeshGrassSO);
                else
                    // adds at the end of the index
                    m_vegetationScriptables.Add(type as MeshGrassSO);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
    
    
    /// <summary>
    /// Handles reinitialising the random generation resetting all of the child grass cards attached to this object. 
    /// </summary>
    public void RebuildGrassCards()
    {
        Random.InitState(seed);
        
        foreach (var scriptable in m_vegetationScriptables)
        {
            switch (scriptable)
            {
                case MeshGrassSO meshGrassSo:
                    
                    // Build the meshes
                    BuildMeshGrassScriptable(meshGrassSo);
                    break;
                
                case GrassCardSO grassCardSo:
                    
                    // Build the grass cards for this scriptable
                    BuildCardGrassScriptable(grassCardSo);
                    break;
            }
        }
        
        CompleteBuildingMesh();
    }

    private void BuildMeshGrassScriptable(MeshGrassSO meshGrassSo)
    {
        if (meshGrassSo.mesh == null) return;
        
        for (int i = 0; i < meshGrassSo.count; i++)
        {
            // creates the mesh for this object
            var tempObject = Instantiate(meshGrassSo.mesh, meshGrassSo.containerObject.go.transform);
            tempObject.tag = "Vegetation";

            // random position
            float newX = Random.Range(-meshGrassSo.positionVariation, meshGrassSo.positionVariation);
            float newZ = Random.Range(-meshGrassSo.positionVariation, meshGrassSo.positionVariation);
            Vector3 newPosition = new Vector3(newX, 0, newZ);
            tempObject.transform.position = newPosition;
            
            // Work out the random rotation of each card
            float randomYaw = Random.Range(-meshGrassSo.rotationVariationYaw, meshGrassSo.rotationVariationYaw);

            var currentEular = tempObject.transform.localEulerAngles;
            currentEular.y = randomYaw;

            tempObject.transform.localEulerAngles = currentEular;
            
            var pos = meshGrassSo.containerObject.go.transform.position;
            var randScale = UnityEngine.Random.Range(-meshGrassSo.scaleVariation, meshGrassSo.scaleVariation);
        }

    }

    private void BuildCardGrassScriptable(GrassCardSO grassCardSo)
    {
        if (grassCardSo.count > 0)
        {
            for (int i = 0; i < grassCardSo.count; i++)
            {
                // creates the mesh for this object
                MeshObject meshObject = new MeshObject(grassCardSo.containerObject.go, grassCardSo.grassMaterial, 
                    "Grass Card", "Vegetation");
                
                // Rotate the actual object to get the correct pitch
                float yAngle = 360.0f * i / grassCardSo.count;
                Quaternion radialRotation = Quaternion.Euler(0.0f, yAngle, 0.0f);
                Vector3 position = radialRotation * Vector3.forward * grassCardSo.radius;

                float newX = Random.Range(-grassCardSo.positionOffset, grassCardSo.positionOffset);
                float newZ = Random.Range(-grassCardSo.positionOffset, grassCardSo.positionOffset);
                Vector3 newPosition = new Vector3(newX, 0, newZ);
                
                float randBend = Random.Range(-grassCardSo.bend, grassCardSo.bend);
                float randPitch = Random.Range(-grassCardSo.pitchVariation, grassCardSo.pitchVariation);
                float randYaw = Random.Range(-grassCardSo.yawVariation, grassCardSo.yawVariation);

                meshObject.go.transform.Rotate(-90 + randPitch, yAngle + randYaw, 0);
                meshObject.go.transform.position = position + newPosition;
                
                var randScale = UnityEngine.Random.Range(-grassCardSo.scaleVariation, grassCardSo.scaleVariation);

                // Builds the leaf segment (just a quad with bottom pivot)
                meshObject.MeshFilter.sharedMesh = BuildCardPart(Vector3.zero, Quaternion.identity, 
                    grassCardSo.widthSegments, grassCardSo.heightSegments, 
                    grassCardSo.height, grassCardSo.width, randBend);
            }
        }
    }

    private Mesh BuildCardPart(Vector3 offset, Quaternion rotation,
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
