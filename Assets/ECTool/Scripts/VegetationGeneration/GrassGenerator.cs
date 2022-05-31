using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles holding all of the data and mesh objects relating to constructing a grass plane object.
/// </summary>
[ExecuteAlways]
public class GrassGenerator : MonoBehaviour
{
    // The seed to initialise the random generation
    public int seed = 1;
    
    // Collection of the spawned objects
    private List<MeshObject> m_grassObjects = new List<MeshObject>();
    private List<MeshObject> m_customGrassObjects = new List<MeshObject>();
    private List<MeshObject> m_meshObjects = new List<MeshObject>();
    
    // Collection of the spawned scriptable objects (these should correspond with the objects created?_
    private List<MeshGrassSO> m_meshSO = new List<MeshGrassSO>();
    private List<GrassCardSO> m_cardSO = new List<GrassCardSO>();
    private List<CustomGrassCardSO> m_customCardSO = new List<CustomGrassCardSO>();
    
    // feel like this can be in its own object as well
    [HideInInspector] public SettingsData settingsData;
    
    /// <summary>
    /// Creates a scriptable object of the specified enum type and adds this object to the correct scriptable object
    /// container.
    /// </summary>
    /// <param name="type"> The type of scriptable object </param>
    /// <param name="option"> The enum option of what needs to be created </param>
    /// <typeparam name="T"> The type of scriptable object to be created </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> default options type exception </exception>
    public void CreateObject<T>( T type, EGrassOptions option) where T : ScriptableObject
    {
        // need to get the parent somewhere here as well
        switch (option)
        {
            case EGrassOptions.GRASSCARD:
                m_cardSO.Add(type as GrassCardSO);
                break;
            case EGrassOptions.CUSTOMCARD:
                m_customCardSO.Add(type as CustomGrassCardSO);
                break;
            case EGrassOptions.MESH:
                m_meshSO.Add(type as MeshGrassSO);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
    
    /// <summary>
    /// When this object is enabled the settings and cards/objects are all rebuilt.
    /// </summary>
    public void OnEnable()
    {
        // Creates a new instance of the settings data
        settingsData = ScriptableObject.CreateInstance<SettingsData>();
        
        // Rebuilds all of the grass cards
        RebuildGrassCards();
    }
    
    
    /// <summary>
    /// Handles reinitialising the random generation resetting all of the child grass cards attached to this object. 
    /// </summary>
    public void RebuildGrassCards()
    {
        ResetGrassChildren();
        
        Random.InitState(seed);
        
        /*
        m_grassObjects = new List<MeshObject>();
        
        for (int i = 0; i < grassData.count; i++)
        {
            MeshObject grassCard = new MeshObject(this.gameObject, grassData.grassMaterial, "grass", tag);
            MeshBuilder meshBuilder = new MeshBuilder();
            
            // need to store the references to each card in here
            // something in here to generate the card.
        }
        */
    }
    
    
    /// <summary>
    /// Gets the current number of children attached to this object and permanently deletes them.
    /// </summary>
    public void ResetGrassChildren()
    {
        // Gets the children
        int children = transform.childCount;
        
        // As long as there is children, loop through and destroy immediate
        if (children > 0)
        {
            for (int i = children - 1; i >= 0; i--)
            {
                // Needs destroy immediate to work with editor.
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}
