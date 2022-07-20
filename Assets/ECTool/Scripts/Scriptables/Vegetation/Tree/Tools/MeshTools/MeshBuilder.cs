using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    // set up lists and define the getters
    private readonly List<Vector3> m_Vertices = new List<Vector3>();
    public List<Vector3> Vertices { get { return m_Vertices; } }
    
    private List<Vector3> m_Normals = new List<Vector3>();
    public List<Vector3> Normals { get { return m_Normals; } }

    private List<Vector2> m_UVs = new List<Vector2>();
    public List<Vector2> UVs { get { return m_UVs; } }

    private List<int> m_Indices = new List<int>();

    /// <summary>
    /// Setter to handle adding the indices in the correct order
    /// </summary>
    /// <param name="index0"> the first index </param>
    /// <param name="index1"> the second index </param>
    /// <param name="index2"> the third index </param>
    public void AddTriangle(int index0, int index1, int index2)
    {
        m_Indices.Add(index0);
        m_Indices.Add(index1);
        m_Indices.Add(index2);
    }
 
    /// <summary>
    /// Creates the actual mesh from all of the assigned vertices, triangles, normals and uvs. Also handles recalculating
    /// the bounds and normals of the object.
    /// </summary>
    /// <returns> Returns the mesh created </returns>
    public Mesh CreateMesh()
    {
        // creates a new mesh
        Mesh mesh = new Mesh();

        // assigns the vertices
        mesh.vertices = m_Vertices.ToArray();
        mesh.triangles = m_Indices.ToArray();

        //Normals are optional. Only use them if we have the correct amount:
        if (m_Normals.Count == m_Vertices.Count)
            mesh.normals = m_Normals.ToArray();

        //UVs are optional. Only use them if we have the correct amount:
        if (m_UVs.Count == m_Vertices.Count)
            mesh.uv = m_UVs.ToArray();

        // calculate the bounds & normals
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // returns the mesh
        return mesh;
    }
}
