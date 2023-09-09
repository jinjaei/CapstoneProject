using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;

namespace InfiniteValue
{
    class FunctionsTest : AUnitTest
    {
        // consts
        static readonly IReadOnlyList<Function> functionsList = new List<Function>
            {
                { new Function("Abs", typeof(float), (d) => Mathf.Abs(d), (iv) => MathInfVal.Abs(iv), "Mathf.Abs({0})", "MathInfVal.Abs({0})") },
                { new Function("Truncate", typeof(double), (d) => Math.Truncate(d), (iv) => MathInfVal.Truncate(iv), "Math.Truncate({0})", "MathInfVal.Truncate({0})") },
                { new Function("Decimal Part", typeof(double), (d) => d - Math.Truncate(d), (iv) => MathInfVal.DecimalPart(iv), "{0} - Math.Truncate({0})", "MathInfVal.DecimalPart({0})") },

                { new Function("ClosestPowerOfTwo", typeof(int), (d) => Mathf.ClosestPowerOfTwo(d), (iv) => MathInfVal.ClosestPowerOfTwo(iv), "Mathf.ClosestPowerOfTwo({0})", "MathInfVal.ClosestPowerOfTwo({0})") },
                { new Function("NextPowerOfTwo", typeof(int), (d) => Mathf.NextPowerOfTwo(d), (iv) => MathInfVal.NextPowerOfTwo(iv), "Mathf.NextPowerOfTwo({0})", "MathInfVal.NextPowerOfTwo({0})") },

                { new Function("Floor", typeof(float), (d) => Mathf.Floor(d), (iv) => MathInfVal.Floor(iv), "Mathf.Floor({0})", "MathInfVal.Floor({0})") },
                { new Function("Ceil", typeof(float), (d) => Mathf.Ceil(d), (iv) => MathInfVal.Ceil(iv), "Mathf.Ceil({0})", "MathInfVal.Ceil({0})") },
                { new Function("Round", typeof(float), (d) => Mathf.Round(d), (iv) => MathInfVal.Round(iv), "Mathf.Round({0})", "MathInfVal.Round({0})") },

                { new Function("Repeat", typeof(float), (d, d2) => Mathf.Repeat(d, d2), (iv, iv2) => MathInfVal.Repeat(iv, iv2), "Mathf.Repeat({0}, {1})", "MathInfVal.Repeat({0}, {1})") },
                { new Function("PingPong", typeof(float), (d, d2) => Mathf.PingPong(d, d2), (iv, iv2) => MathInfVal.PingPong(iv, iv2), "Mathf.PingPong({0}, {1})", "MathInfVal.PingPong({0}, {1})") },
                { new Function("DeltaAngle", typeof(float), (d, d2) => Mathf.DeltaAngle(d, d2), (iv, iv2) => MathInfVal.DeltaAngle(iv, iv2), "Mathf.DeltaAngle({0}, {1})", "MathInfVal.DeltaAngle({0}, {1})") },

                { new Function("Log", typeof(double), (d) => Math.Log(d), (iv) => MathInfVal.Log(iv), "Math.Log({0})", "MathInfVal.Log({0})") },
                { new Function("LogN", typeof(double), (d, n) => Math.Log(d, n), (iv, n) => MathInfVal.Log(iv, (double)n), "Math.Log({0}, {1})", "MathInfVal.Log({0}, {1})") },
                { new Function("Log10", typeof(double), (d) => Math.Log10(d), (iv) => MathInfVal.Log10(iv), "Math.Log10({0})", "MathInfVal.Log10({0})") },

                { new Function("Pow", typeof(double), (d, n) => Math.Pow(d, n), (iv, n) => MathInfVal.Pow(iv, (int)n), "Math.Pow({0}, {1})", "MathInfVal.Pow({0}, {1})") },
                { new Function("Sqrt", typeof(float), (d) => Mathf.Sqrt(d), (iv) => MathInfVal.Sqrt(iv), "Mathf.Sqrt({0})", "MathInfVal.Sqrt({0})") },
                { new Function("NthRoot", typeof(double), (d, n) => Math.Pow(d, 1.0 / n), (iv, n) => MathInfVal.NthRoot(iv, (int)n), "Math.Pow({0}, 1.0 / {1})", "MathInfVal.NthRoot({0}, {1})") },
            };

        IReadOnlyList<string> secondParameterIsFloat = new List<string>() { "Repeat", "PingPong", "DeltaAngle", };
        IReadOnlyList<string> secondParameterIsDouble = new List<string>() { "LogN", };
        IReadOnlyList<string> secondParameterIsInt = new List<string>() { "Pow", "NthRoot", };

        // custom types
        class Function
        {
            public string name { get; private set; }
            public Type primitiveType { get; private set; }

            public string pFormat { get; private set; }
            public string ivFormat { get; private set; }
            public bool hasSecondParam { get; private set; }

            object primitiveFunc;
            object infValFunc;

            public dynamic Primitive(dynamic val, dynamic secondParam)
            {
                if (hasSecondParam)
                    return (primitiveFunc as Func<dynamic, dynamic, dynamic>)(val, secondParam);
                return (primitiveFunc as Func<dynamic, dynamic>)(val);
            }

            public InfVal InfVal(InfVal iv, InfVal secondParam)
            {
                if (hasSecondParam)
                    return (infValFunc as Func<InfVal, InfVal, InfVal>)(iv, secondParam);
                return (infValFunc as Func<InfVal, InfVal>)(iv);
            }

            public Function(string name, Type primitiveType, Func<dynamic, dynamic> primitiveFunc, Func<InfVal, InfVal> infValFunc, string pFormat, string ivFormat)
            {
                this.name = name;
                this.primitiveType = primitiveType;
                this.primitiveFunc = primitiveFunc;
                this.infValFunc = infValFunc;

                this.pFormat = pFormat;
                this.ivFormat = ivFormat;
                hasSecondParam = false;
            }

            public Function(string name, Type primitiveType, Func<dynamic, dynamic, dynamic> primitiveFunc, Func<InfVal, InfVal, InfVal> infValFunc, string pFormat, string ivFormat)
                : this(name, primitiveType, (Func<dynamic, dynamic>)null, null, pFormat, ivFormat)
            {
                this.primitiveFunc = primitiveFunc;
                this.infValFunc = infValFunc;

                hasSecondParam = true;
            }
        }

        // private fields
        int functionIndex = 0;

        float secondFloatParam = 100f;
        double secondDoubleParam = 4.0;
        int secondIntParam = 3;

        // constructor
        public FunctionsTest() : base()
        {
            avoidSpecialValues = true;
        }

        // public overrides
        public override string description => "This test will use a static method with a primitive type, it's equivalent with an InfVal and compare the results.\n" +
            "It will ignore results with a special values like NaN and Infinity and functions that causes an overflow.";

        public override void DrawParameters()
        {
            functionIndex = EditorGUILayout.Popup(new GUIContent("Function", "Which static method should we test for."),
                functionIndex, functionsList.Select((func) => func.name).ToArray());

            Function f = functionsList[functionIndex];

            D_InfoOnFailBecauseOfDecimal(f.primitiveType);

            if (f.hasSecondParam)
            {
                if (secondParameterIsFloat.Contains(f.name))
                    secondFloatParam = EditorGUILayout.FloatField(
                        new GUIContent("Second Parameter", "This is the second parameter of the function we will use for every tests."),
                        secondFloatParam);
                else if (secondParameterIsDouble.Contains(f.name))
                    secondDoubleParam = EditorGUILayout.DoubleField(
                        new GUIContent("Second Parameter", "This is the second parameter of the function we will use for every tests."),
                        secondDoubleParam);
                else if (secondParameterIsInt.Contains(f.name))
                    secondIntParam = EditorGUILayout.IntField(
                        new GUIContent("Second Parameter", "This is the second parameter of the function we will use for every tests."),
                        secondIntParam);
            }

            D_IterationsField();
        }

        public override TestResult Process(ref float threadProgressRatio)
        {
            TestResult res = new TestResult(iterations);

            Function f = functionsList[functionIndex];

            for (long i = 0; i < iterations; i++)
            {
                // get random value
                dynamic val;
                if (f.name == "NextPowerOfTwo" || f.name == "ClosestPowerOfTwo")
                    val = rand.Next(0, (1 << 30));
                else if (f.name == "DeltaAngle")
                    val = (float)(rand.NextDouble() * (3600 + 3600) - 3600);
                else if (f.name == "Repeat" || f.name == "PingPong")
                    val = (float)(rand.NextDouble() * 20 * (secondFloatParam) - 10 * secondFloatParam);
                else
                    val = P_CreateRandomValue(f.primitiveType);

                string valToString = P_ValToString(val);
                InfVal iv = new InfVal(val);

                // generate second param
                dynamic secondParam = 0;
                if (f.hasSecondParam)
                {
                    if (secondParameterIsFloat.Contains(f.name))
                        secondParam = secondFloatParam;
                    else if (secondParameterIsDouble.Contains(f.name))
                        secondParam = secondDoubleParam;
                    else if (secondParameterIsInt.Contains(f.name))
                        secondParam = secondIntParam;
                }

                // get primitive result
                string primitiveRes;
                try { primitiveRes = P_ValToString(f.Primitive(val, secondParam)); }
                catch (Exception e) { primitiveRes = $"{TestsCommon.exceptionPrefix} {e.Message}"; }

                // skip if invalid
                if (!TestsCommon.IsValidNumber(primitiveRes))
                {
                    --res.usedIterations;
                    threadProgressRatio = ((float)i + 1) / iterations;
                    continue;
                }

                // set infVal exp to get a result with the same precision
                int exp = P_GetCalcExponent(primitiveRes, iv.exponent);
                iv = iv.ToExponent(Math.Min(iv.exponent, exp));

                // get infVal result
                string infValRes;
                try { infValRes = P_ConvertInfValToTypeToString(f.InfVal(iv, (InfVal)secondParam), f.primitiveType); }
                catch (Exception e) { infValRes = $"{TestsCommon.exceptionPrefix} {e.Message}"; }

                // subscribe
                string secondToStr = P_ValToString(secondParam);

                res.SubscribeResult(primitiveRes, infValRes, string.Format(f.pFormat, valToString, secondToStr),
                    string.Format(f.ivFormat, P_InfValToSystemLikeString(iv, P_CountDigits(valToString)), secondToStr));

                // set progress
                threadProgressRatio = ((float)i + 1) / iterations;
            }

            return res;
        }
    }
}
