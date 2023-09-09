using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace InfiniteValue
{
    /// Base class for all unit tests.
    abstract class AUnitTest
    {
        // consts
        static readonly IReadOnlyList<Type> possibleVarTypes = new List<Type>
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal),
        };

        // abstract
        public abstract string description { get; }

        public abstract void DrawParameters();

        public abstract TestResult Process(ref float threadProgressRatio);

        // protected fields
        protected long iterations = 10000;
        protected Type varType = typeof(double);
        protected bool avoidSpecialValues = false;

        // protected methods
        protected void D_IterationsField()
        {
            iterations = EditorGUILayout.LongField(new GUIContent("Iterations", "Number of iterations we will do for the test."), iterations);
            if (iterations < 1)
                iterations = 1;
        }

        protected void D_VarTypeField()
        {
            varType = possibleVarTypes[EditorGUILayout.Popup(
                new GUIContent("Value Type", "Primitive value type we will do the test with."),
                possibleVarTypes.ToList().IndexOf(varType),
                possibleVarTypes.Select((t) => t.Name).ToArray())];
        }

        protected void D_InfoOnFailBecauseOfDecimal(Type type = null)
        {
            type = type ?? varType;

            if (type == typeof(float) || type == typeof(double) || type == (typeof(decimal)))
                EditorGUILayout.HelpBox("When using floating point value types, some fails can occur because the InfVal will produce more precise " +
                    "results than a primitive type !", MessageType.Info);
        }

        protected void D_IgnoreSpecialValues(Type type = null)
        {
            type = type ?? varType;

            if (type == typeof(float) || type == typeof(double))
                avoidSpecialValues = EditorGUILayout.Toggle(new GUIContent("Avoid Special Values", "Should we avoid Nan, Infinity and -Infinity."),
                    avoidSpecialValues);
        }

        static protected string P_ValToString(dynamic val)
        {
            string nbStr;
            if (val is float)
                nbStr = val.ToString("G9", CultureInfo.InvariantCulture);
            else if (val is double)
                nbStr = val.ToString("G17", CultureInfo.InvariantCulture);
            else
                nbStr = val.ToString(CultureInfo.InvariantCulture);

            if (!nbStr.Contains('.'))
                return nbStr;

            int eIndex = nbStr.IndexOf('E');
            string toAddBack = null;
            if (eIndex >= 0)
            {
                toAddBack = nbStr.Substring(eIndex);
                nbStr = nbStr.Substring(0, eIndex);
            }

            int toRemove = 0;
            for (int i = nbStr.Length - 1; nbStr[i] == '0' || nbStr[i] == '.'; i--)
            {
                ++toRemove;

                if (nbStr[i] == '.')
                    break;
            }

            if (toRemove > 0)
                nbStr = nbStr.Substring(0, nbStr.Length - toRemove);
            if (toAddBack != null)
                nbStr += toAddBack;

            return nbStr;
        }

        static protected string P_InfValToSystemLikeString(InfVal iv, int digitsCount)
        {
            string infValStr = iv.ToString(digitsCount, null, DisplayOption.None, CultureInfo.InvariantCulture);

            int index = infValStr.IndexOf("e");
            if (index >= 0)
            {
                infValStr = infValStr.Replace('e', 'E');

                if (int.TryParse(infValStr.Substring(index + 1), out int i) && i < 10 && i > -10)
                    infValStr = infValStr.Insert(index + 2, "0");
            }

            return infValStr;
        }

        protected bool P_TryCastValue(dynamic val, out InfVal iv, out string valToString)
        {
            iv = new InfVal(val);
            valToString = P_ValToString(val);

            return (valToString == P_ConvertInfValToTypeToString(iv));
        }

        static protected int P_CountDigits(string nbStr)
        {
            int index = nbStr.IndexOf('E');
            return (index > 0 ? nbStr.Substring(0, index) : nbStr).Count((c) => char.IsDigit(c));
        }

        static protected int P_GetCalcExponent(string valStr, int def)
        {
            if (!TestsCommon.IsValidNumber(valStr))
                return def;

            int index = valStr.IndexOf('E');

            int ret = -(index > 0 ? valStr.Substring(0, index) : valStr).Count((c) => char.IsDigit(c));
            if (index >= 0 && int.TryParse(valStr.Substring(index + 1), out int parsed))
                ret += parsed;

            return Math.Min(def, ret);
        }

        protected dynamic P_CreateRandomValue(Type type = null)
        {
            type = type ?? varType;

            if (type == typeof(decimal))
            {
                byte scale = (byte)rand.Next(29);
                bool sign = rand.Next(2) == 1;
                return new decimal(RandomInt(), RandomInt(), RandomInt(), sign, scale);

                int RandomInt()
                {
                    int firstBits = rand.Next(0, 1 << 4) << 28;
                    int lastBits = rand.Next(0, 1 << 28);
                    return firstBits | lastBits;
                }
            }

            byte[] buf = new byte[Marshal.SizeOf(type)];
            rand.NextBytes(buf);

            switch (type.Name)
            {
                case "Byte": return (byte)buf[0];
                case "SByte": return (sbyte)buf[0];
                case "Int16": return BitConverter.ToInt16(buf, 0);
                case "UInt16": return BitConverter.ToUInt16(buf, 0);
                case "Int32": return BitConverter.ToInt32(buf, 0);
                case "UInt32": return BitConverter.ToUInt32(buf, 0);
                case "Int64": return BitConverter.ToInt64(buf, 0);
                case "UInt64": return BitConverter.ToUInt64(buf, 0);
                case "Single":
                    float f = BitConverter.ToSingle(buf, 0);
                    return (avoidSpecialValues && TestsCommon.specialNumberStr.Contains(f.ToString(CultureInfo.InvariantCulture)) ? P_CreateRandomValue(type) : f);
                case "Double":
                    double d = BitConverter.ToDouble(buf, 0);
                    return (avoidSpecialValues && TestsCommon.specialNumberStr.Contains(d.ToString(CultureInfo.InvariantCulture)) ? P_CreateRandomValue(type) : d);
            }

            throw new Exception("Invalid Type");
        }

        protected string P_ConvertInfValToTypeToString(InfVal iv, Type type = null)
        {
            type = type ?? varType;

            switch (type.Name)
            {
                case "Byte": return P_ValToString((byte)iv);
                case "SByte": return P_ValToString((sbyte)iv);
                case "Int16": return P_ValToString((short)iv);
                case "UInt16": return P_ValToString((ushort)iv);
                case "Int32": return P_ValToString((int)iv);
                case "UInt32": return P_ValToString((uint)iv);
                case "Int64": return P_ValToString((long)iv);
                case "UInt64": return P_ValToString((ulong)iv);
                case "Single": return P_ValToString((float)iv);
                case "Double": return P_ValToString((double)iv);
                case "Decimal": return P_ValToString((decimal)iv);
            }

            throw new Exception("Invalid Type");
        }

        // static fields
        protected static System.Random rand = new System.Random();
    }
}