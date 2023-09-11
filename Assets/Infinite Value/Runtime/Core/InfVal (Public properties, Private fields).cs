using System.Numerics;
using System;
using UnityEngine;

namespace InfiniteValue
{
    public partial struct InfVal
    {
        // private consts
        const string invalidOperationNegExpFormat = "Cannot use {0} on a floating point InfVal (exponent < 0)."; // 0: operation
        const string invalidOperationNonZeroExpFormat = "Cannot use {0} on a not simple integer InfVal (exponent != 0)."; // 0: operation

        const string cannotBeNegOrZeroStr = "Cannot be negative or zero.";

        const string debugStringFormat = "({0}, {1})"; // 0: digits, 1: exponent

        // serialized private fields
        [SerializeField] string s_digits;
        [SerializeField] int s_exponent;

        // not serialized private fields
        BigInteger _digits;

        // private properties
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        string cacheDigitsToString => s_digits ?? "0";

        // public properties

        /// <summary> The full digits that are multiplied by 10 raised to the exponent to get the value. </summary> 
        public BigInteger digits 
        {
#if UNITY_2020_2_OR_NEWER
            readonly
#endif
            get => _digits;
            private set
            {
                if (_digits == value)
                    return;

                _digits = value;

                s_digits = _digits.ToString();
            }
        }

        /// <summary> The exponent that 10 is raised by and multiplied by the digits to get the value. </summary> 
        public int exponent
        {
#if UNITY_2020_2_OR_NEWER
            readonly
#endif
            get => s_exponent;
            private set
            {
                if (value == s_exponent)
                    return;

                if (value - s_exponent >= precision)
                    digits = 0;
                else
                    digits = MathBigInteger.MultiplyByPowerOf10(digits, s_exponent - value);

                s_exponent = value;
            }
        }

        /// <summary> Count of digits of this <see cref="InfVal"/>. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public int precision => (isZero && exponent <= 0 ? -exponent + 1 : cacheDigitsToString.Length - (sign < 0 ? 1 : 0));

        /// <summary> Is this <see cref="InfVal"/> an integer value. Can be true with a negative exponent if every digits after the decimal point are 0. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public bool isInteger => (exponent >= 0 || cacheDigitsToString.EndsWith(new string('0', -exponent)));

        /// <summary> Is this <see cref="InfVal"/> equal to 0. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public bool isZero => digits.IsZero;

        /// <summary> Is this <see cref="InfVal"/> equal to 1. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public bool isOne
        {
            get
            {
                if (exponent > 0 || !isInteger)
                    return false;

                return ToExponent(0).digits.IsOne;
            }
        }

        /// <summary> Is this <see cref="InfVal"/> an even number. Will throw an exception if exponent is negative. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public bool isEven
        {
            get
            {
                if (exponent > 0)
                    return true;
                if (exponent < 0)
                    throw new InvalidOperationException(string.Format(invalidOperationNegExpFormat, ".isEven"));

                return digits.IsEven;
            }
        }

        /// <summary> Is this <see cref="InfVal"/> 2 to an integer power. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public bool isPowerOfTwo
        {
            get
            {
                if (exponent > 0)
                    return false;
                if (exponent < 0)
                {
                    InfVal truncated = ToExponent(0);
                    return (this == truncated && truncated.digits.IsPowerOfTwo);
                }

                return digits.IsPowerOfTwo;
            }
        }

        /// <summary> The sign of this <see cref="InfVal"/>: 1, 0 or -1. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public int sign => digits.Sign;
    }
}
