using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

namespace InfiniteValue
{
    /// <summary> 
    /// This <see cref="MonoBehaviour"/> is meant to be used on the same GameObject as an <see cref="InputField"/> or a <see cref="TMP_InputField"/>.
    /// It uses a generic input field to create an <see cref="InfVal"/> one.
    /// </summary>
    [DefaultExecutionOrder(-80), AddComponentMenu("UI/InfVal Input Field Converter"), DisallowMultipleComponent]
    public class InfValInputField : MonoBehaviour
    {
        // per Type configuration, extend this to add more handled Component Type.
        Dictionary<Type, Action> _targetComponentTypesToInitializeMethods = null;
        IReadOnlyDictionary<Type, Action> targetComponentTypesToInitializeMethods
        {
            get
            {
                if (_targetComponentTypesToInitializeMethods == null)
                {
                    _targetComponentTypesToInitializeMethods = new Dictionary<Type, Action>
                    {
                        { typeof(InputField), Initialize_InputField },
                        { typeof(TMP_InputField), Initialize_TMP_InputField },
                    };
                }

                return _targetComponentTypesToInitializeMethods;
            }
        }

        void Initialize_InputField()
        {
            InputField inputField = target as InputField;

            inputField.onValueChanged.AddListener(OnFieldValueChanged);
            inputField.onValidateInput += ValidateCharacter;
            inputField.onEndEdit.AddListener(OnEndEdit);

            changeTextColorAction = (c) => inputField.textComponent.color = c;
            setTextAction = (s) => inputField.text = s;
            getTextFunc = () => inputField.text;
        }

        void Initialize_TMP_InputField()
        {
            TMP_InputField inputField = target as TMP_InputField;

            inputField.onValueChanged.AddListener(OnFieldValueChanged);
            inputField.onValidateInput += ValidateCharacter;
            inputField.onEndEdit.AddListener(OnEndEdit);

            changeTextColorAction = (c) => inputField.textComponent.color = c;
            setTextAction = (s) => inputField.text = s;
            getTextFunc = () => inputField.text;
        }

        public InputField inputField => GetTarget<InputField>();
        public TMP_InputField TMP_inputField => GetTarget<TMP_InputField>();

        // custom types
        public enum ValidateType
        {
            None,
            ChangeColor,
            DisallowInvalid,
        }

        public enum ForceSign
        {
            None,
            Positive,
            Negative,
        }

        // public fields

        /// <summary> Should we override the Configuration units when parsing and displaying the value. </summary> 
        [Tooltip("Should we override the Configuration units when parsing and displaying the value.")]
        public bool useCustomUnits;
        /// <summary> List of units for every 1000 values. </summary> 
        [Tooltip("List of units for every 1000 values.")]
        public string[] unitsList;
        /// <summary> Should we override the Configuration culture when parsing and displaying the value. </summary> 
        [Tooltip("Should we override the Configuration culture when parsing and displaying the value.")]
        public bool useCustomCulture;
        /// <summary> Culture we will use. </summary> 
        [Tooltip("Culture we will use.")]
        public Culture culture = new Culture(Culture.Type.CurrentCulture);

        /// <summary> Should we override the Configuration number of displayed digits when displaying the value. </summary> 
        [Tooltip("Should we override the Configuration number of displayed digits when displaying the value.")]
        public bool useCustomMaxDisplayedDigits;
        /// <summary> We will never display more than this number of digits. </summary> 
        [Tooltip("We will never display more than this number of digits.")]
        public int maxDisplayedDigits;
        /// <summary> Should we override the Configuration options when displaying the value. </summary> 
        [Tooltip("Should we override the Configuration options when displaying the value.")]
        public bool useCustomDisplayOptions;
        /// <summary> Different options to personalize the display of the value. </summary> 
        [Tooltip("Different options to personalize the display of the value.")]
        public DisplayOption displayOptions;

        /// <summary> How should we handle invalid strings. </summary> 
        [Tooltip("How should we handle invalid strings.")]
        public ValidateType validateType = default;
        /// <summary> Text Color to use when the string is a valid InfVal representation. </summary> 
        [Tooltip("Text Color to use when the string is a valid InfVal representation.")]
        public Color validColor = Color.black;
        /// <summary> Text Color to use when the string is an invalid InfVal representation. </summary> 
        [Tooltip("Text Color to use when the string is an invalid InfVal representation.")]
        public Color invalidColor = Color.red;
        /// <summary> Should we consider valid only integer values. </summary> 
        [Tooltip("Should we consider valid only integer values.")]
        public bool integerOnly = false;
        /// <summary> Should we consider valid only values positive or negatives. </summary> 
        [Tooltip("Should we consider valid only values positive or negatives.")]
        public ForceSign forceSign = default;
        /// <summary> Should we consider valid only non zero values. </summary> 
        [Tooltip("Should we consider valid only non zero values.")]
        public bool nonZero = false;
        /// <summary> UI.Selectable in this array will be interactable only if the string is valid. </summary> 
        [Tooltip("UI.Selectable in this array will be interactable only if the string is valid.")]
        public Selectable[] interactablesIfValid = new Selectable[0];

        /// <summary> Should we replace the inputted text to a clean InfVal representation once the user is done typing. </summary> 
        [Tooltip("Should we replace the inputted text to a clean InfVal representation once the user is done typing.")]
        public bool redrawOnEndEdit = true;

        // public properties

        /// <summary> The current InfVal value of this input field. </summary> 
        public InfVal value
        {
            get => _value;
            set
            {
                if (target != null)
                {
                    ignoreCharValidation = true;
                    setTextAction(FormatInfValToString(value));
                    ignoreCharValidation = false;
                }

                _value = value;

                isValid = IsValueValid(value);
            }
        }

        /// <summary> Whether the value is valid or not. </summary> 
        public bool isValid 
        {
            get => _isValid; 
            private set
            {
                if (value == _isValid)
                    return;

                foreach (Selectable s in interactablesIfValid)
                    if (s != null)
                        s.interactable = value;

                _isValid = value;

                if (_target != null && validateType == ValidateType.ChangeColor)
                    changeTextColorAction(isValid ? validColor : invalidColor);
            }
        }

        /// <summary> The target input field Component that we will turn into an InfVal input field. </summary> 
        public Component target => _target;

        // public methods

        /// <summary> Get the target input field Component casted to the given type. </summary> 
        public T GetTarget<T>() where T : Component => target as T;

        /// <summary> Automatically find the target component (using GetComponent). </summary> 
        public void FindTargetComponent()
        {
            if (_target == null)
            {
                Type[] types = targetComponentTypesToInitializeMethods.Keys.ToArray();
                for (int i = 0; i < types.Length && target == null; i++)
                    if (TryGetComponent(types[i], out _target))
                        break;
            }

            if (_target != null)
            {
                targetComponentTypesToInitializeMethods[target.GetType()]();

                value = InfVal.ParseOrDefault(getTextFunc(),
                            (useCustomUnits ? unitsList : Configuration.unitsList),
                            (useCustomCulture ? culture.info : null));
            }
        }

        /// <summary> Format an InfVal to a string using this component display parameters. </summary> 
        public string FormatInfValToString(in InfVal value) =>
            value.ToString((useCustomMaxDisplayedDigits ? maxDisplayedDigits : Configuration.maxDisplayedDigits),
                           (useCustomUnits ? unitsList : Configuration.unitsList),
                           (useCustomDisplayOptions ? displayOptions : Configuration.displayOptions),
                           (useCustomCulture ? culture.info : null));

        // private fields
        [SerializeField] Component _target;

        Action<Color> changeTextColorAction;
        Action<string> setTextAction;
        Func<string> getTextFunc;

        InfVal _value;
        bool _isValid;

        bool ignoreCharValidation = false;

        // unity
        void Reset()
        {
            FindTargetComponent();

            unitsList = Configuration.unitsList;
            maxDisplayedDigits = Configuration.maxDisplayedDigits;
            displayOptions = Configuration.displayOptions;
        }

        void Awake()
        {
            FindTargetComponent();
        }

        // private methods
        void OnEndEdit(string text)
        {
            if (redrawOnEndEdit)
                value = _value;
        }
        
        void OnFieldValueChanged(string newVal)
        {
            isValid = IsTextValid(newVal, out _value);
        }

        char ValidateCharacter(string text, int charIndex, char addedChar)
        {
            if (ignoreCharValidation || validateType != ValidateType.DisallowInvalid)
                return addedChar;

            // allow white spaces
            if (char.IsWhiteSpace(addedChar))
                return addedChar;

            // generate the text if we were to add the char
            string textWithAdd = text.Insert(charIndex, addedChar.ToString());

            // get all special strings
            string[] units = useCustomUnits ? unitsList : Configuration.unitsList;
            units = Configuration.AreUnitsValid(units) ? units : Configuration.unitsList;
            string[] decimalPoints = useCustomCulture ? new string[] { culture.info.NumberFormat.NumberDecimalSeparator } : Configuration.decimalPoints;
            string[] separations = useCustomCulture ? new string[] { culture.info.NumberFormat.NumberGroupSeparator } : Configuration.separations;
            string[] exponents = useCustomCulture ? Configuration.cultureExponents : Configuration.exponents;

            // get unique special strings indexes (start end end)
            (int start, int end)? unitBounds = null;
            (int start, int end)? decPointBounds = null;
            (int start, int end)? expBounds = null;

            for (int i = 0; i < text.Length; i++)
                if (!char.IsWhiteSpace(text[i]) && !IsDigit(text[i]))
                {
                    string special = GetSpecial(i, text, out (int start, int end) bounds);

                    int indexOfUnit = Array.IndexOf(units, special);
                    int indexOfDecPoint = Array.IndexOf(decimalPoints, special);
                    int indexOfExp = Array.IndexOf(exponents, special);

                    if (indexOfUnit >= 0)
                        unitBounds = bounds;
                    else if (indexOfDecPoint >= 0)
                        decPointBounds = bounds;
                    else if (indexOfExp >= 0)
                        expBounds = bounds;
                    else
                        continue;

                    i = bounds.end;
                }

            // allow 0 even if nonZero is set to true if we can write a floating point number
            if (addedChar == '0' && !integerOnly)
                return addedChar;

            // check whether we are at the beggining of the text
            bool isAtTheBeggining = false;
            {
                int i = charIndex - 1;
                while (i >= 0 && !IsDigit(text[i]))
                    --i;

                if (i < 0)
                    isAtTheBeggining = true;
            }

            // disallow digits if we are not writing a negative and have forceSign set to Negative
            if (char.IsDigit(addedChar) && forceSign == ForceSign.Negative)
            {
                bool isThereMinus = false;
                int i = (expBounds != null ? expBounds.Value.start : text.Length) - 1;
                while (i >= 0)
                {
                    if (text[i] == '-')
                        isThereMinus = true;

                    --i;
                }

                if (!isThereMinus)
                    return '\0';
            }

            // allow + or - at the beggining or after the exponent marker
            if (addedChar == '-' || addedChar == '+')
            {
                if (isAtTheBeggining)
                {
                    if ((forceSign == ForceSign.Negative && addedChar == '+') ||
                        (forceSign == ForceSign.Positive && addedChar == '-'))
                        return '\0';

                    return addedChar;
                }

                if (expBounds != null && charIndex >= expBounds.Value.end)
                {
                    int i = charIndex - 1;
                    while (i > expBounds.Value.end && !IsDigit(text[i]))
                        --i;

                    if (i == expBounds.Value.end)
                        return addedChar;
                }

                return '\0';
            }

            // merge all possibles special strings
            List<string> specialStr = new List<string>();
            if (text.Any((c) => char.IsDigit(c)))
            {
                if ((decPointBounds == null || charIndex > decPointBounds.Value.end) &&
                    (unitBounds == null || Include(charIndex, unitBounds.Value)) &&
                    (expBounds == null || Include(charIndex, expBounds.Value)))
                    specialStr.AddRange(units);

                if (!integerOnly && decPointBounds == null &&
                    (unitBounds == null || charIndex < unitBounds.Value.start || Include(charIndex, unitBounds.Value)) &&
                    (expBounds == null || charIndex < expBounds.Value.start || Include(charIndex, expBounds.Value)))
                    specialStr.AddRange(decimalPoints);

                specialStr.AddRange(separations);

                if ((decPointBounds == null || charIndex > decPointBounds.Value.end) &&
                    (unitBounds == null || Include(charIndex, unitBounds.Value)) &&
                    (expBounds == null || Include(charIndex, expBounds.Value)))
                    specialStr.AddRange(exponents);
            }

            // allow cases when we are in the middle of writing a special string
            if (specialStr.Count > 0 && !IsDigit(addedChar))
            {
                string currentSpecial = GetSpecial(charIndex, textWithAdd, out (int start, int end) bounds);

                foreach (string s in specialStr)
                    if (s.Contains(currentSpecial))
                        return addedChar;
            }

            // return whether adding the char will make the string fail validation
            return (IsTextValid(textWithAdd, out InfVal ignored) ? addedChar : '\0');

            // local functions
            bool IsDigit(char c) => (char.IsDigit(c) || c == '-' || c == '+');

            string GetSpecial(int index, string txt, out (int start, int end) bounds)
            {
                bounds.start = index;
                while (bounds.start >= 0 && !IsDigit(txt[bounds.start]))
                    --bounds.start;
                ++bounds.start;

                bounds.end = index;
                while (bounds.end < txt.Length && !IsDigit(txt[bounds.end]))
                    ++bounds.end;
                --bounds.end;

                return new string(txt.Substring(bounds.start, bounds.end + 1 - bounds.start).Where((c) => !char.IsWhiteSpace(c)).ToArray());
            }

            bool Include(int index, (int start, int end) bounds) => (index >= bounds.start && index <= bounds.end + 1); 
        }

        bool IsValueValid(in InfVal value)
        {
            bool goodSign = (forceSign == ForceSign.None ||
                            (forceSign == ForceSign.Positive && value >= 0) ||
                            (forceSign == ForceSign.Negative && value <= 0));

            return (goodSign && (!integerOnly || value.isInteger) && (!nonZero || !value.isZero));
        }

        bool IsTextValid(string text, out InfVal result)
        {
            bool successParsing = InfVal.TryParse(text, (useCustomUnits ? unitsList : Configuration.unitsList), 
                (useCustomCulture ? culture.info : null), out InfVal res);

            if (!successParsing)
                result = InfVal.ParseOrDefault(text, (useCustomUnits ? unitsList : Configuration.unitsList), (useCustomCulture ? culture.info : null));
            else
                result = res;

            return successParsing && IsValueValid(result);
        }
    }
}