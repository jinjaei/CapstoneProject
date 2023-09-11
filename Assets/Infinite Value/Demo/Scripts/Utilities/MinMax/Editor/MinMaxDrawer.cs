using UnityEditor;
using UnityEngine;

namespace IV_Demo
{
    [CustomPropertyDrawer(typeof(MinMax))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty minProp, maxProp;
            string name;
            bool minInferiorToMax;

            name = property.displayName;

            minProp = property.FindPropertyRelative("_min");
            maxProp = property.FindPropertyRelative("_max");
            minInferiorToMax = property.FindPropertyRelative("minInferiorToMax").boolValue;

            int oldIndentLevel = EditorGUI.indentLevel;

            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);
            EditorGUI.EndProperty();

            //Check if there is enough space to put the name on the same line (to save space)
            if (position.height > 16f)
            {
                position.height = 16f;
                EditorGUI.indentLevel += 1;
                contentPosition = EditorGUI.IndentedRect(position);
                contentPosition.y += 18f;
            }

            float half = contentPosition.width / 2;
            GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);

            //show the X and Y from the point
            EditorGUIUtility.labelWidth = 20f;
            contentPosition.width *= 0.5f;
            EditorGUI.indentLevel = 0;

            // Begin/end property & change check make each field
            // behave correctly when multi-object editing.
            EditorGUI.BeginProperty(contentPosition, label, minProp);
            {
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUI.DelayedFloatField(contentPosition, new GUIContent("Min"), minProp.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    minProp.floatValue = newVal;
                    if (minInferiorToMax && minProp.floatValue > maxProp.floatValue)
                        maxProp.floatValue = minProp.floatValue;
                }
            }
            EditorGUI.EndProperty();

            EditorGUIUtility.labelWidth = 24f;
            contentPosition.x += half;

            EditorGUI.BeginProperty(contentPosition, label, maxProp);
            {
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUI.DelayedFloatField(contentPosition, new GUIContent("Max"), maxProp.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    maxProp.floatValue = newVal;
                    if (minInferiorToMax && maxProp.floatValue < minProp.floatValue)
                        minProp.floatValue = maxProp.floatValue;
                }
            }
            EditorGUI.EndProperty();

            EditorGUI.indentLevel = oldIndentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Screen.width < 333 ? (16f + 18f) : 16f;
        }
    }
}