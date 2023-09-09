// #undef UNITY_EDITOR

using UnityEngine;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InfiniteValue
{
    /// <summary> 
    /// <see cref="ScriptableObject"/> defining how the tool behave.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class Configuration : ScriptableObject, ISerializationCallbackReceiver
    {
        /*
            If folder path cannot be found automatically, try replacing null with the InfiniteValue folder path relative to the project location.
                for example:

            static string InfiniteValue_FolderPath = "Assets/Infinite Value";
        */
        static string InfiniteValue_FolderPath = null;

        // custom type
        enum NumberFormat
        {
            Manual,
            Culture,
        }

        // editor fields
#pragma warning disable CS0414
        [SerializeField] Texture2D icon = null;
#pragma warning restore CS0414

        [Tooltip("By default, we will never display more than this number of digits.")]
        [SerializeField] int _maxDisplayedDigits = 8;
        [Tooltip("By default, use this list of units for every 1000 values.")]
        [SerializeField] string[] _unitsList = new string[] { "k", "M", "G", "T", "P", "E", "Z", "Y" };
        [Tooltip("By default, use these options to personalize the display of the value.")]
        [SerializeField] DisplayOption _displayOptions = DisplayOption.AddSeparatorsBeforeDecimalPoint;

        [Tooltip("Should default specials number characters be defined manually or retrieved from a C# Culture.")]
        [SerializeField] NumberFormat _numberFormatType = default;
        [Tooltip("List of characters that we will use to display (or consider valid when parsing) the decimal point.")]
        [SerializeField] string[] _decimalPoints = new string[] { CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator };
        [Tooltip("List of characters that we will use to display (or consider valid when parsing) a separation.")]
        [SerializeField] string[] _separations = new string[] { CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator };
        [Tooltip("List of characters that we will use to display (or consider valid when parsing) the exponent marker.")]
        [SerializeField] string[] _exponents = cultureExponents;
        [Tooltip("Default culture we should get specials number characters from.")]
        [SerializeField] Culture _culture = new Culture(Culture.Type.InvariantCulture);

        [Tooltip("Time in seconds for two consecutives clicks to be considered a double click (you can double click on an Infinite Value label " +
                 "in the inspector to unfold it).")]
        [SerializeField] double _doubleClickDelay = 0.5f;
        [Tooltip("Split the InfVal full string representation at the decimal point.")]
        [SerializeField] bool _splitFullString = true;
        [Tooltip("Show fields to edit the digits and exponent directly in the inspector.")]
        [SerializeField] bool _drawRawEdit = true;
        [Tooltip("Add buttons allowing you to trigger methods on the Infinite Value in the inspector.")]
        [SerializeField] bool _drawMethods = true;
        [Tooltip("Show the readonly Infinite Value properties in the inspector.")]
        [SerializeField] bool _drawReadOnlyProperties = true;
        [Tooltip("Add titles on top of every section, this will increase clarity but make the inspector bigger.")]
        [SerializeField] bool _drawTitles = true;

        // private fields
        string[] usedDecimalPoints;
        string[] usedSeparations;
        string[] usedExponents;

        // public consts
        public static readonly string[] cultureExponents = new string[] { "e", "E" };

        public const char formatUnitsCharacter = '\'';
        public static readonly IReadOnlyDictionary<string, DisplayOption> formatCharsToOptionDico = new Dictionary<string, DisplayOption>
        {
            { "_/", DisplayOption.None },
            { "<", DisplayOption.AddSeparatorsBeforeDecimalPoint },
            { ">", DisplayOption.AddSeparatorsAfterDecimalPoint },
            { "eE", DisplayOption.ForceScientificNotationOrUnit },
            { "zZ", DisplayOption.KeepZerosAfterDecimalPoint },
        };

        // public static properties
        public static Configuration asset => instance;
        public static string folderPath
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                return GetFolderPath();
            }
        }

        public static int maxDisplayedDigits => instance._maxDisplayedDigits;
        public static string[] unitsList => instance._unitsList;
        public static DisplayOption displayOptions => instance._displayOptions;

        public static string[] decimalPoints => instance.usedDecimalPoints;
        public static string[] separations => instance.usedSeparations;
        public static string[] exponents => instance.usedExponents;

        internal static double doubleClickDelay => instance._doubleClickDelay;
        internal static bool splitFullString => instance._splitFullString;
        internal static bool drawRawEdit => instance._drawRawEdit;
        internal static bool drawMethods => instance._drawMethods;
        internal static bool drawReadOnlyProperties => instance._drawReadOnlyProperties;
        internal static bool drawTitles => instance._drawTitles;

        // public static methods
        public static bool AreUnitsValid(string[] unitsList)
        {
            if (unitsList == null || unitsList == Configuration.unitsList)
                return true;

            for (int i = 0; i < unitsList.Length; i++)
            {
                string unit = unitsList[i];

                if (string.IsNullOrEmpty(unit))
                {
                    Debug.LogError($"Invalid {nameof(unitsList)}, contains a null or empty unit.");
                    return false;
                }

                if (unit.Any((c) => char.IsDigit(c) || char.IsWhiteSpace(c) || c == '\''))
                {
                    Debug.LogError($"Invalid {nameof(unitsList)}, contains a unit with invalid character(s).");
                    return false;
                }

                for (int j = i + 1; j < unitsList.Length; j++)
                    if (unitsList[j] == unit)
                    {
                        Debug.LogError($"Invalid {nameof(unitsList)}, contains a duplicate.");
                        return false;
                    }
            }

            return true;
        }

        // private consts
        const string invalidConfigurationPathFormat_folderPath = "Invalid path for configuration file: \"{0}\", do not change folder hierarchy or " +
            "set InfiniteValue_FolderPath manually in Infinite Value/Core/Configuration.cs";
        const string noConfigurationFileFormat_folderPath = "No configuration file found at \"{0}\", will be created automatically";
        const string couldntLoadConfigurationStr = "Couldn't load Infinite Value configuration.";

        const string configResourceName = "Configuration";
        const string configFileName = configResourceName + ".asset";
        const string folderExtraPath = "/Runtime/Utilities/Configuration.cs";
        const string iconExtraPath = "/Icons/IV Config icon.png";

        // private statics
        static Configuration _instance = null;
        static Configuration instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<Configuration>(configResourceName);

                if (_instance == null)
                {
#if UNITY_EDITOR
                    {
                        string resourcesFolder = $"{folderPath}/Resources";

                        if (File.Exists($"{resourcesFolder}/{configFileName}"))
                            return CreateInstance(typeof(Configuration)) as Configuration;

                        if (!AssetDatabase.IsValidFolder(resourcesFolder))
                        {
                            Debug.LogError(string.Format(invalidConfigurationPathFormat_folderPath, resourcesFolder));
                            _instance = CreateInstance(typeof(Configuration)) as Configuration;
                        }
                        else
                        {
                            Debug.LogWarning(string.Format(noConfigurationFileFormat_folderPath, resourcesFolder));
                            _instance = CreateInstance(typeof(Configuration)) as Configuration;
                            AssetDatabase.CreateAsset(_instance, $"{resourcesFolder}/{configFileName}");
                        }
                    }
#else
                    {
                        Debug.LogError(couldntLoadConfigurationStr);
                        _instance = CreateInstance(typeof(Configuration)) as Configuration;
                    }
#endif
                }

                return _instance;
            }
        }

        static string GetFolderPath([CallerFilePath] string sourceFilePath = "")
        {
            if (InfiniteValue_FolderPath == null)
            {
                sourceFilePath = sourceFilePath.Replace('\\', '/');
                sourceFilePath = sourceFilePath.Remove(sourceFilePath.IndexOf(folderExtraPath));
                sourceFilePath = sourceFilePath.Substring(sourceFilePath.IndexOf("Assets"));

                InfiniteValue_FolderPath = sourceFilePath;
            }

            return InfiniteValue_FolderPath;
        }

        // interface implementation
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_numberFormatType == NumberFormat.Manual)
            {
                usedDecimalPoints = _decimalPoints;
                usedSeparations = _separations;
                usedExponents = _exponents;
            }
            else
            {
                usedDecimalPoints = new string[] { _culture.info.NumberFormat.NumberDecimalSeparator };
                usedSeparations = new string[] { _culture.info.NumberFormat.NumberGroupSeparator };
                usedExponents = cultureExponents;
            }
        }

        // custom icon
#if UNITY_EDITOR
        static string instanceGuid = null;

        [UnityEditor.Callbacks.DidReloadScripts]
        static void GizmoIconUtility() => EditorApplication.projectWindowItemOnGUI += ItemOnGUI;

        static void ItemOnGUI(string guid, Rect rect)
        {
            if (instance.icon == null)
                return;

            if (instanceGuid == null && !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instance, out instanceGuid, out long _))
                instanceGuid = null;

            if (guid != instanceGuid)
                return;

            rect.width = rect.height;
            GUI.DrawTexture(rect, instance.icon);
        }

        void Reset()
        {
            icon = (Texture2D)AssetDatabase.LoadAssetAtPath($"{folderPath}{iconExtraPath}", typeof(Texture2D));
            if (icon == null)
                Debug.LogWarning("Couldn't find Configuration icon at " + $"{folderPath}{iconExtraPath}");
        }

        static Configuration()
        {
            GizmoIconUtility();
        }
#endif
    }
}