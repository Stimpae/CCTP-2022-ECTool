using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ALL CREDITS 
// https://github.com/GucioDevs/SimpleMinMaxSlider
// Adjusted for needs of this project.

[CustomPropertyDrawer(typeof(RangeSliderAttribute))]
public class RangeSliderDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var minMaxAttribute = (RangeSliderAttribute)attribute;
        
        Rect controlRect = EditorGUI.PrefixLabel(position, label);
        
        Rect[] splittedRect = SplitRect(controlRect,3);
        
        EditorGUI.BeginChangeCheck();

        Vector2 vector = property.vector2Value;
        float minVal = vector.x;
        float maxVal = vector.y;
        
        minVal = EditorGUI.FloatField(splittedRect[0], float.Parse(minVal.ToString("")));
        maxVal = EditorGUI.FloatField(splittedRect[2], float.Parse(maxVal.ToString("")));

        EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal,
            minMaxAttribute.min,minMaxAttribute.max);

        if(minVal < minMaxAttribute.min){
            minVal = minMaxAttribute.min;
        }

        if(maxVal > minMaxAttribute.max){
            maxVal = minMaxAttribute.max;
        }

        vector = new Vector2(minVal > maxVal ? maxVal : minVal, maxVal);

        if(EditorGUI.EndChangeCheck()){
            property.vector2Value = vector;
        }
        
        Rect[] SplitRect(Rect rectToSplit, int n){
            
            Rect[] rects = new Rect[n];

            for(int i = 0; i < n; i++){

                rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n), rectToSplit.position.y, rectToSplit.width / n, rectToSplit.height);
            }

            int padding = (int)rects[0].width - 40;
            int space = 5;

            rects[0].width -= padding + space;
            rects[2].width -= padding + space;

            rects[1].x -= padding;
            rects[1].width += padding * 2;

            rects[2].x += padding + space;
        

            return rects;

        }
    }
}
