using System.Linq;
using System.Numerics;
using UnityEngine;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace InfiniteValue
{
    /// <summary> 
    /// Define how to draw an <see cref="InfVal"/>.
    /// </summary>
    [Flags] public enum DisplayOption
    {
        None = 0,
        AddSeparatorsBeforeDecimalPoint = (1 << 0),
        AddSeparatorsAfterDecimalPoint = (1 << 1),
        ForceScientificNotationOrUnit = (1 << 2),
        KeepZerosAfterDecimalPoint = (1 << 3),
        Everything = AddSeparatorsBeforeDecimalPoint | AddSeparatorsAfterDecimalPoint | ForceScientificNotationOrUnit | KeepZerosAfterDecimalPoint,
    }

    public partial struct InfVal : IFormattable
    {
        // public ToString methods

        /// <summary> Converts the numeric value of this instance to its equivalent <see langword="string"/> representation. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(int maxDisplayedDigits, string[] unitsList, DisplayOption displayOptions, IFormatProvider provider = null) =>
            InternalToString((maxDisplayedDigits, unitsList, displayOptions), ProviderToFormatParams(provider));

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(int maxDisplayedDigits, IFormatProvider provider = null) =>
            ToString(maxDisplayedDigits, Configuration.unitsList, Configuration.displayOptions, provider);

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(int maxDisplayedDigits, string[] unitsList, IFormatProvider provider = null) =>
            ToString(maxDisplayedDigits, unitsList, Configuration.displayOptions, provider);

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(int maxDisplayedDigits, DisplayOption displayOptions, IFormatProvider provider = null) =>
            ToString(maxDisplayedDigits, Configuration.unitsList, displayOptions, provider);

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(string[] unitsList, IFormatProvider provider = null) =>
            ToString(Configuration.maxDisplayedDigits, unitsList, Configuration.displayOptions, provider);

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(string[] unitsList, DisplayOption displayOptions, IFormatProvider provider = null) =>
            ToString(Configuration.maxDisplayedDigits, unitsList, displayOptions, provider);

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(DisplayOption displayOptions, IFormatProvider provider = null) =>
            ToString(Configuration.maxDisplayedDigits, Configuration.unitsList, displayOptions, provider);

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(string format, IFormatProvider provider = null) =>
            InternalToString(StrToDisplayParams(format), ProviderToFormatParams(provider));

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToString(IFormatProvider provider) =>
            ToString(Configuration.maxDisplayedDigits, Configuration.unitsList, Configuration.displayOptions, provider);

        /// <summary> <inheritdoc cref="ToString(int, string[], DisplayOption, IFormatProvider)"/> </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public override string ToString() =>
            ToString(Configuration.maxDisplayedDigits, Configuration.unitsList, Configuration.displayOptions);

        /// <summary> Returns a string detailing the digits and exponent of this <see cref="InfVal"/>, useful for debugging purposes. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public string ToDebugString() => string.Format(debugStringFormat, digits, exponent);

        // public static parse methods

        /// <summary> Converts the <see langword="string"/> representation of a number to its <see cref="InfVal"/> equivalent. 
        /// This will throw an exception if the string <paramref name="str"/> isn't a valid number. </summary> 
        public static InfVal Parse(string str, string[] unitsList, IFormatProvider provider)
        {
            InfVal ret = new InfVal();
            ret.SetValueFromString(str, unitsList, ProviderToFormatParams(provider), true);
            return ret;
        }
        /// <summary> <inheritdoc cref="ParseOrDefault(string, string[], IFormatProvider)"/> </summary> 
        public static InfVal Parse(string str, string[] unitsList) => Parse(str, unitsList, null);
        /// <summary> <inheritdoc cref="ParseOrDefault(string, string[], IFormatProvider)"/> </summary>
        public static InfVal Parse(string str, IFormatProvider provider) => Parse(str, Configuration.unitsList, provider);
        /// <summary> <inheritdoc cref="ParseOrDefault(string, string[], IFormatProvider)"/> </summary>
        public static InfVal Parse(string str) => Parse(str, Configuration.unitsList, null);

        /// <summary> Converts the <see langword="string"/> representation of a number to its <see cref="InfVal"/> equivalent. 
        /// This won't throw an exception but the returned <see langword="bool"/> indicates whether the operation succeeded. </summary> 
        public static bool TryParse(string str, string[] unitsList, IFormatProvider provider, out InfVal result)
        {
            result = new InfVal();
            try { result.SetValueFromString(str, unitsList, ProviderToFormatParams(provider), true); }
            catch { return false; }
            return true;
        }
        /// <summary> <inheritdoc cref="TryParse(string, string[], IFormatProvider, out InfVal)"/> </summary> 
        public static bool TryParse(string str, string[] unitsList, out InfVal result) => TryParse(str, unitsList, null, out result);
        /// <summary> <inheritdoc cref="TryParse(string, string[], IFormatProvider, out InfVal)"/> </summary> 
        public static bool TryParse(string str, IFormatProvider provider, out InfVal result) => TryParse(str, Configuration.unitsList, provider, out result);
        /// <summary> <inheritdoc cref="TryParse(string, string[], IFormatProvider, out InfVal)"/> </summary> 
        public static bool TryParse(string str, out InfVal result) => TryParse(str, Configuration.unitsList, null, out result);

        /// <summary> Converts the <see langword="string"/> representation of a number to its <see cref="InfVal"/> equivalent. 
        /// This won't throw an exception, if the string <paramref name="str"/> isn't a valid number the method will do it's best to return a valid <see cref="InfVal"/>. </summary> 
        public static InfVal ParseOrDefault(string str, string[] unitsList, IFormatProvider provider)
        {
            InfVal ret = new InfVal();
            ret.SetValueFromString(str, unitsList, ProviderToFormatParams(provider), false);
            return ret;
        }
        /// <summary> <inheritdoc cref="ParseOrDefault(string, string[], IFormatProvider)"/> </summary> 
        public static InfVal ParseOrDefault(string str, string[] unitsList) => ParseOrDefault(str, unitsList, null);
        /// <summary> <inheritdoc cref="ParseOrDefault(string, string[], IFormatProvider)"/> </summary> 
        public static InfVal ParseOrDefault(string str, IFormatProvider provider) => ParseOrDefault(str, Configuration.unitsList, provider);
        /// <summary> <inheritdoc cref="ParseOrDefault(string, string[], IFormatProvider)"/> </summary> 
        public static InfVal ParseOrDefault(string str) => ParseOrDefault(str, Configuration.unitsList, null);

        // private methods
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        string InternalToString((int maxDigits, string[] unitsList, DisplayOption options) display,
            (string[] decimalPoints, string[] separations, string[] exponents) format)
        {
            // if is zero, return easily
            if (isZero)
            {
                if (!display.options.HasFlag(DisplayOption.KeepZerosAfterDecimalPoint) || display.maxDigits == 1 || exponent >= 0)
                    return "0";

                return "0" + format.decimalPoints[0] + new string('0', display.maxDigits > 0 ? Math.Min(-exponent, display.maxDigits - 1) : -exponent);
            }

            // check for units
            if (!Configuration.AreUnitsValid(display.unitsList))
                display.unitsList = Configuration.unitsList;

            // initialize a string builder with the digits (without the -) and an exponent
            StringBuilder ret = new StringBuilder(cacheDigitsToString.StartsWith("-") ? cacheDigitsToString.Substring(1) : cacheDigitsToString);
            int drawExp = exponent;

            // remove extra digits
            if (display.maxDigits > 0 && ret.Length > display.maxDigits)
            {
                drawExp += (ret.Length - display.maxDigits);
                ret.Length = display.maxDigits;
            }

            // trim ending zeros if needed
            if (!display.options.HasFlag(DisplayOption.KeepZerosAfterDecimalPoint))
                while (ret.Length > 1 && ret[ret.Length - 1] == '0')
                {
                    ++drawExp;
                    --ret.Length;
                }

            // scientific notation or units
            if (display.options.HasFlag(DisplayOption.ForceScientificNotationOrUnit) || (display.maxDigits > 0 &&
               ((drawExp > 0 && ret.Length + drawExp > display.maxDigits) ||
                (drawExp <= -ret.Length && -drawExp + 1 > display.maxDigits))))
            {
                drawExp += ret.Length - 1;

                // using units
                int unitIndex = (drawExp / 3) - 1;
                int pointPos = drawExp % 3 + 1;

                if (display.unitsList != null && unitIndex >= 0 && unitIndex < display.unitsList.Length &&
                    (pointPos <= ret.Length || pointPos <= display.maxDigits))
                {
                    // add back some zeros if needed
                    if (pointPos > ret.Length)
                        ret.Append(new string('0', pointPos - ret.Length));

                    // add point and separators
                    AddDecimalPointAndSeparators(pointPos);

                    // add unit
                    ret.Append(display.unitsList[unitIndex]);
                }
                // using exponent
                else
                {
                    // add point and separators
                    AddDecimalPointAndSeparators(1);

                    // add exponent
                    if (drawExp != 0)
                    {
                        ret.Append(format.exponents[0]);
                        ret.Append(drawExp > 0 ? "+" : "");
                        ret.Append(drawExp);
                    }
                }
            }
            // no exponent or unit
            else
            {
                // add 0s if exp is positive
                if (drawExp > 0)
                {
                    ret.Append(new string('0', drawExp));
                    drawExp = 0;
                }
                // add 0 at the beggining if needed
                else if (drawExp <= -ret.Length)
                    ret.Insert(0, new string('0', -drawExp - ret.Length + 1));

                // add point and separators
                AddDecimalPointAndSeparators(ret.Length + drawExp);
            }

            // add - if the value is negative
            if (sign < 0)
                ret.Insert(0, '-');

            return ret.ToString();

            // separate every 3 characters local function
            void AddDecimalPointAndSeparators(int decimalPointPos)
            {
                decimalPointPos = Mathf.Clamp(decimalPointPos, 0, ret.Length);

                if (decimalPointPos < ret.Length)
                    ret.Insert(decimalPointPos, format.decimalPoints[0]);

                if (!display.options.HasFlag(DisplayOption.AddSeparatorsBeforeDecimalPoint) && !display.options.HasFlag(DisplayOption.AddSeparatorsAfterDecimalPoint))
                    return;

                // add separators after the decimal point
                if (display.options.HasFlag(DisplayOption.AddSeparatorsAfterDecimalPoint))
                    for (int i = decimalPointPos + 3 + format.separations[0].Length; i < ret.Length; i += 4)
                        ret.Insert(i, format.separations[0]);

                // add separators before the decimal point
                if (display.options.HasFlag(DisplayOption.AddSeparatorsBeforeDecimalPoint))
                    for (int i = decimalPointPos - 3; i > 0; i -= 3)
                        ret.Insert(i, format.separations[0]);
            }
        }

        void SetValueFromString(string valStr, string[] unitsList, (string[] decimalPoints, string[] separations,
            string[] exponents) format, bool throwExceptions)
        {
            // check for units
            if (!Configuration.AreUnitsValid(unitsList))
                unitsList = Configuration.unitsList;

            // if string is empty, default to zero or throw exception
            if (string.IsNullOrWhiteSpace(valStr))
            {
                if (throwExceptions)
                    throw new FormatException();

                exponent = 0;
                digits = 0;
            }
            // gotta parse the string
            else
            {
                // filter out white space characters
                valStr = new string(valStr.Where((c) => !char.IsWhiteSpace(c)).ToArray());

                // detect multiples separation in a row
                if (throwExceptions)
                    foreach (string sep in format.separations)
                        if (!string.IsNullOrWhiteSpace(sep) && valStr.Contains(sep + sep))
                            throw new FormatException();

                // filter out separation strs
                foreach (string sep in format.separations)
                    if (!string.IsNullOrWhiteSpace(sep))
                        valStr = valStr.Replace(sep, string.Empty);

                int i = 0;
                int calculatedExponent = 0;

                // skip over initial + or -
                if (valStr[i] == '+' || valStr[i] == '-')
                    ++i;

                // count numbers before decimal point
                while (i < valStr.Length && char.IsDigit(valStr[i]))
                    ++i;

                // if we found a decimal point
                if (i < valStr.Length && IsFollowedBy(valStr, i, format.decimalPoints, out int pointSize))
                {
                    // remove decimal point
                    valStr = valStr.Remove(i, pointSize);

                    // throw an exception if the character after the point isn't a digit
                    if (throwExceptions && !char.IsDigit(valStr[i]))
                        throw new FormatException();

                    // count numbers after decimal point and reduce exponent
                    while (i < valStr.Length && char.IsDigit(valStr[i]))
                    {
                        --calculatedExponent;
                        ++i;
                    }
                }

                // parse the string before the unit or exponent
                if (BigInteger.TryParse(valStr.Substring(0, i), out BigInteger res))
                {
                    // set digits
                    digits = res;

                    // remove numbers before the unit or exponent 
                    valStr = valStr.Substring(i);

                    // if there is something after the digits
                    if (valStr.Length != 0)
                    {
                        int unitIndex = (unitsList != null ? Array.IndexOf(unitsList, valStr) : -1);
                        // add to exponent based on unit if we found one
                        if (unitIndex >= 0)
                            calculatedExponent += (unitIndex + 1) * 3;
                        // check if there is an exponent character
                        else if (IsFollowedBy(valStr, 0, format.exponents, out int expSize))
                        {
                            // parse exponent digits
                            if (int.TryParse(valStr.Substring(expSize), out int exp))
                                calculatedExponent += exp;
                            // if it failed and we throw exceptions, do it
                            else if (throwExceptions)
                                throw new FormatException();
                        }
                        // if chars after digits are neither a unit or exponent
                        else if (throwExceptions)
                            throw new FormatException();
                    }

                    // set exponent
                    s_exponent = calculatedExponent;
                }
                // parsing failed, default to zero or throw an exception
                else
                {
                    if (throwExceptions)
                        throw new FormatException();

                    exponent = 0;
                    digits = 0;
                }
            }
        }

        static bool IsFollowedBy(string str, int index, string[] follow, out int followSize)
        {
            foreach (string f in follow)
            {
                if (str.Length >= index + f.Length && str.Substring(index, f.Length) == f)
                {
                    followSize = f.Length;
                    return true;
                }
            }

            followSize = default;
            return false;
        }

        static (string[], string[], string[]) ProviderToFormatParams(IFormatProvider provider)
        {
            if (provider == null)
                return (Configuration.decimalPoints, Configuration.separations, Configuration.exponents);

            NumberFormatInfo info = (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo));
            if (info == null)
                return (Configuration.decimalPoints, Configuration.separations, Configuration.exponents);

            return (new string[] { info.NumberDecimalSeparator }, new string[] { info.NumberGroupSeparator }, Configuration.cultureExponents);
        }

        static (int, string[], DisplayOption) StrToDisplayParams(string format)
        {
            if (string.IsNullOrWhiteSpace(format) || format == "G" || format == "g")
                return (Configuration.maxDisplayedDigits, Configuration.unitsList, Configuration.displayOptions);

            int? maxDisplayedDigits = null;
            List<string> units = (format.Contains(Configuration.formatUnitsCharacter) ? new List<string>() : null);
            DisplayOption? displayOptions = null;

            // filter out white space character
            format = new string(format.Where((c) => !char.IsWhiteSpace(c)).ToArray());

            for (int i = 0; i < format.Length; i++)
            {
                // get max displayed digits
                if (format[i] == '-' || format[i] == '+' || char.IsDigit(format[i]))
                {
                    int end = i + 1;
                    for (; end < format.Length && char.IsDigit(format[end]); end++);

                    if (maxDisplayedDigits == null && int.TryParse(format.Substring(i, end - i), out int res))
                        maxDisplayedDigits = res;

                    i = end - 1;
                }
                // get units
                else if (format[i] == Configuration.formatUnitsCharacter)
                {
                    int end = i + 1;
                    for (; end < format.Length && format[end] != Configuration.formatUnitsCharacter; end++);

                    if (end < format.Length)
                    {
                        string unit = format.Substring(i + 1, end - (i + 1));

                        // check if unit is valid
                        if (!string.IsNullOrEmpty(unit) && !units.Contains(unit) && !unit.Any(char.IsDigit) && !unit.Any(char.IsWhiteSpace))
                            units.Add(unit);

                        i = end;
                    }
                }
                // get options
                else
                {
                    foreach (KeyValuePair<string, DisplayOption> kvPair in Configuration.formatCharsToOptionDico)
                        if (kvPair.Key.Contains(format[i]))
                        {
                            displayOptions = displayOptions.GetValueOrDefault() | kvPair.Value;
                            break;
                        }
                }
            }

            // return, default to Configuration if unspecified
            return (maxDisplayedDigits ?? Configuration.maxDisplayedDigits,
                    units != null ? units.ToArray() : Configuration.unitsList,
                    displayOptions ?? Configuration.displayOptions);
        }
    }
}
