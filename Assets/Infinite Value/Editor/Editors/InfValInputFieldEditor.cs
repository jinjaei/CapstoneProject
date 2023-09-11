using UnityEngine;
using UnityEditor;

namespace InfiniteValue
{
    /// <summary> 
    /// <see cref="Editor"/> inheriting <see langword="class"/> in charge of drawing the inspector for an <see cref="InfValInputField"/>.
    /// </summary>
    [CustomEditor(typeof(InfValInputField))]
    public class InfValInputFieldEditor : Editor
    {
        // consts
        const string titleParse = "Parsing";
        const string titleDisplay = "Display";
        const string titleValidation = "Validation";
        const string titleOther = "Other";
        const string titleInfo = "Info";

        const string mayStillBeZeroInfo = "Value may still be 0, check the isValid property in your script.";

        static readonly GUIContent targetLabel = new GUIContent("Target", "The target input field Component that we will turn into an InfVal input field.");
        static readonly GUIContent valueLabel = new GUIContent("Value", "The current InfVal value of this input field.");
        static readonly GUIContent isValidLabel = new GUIContent("Is Valid", "Wether the value is valid or not.");

        // unity overrides
        public override void OnInspectorGUI()
        {
            // special draw if no target
            InfValInputField inputField = target as InfValInputField;
            if (inputField.target == null)
            {
                EditorGUILayout.HelpBox("This component must be used on the same object as an InputField or TMP_InputField.", MessageType.Error);

                if (GUILayout.Button("Refresh"))
                    inputField.FindTargetComponent();

                return;
            }

            // update object
            serializedObject.Update();

            // get serialized properties
            SerializedProperty useCustomUnitsProp = serializedObject.FindProperty("useCustomUnits");
            SerializedProperty unitsListProp = serializedObject.FindProperty("unitsList");
            SerializedProperty useCustomCultureProp = serializedObject.FindProperty("useCustomCulture");
            SerializedProperty cultureProp = serializedObject.FindProperty("culture");

            SerializedProperty validateTypeProp = serializedObject.FindProperty("validateType");
            SerializedProperty validColorProp = serializedObject.FindProperty("validColor");
            SerializedProperty invalidColorProp = serializedObject.FindProperty("invalidColor");
            SerializedProperty integerOnlyProp = serializedObject.FindProperty("integerOnly");
            SerializedProperty forceSignProp = serializedObject.FindProperty("forceSign");
            SerializedProperty nonZeroProp = serializedObject.FindProperty("nonZero");
            SerializedProperty interactablesIfValidProp = serializedObject.FindProperty("interactablesIfValid");

            SerializedProperty redrawOnEndEditProp = serializedObject.FindProperty("redrawOnEndEdit");
            SerializedProperty useCustomMaxDisplayedDigitsProp = serializedObject.FindProperty("useCustomMaxDisplayedDigits");
            SerializedProperty maxDisplayedDigitsProp = serializedObject.FindProperty("maxDisplayedDigits");
            SerializedProperty useCustomDisplayOptionsProp = serializedObject.FindProperty("useCustomDisplayOptions");
            SerializedProperty displayOptionsProp = serializedObject.FindProperty("displayOptions");

            // draw parsing
            EditorGUILayout.LabelField(titleParse, EditorStyles.boldLabel);

            useCustomUnitsProp.boolValue = EditorGUILayout.ToggleLeft(GetLabel(useCustomUnitsProp), useCustomUnitsProp.boolValue);
            if (useCustomUnitsProp.boolValue)
                ExtraEditorGUILayout.StringArrayFieldWithConstraints(unitsListProp, 0,
                (s) => ConfigurationEditor.FilterInvalidEntry(s, true, unitsListProp),
                (i) => new GUIContent($"10^{(i + 1) * 3}"),
                "{0} Units",
                ("{0}", " "));
            useCustomCultureProp.boolValue = EditorGUILayout.ToggleLeft(GetLabel(useCustomCultureProp), useCustomCultureProp.boolValue);
            if (useCustomCultureProp.boolValue)
                EditorGUILayout.PropertyField(cultureProp);

            // draw display
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(titleDisplay, EditorStyles.boldLabel);

            useCustomMaxDisplayedDigitsProp.boolValue = EditorGUILayout.ToggleLeft(GetLabel(useCustomMaxDisplayedDigitsProp), useCustomMaxDisplayedDigitsProp.boolValue);
            if (useCustomMaxDisplayedDigitsProp.boolValue)
                EditorGUILayout.PropertyField(maxDisplayedDigitsProp);
            useCustomDisplayOptionsProp.boolValue = EditorGUILayout.ToggleLeft(GetLabel(useCustomDisplayOptionsProp), useCustomDisplayOptionsProp.boolValue);
            if (useCustomDisplayOptionsProp.boolValue)
                EditorGUILayout.PropertyField(displayOptionsProp);

            // draw validation
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(titleValidation, EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(validateTypeProp);
            if (validateTypeProp.intValue == (int)InfValInputField.ValidateType.ChangeColor)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(validColorProp);
                EditorGUILayout.PropertyField(invalidColorProp);
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.PropertyField(integerOnlyProp);
            EditorGUILayout.PropertyField(forceSignProp);
            EditorGUILayout.PropertyField(nonZeroProp);
            if (nonZeroProp.boolValue && validateTypeProp.intValue == (int)InfValInputField.ValidateType.DisallowInvalid)
                EditorGUILayout.HelpBox(mayStillBeZeroInfo, MessageType.Info);
            EditorGUILayout.PropertyField(interactablesIfValidProp, true);

            // draw on end
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(titleOther, EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(redrawOnEndEditProp);

            // draw info
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(titleInfo, EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.ObjectField(targetLabel, inputField.target, typeof(Component), true);
            EditorGUILayout.TextField(valueLabel, inputField.FormatInfValToString(inputField.value));
            EditorGUILayout.Toggle(isValidLabel, inputField.isValid);
            GUI.enabled = true;

            // apply changes
            serializedObject.ApplyModifiedProperties();
        }

        // private method
        GUIContent GetLabel(SerializedProperty prop)
        {
            GUIContent label = EditorGUI.BeginProperty(new Rect(), new GUIContent(prop.displayName), prop);
            EditorGUI.EndProperty();

            return label;
        }
    }
}