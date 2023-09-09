using UnityEngine;
using UnityEditor;
using System.Linq;

namespace InfiniteValue
{
    /// <summary> 
    /// <see cref="Editor"/> inheriting <see langword="class"/> in charge of drawing the inspector for the <see cref="Configuration"/>.
    /// </summary>
    [CustomEditor(typeof(Configuration))]
    public class ConfigurationEditor : Editor
    {
        const string stringTitle = "Default ToString and Parse";
        const string inspectorTitle = "Inspector";

        // unity overrides
        public override void OnInspectorGUI()
        {
            // update object
            serializedObject.Update();

            // get variables
            SerializedProperty maxDisplayedDigitsProp = serializedObject.FindProperty("_maxDisplayedDigits");
            SerializedProperty unitsListProp = serializedObject.FindProperty("_unitsList");
            SerializedProperty displayOptionsProp = serializedObject.FindProperty("_displayOptions");

            SerializedProperty numberFormatTypeProp = serializedObject.FindProperty("_numberFormatType");
            SerializedProperty manualDecimalPointsProp = serializedObject.FindProperty("_decimalPoints");
            SerializedProperty manualSeparationsProp = serializedObject.FindProperty("_separations");
            SerializedProperty manualExponentsProp = serializedObject.FindProperty("_exponents");
            SerializedProperty cultureProp = serializedObject.FindProperty("_culture");

            SerializedProperty splitFullStringProp = serializedObject.FindProperty("_splitFullString");
            SerializedProperty drawRawEditProp = serializedObject.FindProperty("_drawRawEdit");
            SerializedProperty drawMethodsProp = serializedObject.FindProperty("_drawMethods");
            SerializedProperty drawReadOnlyPropertiesProp = serializedObject.FindProperty("_drawReadOnlyProperties");
            SerializedProperty drawTitlesProp = serializedObject.FindProperty("_drawTitles");
            SerializedProperty doubleClickDelayProp = serializedObject.FindProperty("_doubleClickDelay");

            // draw to/from string
            EditorGUILayout.LabelField(stringTitle, EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(maxDisplayedDigitsProp);
            ExtraEditorGUILayout.StringArrayFieldWithConstraints(unitsListProp, 0,
                (s) => FilterInvalidEntry(s, true, unitsListProp),
                (i) => new GUIContent($"10^{(i + 1) * 3}"), 
                "{0} Units",
                ("{0}", " "));
            EditorGUILayout.PropertyField(displayOptionsProp);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(numberFormatTypeProp);
            ++EditorGUI.indentLevel;
            if (numberFormatTypeProp.intValue == 0)
            {
                ExtraEditorGUILayout.StringArrayFieldWithConstraints(manualDecimalPointsProp, 1,
                    (s) => FilterInvalidEntry(s, true, manualDecimalPointsProp, manualSeparationsProp, manualExponentsProp),
                    (i) => new GUIContent(i == 0 ? "0 (Parse + Display)" : $"{i} (Parse)"), 
                    "{0}",
                    ("'{0}'", " "));

                ExtraEditorGUILayout.StringArrayFieldWithConstraints(manualSeparationsProp, 1,
                    (s) => FilterInvalidEntry(s, false, manualDecimalPointsProp, manualSeparationsProp, manualExponentsProp),
                    (i) => new GUIContent(i == 0 ? "0 (Parse + Display)" : $"{i} (Parse)"),
                    "{0}",
                    ("'{0}'", " "));

                ExtraEditorGUILayout.StringArrayFieldWithConstraints(manualExponentsProp, 1,
                    (s) => FilterInvalidEntry(s, true, manualDecimalPointsProp, manualSeparationsProp, manualExponentsProp),
                    (i) => new GUIContent(i == 0 ? "0 (Parse + Display)" : $"{i} (Parse)"),
                    "{0}",
                    ("'{0}'", " "));
            }
            else
            {
                CultureDrawer.dontDoIndentNextDraw = true;
                EditorGUILayout.PropertyField(cultureProp);

                GUI.enabled = false;

                EditorGUILayout.TextField(new GUIContent("Decimal Point", "Character that we will use to display (or consider valid when parsing) the decimal point."), 
                    Configuration.decimalPoints[0]);
                EditorGUILayout.TextField(new GUIContent("Separation", "Character that we will use to display (or consider valid when parsing) a separation."),
                    Configuration.separations[0]);
                EditorGUILayout.TextField(new GUIContent("Exponent", "Characters that we will use to display (or consider valid when parsing) the exponent marker."),
                    $"'{Configuration.exponents[0]}' '{Configuration.exponents[1]}'");

                GUI.enabled = true;
            }
            --EditorGUI.indentLevel;

            // draw inspector
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(inspectorTitle, EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(doubleClickDelayProp);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(splitFullStringProp);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(drawRawEditProp);
            EditorGUILayout.PropertyField(drawMethodsProp);
            EditorGUILayout.PropertyField(drawReadOnlyPropertiesProp);
            if (drawRawEditProp.boolValue || drawMethodsProp.boolValue || drawReadOnlyPropertiesProp.boolValue)
                EditorGUILayout.PropertyField(drawTitlesProp);

            // apply changes
            serializedObject.ApplyModifiedProperties();
        }

        // public static function
        public static bool FilterInvalidEntry(string entry, bool filterWhiteSpace, params SerializedProperty[] propNoDuplicates)
        {
            if (string.IsNullOrEmpty(entry))
                return false;

            if (entry.Any((c) => char.IsDigit(c) || (filterWhiteSpace && char.IsWhiteSpace(c)) || c == '\''))
                return false;

            foreach (SerializedProperty prop in propNoDuplicates)
                for (int i = 0; i < prop.arraySize; i++)
                    if (prop.GetArrayElementAtIndex(i).stringValue == entry)
                        return false;

            return true;
        }
    }
}