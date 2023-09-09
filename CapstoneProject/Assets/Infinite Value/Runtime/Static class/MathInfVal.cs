using System.Numerics;
using UnityEngine;
using System;

namespace InfiniteValue
{
    /// <summary> 
    /// Provides some math related methods for an <see cref="InfVal"/>, similarly to <see cref="Math"/> or <see cref="Mathf"/>.
    /// </summary>
    public static class MathInfVal
    {
        // consts
        const string noArgumentsExceptionFormat_Method = "Cannot use {0} with no parameters";

        static readonly double log10Cache = Math.Log(10);

        // public methods

        /// <summary> Returns the absolute value of an <see cref="InfVal"/>. </summary> 
        public static InfVal Abs(in InfVal value) => InfVal.ManualFactory(BigInteger.Abs(value.digits), value.exponent);

        /// <summary> Compares two <see cref="InfVal"/> and returns true if they are similar. </summary> 
        public static bool Approximately(in InfVal a, in InfVal b, int precision) => (MathBigInteger.MultiplyByPowerOf10(a.digits, precision - a.precision) == MathBigInteger.MultiplyByPowerOf10(b.digits, precision - b.precision));

        /// <summary> Clamps a <paramref name="value"/> between <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive]. </summary> 
        public static InfVal Clamp(in InfVal value, in InfVal min, in InfVal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary> Clamps a <paramref name="value"/> between 0 and 1. </summary> 
        public static InfVal Clamp01(in InfVal value) => Clamp(value, 0, 1);

        /// <summary> Returns the closest power of two of <paramref name="value"/>. </summary> 
        public static InfVal ClosestPowerOfTwo(in InfVal value)
        {
            if (value.exponent == 0 && value.isPowerOfTwo)
                return value.ToExponent(0);
            if (value.sign < 0)
                return 0;
            if (value <= 1)
                return 1;

            InfVal previous = PreviousPowerOfTwo(value);
            InfVal next = NextPowerOfTwo(value);

            return (value - previous < next - value ? previous : next);
        }

        /// <summary> Returns the next power of two that is equal to, or greater than <paramref name="value"/>. </summary> 
        public static InfVal NextPowerOfTwo(in InfVal value)
        {
            if (value.isPowerOfTwo)
                return value.ToExponent(0);
            if (value.sign < 0)
                return 0;

            InfVal compareVal = value.ToExponent(0);
            byte[] bytes = compareVal.digits.ToByteArray();

            for (int i = 0; i < bytes.Length - 1; i++)
                bytes[i] = 0;
            bytes[bytes.Length - 1] = 1;

            BigInteger bi = new BigInteger(bytes);
            while (bi < compareVal.digits)
                bi <<= 1;

            return InfVal.ManualFactory(bi, 0);
        }

        /// <summary> Returns the previous power of two that is equal to, or lower than <paramref name="value"/>. </summary> 
        public static InfVal PreviousPowerOfTwo(in InfVal value)
        {
            if (value.isPowerOfTwo)
                return value.ToExponent(0);
            if (value.sign < 0)
                return 0;

            return NextPowerOfTwo(value) >> 1;
        }

        /// <summary> Loops the <paramref name="value"/>, so that it is never larger than <paramref name="length"/> and never smaller than 0. </summary> 
        public static InfVal Repeat(in InfVal value, in InfVal length) => ((value % length) + length) % length;

        /// <summary> PingPong returns a <paramref name="value"/> that will increment and decrement between the value 0 and <paramref name="length"/>. </summary> 
        public static InfVal PingPong(in InfVal value, in InfVal length)
        {
            InfVal mod = Abs(value) % (length * 2);
            if (mod >= length)
                return (length - (mod % length));

            return mod % length;
        }

        /// <summary> Calculates the shortest difference between two angles given in degrees. </summary> 
        public static InfVal DeltaAngle(in InfVal current, in InfVal target) => (((((target - current) + 180) % 360) + 360) % 360) - 180;

        /// <summary> Returns the natural logarithm of a specified <see cref="InfVal"/>. </summary> 
        public static double Log(in InfVal value) => BigInteger.Log(value.digits) + value.exponent * log10Cache;

        /// <summary> Returns the logarithm of a specified <see cref="InfVal"/> in a specified base. </summary> 
        public static double Log(in InfVal value, double logBase) => BigInteger.Log(value.digits, logBase) + value.exponent * Math.Log(10, logBase);

        /// <summary> Returns the base 10 logarithm of a specified <see cref="InfVal"/>. </summary> 
        public static double Log10(in InfVal value) => BigInteger.Log10(value.digits) + value.exponent;

        /// <summary> Returns the largest integer smaller than or equal to an <see cref="InfVal"/>. </summary> 
        public static InfVal Floor(in InfVal value)
        {
            if (value.isInteger)
                return value;
            if (value.sign >= 0)
                return value.ToExponent(0).ToExponent(value.exponent);

            return (value.ToExponent(0) - 1).ToExponent(value.exponent);
        }

        /// <summary> Returns the smallest integer greater to or equal to an <see cref="InfVal"/>. </summary> 
        public static InfVal Ceil(in InfVal value)
        {
            if (value.isInteger)
                return value;
            if (value.sign <= 0)
                return value.ToExponent(0).ToExponent(value.exponent);

            return (value.ToExponent(0) + 1).ToExponent(value.exponent);
        }

        /// <summary> Returns an <see cref="InfVal"/> rounded to the nearest integer. </summary> 
        public static InfVal Round(in InfVal value)
        {
            if (value.isInteger)
                return value;

            int digitAfterDecimalPoint = (int)(BigInteger.Abs(value.ToExponent(-1).digits) % 10);
            if (digitAfterDecimalPoint >= 5)
                return (value.ToExponent(0) + value.sign).ToExponent(value.exponent);
            return value.ToExponent(0).ToExponent(value.exponent);
        }

        /// <summary> Returns the largest of two or more values. </summary> 
        public static InfVal Max(in InfVal a, in InfVal b) => (a >= b ? a : b);

        /// <summary> Returns the largest of two or more values. </summary> 
        public static InfVal Max(params InfVal[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException(string.Format(noArgumentsExceptionFormat_Method, "MathInfVal.Max"));

            InfVal ret = values[0];
            for (int i = 1; i < values.Length; i++)
                ret = Max(ret, values[i]);
            return ret;
        }

        /// <summary> Returns the smallest of two or more values. </summary> 
        public static InfVal Min(in InfVal a, in InfVal b) => (a <= b ? a : b);

        /// <summary> Returns the smallest of two or more values. </summary> 
        public static InfVal Min(params InfVal[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException(string.Format(noArgumentsExceptionFormat_Method, "MathInfVal.Min"));

            InfVal ret = values[0];
            for (int i = 1; i < values.Length; i++)
                ret = Min(ret, values[i]);
            return ret;
        }

        /// <summary> Returns the value with the highest exponent. </summary> 
        public static int MaxExponent(in InfVal a, in InfVal b) => (a.exponent >= b.exponent ? a.exponent : b.exponent);

        /// <summary> Returns the value with the highest exponent. </summary> 
        public static int MaxExponent(params InfVal[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException(string.Format(noArgumentsExceptionFormat_Method, "MathInfVal.MaxExponent"));

            int ret = values[0].exponent;
            for (int i = 1; i < values.Length; i++)
                ret = MinExponent(ret, values[i].exponent);
            return ret;
        }

        /// <summary> Returns the value with the lowest exponent.</summary
        public static int MinExponent(in InfVal a, in InfVal b) => (a.exponent <= b.exponent ? a.exponent : b.exponent);

        /// <summary> Returns the value with the lowest exponent.</summary
        public static int MinExponent(params InfVal[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException(string.Format(noArgumentsExceptionFormat_Method, "MathInfVal.MinExponent"));

            int ret = values[0].exponent;
            for (int i = 1; i < values.Length; i++)
                ret = MinExponent(ret, values[i].exponent);
            return ret;
        }

        /// <summary> Returns the value with the highest precision.</summary
        public static int MaxPrecision(in InfVal a, in InfVal b) => (a.precision >= b.precision ? a.precision : b.precision);

        /// <summary> Returns the value with the highest precision. </summary> 
        public static int MaxPrecision(params InfVal[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException(string.Format(noArgumentsExceptionFormat_Method, "MathInfVal.MaxPrecision"));

            int ret = values[0].precision;
            for (int i = 1; i < values.Length; i++)
                ret = MinPrecision(ret, values[i].precision);
            return ret;
        }

        /// <summary> Returns the value with the lowest precision. </summary> 
        public static int MinPrecision(in InfVal a, in InfVal b) => (a.precision <= b.precision ? a.precision : b.precision);

        /// <summary> Returns the value with the lowest precision. </summary> 
        public static int MinPrecision(params InfVal[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException(string.Format(noArgumentsExceptionFormat_Method, "MathInfVal.MinPrecision"));

            int ret = values[0].precision;
            for (int i = 1; i < values.Length; i++)
                ret = MinPrecision(ret, values[i].precision);
            return ret;
        }

        /// <summary> Returns <paramref name="value"/> raised to <paramref name="power"/>. </summary> 
        /// <param name="conservePrecision">Optionally define whether the result should have the same precision as the given <paramref name="value"/>.</param>
        public static InfVal Pow(in InfVal value, int power, bool conservePrecision = true)
        {
            if (power == 0)
                return new InfVal(1, value.precision);
            if (value.isZero || value.isOne)
                return value;

            InfVal result = InfVal.ManualFactory(1, 0);
            InfVal factor = value.RemoveTrailingZeros();
            int exp = Math.Abs(power);

            while (exp > 0)
            {
                if ((exp & 1) == 1)
                    result *= factor;
                factor *= factor;
                exp >>= 1;
            }

            if (power < 0)
                result = 1 / result;

            return (conservePrecision ? result.ToPrecision(value.precision) : result);
        }

        /// <summary> Returns the square root of <paramref name="value"/>. </summary> 
        /// <param name="conservePrecision">Optionally define whether the result should have the same precision as the given <paramref name="value"/>.</param>
        public static InfVal Sqrt(in InfVal value, bool conservePrecision = true)
        {
            if (value.isZero || value.isOne)
                return value;
            if (value.sign < 0)
                throw new ArgumentException("Cannot use Sqrt with a negative value (The result is a complex number).", nameof(value));

            InfVal calcVal = value.RemoveTrailingZeros().ToExponent(value.exponent * 2);

            InfVal res = InfVal.ManualFactory(MathBigInteger.Sqrt(calcVal.digits), calcVal.exponent / 2);

            return (conservePrecision ? res.ToPrecision(value.precision) : res);
        }

        /// <summary> Returns the <paramref name="n"/>th root of <paramref name="value"/>. (The number that raised to <paramref name="n"/> will equal <paramref name="value"/>). </summary> 
        /// <param name="conservePrecision">Optionally define whether the result should have the same precision as the given <paramref name="value"/>.</param>
        public static InfVal NthRoot(in InfVal value, int n, bool conservePrecision = true)
        {
            if (n == 0)
                throw new ArgumentException("0th root of a value is undefined.", nameof(n));
            if (value.isZero || value.isOne)
                return value;

            if (value.sign < 0 && n % 2 == 0)
                throw new ArgumentException("Result of NthRoot with a negative value and an even n is a complex number.", $"{nameof(value)}, {nameof(n)}");

            int calcN = Math.Abs(n);
            int calcExp = value.exponent;
            if ((value.exponent / (float)calcN) % 1 != 0)
                calcExp = (value.exponent / calcN) * calcN;
            InfVal calcVal = value.RemoveTrailingZeros().ToExponent(calcExp * (int)-Mathf.Sign(calcExp) * calcN);

            InfVal res = InfVal.ManualFactory(MathBigInteger.NthRoot(calcVal.digits, calcN), calcVal.exponent / calcN);
            if (n < 0)
                res = 1 / res;

            return (conservePrecision ? res.ToPrecision(value.precision) : res);
        }

        /// <summary> Calculates the integral part of a <paramref name="value"/>. The returned <see cref="InfVal"/> will have the same exponent as the <paramref name="value"/> argument. </summary> 
        public static InfVal Truncate(in InfVal value) => (!value.isInteger ? value.ToExponent(0).ToExponent(value.exponent) : value);

        /// <summary> Calculates the decimal part of an <see cref="InfVal"/>. </summary> 
        public static InfVal DecimalPart(in InfVal value) => (value.exponent < 0 ? (value - value.ToExponent(0)) : 0);

        /// <summary> Finds the greatest common divisor of two <see cref="InfVal"/>. </summary> 
        public static InfVal GreatestCommonDivisor(in InfVal a, in InfVal b)
        {
            int lowestExponent = Mathf.Min(a.exponent, b.exponent);
            return InfVal.ManualFactory(BigInteger.GreatestCommonDivisor(a.ToExponent(lowestExponent).digits, b.ToExponent(lowestExponent).digits), lowestExponent);
        }

        /// <summary> Return a random value between <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive]. </summary> 
        public static InfVal RandomRange(in InfVal min, in InfVal max)
        {
            int lowestExponent = Mathf.Min(min.exponent, max.exponent);
            return InfVal.ManualFactory(MathBigInteger.RandomInRange(min.ToExponent(lowestExponent).digits, max.ToExponent(lowestExponent).digits), lowestExponent);
        }
    }
}