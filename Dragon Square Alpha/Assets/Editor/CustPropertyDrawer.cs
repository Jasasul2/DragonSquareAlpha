using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(LevelLayout))]
public class CustPropertyDrawer : PropertyDrawer
{
    public static readonly int height = 9, weight = 17;
    public static float space = 25f;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        Rect newposition = position;
        newposition.y += space;
        SerializedProperty rows = property.FindPropertyRelative("rows");


        for (int y = height - 1; y > -1; y--)
        {
            SerializedProperty columns = rows.GetArrayElementAtIndex(y).FindPropertyRelative("columns");
            newposition.height = space;
            if (columns.arraySize != weight)
            {
                columns.arraySize = weight;
            }

            newposition.width = position.width / weight;

            for (int x = 0; x < weight; x++)
            {
                EditorGUI.PropertyField(newposition, columns.GetArrayElementAtIndex(x), GUIContent.none);
                newposition.x += newposition.width; 
            }
            newposition.x = position.x;
            newposition.y += space;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return space * (height + 1); 
    }
}
