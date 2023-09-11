using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

namespace InfiniteValue
{
    /// <summary> 
    /// <see cref="PropertyDrawer"/> inheriting <see langword="class"/> in charge of drawing the inspector field for an <see cref="InfVal"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(InfVal))]
    public class InfValDrawer : PropertyDrawer
    {
        // consts
        const float bigSpaceHeight = 6;
        const float executeButtonWidth = 60;

        const int tooMuchCharacters = 512;
        const string tooMuchDigitsStr = "Too much digits to display";

        const string undefinedStr = "Undefined";
        const string executeButtonText = "Execute";

        static readonly GUIContent rawEditTitle = new GUIContent("Raw Edit");
        static readonly GUIContent methodsTitle = new GUIContent("Edit Methods");
        static readonly GUIContent propertiesTitle = new GUIContent("Properties");

        static readonly GUIContent digitsLabel = new GUIContent("Digits", "BigInteger representing all the digits of this InfVal.");
        static readonly GUIContent exponentLabel = new GUIContent("Exponent", 
            "By what power of 10 should we multiply the digits to get this Infval value.");

        static readonly GUIContent toPrecisionLabel = new GUIContent("To Precision", "Modify the precision of this InfVal (the digits count).");
        static readonly GUIContent removeTrailingZerosLabel = new GUIContent("Remove Trailing Zeros", 
            "Remove Zeros at the end of the InfVal returning one with the exact same value but a greater exponent.");
        static readonly GUIContent movePointLeftLabel = new GUIContent("Move Point Left", "Move the decimal point to the left.");
        static readonly GUIContent movePointRightLabel = new GUIContent("Move Point Right", "Move the decimal point to the right.");

        static readonly GUIContent precisionLabel = new GUIContent("Precision", "Count of digits of this InfVal.");
        static readonly GUIContent isIntegerLabel = new GUIContent("Is Integer", "Is this InfVal an integer value.");
        static readonly GUIContent isZeroLabel = new GUIContent("Is Zero", "Is this InfVal equal to 0.");
        static readonly GUIContent isOneLabel = new GUIContent("Is One", "Is this InfVal equal to 1.");
        static readonly GUIContent isEvenLabel = new GUIContent("Is Even", "Is this InfVal an even number.");
        static readonly GUIContent isPowerOfTwoLabel = new GUIContent("Is Power Of Two", "Is this Infval 2 to an integer power.");
        static readonly GUIContent signLabel = new GUIContent("Sign", "The sign of this InfVal: 1, 0 or -1.");

        const float spaceLabelToFieldWidth = 3.5f;
        const float extraViewWidth = 34;

        // private properties
        float lineHeight => EditorGUIUtility.singleLineHeight;
        float spaceHeight => EditorGUIUtility.standardVerticalSpacing;

        float spaceWidth => 2;

        GUIStyle wrapLabelStyle
        {
            get
            {
                if (_wrapLabelStyle == null)
                {
                    _wrapLabelStyle = new GUIStyle(EditorStyles.label);
                    _wrapLabelStyle.wordWrap = true;
                }
                return _wrapLabelStyle;
            }
        }

        // private types
        class PerInfValParams
        {
            public int precision;
            public int moveLeft;
            public int moveRight;
        }

        // private fields
        float indentWidth = -1;

        string delayedStr = null;
        bool selectAllNextMouseUp = false;

        GUIStyle _wrapLabelStyle = null;
        Color? cacheLabelNormalColor = null;

        double lastClickInZoneTime = -1;

        Dictionary<string, PerInfValParams> paramsDico = new Dictionary<string, PerInfValParams>();

        // public unity overrides
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // get variables
            SerializedProperty digitsStrProp = property.FindPropertyRelative("s_digits");
            SerializedProperty exponentProp = property.FindPropertyRelative("s_exponent");

            BigInteger bigInt = BigInteger.Parse(digitsStrProp.stringValue);
            InfVal target = InfVal.ManualFactory(bigInt, exponentProp.intValue);

            bool cacheGUIenabled = GUI.enabled;

            if (indentWidth < 0)
            {
                Rect newRect = rect;
                float w1 = EditorGUI.IndentedRect(newRect).width;
                ++EditorGUI.indentLevel;
                float w2 = EditorGUI.IndentedRect(newRect).width;
                --EditorGUI.indentLevel;
                indentWidth = w1 - w2;
            }

            // start (get tooltip)
            label = EditorGUI.BeginProperty(rect, label, property);

            // cache original rect
            rect.height = lineHeight;
            Rect originalRect = new Rect(rect);

            // show foldout
            rect.width = EditorGUI.indentLevel * indentWidth;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, " ", false);

            // reset rect
            rect = originalRect;

            // generate a unique name for the input field
            string inputFieldControlName = $"InfVal NormalInputField {property.serializedObject.targetObject.GetInstanceID()}" +
                $" {property.propertyPath}";

            // show label as an int field for scroll capabilities
            {
                rect.width = EditorGUIUtility.labelWidth + spaceLabelToFieldWidth;

                // support for double click to fold/unfold the property
                if (DoubleClick(rect))
                    property.isExpanded = !property.isExpanded;

                SetLabelStyle(GUI.GetNameOfFocusedControl() == inputFieldControlName);

                EditorGUI.BeginChangeCheck();
                int scrollVal = EditorGUI.IntField(rect, label, 0);
                if (EditorGUI.EndChangeCheck())
                {
                    bigInt += scrollVal;
                    digitsStrProp.stringValue = bigInt.ToString();
                }

                SetLabelStyle();
            }

            // show text area
            {
                int cacheIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                rect.x += rect.width - spaceLabelToFieldWidth;
                rect.width = originalRect.width - rect.width + spaceLabelToFieldWidth;

                GUI.SetNextControlName(inputFieldControlName);

                EditorGUI.BeginChangeCheck();
                string newStr = EditorGUI.DelayedTextField(rect, target.ToString(), EditorStyles.numberField);
                if (EditorGUI.EndChangeCheck())
                    ChangeTarget(new InfVal(newStr));

                EditorGUI.indentLevel = cacheIndent;
            }

            // foldout only draw
            if (property.isExpanded)
            {
                ++EditorGUI.indentLevel;

                // reset rect
                rect = originalRect;

                rect.y += lineHeight + spaceHeight;

                // draw read only full InfVal value
                if (DigitsToDisplay(target) <= tooMuchCharacters)
                {
                    // no split
                    if (!Configuration.splitFullString)
                    {
                        string toDisplay = target.ToString(-1, null, DisplayOption.AddSeparatorsBeforeDecimalPoint |
                            DisplayOption.AddSeparatorsAfterDecimalPoint | DisplayOption.KeepZerosAfterDecimalPoint);
                        rect.height = wrapLabelStyle.CalcHeight(new GUIContent(toDisplay), EditorGUI.IndentedRect(rect).width);

                        GUI.enabled = false;

                        EditorGUI.LabelField(rect, toDisplay, wrapLabelStyle);
                        rect.y += rect.height + spaceHeight;

                        GUI.enabled = cacheGUIenabled;
                    }
                    // with split
                    else
                    {
                        string[] toDisplay = target.ToString(-1, null, DisplayOption.AddSeparatorsBeforeDecimalPoint |
                            DisplayOption.AddSeparatorsAfterDecimalPoint | DisplayOption.KeepZerosAfterDecimalPoint)
                            .Split(Configuration.decimalPoints, StringSplitOptions.RemoveEmptyEntries);

                        GUI.enabled = false;

                        // integer part
                        rect.height = wrapLabelStyle.CalcHeight(new GUIContent(toDisplay[0]), EditorGUI.IndentedRect(rect).width);
                        EditorGUI.LabelField(rect, toDisplay[0], wrapLabelStyle);
                        rect.y += rect.height + spaceHeight;

                        if (toDisplay.Length > 1)
                        {
                            toDisplay[1] = Configuration.decimalPoints[0] + toDisplay[1];
                            TextAnchor cacheAnchor = wrapLabelStyle.alignment;
                            wrapLabelStyle.alignment = TextAnchor.LowerRight;

                            // decimal part
                            rect.height = wrapLabelStyle.CalcHeight(new GUIContent(toDisplay[1]), EditorGUI.IndentedRect(rect).width);
                            EditorGUI.LabelField(rect, toDisplay[1], wrapLabelStyle);
                            rect.y += rect.height + spaceHeight;

                            wrapLabelStyle.alignment = cacheAnchor;
                        }

                        GUI.enabled = cacheGUIenabled;
                    }
                }
                else
                {
                    GUI.enabled = false;

                    rect.height = lineHeight;
                    EditorGUI.LabelField(rect, tooMuchDigitsStr);
                    rect.y += rect.height + spaceHeight;

                    GUI.enabled = cacheGUIenabled;
                }
                    
                // draw raw edit
                if (Configuration.drawRawEdit)
                {
                    rect.y += bigSpaceHeight;

                    // title
                    if (Configuration.drawTitles)
                        rect = DrawTitle(rect, rawEditTitle);

                    // digits
                    {
                        rect.height = lineHeight;

                        string textAreaControlName = $"InfVal DelayedDigitsArea {property.serializedObject.targetObject.GetInstanceID()}" +
                            $" {property.propertyPath}";

                        SetLabelStyle(GUI.GetNameOfFocusedControl() == textAreaControlName);
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height), digitsLabel);
                        SetLabelStyle();
                        rect.y += lineHeight + spaceHeight;

                        if (target.precision <= tooMuchCharacters)
                        {
                            EditorGUI.BeginChangeCheck();
                            string newDigitsStr = DigitsDelayedArea(ref rect, digitsStrProp.stringValue, textAreaControlName);
                            if (EditorGUI.EndChangeCheck())
                                digitsStrProp.stringValue = newDigitsStr;
                        }
                        else
                        {
                            GUI.enabled = false;

                            rect.height = lineHeight;
                            EditorGUI.LabelField(rect, tooMuchDigitsStr);

                            GUI.enabled = cacheGUIenabled;
                        }
                        
                        rect.y += rect.height + spaceHeight;
                    }

                    // exponent
                    {
                        rect.height = lineHeight;

                        EditorGUI.BeginChangeCheck();
                        int newExp = EditorGUI.DelayedIntField(rect, exponentLabel, exponentProp.intValue);
                        if (EditorGUI.EndChangeCheck())
                            exponentProp.intValue = newExp;

                        rect.y += lineHeight + spaceHeight;
                    }
                }

                // edit methods button
                if (Configuration.drawMethods)
                {
                    rect.y += bigSpaceHeight;

                    // title
                    if (Configuration.drawTitles)
                        rect = DrawTitle(rect, methodsTitle);

                    PerInfValParams param = null;
                    if (!paramsDico.TryGetValue(property.UniqueKey(), out param))
                        param = paramsDico[property.UniqueKey()] = new PerInfValParams() { precision = target.precision };

                    Rect methodRect = new Rect(rect);
                    methodRect.width -= executeButtonWidth + spaceWidth;
                    Rect buttonRect = new Rect(rect);
                    buttonRect.xMin += methodRect.width + spaceWidth;

                    // to precision method
                    param.precision = EditorGUI.IntField(methodRect, toPrecisionLabel, param.precision);
                    param.precision = Mathf.Max(1, param.precision);

                    GUI.enabled = (param.precision != target.precision);
                    if (GUI.Button(buttonRect, executeButtonText))
                    {
                        GUI.FocusControl(null);
                        ChangeTarget(target.ToPrecision(param.precision));
                    }
                    GUI.enabled = cacheGUIenabled;

                    rect.y += lineHeight + spaceHeight;
                    methodRect.y = buttonRect.y = rect.y;

                    // remove trailing zeros method
                    string di = digitsStrProp.stringValue;

                    EditorGUI.LabelField(methodRect, removeTrailingZerosLabel);
                    GUI.enabled = (!target.isZero && di[di.Length - 1] == '0');
                    if (GUI.Button(buttonRect, executeButtonText))
                    {
                        GUI.FocusControl(null);
                        ChangeTarget(target.RemoveTrailingZeros());
                    }
                    GUI.enabled = cacheGUIenabled;

                    rect.y += lineHeight + spaceHeight;
                    methodRect.y = buttonRect.y = rect.y;

                    // move point left method
                    param.moveLeft = EditorGUI.IntField(methodRect, movePointLeftLabel, param.moveLeft);
                    GUI.enabled = (param.moveLeft != 0);
                    if (GUI.Button(buttonRect, executeButtonText))
                    {
                        GUI.FocusControl(null);
                        ChangeTarget(target.MovePointLeft(param.moveLeft));
                        param.moveLeft = 0;
                    }
                    GUI.enabled = cacheGUIenabled;

                    rect.y += lineHeight + spaceHeight;
                    methodRect.y = buttonRect.y = rect.y;

                    // move point right method
                    param.moveRight = EditorGUI.IntField(methodRect, movePointRightLabel, param.moveRight);
                    GUI.enabled = (param.moveRight != 0);
                    if (GUI.Button(buttonRect, executeButtonText))
                    {
                        GUI.FocusControl(null);
                        ChangeTarget(target.MovePointRight(param.moveRight));
                        param.moveRight = 0;
                    }
                    GUI.enabled = cacheGUIenabled;

                    rect.y += lineHeight + spaceHeight;
                    methodRect.y = buttonRect.y = rect.y;
                }

                // read only properties
                if (Configuration.drawReadOnlyProperties)
                {
                    rect.y += bigSpaceHeight;

                    // title
                    if (Configuration.drawTitles)
                        rect = DrawTitle(rect, propertiesTitle);

                    GUI.enabled = false;

                    DrawOneInfValProperty(ref rect, precisionLabel, () => target.precision.ToString());
                    DrawOneInfValProperty(ref rect, isIntegerLabel, () => target.isInteger.ToString());
                    DrawOneInfValProperty(ref rect, isZeroLabel, () => target.isZero.ToString());
                    DrawOneInfValProperty(ref rect, isOneLabel, () => target.isOne.ToString());
                    DrawOneInfValProperty(ref rect, isEvenLabel, () => target.isEven.ToString());
                    DrawOneInfValProperty(ref rect, isPowerOfTwoLabel, () => target.isPowerOfTwo.ToString());
                    DrawOneInfValProperty(ref rect, signLabel, () => target.sign.ToString());

                    GUI.enabled = cacheGUIenabled;
                }

                --EditorGUI.indentLevel;
            }

            // end
            EditorGUI.EndProperty();

            // local functions
            void ChangeTarget(InfVal value)
            {
                target = value;

                digitsStrProp.stringValue = target.digits.ToString();
                exponentProp.intValue = target.exponent;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // if folded, return one line
            if (!property.isExpanded)
                return lineHeight;

            // get vars
            SerializedProperty digitsStrProp = property.FindPropertyRelative("s_digits");
            SerializedProperty exponentProp = property.FindPropertyRelative("s_exponent");

            InfVal target = InfVal.ManualFactory(BigInteger.Parse(digitsStrProp.stringValue), exponentProp.intValue);
            
            Rect virtualRect = new Rect();
            virtualRect.width = EditorGUIUtility.currentViewWidth - extraViewWidth;

            // add to height based on what is drawn
            float height = lineHeight;

            ++EditorGUI.indentLevel;

            // read only full representation
            if (DigitsToDisplay(target) <= tooMuchCharacters)
            {
                if (!Configuration.splitFullString)
                {
                    string toDisplay = target.ToString(-1, null, DisplayOption.AddSeparatorsBeforeDecimalPoint |
                        DisplayOption.AddSeparatorsAfterDecimalPoint | DisplayOption.KeepZerosAfterDecimalPoint);
                    height += Mathf.Ceil(wrapLabelStyle.CalcHeight(new GUIContent(toDisplay), EditorGUI.IndentedRect(virtualRect).width)) + spaceHeight;
                }
                else
                {
                    string[] toDisplay = target.ToString(-1, null, DisplayOption.AddSeparatorsBeforeDecimalPoint |
                        DisplayOption.AddSeparatorsAfterDecimalPoint | DisplayOption.KeepZerosAfterDecimalPoint)
                        .Split(Configuration.decimalPoints, StringSplitOptions.RemoveEmptyEntries);

                    height += Mathf.Ceil(wrapLabelStyle.CalcHeight(new GUIContent(toDisplay[0]), EditorGUI.IndentedRect(virtualRect).width)) + spaceHeight;

                    if (toDisplay.Length > 1)
                        height += Mathf.Ceil(wrapLabelStyle.CalcHeight(new GUIContent(Configuration.decimalPoints[0] + toDisplay[1]),
                            EditorGUI.IndentedRect(virtualRect).width)) + spaceHeight;
                }
            }
            else
                height += lineHeight + spaceHeight;

            // titles
            if (Configuration.drawTitles)
                height += (lineHeight + spaceHeight) * (
                    (Configuration.drawRawEdit ? 1 : 0) + // raw edit
                    (Configuration.drawMethods ? 1 : 0) + // methods
                    (Configuration.drawReadOnlyProperties ? 1 : 0)); // properties

            // raw edit
            if (Configuration.drawRawEdit) 
            {
                string toDisplay = (delayedStr != null ? delayedStr : digitsStrProp.stringValue);

                height += bigSpaceHeight // space
                        + lineHeight + spaceHeight; // exponent

                if (target.precision <= tooMuchCharacters)
                    height += lineHeight + spaceHeight // digits label
                            + Mathf.Ceil(EditorStyles.textArea.CalcHeight(new GUIContent(toDisplay), EditorGUI.IndentedRect(virtualRect).width)) + spaceHeight; // digit area
                else
                    height += (lineHeight + spaceHeight) * 2; // digits label + too much digits display
            }

            // methods
            if (Configuration.drawMethods)
                height += bigSpaceHeight // space
                        + (lineHeight + spaceHeight) * 4; // methods

            // properties
            if (Configuration.drawReadOnlyProperties)
                height += bigSpaceHeight // space
                        + (lineHeight + spaceHeight) * 7; // read only properties

            if (Configuration.drawRawEdit || Configuration.drawMethods || Configuration.drawReadOnlyProperties)
                height += bigSpaceHeight + spaceHeight; // space at the end

            --EditorGUI.indentLevel;

            // return
            return height;
        }

        // private methods
        int DigitsToDisplay(in InfVal target)
        {
            if (target.exponent >= 0)
                return target.exponent + target.precision;
            else
                return (target.precision > -target.exponent ? target.precision : 1 - target.exponent);
        }

        void DrawOneInfValProperty(ref Rect rect, GUIContent label, Func<string> getResultFunc)
        {
            string result = undefinedStr;

            try { result = getResultFunc(); }
            catch { }

            EditorGUI.LabelField(rect, label, new GUIContent(result));

            rect.y += rect.height + spaceHeight;
        }

        string DigitsDelayedArea(ref Rect rect, string text, string textAreaControlName)
        {
            // get variables
            Event evt = Event.current;
            TextEditor textState = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

            // select text on focus
            if (selectAllNextMouseUp && evt.type == EventType.MouseUp)
            {
                textState.SelectAll();
                selectAllNextMouseUp = false;
            }

            // intercept event if not a digit/minus (allow for tab to exit the field)
            if (delayedStr != null)
            {
                char chr = Event.current.character;
                if (!char.IsDigit(chr) && (chr != '-' || textState.cursorIndex != 0) && chr != '\t')
                    Event.current.character = '\0';
            }

            // draw text area
            string toDisplay = (delayedStr != null ? delayedStr : text);

            rect.height = EditorStyles.textArea.CalcHeight(new GUIContent(toDisplay), EditorGUI.IndentedRect(rect).width);

            GUI.SetNextControlName(textAreaControlName);

            EditorGUI.BeginChangeCheck();
            string newText = GUI.TextField(EditorGUI.IndentedRect(rect), toDisplay, EditorStyles.textArea);
            if (EditorGUI.EndChangeCheck())
                delayedStr = newText;

            // swap state on focus
            if (delayedStr == null && GUI.GetNameOfFocusedControl() == textAreaControlName)
            {
                delayedStr = text;
                selectAllNextMouseUp = true;
            }

            // swap state on unfocus
            if (delayedStr != null && (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter))
                GUI.FocusControl(null);

            if (delayedStr != null && GUI.GetNameOfFocusedControl() != textAreaControlName)
            {
                GUI.changed |= (delayedStr != text);
                text = delayedStr;
                delayedStr = null;
            }

            //end
            return text;
        }

        void SetLabelStyle(bool? isSelected = null)
        {
            if (cacheLabelNormalColor == null)
                cacheLabelNormalColor = EditorStyles.label.normal.textColor;

            if (!isSelected.GetValueOrDefault())
            {
                EditorStyles.label.normal.textColor = cacheLabelNormalColor.Value;
                return;
            }

            EditorStyles.label.normal.textColor = EditorStyles.label.active.textColor;
        }

        Rect DrawTitle(Rect rect, GUIContent title)
        {
            rect.height = lineHeight;
            EditorGUI.LabelField(rect, title, EditorStyles.boldLabel);

            rect.y += lineHeight + spaceHeight;
            return rect;
        }

        bool DoubleClick(Rect zoneRect)
        {
            Event evt = Event.current;

            if (evt.type == EventType.MouseDown && zoneRect.Contains(evt.mousePosition))
            {
                if (lastClickInZoneTime < 0 || EditorApplication.timeSinceStartup - lastClickInZoneTime > Configuration.doubleClickDelay)
                    lastClickInZoneTime = EditorApplication.timeSinceStartup;
                else
                {
                    lastClickInZoneTime = -1;
                    return true;
                }
            }

            return false;
        }
    }
}