using System;
using System.Collections.Generic;
using ECTool.Scripts.MeshTools;
using ECTool.Scripts.Scriptables;
using ECTool.Scripts.Scriptables.Vegetation.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ECTool.Scripts.Generation.VegetationGeneration
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
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
                        m_vegetationScriptables.Insert(index, type as TrunkSO);
                    else
                        // adds at the end of the index
                        m_vegetationScriptables.Add(type as TrunkSO);
                    break;
                case ETreeOptions.E_BRANCH:
                    if (index > 0)
                        // Insert this at a specific position in the List.
                        m_vegetationScriptables.Insert(index, type as BranchSO);
                    else
                        // adds at the end of the index
                        m_vegetationScriptables.Add(type as BranchSO);
                    break;
                case ETreeOptions.E_LEAVES:
                    if (index > 0)
                        // Insert this at a specific position in the List.
                        m_vegetationScriptables.Insert(index, type as LeavesSO);
                    else
                        // adds at the end of the index
                        m_vegetationScriptables.Add(type as LeavesSO);
                    break;
                case ETreeOptions.E_NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        public void OnEnable()
        {
            // Deletes all of the children
            ResetChildren();
            
            // Creates a new instance of the settings data
            settingsData = ScriptableObject.CreateInstance<SettingsData>();

            // Resets the scriptable
            m_vegetationScriptables = new List<ScriptableObject>();
        }

        public void RebuildTree()
        {
            Random.InitState(seed);

            ResetChildren();
            
            // Loops through each of the scriptables in this list and
            // Starts constructing them
            foreach (var scriptable in m_vegetationScriptables)
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
            trunk.segmentNodes = new List<PlacementNodes>();
            
            // available nodes are the multitude of nodes spread along the 
            // area of the mesh object and where children can positions themselves
            trunk.availableNodes = new List<PlacementNodes>();
            
            // placement nodes are the actual node positions that we are going to place objects
            trunk.placementNodes = new List<PlacementNodes>();

            // creates a new mesh object (game object, mesh, meshfilter, mesh renderer)
            MeshObject meshObject = new MeshObject(trunk.containerObject.go, trunk.material, "Trunk", "Vegetation");
            MeshBuilder meshBuilder = new MeshBuilder();

            float steps = (float) 1 / trunk.segments;
            
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
                float uvMap = (float) i / trunk.segments * 10.0f;

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
                    MeshHelper.BuildRingSegment(meshBuilder, trunk.sides, centrePos, radius, uvMap,
                        i > 0, rotation, rootCurvature, trunk.rootFrequency);

                // might not actually need the mesh object here?
                trunk.segmentNodes.Add(new PlacementNodes(centrePos, radius, meshObject));
                segmentTwist += trunk.twist;
            }
            
            // 3 seems like a good amount? (to small)
            trunk.availableNodes = CalculateAvailableNodes(trunk, 10.0f, 10.0f);
        }

        private void BuildBranchScriptable(BranchSO branch)
        {
            // Reset the placement nodes for this type
            // segment nodes are just the segment positions
            branch.segmentNodes = new List<PlacementNodes>();
            
            // available nodes are the multitude of nodes spread along the 
            // area of the mesh object and where children can positions themselves
            branch.availableNodes = new List<PlacementNodes>();
            
            // placement nodes are the actual node positions that we are going to place objects
            branch.placementNodes = new List<PlacementNodes>();
            
            // only build the branches if the branch count is > 0
            if (branch.count > 0)
            {
                if (branch.parent != null && branch.parentSo)
                {
                    branch.placementNodes = CalculatePlacementNodes(branch.parentSo,
                        branch.start, branch.end, branch.count, branch.placementType);
                }
                
                float rotationIncrement = 360.0f / 6;
                float rotationOffset = Random.Range(-20, 20);
                float rotationStart = 0 + rotationOffset;

                foreach (var node in branch.placementNodes)
                {
                    MeshObject meshObject = new MeshObject(branch.containerObject.go, branch.material, "Branch",
                        "Vegetation");
                    MeshBuilder meshBuilder = new MeshBuilder();

                    // set the radius from this node
                    float percentRadius = (node.Radius / 100) * branch.radiusPercentage;
                    float radius = percentRadius;

                    int steps = 1 / branch.segments;
                    float lengthSteps = 1 / (branch.end - branch.start);

                    float newLength = branch.length *
                                      branch.lengthShape.Evaluate(lengthSteps * node.Position.y);

                    float startingPitch = branch.pitch + Random.Range(-10, 10);
                    float startingRoll = branch.roll + Random.Range(-10, 10);
                    startingPitch -= node.Position.y * branch.upAttraction;
                    
                    meshObject.go.transform.localPosition = new Vector3(node.Position.x, node.Position.y, node.Position.z);
                    meshObject.go.transform.Rotate(startingRoll, rotationStart, startingPitch);

                    float randomAngle = branch.bend + Random.Range(-branch.randomness, branch.randomness);
                    float bendAngleRadians = randomAngle * Mathf.Deg2Rad;
                    float bendRadius = newLength / bendAngleRadians;
                    float angleInc = bendAngleRadians / branch.segments;
                    Vector3 startOffset = new Vector3(bendRadius, 0, 0);

                    // calculate random frequency and random strength for sin and cos waves
                    float frequency = Random.Range(-branch.sinFrequency, branch.sinFrequency);
                    float strength = Random.Range(-branch.sinStrength, branch.sinStrength);

                    // need the bend angle code in here as well for the branches -> could have this as function
                    for (int i = 0; i <= branch.segments; i++)
                    {
                        Vector3 centrePos = Vector3.zero;

                        // sin frequency on the x as well as the y. use sin for this
                        // maybe randomise this a little bit as well invert each one depending on the amount that is chosen?
                        centrePos.y = Mathf.Sin(angleInc * i);
                        centrePos.x = Mathf.Cos(angleInc * i);
                        centrePos.z = Mathf.Sin(frequency * i) * (strength);

                        centrePos.x *= bendRadius;
                        centrePos.y *= bendRadius;
                        centrePos -= startOffset;

                        //centrePos += new Vector3(randomX, 0, randomY);

                        float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
                        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, zAngleDegrees);

                        radius = Mathf.SmoothStep(radius, 0.0f, branch.shape.Evaluate(steps * i));

                        // Dont add noise for the initial branch segment
                        if (i > 0)
                        {
                            float noiseX = Random.Range(-branch.noise, branch.noise);
                            float noiseZ = Random.Range(-branch.noise, branch.noise);
                            Vector3 noise = new Vector3(noiseX, 0, noiseZ);
                            centrePos += noise;
                        }
                        
                        float v = (float) i / 3 * 5.0f;
                        meshObject.MeshFilter.sharedMesh =
                            MeshHelper.BuildRingSegment(meshBuilder, branch.sides, centrePos, radius, v,
                                i > 0, rotation, 0.18f, 4);

                        // Adds default nodes for this branch for its children to begin calculating

                        Vector3 worldPosition = meshObject.go.transform.TransformPoint(centrePos);
                        branch.segmentNodes.Add(new PlacementNodes(worldPosition, radius, meshObject));
                    }

                    rotationStart += rotationIncrement + rotationOffset;
                }

                branch.availableNodes = CalculateAvailableNodes(branch, 4.0f, 0.2f);
            }
        }

        private void BuildLeavesScriptable(LeavesSO leaves)
        {
            // placement nodes are the actual node positions that we are going to place objects
            leaves.placementNodes = new List<PlacementNodes>();

            if (leaves.parent != null && leaves.parentSo)
            {
                leaves.placementNodes = CalculatePlacementNodes(leaves.parentSo,
                    leaves.start, leaves.end, leaves.count, leaves.placementType);
       
            }
            
            // only build the branches if the branch count is > 0
            if (leaves.count > 0)
            {
                foreach (var node in leaves.placementNodes)
                {
                    MeshObject meshObject = new MeshObject(leaves.containerObject.go, leaves.material, "Leaves",
                            "Vegetation");
                    
                        meshObject.go.transform.localPosition = new Vector3(node.Position.x, node.Position.y, node.Position.z);
                        
                        var rotation = node.RelatedObject.go.transform.localRotation;
                        
                        meshObject.go.transform.localRotation = rotation * Quaternion.Euler(leaves.yaw, leaves.roll, leaves.pitch);
                        
                        meshObject.MeshFilter.sharedMesh =
                            MeshHelper.BuildQuadCanopy(Vector3.zero, leaves.size, leaves.size, leaves.scale, 0.15f);
                }
            }
        }
    }
}