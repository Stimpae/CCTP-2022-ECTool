using UnityEngine;

namespace ECTool.Scripts.Tools.SpawningTools
{
    /// <summary>
    /// Contains terrain helper functions for a variety of uses
    /// </summary>
    public static class  TerrainHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="texture"></param>
        public static void SetTerrainFromHeightmap(Terrain terrain, Texture2D texture)
        {
            var terrainData = terrain.terrainData;

            terrainData.heightmapResolution = texture.width + 1;
            terrainData.baseMapResolution = texture.width;
            
            float[,] heights = new float[texture.width, texture.height];

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    heights[x, y] = texture.GetPixel(x, y).grayscale;
                }
            }
            
            // run the heights through a guassian blur function to smooth out
            // the terrain

            terrainData.SetHeights(0, 0, heights);
            terrain.terrainData = terrainData;
            
        }
    }
}
