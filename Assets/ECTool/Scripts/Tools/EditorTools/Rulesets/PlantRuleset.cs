using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.Scriptables.Vegetation.Tree;
using UnityEditor;
using UnityEngine;

public class PlantRuleset
{
    public bool IsValidParent(EPlantOptions addedType, EPlantOptions parentType)
    {
        switch (parentType)
        {
            case EPlantOptions.NONE:
                return addedType == EPlantOptions.STEM || addedType == EPlantOptions.LEAFRING;
            case EPlantOptions.LEAF:
                return false;
            case EPlantOptions.LEAFRING:
                return addedType == EPlantOptions.STEM || addedType == EPlantOptions.LEAFRING;
            case EPlantOptions.STEM:
                return addedType == EPlantOptions.HEAD || addedType == EPlantOptions.LEAF;
            case EPlantOptions.HEAD:
                return false;
            default:
                throw new ArgumentOutOfRangeException(nameof(addedType), addedType, null);
        }
    }

    public EPlantOptions GetEnumValueFromProperty(SerializedProperty property)
    {
        return property.objectReferenceValue switch
        {
            PlantLeafSO leafSo => EPlantOptions.LEAF,
            PlantLeafRingSO leafRingSo => EPlantOptions.LEAFRING,
            PlantStemSO leafStemSo => EPlantOptions.STEM,
            PlantHeadSO leafHeadSo => EPlantOptions.HEAD,
            _ => EPlantOptions.NONE
        };
    }

    public GameObject GetParentFromProperty(SerializedProperty property)
    {
        return property.objectReferenceValue switch
        {
            VegetationSo vegetationSo => vegetationSo.parent,
            _ => null
        };
    }
}
