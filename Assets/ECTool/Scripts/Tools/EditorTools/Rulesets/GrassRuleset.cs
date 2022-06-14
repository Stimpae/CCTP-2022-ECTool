using System;
using ECTool.Scripts.EditorTools.Enumerations;
using UnityEditor;
using UnityEngine;

namespace ECTool.Scripts.EditorTools.Rulesets
{
    public class GrassRuleset
    {
        public bool IsValidParent(EGrassOptions addedType, EGrassOptions parentType)
        {
            switch (parentType)
            {
                case EGrassOptions.E_NONE:
                    return addedType == EGrassOptions.E_GRASSCARD || addedType == EGrassOptions.E_MESH;
                    break;
                case EGrassOptions.E_MESH:
                    return addedType == EGrassOptions.E_MESH;
                case EGrassOptions.E_GRASSCARD:
                    return addedType == EGrassOptions.E_MESH;
                default:
                    throw new ArgumentOutOfRangeException(nameof(addedType), addedType, null);
            }
        }

        public EGrassOptions GetEnumValueFromString(string name)
        {
            return name switch
            {
                "E_GRASSCARD" => EGrassOptions.E_GRASSCARD,
                "E_MESH" => EGrassOptions.E_MESH,
                _ => EGrassOptions.E_NONE
            };
        }

        public EGrassOptions GetEnumValueFromProperty(SerializedProperty property)
        {
            return property.objectReferenceValue switch
            {
                MeshGrassSO meshGrassSo => EGrassOptions.E_MESH,
                GrassCardSO enemyObject => EGrassOptions.E_GRASSCARD,
                _ => EGrassOptions.E_NONE
            };
        }

        public GameObject GetGameObjectFromProperty(SerializedProperty property)
        {
            return property.objectReferenceValue switch
            {
                VegetationSO vegetationSo => vegetationSo.Parent,
                _ => null
            };
        }
    }
    
}
