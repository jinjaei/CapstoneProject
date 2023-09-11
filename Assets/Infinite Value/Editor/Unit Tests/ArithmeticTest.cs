using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace InfiniteValue
{
    class ArithmeticTest : AUnitTest
    {
        // consts
        static readonly IReadOnlyDictionary<UnaryOperation, Func<dynamic, dynamic>> unaryOperationToFunction
            = new Dictionary<UnaryOperation, Func<dynamic, dynamic>>
        {
                { UnaryOperation.Plus, (d) => checked(+d) },
                { UnaryOperation.Minus, (d) => checked(-d) },
                { UnaryOperation.OnesComplement, (d) => checked(~d) },
                { UnaryOperation.Increment, (d) => checked(++d) },
                { UnaryOperation.Decrement, (d) => checked(--d) },
        };

        static readonly IReadOnlyDictionary<BinaryOperation, Func<dynamic, dynamic, dynamic>> binaryOperationToFunction
            = new Dictionary<BinaryOperation, Func<dynamic, dynamic, dynamic>>
        {
                { BinaryOperation.Add, (a, b) => checked(a + b) },
                { BinaryOperation.Subtract, (a, b) => checked(a - b) },
                { BinaryOperation.Multiply, (a, b) => checked(a * b) },
                { BinaryOperation.Divide, (a, b) => checked(a / b) },
                { BinaryOperation.Modulo, (a, b) => checked(a % b) },
                { BinaryOperation.And, (a, b) => checked(a & b) },
                { BinaryOperation.Or, (a, b) => checked(a | b) },
                { BinaryOperation.Xor, (a, b) => checked(a ^ b) },
        };

        static readonly IReadOnlyDictionary<UnaryOperation, string> unarySymbols = new Dictionary<UnaryOperation, string>
            {
                { UnaryOperation.Plus, "+" },
                { UnaryOperation.Minus, "-" },
                { UnaryOperation.OnesComplement, "~" },
                { UnaryOperation.Increment, "++" },
                { UnaryOperation.Decrement, "--" },
            };

        static readonly IReadOnlyDictionary<BinaryOperation, string> binarySymbols = new Dictionary<BinaryOperation, string>
            {
                { BinaryOperation.Add, "+" },
                { BinaryOperation.Subtract, "-" },
                { BinaryOperation.Multiply, "*" },
                { BinaryOperation.Divide, "/" },
                { BinaryOperation.Modulo, "%" },
                { BinaryOperation.And, "&" },
                { BinaryOperation.Or, "|" },
                { BinaryOperation.Xor, "^" },
            };

        // custom types
        enum UnaryOperation
        {
            Plus,
            Minus,
            OnesComplement,
            Increment,
            Decrement,
        }
        const int unaryCount = 5;

        enum BinaryOperation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Modulo,
            And,
            Or,
            Xor,
        }
        const int binaryCount = 8;

        // private fields
        bool binary = false;
        UnaryOperation unaryOperation = default;
        BinaryOperation binaryOperation = default;

        GUIContent[] operationsPopUpNames;

        // constructors
        public ArithmeticTest() : base()
        {
            List<GUIContent> popUpList = new List<GUIContent>();
            popUpList.AddRange(Enum.GetNames(typeof(UnaryOperation)).Select((s) => new GUIContent(ObjectNames.NicifyVariableName(s))).ToArray());
            popUpList.Add(new GUIContent(""));
            popUpList.AddRange(Enum.GetNames(typeof(BinaryOperation)).Select((s) => new GUIContent(ObjectNames.NicifyVariableName(s))).ToArray());
            operationsPopUpNames = popUpList.ToArray();

            avoidSpecialValues = true;
        }

        // public overrides
        public override string description => "This test will perform arithmetic operations on randoms primitive type values, do the same on those values " +
            "casted to an InfVal and compare the results.\n" +
            "It will ignore results with a special values like NaN and Infinity and operations that causes an overflow.";

        public override void DrawParameters()
        {
            D_VarTypeField();
            D_InfoOnFailBecauseOfDecimal();

            int op = EditorGUILayout.Popup(new GUIContent("Operation", "Which operation should we test"),
                (binary ? unaryCount + 1 + (int)binaryOperation : (int)unaryOperation),
                operationsPopUpNames);

            if (op != unaryCount)
            {
                binary = (op > unaryCount);
                unaryOperation = (UnaryOperation)op;
                binaryOperation = (BinaryOperation)(op - unaryCount - 1);
            }

            D_IterationsField();
        }

        public override TestResult Process(ref float threadProgressRatio)
        {
            TestResult res = new TestResult(iterations);

            for (long i = 0; i < iterations; i++)
            {
                if (!binary)
                {
                    // get random value
                    dynamic val = P_CreateRandomValue();
                    InfVal iv = new InfVal(val);
                    string valToString = P_ValToString(val);

                    // get primitive result
                    string primitiveRes;
                    try { primitiveRes = P_ValToString(Convert.ChangeType(unaryOperationToFunction[unaryOperation](val), varType)); }
                    catch (Exception e) { primitiveRes = $"{TestsCommon.exceptionPrefix} {e.Message}"; }

                    // skip if invalid
                    if (!TestsCommon.IsValidNumber(primitiveRes))
                    {
                        --res.usedIterations;
                        threadProgressRatio = ((float)i + 1) / iterations;
                        continue;
                    }

                    // get infVal result
                    string infValRes;
                    try { infValRes = P_ConvertInfValToTypeToString((InfVal)unaryOperationToFunction[unaryOperation](iv)); }
                    catch (Exception e) { infValRes = $"{TestsCommon.exceptionPrefix} {e.Message}"; }

                    // subscribe
                    res.SubscribeResult(primitiveRes, infValRes, $"{unarySymbols[unaryOperation]}({valToString})",
                        $"{unarySymbols[unaryOperation]}({P_InfValToSystemLikeString(iv, P_CountDigits(valToString))})");
                }
                else //(binary)
                {
                    // get random values
                    dynamic val1 = P_CreateRandomValue();
                    dynamic val2 = P_CreateRandomValue();

                    InfVal iv1 = new InfVal(val1);
                    InfVal iv2 = new InfVal(val2);
                    string valToString1 = P_ValToString(val1);
                    string valToString2 = P_ValToString(val2);

                    // get primitive result
                    string primitiveRes;
                    try { primitiveRes = P_ValToString(Convert.ChangeType(binaryOperationToFunction[binaryOperation](val1, val2), varType)); }
                    catch (Exception e) { primitiveRes = $"{TestsCommon.exceptionPrefix} {e.Message}"; }

                    // skip if invalid
                    if (!TestsCommon.IsValidNumber(primitiveRes))
                    {
                        --res.usedIterations;
                        threadProgressRatio = ((float)i + 1) / iterations;
                        continue;
                    }

                    // get infVal result
                    string infValRes;
                    try { infValRes = P_ConvertInfValToTypeToString((InfVal)binaryOperationToFunction[binaryOperation](iv1, iv2)); }
                    catch (Exception e) { infValRes = $"{TestsCommon.exceptionPrefix} {e.Message}"; }

                    // subscribe
                    res.SubscribeResult(primitiveRes, infValRes, $"{valToString1} {binarySymbols[binaryOperation]} {valToString2}",
                        $"{P_InfValToSystemLikeString(iv1, P_CountDigits(valToString1))} {binarySymbols[binaryOperation]} " +
                        $"{P_InfValToSystemLikeString(iv2, P_CountDigits(valToString2))}");
                }

                threadProgressRatio = ((float)i + 1) / iterations;
            }

            return res;
        }
    }
}
