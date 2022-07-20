using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ECTool.Scripts.Tools.SpawningTools
{
	public static class PoissonHelper
	{
		private const int Attempts = 10;

		private static List<Vector2> _samples = new List<Vector2>();
		private static List<Vector2> _points = new List<Vector2>();
		private static List<Vector3> _spawnPoints = new List<Vector3>();

		private static int[,] _grid;
		private static float _cellSize;
		private static float _radius;
		private static Bounds _bounds;

		public static List<Vector3> GeneratePoints(Terrain terrain, float radius, int seed)
		{
			// Initialise seed
			Random.InitState(seed);

			// Reset all lists
			_samples = new List<Vector2>();
			_points = new List<Vector2>();
			_spawnPoints = new List<Vector3>();

			// Initialise base vars
			_radius = radius;
			_bounds = terrain.terrainData.bounds;
			_cellSize = radius / Mathf.Sqrt(2);
			_grid = new int[Mathf.CeilToInt(_bounds.size.x / _cellSize), Mathf.CeilToInt(_bounds.size.z / _cellSize)];

			// Select a random points within the bounds to start
			Vector2 randStartPosition = new Vector2(Random.value * _bounds.size.x,
				Random.value * _bounds.size.z);
			
			_samples.Add(randStartPosition);

			while (_samples.Count > 0)
			{
				
				int index = Random.Range(0, _samples.Count);
				Vector2 sampleCentre = _samples[index];
				bool sampleAccepted = false;

				for (int i = 0; i < Attempts; i++)
				{
					Random.InitState(seed + i + index);

					float angle = Random.value * Mathf.PI * 2;
					Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					float dist = Random.Range(radius, radius * 2);
					Vector2 sample = sampleCentre + dir * dist;

					if (IsValidSample(sample))
					{
						_points.Add(sample);
						_samples.Add(sample);
						
						// Calculate the spawn point in relation to terrain position and bounds.
						Vector3 newSpawnPoint = new Vector3(
							sample.x + (terrain.GetPosition().x + _bounds.center.x) - _bounds.extents.x, 
							0f, 
							sample.y + (terrain.GetPosition().z + _bounds.center.z) - _bounds.extents.z);
						
						_spawnPoints.Add(newSpawnPoint);

						_grid[(int)(sample.x / _cellSize), (int)(sample.y / _cellSize)] = _points.Count;
						
						sampleAccepted = true;
						break;
					}
				}
				if (!sampleAccepted)
				{
					_samples.RemoveAt(index);
				}
			}

			return _spawnPoints;
		}
		
		private static bool IsValidSample(Vector2 sample)
		{
			// Check if the sample is inside of the bounds of the terrain
			if (sample.x >= 0 && sample.x < _bounds.size.x && sample.y >= 0 && sample.y < _bounds.size.z)
			{
				Vector2Int gridPosition = new Vector2Int((int)(sample.x / _cellSize), (int)(sample.y / _cellSize));
				
				int xMin = Mathf.Max(gridPosition.x - 2,0);
				int xMax = Mathf.Min(gridPosition.x + 2,_grid.GetLength(0)-1);
				int yMin = Mathf.Max(gridPosition.y - 2,0);
				int yMax = Mathf.Min(gridPosition.y + 2,_grid.GetLength(1)-1);

				for (int y = yMin; y <= yMax; y++) 
				{
					for (int x = xMin; x <= xMax; x++) 
					{
						int index = _grid[x,y]-1;
						
						if (index != -1) {
							
							// check if this index is outside of the radius
							float sqrDst = (sample - _points[index]).sqrMagnitude;
							if (sqrDst < _radius*_radius) {
								return false;
							}
						}
					}
				}

				return true;
			}
			return false;
		}
	}
	
	
}
