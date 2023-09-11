using System.Numerics;
using System;

namespace InfiniteValue
{
    public partial struct InfVal : IComparable, IComparable<InfVal>, IEquatable<InfVal>
    {
        // unary operators overload
        public static InfVal operator +(in InfVal iv) => iv;

        public static InfVal operator -(in InfVal iv) => ManualFactory(-iv.digits, iv.exponent);

        public static InfVal operator ~(in InfVal iv)
        {
            if (iv.exponent != 0)
                throw new InvalidOperationException(string.Format(invalidOperationNonZeroExpFormat, "operator ~"));

            return ManualFactory(~iv.digits, 0);
        }

        public static InfVal operator ++(in InfVal iv) => ManualFactory(iv.digits, iv.exponent) + 1;

        public static InfVal operator --(in InfVal iv) => ManualFactory(iv.digits, iv.exponent) - 1;

        // binary operators overload
        public static InfVal operator +(in InfVal a, in InfVal b)
        {
            int resPrecision = Math.Max(a.precision, b.precision);

            if (a.isZero)
                return b.ToPrecision(resPrecision);
            if (b.isZero)
                return a.ToPrecision(resPrecision);

            int calcExponent = Math.Max(a.exponent + a.precision - resPrecision, b.exponent + b.precision - resPrecision);

            return ManualFactory(a.ToExponent(calcExponent).digits + b.ToExponent(calcExponent).digits, calcExponent).ToPrecision(resPrecision);
        }

        public static InfVal operator -(in InfVal a, in InfVal b)
        {
            int resPrecision = Math.Max(a.precision, b.precision);

            if (a.isZero)
                return -b.ToPrecision(resPrecision);
            if (b.isZero)
                return a.ToPrecision(resPrecision);
            
            int calcExponent = Math.Max(a.exponent + a.precision - resPrecision, b.exponent + b.precision - resPrecision);

            return ManualFactory(a.ToExponent(calcExponent).digits - b.ToExponent(calcExponent).digits, calcExponent).ToPrecision(resPrecision);
        }

        public static InfVal operator *(in InfVal a, in InfVal b)
        {
            int resPrecision = Math.Max(a.precision, b.precision);

            if (a.isZero || b.isZero)
                return new InfVal(0, resPrecision);

            InfVal aNoZero = a.RemoveTrailingZeros();
            InfVal bNoZero = b.RemoveTrailingZeros();

            return ManualFactory(aNoZero.digits * bNoZero.digits, aNoZero.exponent + bNoZero.exponent).ToPrecision(resPrecision);
        }

        public static InfVal operator /(in InfVal a, in InfVal b)
        {
            if (b.isZero)
                throw new DivideByZeroException();

            int resPrecision = Math.Max(a.precision, b.precision);

            if (a.isZero)
                return new InfVal(0, resPrecision);

            return ManualFactory(MathBigInteger.MultiplyByPowerOf10(a.digits, resPrecision) / b.digits, a.exponent - b.exponent - resPrecision).ToPrecision(resPrecision);
        }

        public static InfVal operator %(in InfVal a, in InfVal b)
        {
            if (b.isZero)
                throw new DivideByZeroException();

            int resPrecision = Math.Max(a.precision, b.precision);

            if (a.isZero)
                return new InfVal(0, resPrecision);

            int lowestExponent = Math.Min(a.exponent, b.exponent);
            return ManualFactory(a.ToExponent(lowestExponent).digits % b.ToExponent(lowestExponent).digits, lowestExponent).ToPrecision(Math.Max(a.precision, b.precision));
        }

        public static InfVal operator &(in InfVal a, in InfVal b)
        {
            if (a.exponent != 0 || b.exponent != 0)
                throw new InvalidOperationException(string.Format(invalidOperationNonZeroExpFormat, "operator &"));

            return ManualFactory(a.digits & b.digits, 0);
        }

        public static InfVal operator |(in InfVal a, in InfVal b)
        {
            if (a.exponent != 0 || b.exponent != 0)
                throw new InvalidOperationException(string.Format(invalidOperationNonZeroExpFormat, "operator |"));

            return ManualFactory(a.digits | b.digits, 0);
        }

        public static InfVal operator ^(in InfVal a, in InfVal b)
        {
            if (a.exponent != 0 || b.exponent != 0)
                throw new InvalidOperationException(string.Format(invalidOperationNonZeroExpFormat, "operator ^"));

            return ManualFactory(a.digits ^ b.digits, 0);
        }

        public static InfVal operator <<(in InfVal iv, int decal)
        {
            if (iv.exponent != 0)
                throw new InvalidOperationException(string.Format(invalidOperationNonZeroExpFormat, "operator <<"));

            return ManualFactory(iv.digits << decal, 0);
        }

        public static InfVal operator >>(in InfVal iv, int decal)
        {
            if (iv.exponent != 0)
                throw new InvalidOperationException(string.Format(invalidOperationNonZeroExpFormat, "operator >>"));

            return ManualFactory(iv.digits >> decal, 0);
        }

        public static bool operator ==(in InfVal a, in InfVal b)
        {
            if (a.sign != b.sign)
                return false;

            long aE = (long)a.exponent + (long)a.precision;
            long bE = (long)b.exponent + (long)b.precision;
            if (aE != bE)
                return false;

            int lowestExponent = Math.Min(a.exponent, b.exponent);
            return (a.ToExponent(lowestExponent).digits == b.ToExponent(lowestExponent).digits);
        }

        public static bool operator !=(in InfVal a, in InfVal b)
        {
            if (a.sign != b.sign)
                return true;

            long aE = (long)a.exponent + (long)a.precision;
            long bE = (long)b.exponent + (long)b.precision;
            if (aE != bE)
                return true;

            int lowestExponent = Math.Min(a.exponent, b.exponent);
            return (a.ToExponent(lowestExponent).digits != b.ToExponent(lowestExponent).digits);
        }

        public static bool operator <(in InfVal a, in InfVal b)
        {
            if (a.sign != b.sign)
                return a.sign < b.sign;

            long aE = (long)a.exponent + (long)a.precision;
            long bE = (long)b.exponent + (long)b.precision;
            if (aE != bE)
                return aE < bE;

            int lowestExponent = Math.Min(a.exponent, b.exponent);
            return (a.ToExponent(lowestExponent).digits < b.ToExponent(lowestExponent).digits);
        }

        public static bool operator >(in InfVal a, in InfVal b)
        {
            if (a.sign != b.sign)
                return a.sign > b.sign;

            long aE = (long)a.exponent + (long)a.precision;
            long bE = (long)b.exponent + (long)b.precision;
            if (aE != bE)
                return aE > bE;

            int lowestExponent = Math.Min(a.exponent, b.exponent);
            return (a.ToExponent(lowestExponent).digits > b.ToExponent(lowestExponent).digits);
        }

        public static bool operator <=(in InfVal a, in InfVal b)
        {
            if (a.sign != b.sign)
                return a.sign < b.sign;

            long aE = (long)a.exponent + (long)a.precision;
            long bE = (long)b.exponent + (long)b.precision;
            if (aE != bE)
                return aE < bE;

            int lowestExponent = Math.Min(a.exponent, b.exponent);
            return (a.ToExponent(lowestExponent).digits <= b.ToExponent(lowestExponent).digits);
        }

        public static bool operator >=(in InfVal a, in InfVal b)
        {
            if (a.sign != b.sign)
                return a.sign > b.sign;

            long aE = (long)a.exponent + (long)a.precision;
            long bE = (long)b.exponent + (long)b.precision;
            if (aE != bE)
                return aE > bE;

            int lowestExponent = Math.Min(a.exponent, b.exponent);
            return (a.ToExponent(lowestExponent).digits >= b.ToExponent(lowestExponent).digits);
        }

        // overrides

        /// <summary> Returns a <see langword="bool"/> indicating whether this instance is equal to another <see langword="object"/>. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
                return false;

            return (Equals((InfVal)obj));
        }

        /// <summary> Returns the hash code for this instance. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public override int GetHashCode() => (digits, exponent).GetHashCode();

        // interfaces implementation

        /// <summary> Returns a <see langword="bool"/> indicating whether this instance is equal to a specified <see cref="InfVal"/>. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public bool Equals(InfVal other) => (this == other);

        /// <summary> Compares this instance to a specified <see cref="InfVal"/> and returns an indication of their relative values. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public int CompareTo(InfVal other)
        {
            if (sign != other.sign)
                return sign.CompareTo(other.sign);

            long thisE = (long)exponent + (long)precision;
            long otherE = (long)other.exponent + (long)other.precision;
            if (thisE != otherE)
                return thisE.CompareTo(otherE);

            int lowestExponent = Math.Min(exponent, other.exponent);
            return ToExponent(lowestExponent).digits.CompareTo(other.ToExponent(lowestExponent).digits);
        }

        /// <summary> Compares this instance to a specified <see langword="object"/> and returns an indication of their relative values. </summary> 
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case BigInteger bi: return CompareTo((InfVal)bi);

                case string valStr: return CompareTo((InfVal)valStr);

                case sbyte b: return CompareTo((InfVal)b);
                case byte b: return CompareTo((InfVal)b);
                case short s: return CompareTo((InfVal)s);
                case ushort s: return CompareTo((InfVal)s);
                case int i: return CompareTo((InfVal)i);
                case uint i: return CompareTo((InfVal)i);
                case long l: return CompareTo((InfVal)l);
                case ulong l: return CompareTo((InfVal)l);

                case float f: return CompareTo((InfVal)f);
                case double d: return CompareTo((InfVal)d);
                case decimal d: return CompareTo((InfVal)d);

                default: throw new ArgumentException("The parameter isn't an InfVal or castable to it.", nameof(obj));
            }
        }
    }
}
