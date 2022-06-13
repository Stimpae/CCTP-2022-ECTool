using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.EditorTools.Enumerations;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables;
using Unity.VisualScripting;
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
    private List<MeshObject> m_meshObjects = new List<MeshObject>();
    
    // Collection of the spawned scriptable objects (these should correspond with the objects created?_
    [SerializeField, HideInInspector]
    private List<ScriptableObject>  m_testing = new List<ScriptableObject>();
    
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
    public void CreateObject<T>( T type, EGrassOptions option, int index) where T : ScriptableObject
    {
        // need to get the parent somewhere here as well
        MeshObject meshObject;
        
        switch (option)
        {
            // need to check if something can be parented down here
            case EGrassOptions.E_GRASSCARD:
                
                if (index > 0)
                {
                    var scriptable = m_testing[index - 1];
                    var card = type as GrassCardSO;
                  
                    switch (scriptable)
                    {
                        case MeshGrassSO meshGrassSo:
                            if (card is { }) card.parent = meshGrassSo.Object.go;
                            break;
                        case GrassCardSO grassCardSo:
                            if (card is { }) card.parent = grassCardSo.Object.go;
                            break;
                    }
                    
                    meshObject = new MeshObject(card.parent, null, "GrassCard", "Player");
                    if (type is GrassCardSO { } grassCard) grassCard.Object = meshObject;

                    // adds at the targeted index
                    m_testing.Insert(index, type as GrassCardSO);
                }
                else
                {
                    meshObject = new MeshObject(gameObject, null, "GrassCard", "Player");
                    if (type is GrassCardSO { } grassCard)
                    {
                        grassCard.Object = meshObject;
                        grassCard.parent = null;
                    } 
                    
                    // adds at the end of the index
                    m_testing.Add(type as GrassCardSO);
                }
                
                break;
            case EGrassOptions.E_MESH:
                
                if (index > 0)
                {
                    var scriptable = m_testing[index - 1];
                    var card = type as MeshGrassSO;
                    
                    switch (scriptable)
                    {
                        case MeshGrassSO meshGrassSo:
                            if (card is { }) card.parent = meshGrassSo.Object.go;
                            break;
                        case GrassCardSO grassCardSo:
                            if (card is { }) card.parent = grassCardSo.Object.go;
                            break;
                    }
                    
                    meshObject = new MeshObject(card.parent, null, "MeshCard", "Player");
                    if (type is MeshGrassSO { } meshCard) meshCard.Object = meshObject;
                    
                    m_testing.Insert(index, type as MeshGrassSO);
                }
                else
                {    
                    meshObject = new MeshObject(gameObject, null, "MeshCard", "Player");
                    if (type is MeshGrassSO { } meshCard)
                    {
                        meshCard.Object = meshObject;
                        meshCard.parent = null;
                    }

                    // adds at the end of the index
                    m_testing.Add(type as MeshGrassSO);
                }
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
        
        m_testing = new List<ScriptableObject>();
        
        ResetGrassChildren();
    }
    
    
    /// <summary>
    /// Handles reinitialising the random generation resetting all of the child grass cards attached to this object. 
    /// </summary>
    public void RebuildGrassCards()
    {
        Random.InitState(seed);
        
        // make a grass object for each type of mesh 
        // have to do something different for meshes but get this working first
        
        // create a mesh builder for each mesh as well object as well

        foreach (var scriptable in m_testing)
        {
            switch (scriptable)
            {
                case MeshGrassSO meshGrassSo:
                    
                    // can use this to get the previous index for parenting etc;
                    //int meshIndex = m_testing.IndexOf(meshGrassSo);
                    
                    //BuildMeshGrassScriptable(meshGrassSo,);
                    break;
                case GrassCardSO grassCardSo:
                    // call the card stuff 
                    
                    break;
            }
        }
    }

    private void BuildMeshGrassScriptable(MeshGrassSO meshGrassSo)
    {
        
    }

    private void BuildCardGrassScriptable(GrassCardSO grassCardSo)
    {
        
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
