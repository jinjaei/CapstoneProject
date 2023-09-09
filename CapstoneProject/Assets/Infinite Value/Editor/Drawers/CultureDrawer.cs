using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.Linq;
using System;

namespace InfiniteValue
{
    /// <summary> 
    /// <see cref="PropertyDrawer"/> inheriting <see langword="class"/> in charge of drawing the inspector field for a <see cref="CultureDrawer"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(Culture))]
    public class CultureDrawer : PropertyDrawer
    {
        // public static field
        public static bool dontDoIndentNextDraw = false;

        // consts
        static readonly GUIContent nameLabel = new GUIContent("Name", "The culture we will get the info from.");
        static Func<CultureInfo, string> getCultureNameFunc => ((c) => c.EnglishName);

        // private properties
        float lineHeight => EditorGUIUtility.singleLineHeight;
        float spaceHeight => EditorGUIUtility.standardVerticalSpacing;

        // unity overrides
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // get serialized properties
            SerializedProperty typeProp = property.FindPropertyRelative("type");
            SerializedProperty nameProp = property.FindPropertyRelative("name");

            // we do this to get the tooltip
            label = EditorGUI.BeginProperty(rect, label, property);

            // draw type
            rect.height = lineHeight;
            EditorGUI.PropertyField(rect, typeProp, label);
            rect.y += rect.height + spaceHeight;

            // draw specific culture popup
            if (typeProp.intValue == (int)Culture.Type.SpecificCulture)
            {
                if (!dontDoIndentNextDraw)
                    ++EditorGUI.indentLevel;

                List<GUIContent> namesList = CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Where((c) => c.Name != string.Empty)
                    .Select((c) => new GUIContent(getCultureNameFunc(c), c.Name)).ToList();

                int index = namesList.FindIndex((gc) => gc.tooltip == nameProp.stringValue);
                
                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(rect, nameLabel, index, namesList.ToArray());
                if (EditorGUI.EndChangeCheck())
                    nameProp.stringValue = namesList[index].tooltip;

                if (!dontDoIndentNextDraw)
                    --EditorGUI.indentLevel;
            }

            // the end
            EditorGUI.EndProperty();

            dontDoIndentNextDraw = false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty typeProp = property.FindPropertyRelative("type");

            return (typeProp.intValue == (int)Culture.Type.SpecificCulture ? 2 * lineHeight + spaceHeight : lineHeight);
        }
    }
}