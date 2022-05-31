using UnityEngine;

namespace ECTool.Scripts.MeshTools
{
    /// <summary>
    /// Encapsulates all of the data and methods for constructing a game object and applying important mesh components
    /// and the handling of these.
    /// </summary>
    [System.Serializable]
    public class MeshObject
    {    
        // Getters
        private MeshFilter m_meshFilter;
        public MeshFilter MeshFilter { get { return m_meshFilter; } }

        private MeshRenderer m_meshRenderer;
        public MeshRenderer MeshRenderer { get { return m_meshRenderer; } }

        private GameObject m_go; 
        public GameObject go { get { return m_go; } }
    
        /// <summary>
        /// Encapsulates all of the methods to create a branch new game object and add all of the import mesh components
        /// to the object.
        /// </summary>
        /// <param name="parent"> The parent of this object </param>
        /// <param name="material"> The material applied to this object </param>
        /// <param name="name"> The name of this game object</param>
        /// <param name="tag"> The tag of this game object </param>
        public MeshObject(GameObject parent, Material material, string name, string tag)
        {
            // creates the new game object
            m_go = new GameObject();

            // assigns all details and sets parent
            m_go.name = name;
            m_go.tag = tag;
            m_go.transform.SetParent(parent.transform);

            // creates needed components for the mesh and materials
            m_meshFilter = m_go.AddComponent<MeshFilter>();
            m_meshRenderer = m_go.AddComponent<MeshRenderer>();
            m_meshRenderer.material = material;

            // assigns the mesh to the shared mesh and names it (for use later when saving)
            Mesh newMesh = new Mesh();
            m_meshFilter.sharedMesh = newMesh;
            m_meshFilter.sharedMesh.name = m_go.name + Random.Range(0, 1000);
        }
    }
}
