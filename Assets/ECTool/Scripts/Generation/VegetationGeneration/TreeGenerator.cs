using System;
using System.Collections.Generic;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables;
using ECTool.Scripts.Scriptables.Vegetation.Tree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ECTool.Scripts.Generation.VegetationGeneration
{
    [ExecuteAlways]
    public class TreeGenerator : Generator
    {
        /// <summary>
        /// Creates a scriptable object of the specified enum type and adds this object to the correct scriptable object
        /// container.
        /// </summary>
        /// <param name="type"> The type of scriptable object </param>
        /// <param name="option"> The enum option of what needs to be created </param>
        /// <param name="index"></param>
        /// <typeparam name="T"> The type of scriptable object to be created </typeparam>
        /// <exception cref="ArgumentOutOfRangeException"> default options type exception </exception>
        public void CreateObject<T>(T type, ETreeOptions option, int index) where T : ScriptableObject
        {
            // Call to parent to create all of the containers for this object
            CreateObjectContainers(type, index, "Grass Containers");

            switch (option)
            {
                case ETreeOptions.E_TRUNK:
                    if (index > 0)
                        // Insert this at a specific position in the List.
                        m_testing.Insert(index, type as TrunkSO);
                    else
                        // adds at the end of the index
                        m_testing.Add(type as TrunkSO);
                    break;
                case ETreeOptions.E_BRANCH:
                    if (index > 0)
                        // Insert this at a specific position in the List.
                        m_testing.Insert(index, type as BranchSO);
                    else
                        // adds at the end of the index
                        m_testing.Add(type as BranchSO);
                    break;
                case ETreeOptions.E_LEAVES:
                    if (index > 0)
                        // Insert this at a specific position in the List.
                        m_testing.Insert(index, type as LeavesSO);
                    else
                        // adds at the end of the index
                        m_testing.Add(type as LeavesSO);
                    break;
                case ETreeOptions.E_NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        public void OnEnable()
        {
            // Creates a new instance of the settings data
            settingsData = ScriptableObject.CreateInstance<SettingsData>();

            // MIGHT JUST CHANGE BELOW TO REBUILD?

            // Resets the scriptable
            m_testing = new List<ScriptableObject>();

            // Deletes all of the children
            ResetChildren();
        }

        public void RebuildTree()
        {
            Random.InitState(seed);

            ResetChildren();

            // Loops through each of the scriptables in this list and
            // Starts constructing them
            foreach (var scriptable in m_testing)
            {
                switch (scriptable)
                {
                    case TrunkSO trunkSo:
                        BuildTrunkScriptable(trunkSo);
                        break;
                    case BranchSO branchSo:
                        BuildBranchScriptable(branchSo);
                        break;
                    case LeavesSO leavesSo:
                        BuildLeavesScriptable(leavesSo);
                        break;
                }
            }
        }

        private void BuildTrunkScriptable(TrunkSO trunk)
        {
            // Reset the placement nodes for this type
            // trunk doesn't need any placement nodes
            trunk.defaultNodes = new List<PlacementNodes>();
            
            // creates a new mesh object (game object, mesh, meshfilter, mesh renderer)
            MeshObject meshObject = new MeshObject(trunk.containerObject.go, trunk.material, "Trunk", "Vegetation");
            MeshBuilder meshBuilder = new MeshBuilder();

            float steps = (float)1 / trunk.segments;

            float randomAngle = Random.Range(-trunk.bend, trunk.bend);
            float bendAngleRadians = randomAngle * Mathf.Deg2Rad;
            float bendRadius = trunk.length / bendAngleRadians;
            float angleInc = bendAngleRadians / trunk.segments;
            Vector3 startOffset = new Vector3(bendRadius, 0, 0);

            float rootCurvature = trunk.rootCurvature;
            float segmentTwist = 0;

            for (int i = 0; i <= trunk.segments; i++)
            {
                Vector3 centrePos = new Vector3(0, 0, 0);
                float radius = trunk.radius;

                centrePos.y = Mathf.Sin(angleInc * i);
                centrePos.x = Mathf.Cos(angleInc * i);
                centrePos.z = Mathf.Sin(trunk.sinFrequency * i) * (trunk.sinStrength);

                centrePos.x *= bendRadius;
                centrePos.y *= bendRadius;
                centrePos -= startOffset;

                float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0.0f, segmentTwist, zAngleDegrees);

                // calculate the uv and the tiling (remove tiling later possibly)
                float v = (float)i / trunk.segments * 10.0f;

                // the first two segments are dedicated to the root
                if (i < 1)
                {
                    // create ring segment in here with root height
                    radius = trunk.rootRadius;
                }
                else // build the rest of the trunk 
                {
                    float noiseX = Random.Range(-trunk.noise, trunk.noise);
                    float noiseZ = Random.Range(-trunk.noise, trunk.noise);
                    Vector3 noise = new Vector3(noiseX, 0, noiseZ);
                    centrePos += noise;

                    radius = Mathf.SmoothStep(radius, 0.0f, trunk.shape.Evaluate(steps * i));
                }

                // reset the root curve back to zero once the target height hs been reached
                if (centrePos.y > trunk.rootHeight)
                {
                    rootCurvature = 0;
                }

                // create each segment and assigns this to our mesh builder
                meshObject.MeshFilter.sharedMesh =
                    MeshHelper.BuildRingSegment(meshBuilder, trunk.sides, centrePos, radius, v,
                        i > 0, rotation, rootCurvature, trunk.rootFrequency);

                trunk.defaultNodes.Add(new PlacementNodes(centrePos, radius));
                segmentTwist += trunk.twist;
            }
        }

        private void BuildBranchScriptable(BranchSO branch)
        {
            // Reset the placement nodes for this type
            branch.defaultNodes = new List<PlacementNodes>();
            branch.placementNodes = new List<PlacementNodes>();
            
            // need to somehow get the parents scriptable object
            if (branch.parent != null)
            {
            }
        }

        private void BuildLeavesScriptable(LeavesSO leaves)
        {
        }
    }
}