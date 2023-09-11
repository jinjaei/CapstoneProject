using System.Numerics;
using UnityEngine;
using System;

namespace InfiniteValue
{
    [Serializable] public partial struct InfVal : ISerializationCallbackReceiver
    {
        // public methods

        /// <summary> 
        /// Return this <see cref="InfVal"/> with a modified <paramref name="exponent"/>. 
        /// The default behaviour is to add zeros at the end of the digits if you decrease the exponent or trim ending digits if you increase the exponent.
        /// </summary>
        /// <param name="raw"> Optionally set the <paramref name="exponent"/> directly without changing the digits. </param>
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public InfVal ToExponent(int exponent, bool raw = false)
        {
            if (exponent == this.exponent)
                return this;

            InfVal ret = ManualFactory(this.digits, this.exponent);
            if (!raw)
                ret.exponent = exponent;
            else
                ret.s_exponent = exponent;
            return ret;
        }

        /// <summary> 
        /// Return this <see cref="InfVal"/> with modified <paramref name="digits"/>.
        /// </summary>
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public InfVal ToDigits(in BigInteger digits)
        {
            if (digits == this.digits)
                return this;

            InfVal ret = ManualFactory(this.digits, this.exponent);
            ret.digits = digits;
            return ret;
        }

        /// <summary> 
        /// Return this <see cref="InfVal"/> with a modified <paramref name="precision"/> by changing the exponent.
        /// This will throw an exception if precision is negative or zero.
        /// </summary>
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public InfVal ToPrecision(int precision)
        {
            if (precision <= 0)
                throw new ArgumentException(cannotBeNegOrZeroStr, nameof(precision));

            if (precision == this.precision)
                return this;

            return ToExponent(this.exponent + this.precision - precision);
        }

        /// <summary> 
        /// Return this <see cref="InfVal"/> with all zeros at the end of the digits removed and the exponent modified accordingly.
        /// This wont change the <see cref="InfVal"/> value.
        /// </summary>
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public InfVal RemoveTrailingZeros()
        {
            InfVal ret = ManualFactory(digits, exponent);

            int toRemove = 0;
            while (toRemove < cacheDigitsToString.Length - 1 && cacheDigitsToString[cacheDigitsToString.Length - (1 + toRemove)] == '0')
                ++toRemove;

            ret.exponent += toRemove;
            return ret;
        }

        /// <summary> 
        /// Return this <see cref="InfVal"/> with the decimal point moved left by <paramref name="move"/> amount.
        /// This will change the exponent without changing the digits and therefore will change the <see cref="InfVal"/> value.
        /// </summary>
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public InfVal MovePointLeft(int move)
        {
            InfVal ret = ManualFactory(digits, exponent);
            ret.s_exponent -= move;
            return ret;
        }

        /// <summary> 
        /// Return this <see cref="InfVal"/> with the decimal point moved right by <paramref name="move"/> amount.
        /// This will change the exponent without changing the digits and therefore will change the <see cref="InfVal"/> value.
        /// </summary>
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public InfVal MovePointRight(int move)
        {
            InfVal ret = ManualFactory(digits, exponent);
            ret.s_exponent += move;
            return ret;
        }

        /// <summary> 
        /// Extract the <paramref name="digits"/> and <paramref name="exponent"/> of this <see cref="InfVal"/>.
        /// Like every Deconstruct method with a similar signature, it can be used by typing:
        /// <code> (BigInteger d, int e) = infValVariable; </code>                                                  
        /// </summary>
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        public void Deconstruct(out BigInteger digits, out int exponent)
        {
            digits = this.digits;
            exponent = this.exponent;
        }

        // serialization methods
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            s_digits = cacheDigitsToString;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (BigInteger.TryParse(cacheDigitsToString, out BigInteger res))
                digits = res;
            else
                digits = 0;
        }
    }
}
