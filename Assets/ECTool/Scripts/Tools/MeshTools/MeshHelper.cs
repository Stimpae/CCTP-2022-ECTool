using Unity.VisualScripting;
using UnityEngine;

namespace ECTool.Scripts.MeshTools
{
    /// <summary>
    /// This class encapsulates all of the mesh creation methods for constructing either single use planes or for building
    /// multiple types of mesh segments to work with the mesh creation technique that is currently being used.
    /// </summary>
    public static class MeshHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
         public static Mesh BuildQuad(Vector3 offset, float width, float height,
            float scale)
        {
            // creates a new mesh builder (handles creating and putting together the mesh vert/tri/uvs
            MeshBuilder meshBuilder = new MeshBuilder();

            var halfWidth = width / 2;
            
            // adds the first bottom left vert
            meshBuilder.Vertices.Add(new Vector3(-halfWidth - scale, 0.0f, 0.0f) + offset);
            meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
            meshBuilder.Normals.Add(Vector3.up);

            // adds the top left vert
            meshBuilder.Vertices.Add(new Vector3(-halfWidth- scale, height + scale, 0.0f) + offset);
            meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
            meshBuilder.Normals.Add(Vector3.up);

            // adds the bottom right vert
            meshBuilder.Vertices.Add(new Vector3(halfWidth + scale, 0.0f, 0.0f) + offset);
            meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
            meshBuilder.Normals.Add(Vector3.up);

            // adds the top right vert
            meshBuilder.Vertices.Add(new Vector3(halfWidth + scale, height + scale, 0.0f) + offset);
            meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
            meshBuilder.Normals.Add(Vector3.up);

            // gets the base index (0)
            int baseIndex = meshBuilder.Vertices.Count - 4;

            // calculates the position of each triangle
            meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
            meshBuilder.AddTriangle(baseIndex + 3, baseIndex + 2, baseIndex + 1);

            // creates and returns the mesh
            return meshBuilder.CreateMesh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshBuilder"></param>
        /// <param name="position"></param>
        /// <param name="uvs"></param>
        /// <param name="buildTriangles"></param>
        /// <param name="verticesPerRow"></param>
        /// <param name="vector3"></param>
        /// <param name="segmentCount"></param>
        /// <returns></returns>
        public static Mesh BuildQuadGrid(MeshBuilder meshBuilder, Vector3 position, Vector2 uvs, bool buildTriangles,
            int verticesPerRow, Vector3 normal)
        {
            meshBuilder.Vertices.Add(position);
            meshBuilder.UVs.Add(uvs);

            if (buildTriangles)
            {
                int baseIndex = meshBuilder.Vertices.Count - 1;

                int index0 = baseIndex;
                int index1 = baseIndex - 1;
                int index2 = baseIndex - verticesPerRow;
                int index3 = baseIndex - verticesPerRow - 1;

                meshBuilder.AddTriangle(index0, index2, index1);
                meshBuilder.AddTriangle(index2, index3, index1);
            }
            
            return meshBuilder.CreateMesh();
        }
         
        /// <summary>
        /// Builds a quad canopy mesh with a single point in the centre, mostly used for leaf billboard but can be used
        /// as a grass / plant option as well.
        /// </summary>
        /// <param name="offset"> Adds a position offset to all vertices </param>
        /// <param name="width"> Determines the width of the overall mesh </param>
        /// <param name="height"> Determines the height of the overall mesh </param>
        /// <param name="scale"> Determines the scale of the overall mesh </param>
        /// <returns></returns>
        public static Mesh BuildQuadCanopy(Vector3 offset, float width, float height,
            float scale, float canopyDip)
        {
            // creates a new mesh builder (handles creating and putting together the mesh vert/tri/uvs
            MeshBuilder meshBuilder = new MeshBuilder();
        
            // adds the first bottom left vert
            meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, 0.0f) + offset);
            meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
            meshBuilder.Normals.Add(Vector3.up);

            // adds the top left vert
            meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, width) + offset);
            meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
            meshBuilder.Normals.Add(Vector3.up);

            // adds the centre vert (for the canopy)
            meshBuilder.Vertices.Add(new Vector3(height / 2, canopyDip, width / 2) + offset);
            meshBuilder.UVs.Add(new Vector2(0.5f, 0.5f));
            meshBuilder.Normals.Add(Vector3.up);
    
            // adds the bottom right vert
            meshBuilder.Vertices.Add(new Vector3(height, 0.0f, 0.0f) + offset);
            meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
            meshBuilder.Normals.Add(Vector3.up);

            // adds the top right vert
            meshBuilder.Vertices.Add(new Vector3(height, 0.0f, width) + offset);
            meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
            meshBuilder.Normals.Add(Vector3.up);

            // gets the base index (0)
            int baseIndex = meshBuilder.Vertices.Count - 5;

            // calculates the position of each triangle
            meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
            meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
            meshBuilder.AddTriangle(baseIndex + 3, baseIndex + 2, baseIndex + 4);
            meshBuilder.AddTriangle(baseIndex + 4, baseIndex + 2, baseIndex + 1);
        
            // creates and returns the mesh
            return meshBuilder.CreateMesh();
        }

        /// <summary>
        /// Handles building a ring segment mesh, used for building any kind of circular mesh.
        /// </summary>
        /// <param name="meshBuilder"> The targeted mesh builder to be used to create this mesh</param>
        /// <param name="segmentCount"> The number of segments in this mesh, typically the number of vertices in the radius</param>
        /// <param name="centre"> The centre position of the mesh </param>
        /// <param name="radius"> The radius of the mesh </param>
        /// <param name="v"> controls the v axis for the UV mapping </param>
        /// <param name="buildTriangles"> The option to build the triangles, used to skip building triangles for the first and
        /// the last segments </param>
        /// <param name="rotation"> The rotation of the mesh </param>
        /// <param name="deformation"> The noise applied to each vertex </param>
        /// <param name="frequency">y The frequency in which to apply the segment bend </param>
        /// <returns> Return the mesh created </returns>
        public static Mesh BuildRingSegment(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles, Quaternion rotation, float deformation, int frequency)
        {
            // Calculates the number of steps to determine the rotation angle for bending
            float angleStep = 360.0f / (segmentCount);
            
            // Calculates the number of steps to determine the angle/position for sin calculations to the mesh
            float dAngleStep = 360.0f * frequency/ (segmentCount);
        
            // Loops each rotation segment in the ring 
            for (int i = 0; i < segmentCount + 1; i++)
            {
                float angle = i * angleStep;
                angle = angle * Mathf.Deg2Rad;
            
                float d = radius * deformation * Mathf.Sin(dAngleStep * i * Mathf.Deg2Rad);

                // Applies curve calculation to the ring segment
                var unit = Vector3.zero;
                unit.x = centre.x + radius * Mathf.Cos(angle) + d * Mathf.Cos(angle);
                unit.z = centre.z + radius * Mathf.Sin(angle) + d * Mathf.Sin(angle);
                unit.y = centre.y;
                
                // Outlines the new vertex position
                unit = rotation * (unit - centre) + centre;
            
                // Adds the position to the vertex list
                meshBuilder.Vertices.Add(unit);
                meshBuilder.UVs.Add(new Vector2((float) i / segmentCount, v));

                // if we should build the triangles than do so
                if (i <= 0 || !buildTriangles) continue;
                
                int baseIndex = meshBuilder.Vertices.Count - 1;
                int vertsPerRow = segmentCount + 1;

                int index0 = baseIndex;
                int index1 = baseIndex - 1;
                int index2 = baseIndex - vertsPerRow;
                int index3 = baseIndex - vertsPerRow - 1;

                meshBuilder.AddTriangle(index0, index2, index1);
                meshBuilder.AddTriangle(index2, index3, index1);
            }
            return meshBuilder.CreateMesh();
        }
    }
    
}
