using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationContainerObject
{
    private GameObject m_go; 
    public GameObject go { get { return m_go; } }

    private GameObject m_parent = null;
    
    public GameObject Parent { get { return m_parent; } }
    
    public VegetationContainerObject(GameObject parent,string name, string tag)
    {
        // creates the new game object
        m_go = new GameObject
        {
            // assigns all details and sets parent
            name = name,
            tag = tag
        };

        if (parent == null) return;
        m_parent = parent;
        m_go.transform.SetParent(parent.transform);
    }
}
