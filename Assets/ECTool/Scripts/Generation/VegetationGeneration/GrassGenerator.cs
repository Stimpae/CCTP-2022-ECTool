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
        for (int i = 0; i < grassCardSo.count; i++)
        {
            // creates the mesh for this object
            var cardObject = new MeshObject(grassCardSo.containerObject.go, grassCardSo.grassMaterial, 
                "Grass Card", "Vegetation");
            
            // random position
            float newX = Random.Range(-grassCardSo.positionVariation, grassCardSo.positionVariation);
            float newZ = Random.Range(-grassCardSo.positionVariation, grassCardSo.positionVariation);
            Vector3 newPosition = new Vector3(newX, 0, newZ);
            cardObject.go.transform.localPosition = newPosition;
            
            // Work out the random rotation of each card
            float randomYaw = Random.Range(-grassCardSo.rotationVariationYaw, grassCardSo.rotationVariationYaw);
            float randomPitch = Random.Range(-grassCardSo.rotationVariationPitch, grassCardSo.rotationVariationPitch);

            var currentEular = cardObject.go.transform.localEulerAngles;
            currentEular.x = randomPitch;
            currentEular.y = randomYaw;

            cardObject.go.transform.localEulerAngles = currentEular;
            
            var pos = grassCardSo.containerObject.go.transform.position;
            var randScale = UnityEngine.Random.Range(-grassCardSo.scaleVariation, grassCardSo.scaleVariation);

            // Create the actual mesh and apply it to our shared mesh.
            cardObject.MeshFilter.sharedMesh =
                MeshHelper.BuildQuad(pos, grassCardSo.width, grassCardSo.height, randScale);
        }
    }
}
