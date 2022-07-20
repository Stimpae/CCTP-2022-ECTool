using System;
using System.Collections;
using System.Collections.Generic;
using ECTool.Scripts.Scriptables.Vegetation.Tree;
using UnityEditor;
using UnityEngine;

public class TreeRuleset
{
    public bool IsValidParent(ETreeOptions addedType, ETreeOptions parentType)
    {
        switch (parentType)
        {
            case ETreeOptions.E_NONE:
                return addedType == ETreeOptions.E_TRUNK;
            case ETreeOptions.E_TRUNK:
                return addedType == ETreeOptions.E_BRANCH || addedType == ETreeOptions.E_LEAVES;
            case ETreeOptions.E_BRANCH:
                return addedType == ETreeOptions.E_BRANCH || addedType == ETreeOptions.E_LEAVES;
            case ETreeOptions.E_LEAVES:
                return false;
            default:
                throw new ArgumentOutOfRangeException(nameof(addedType), addedType, null);
        }
    }

    public ETreeOptions GetEnumValueFromProperty(SerializedProperty property)
    {
        return property.objectReferenceValue switch
        {
            TrunkSO trunkSo => ETreeOptions.E_TRUNK,
            BranchSO branchSo => ETreeOptions.E_BRANCH,
            LeavesSO leavesSo => ETreeOptions.E_LEAVES,
            _ => ETreeOptions.E_NONE
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
