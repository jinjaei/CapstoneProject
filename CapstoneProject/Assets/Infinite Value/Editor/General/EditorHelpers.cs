using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;

namespace InfiniteValue
{
    /// Editor utility class.
    static class ExtraEditorGUILayout
    {
        // consts
        const float stringArrayButtonWidth = 22;
        const string stringArrayMoveUpText = "▲";
        const string stringArrayMoveDownText = "▼";
        const string stringArrayDeleteText = "X";
        const string stringArrayAddText = "Insert";
        const string newLabel = "New";

        // private fields
        static Dictionary<string, (string entry, int insertPos)> addFieldsDico = new Dictionary<string, (string, int)>();

        static StringBuilder strBuilder = new StringBuilder();

        // public methods

        /// <summary> 
        /// Draw a <see cref="SerializedProperty"/> of a <see langword="string"/> array or List field with a check before adding a value.
        /// </summary>
        /// <param name="arrayProp">The array or List SerializedProperty.</param>
        /// <param name="minArrayCount">Minimum number of elements in the array.</param>
        /// <param name="canBeAddedFunc">Function that return wether a string can be added to the array.</param>
        /// <param name="indexToLabelFunc">Function that return the label of an element from it's index.</param>
        /// <param name="sizeFormat">Format used to display the array size ({0}).</param>
        /// <param name="foldedStr">Parameter that define how to draw the string used when the property is folded. elemFormat is a format applied to every elements and separator is added in between them.</param>
        public static void StringArrayFieldWithConstraints(SerializedProperty arrayProp, int minArrayCount, Func<string, bool> canBeAddedFunc, 
            Func<int, GUIContent> indexToLabelFunc = null, string sizeFormat = null, (string elemFormat, string separator) foldedStr = default)
        {
            bool guiEnabled = GUI.enabled;

            // folded draw
            if (!arrayProp.isExpanded)
            {
                Rect rect = EditorGUILayout.GetControlRect();

                arrayProp.isExpanded = EditorGUI.Foldout(rect, arrayProp.isExpanded, " ", true);

                // we do this to get the tooltip
                GUIContent label = EditorGUI.BeginProperty(rect, new GUIContent(arrayProp.displayName), arrayProp);
                EditorGUI.LabelField(rect, label, new GUIContent(" "));
                EditorGUI.EndProperty();

                // show values as a single string
                string format = foldedStr.elemFormat ?? "{0}";
                string sep = foldedStr.separator ?? ", ";

                strBuilder.Clear();
                for (int i = 0; i < arrayProp.arraySize; i++)
                {
                    if (strBuilder.Length > 0)
                        strBuilder.Append(sep);

                    strBuilder.AppendFormat(format, arrayProp.GetArrayElementAtIndex(i).stringValue);
                }

                GUI.enabled = false;
                EditorGUI.TextField(rect, " ", strBuilder.ToString());
                GUI.enabled = guiEnabled;
            }
            // unfolded draw
            else
            {
                // set/get base variables
                indexToLabelFunc = indexToLabelFunc ?? ((i) => new GUIContent($"Element {i}"));
                sizeFormat = sizeFormat ?? "Size: {0}";

                Color cacheGuiBackgroundColor = GUI.backgroundColor;

                float buttonWidth = stringArrayButtonWidth;
                float buttonSpacing = EditorGUIUtility.standardVerticalSpacing;
                float fieldReducedWidth = (buttonWidth + buttonSpacing) * 3;

                // main label and array size
                {
                    Rect rect = EditorGUILayout.GetControlRect();

                    arrayProp.isExpanded = EditorGUI.Foldout(rect, arrayProp.isExpanded, " ", true);

                    // we do this to get the tooltip
                    GUIContent label = EditorGUI.BeginProperty(rect, new GUIContent(arrayProp.displayName), arrayProp);
                    EditorGUI.LabelField(rect, label, new GUIContent(" "));
                    EditorGUI.EndProperty();

                    // display the array size
                    GUI.enabled = false;
                    EditorGUI.LabelField(rect, " ", string.Format(sizeFormat, arrayProp.arraySize));
                    GUI.enabled = guiEnabled;
                }

                ++EditorGUI.indentLevel;

                string key = arrayProp.UniqueKey();

                if (!addFieldsDico.ContainsKey(key))
                    addFieldsDico[key] = ("", arrayProp.arraySize);

                // show added elements
                for (int i = 0; i < arrayProp.arraySize; i++)
                {
                    if (addFieldsDico[key].insertPos == i)
                        ShowInsertMarker();

                    Rect rect = EditorGUILayout.GetControlRect();

                    // draw elem field readonly
                    rect.xMax -= fieldReducedWidth;

                    GUIContent gc = indexToLabelFunc(i);
                    if (string.IsNullOrEmpty(gc.tooltip))
                        gc.tooltip = gc.text;
                    EditorGUI.LabelField(rect, gc, new GUIContent(" "));

                    if (guiEnabled)
                    {
                        GUI.enabled = false;
                        GUI.backgroundColor = new Color(GUI.contentColor.r, GUI.contentColor.g, GUI.contentColor.b, GUI.contentColor.a * 2f);
                    }

                    EditorGUI.PropertyField(rect, arrayProp.GetArrayElementAtIndex(i), new GUIContent(" "));

                    if (guiEnabled)
                    {
                        GUI.enabled = true;
                        GUI.backgroundColor = cacheGuiBackgroundColor;
                    }

                    // draw buttons move up, down and clear
                    rect.xMin = rect.xMax + buttonSpacing;
                    rect.width = buttonWidth;

                    GUI.enabled = guiEnabled && (i != 0);
                    if (GUI.Button(rect, stringArrayMoveUpText))
                        arrayProp.MoveArrayElement(i, i - 1);

                    rect.x += rect.width + buttonSpacing;
                    GUI.enabled = guiEnabled && (i != arrayProp.arraySize - 1);
                    if (GUI.Button(rect, stringArrayMoveDownText))
                        arrayProp.MoveArrayElement(i, i + 1);

                    rect.x += rect.width + buttonSpacing;
                    GUI.enabled = guiEnabled && (minArrayCount < arrayProp.arraySize);
                    if (GUI.Button(rect, stringArrayDeleteText))
                        arrayProp.DeleteArrayElementAtIndex(i);

                    GUI.enabled = guiEnabled;
                }

                if (arrayProp.arraySize > 0)
                {
                    if (addFieldsDico[key].insertPos == arrayProp.arraySize)
                        ShowInsertMarker();

                    // small space (not using EditorGUILayout.Space(4) for compatibility with old Unity versions)
                    EditorGUILayout.GetControlRect(true, 4);
                }

                // add field
                {
                    if (addFieldsDico[key].insertPos > arrayProp.arraySize)
                        addFieldsDico[key] = (addFieldsDico[key].entry, arrayProp.arraySize);

                    Rect rect = EditorGUILayout.GetControlRect();
                    Rect sliderRect = (new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth - buttonSpacing, rect.height));

                    int cacheIndentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;

                    // text field
                    rect.xMin += EditorGUIUtility.labelWidth;
                    rect.xMax -= fieldReducedWidth;

                    addFieldsDico[key] = (EditorGUI.TextField(rect, addFieldsDico[key].entry), addFieldsDico[key].insertPos);

                    // insert button
                    rect.xMin = rect.xMax + buttonSpacing;
                    rect.width = fieldReducedWidth - buttonSpacing;

                    GUI.enabled = guiEnabled && canBeAddedFunc(addFieldsDico[key].entry);
                    if (GUI.Button(rect, stringArrayAddText))
                    {
                        GUI.FocusControl(null);
                        arrayProp.InsertArrayElementAtIndex(addFieldsDico[key].insertPos);
                        arrayProp.GetArrayElementAtIndex(addFieldsDico[key].insertPos).stringValue = addFieldsDico[key].entry;
                        addFieldsDico[key] = ("", arrayProp.arraySize);
                    }
                    GUI.enabled = guiEnabled;

                    EditorGUI.indentLevel = cacheIndentLevel;

                    // insert pos slider
                    if (arrayProp.arraySize > 0 && canBeAddedFunc(addFieldsDico[key].entry))
                    {
                        float cacheFieldW = EditorGUIUtility.fieldWidth;
                        EditorGUIUtility.fieldWidth = 22f;

                        addFieldsDico[key] = (addFieldsDico[key].entry, EditorGUI.IntSlider(sliderRect, addFieldsDico[key].insertPos, 0, arrayProp.arraySize));

                        EditorGUIUtility.fieldWidth = cacheFieldW;
                    }
                    else
                        EditorGUI.LabelField(sliderRect, newLabel);
                }

                // small space (not using EditorGUILayout.Space(4) for compatibility with old Unity versions)
                EditorGUILayout.GetControlRect(true, 4);

                --EditorGUI.indentLevel;

                // local function
                void ShowInsertMarker()
                {
                    if (!canBeAddedFunc(addFieldsDico[key].entry))
                        return;

                    Rect rect = EditorGUILayout.GetControlRect(false, 6);

                    rect.xMax -= fieldReducedWidth;
                    rect.xMin += EditorGUIUtility.labelWidth;
                    rect.height = 2;
                    rect.y += 2;

                    EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? Color.white : Color.black);
                }
            }
        }
    }

    /// Editor utility class.
    static class SerializedPropertyExtensions
    {
        /// <summary> 
        /// This function return a string unique to a <see cref="SerializedProperty"/>, it is useful for making parameters fields related to one property in an editor
        /// or property drawer without adding a useless out of editor usage field to the base <see langword="class"/>.
        /// </summary>
        public static string UniqueKey(this SerializedProperty prop) => $"{prop.serializedObject.targetObject.GetInstanceID()} {prop.propertyPath}";
    }
}