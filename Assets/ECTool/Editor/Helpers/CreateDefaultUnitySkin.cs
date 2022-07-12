using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateDefaultUnitySkin : Editor
{
    [MenuItem("Assets/Save Editor Skin")]
    public static void SaveEditorSkinToAssets()
    {
        var skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene));
        AssetDatabase.CreateAsset(skin, "Assets/ECTool/Editor/EditorSkin.guiskin");
    }
}
