using UnityEngine;
using UnityEditor;

namespace InfiniteValue
{
    /// <summary> 
    /// <see cref="PropertyDrawer"/> inheriting <see langword="class"/> in charge of drawing the inspector field for a <see cref="DisplayOption"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(DisplayOption))]
    public class DisplayOptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(rect, label, property);
            property.intValue = (int)(object)EditorGUI.EnumFlagsField(rect, label, (DisplayOption)property.intValue);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}