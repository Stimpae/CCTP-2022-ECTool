using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TreeRuleset
{
    public bool IsValidParent(ETreeOptions addedType, ETreeOptions parentType)
    {
        switch (parentType)
        {
            case ETreeOptions.E_NONE:
                return false;
            case ETreeOptions.E_TRUNK:
                return false;
            case ETreeOptions.E_BRANCH:
                return false;
            case ETreeOptions.E_LEAVES:
                return false;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(addedType), addedType, null);
        }
    }

    public GameObject GetParentFromProperty(SerializedProperty property)
    {
        return property.objectReferenceValue switch
        {
            VegetationSO vegetationSo => vegetationSo.Parent,
            _ => null
        };
    }
}
