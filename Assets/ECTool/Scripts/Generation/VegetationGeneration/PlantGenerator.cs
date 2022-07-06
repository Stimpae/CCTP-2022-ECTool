using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlantGenerator : Generator
{
    /// <summary>
    /// Creates a scriptable object of the specified enum type and adds this object to the correct scriptable object
    /// container.
    /// </summary>
    /// <param name="type"> The type of scriptable object </param>
    /// <param name="option"> The enum option of what needs to be created </param>
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
                    m_testing.Insert(index, type as PlantLeafSO);
                else
                    // adds at the end of the index
                    m_testing.Add(type as PlantLeafSO);
                
                break;
            case EPlantOptions.LEAFRING:
                
                if (index > 0)
                    // Insert this at a specific position in the List.
                    m_testing.Insert(index, type as PlantLeafRingSO);
                else
                    // adds at the end of the index
                    m_testing.Add(type as PlantLeafRingSO);
                break;
            
            case EPlantOptions.STEM:

                if (index > 0)
                    // Inset this at a specific position in the List.
                    m_testing.Insert(index, type as PlantStemSO);
                else
                    // adds at the end of the index
                    m_testing.Add(type as PlantStemSO);
                
                break;
            case EPlantOptions.HEAD:
                
                if (index > 0)
                    // Insert this at a specific position in the List.
                    m_testing.Insert(index, type as PlantHeadSO);
                else
                    // adds at the end of the index
                    m_testing.Add(type as PlantHeadSO);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
    
    public void OnEnable()
    {
        // Creates a new instance of the settings data
        settingsData = ScriptableObject.CreateInstance<SettingsData>();
        m_testing = new List<ScriptableObject>();
        ResetChildren();
    }
    
    public void RebuildPlant()
    {
        Random.InitState(seed);
        
        ResetChildren();
     
        foreach (var scriptable in m_testing)
        {
           // just need to implement the functionality for this tomorrow
           // should be able to grab 
        }
    }
}
