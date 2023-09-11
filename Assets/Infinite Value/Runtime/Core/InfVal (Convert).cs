using System;
using System.Numerics;

namespace InfiniteValue
{
    public partial struct InfVal : IConvertible
    {
#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        byte IConvertible.ToByte(IFormatProvider provider) => (byte)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        sbyte IConvertible.ToSByte(IFormatProvider provider) => (sbyte)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        short IConvertible.ToInt16(IFormatProvider provider) => (short)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        ushort IConvertible.ToUInt16(IFormatProvider provider) => (ushort)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        int IConvertible.ToInt32(IFormatProvider provider) => (int)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        uint IConvertible.ToUInt32(IFormatProvider provider) => (uint)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        long IConvertible.ToInt64(IFormatProvider provider) => (long)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        ulong IConvertible.ToUInt64(IFormatProvider provider) => (ulong)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        float IConvertible.ToSingle(IFormatProvider provider) => (float)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        double IConvertible.ToDouble(IFormatProvider provider) => (double)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        decimal IConvertible.ToDecimal(IFormatProvider provider) => (decimal)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        bool IConvertible.ToBoolean(IFormatProvider provider) => !isZero;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        char IConvertible.ToChar(IFormatProvider provider) => (char)this;

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => throw new InvalidCastException();

#if UNITY_2020_2_OR_NEWER
        readonly
#endif
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(InfVal)) return this;
            if (conversionType == typeof(BigInteger)) return (BigInteger)this;
            if (conversionType == typeof(string)) return ToString(provider);

            if (conversionType == typeof(byte)) return ((IConvertible)this).ToByte(provider);
            if (conversionType == typeof(sbyte)) return ((IConvertible)this).ToSByte(provider);
            if (conversionType == typeof(short)) return ((IConvertible)this).ToInt16(provider);
            if (conversionType == typeof(ushort)) return ((IConvertible)this).ToUInt16(provider);
            if (conversionType == typeof(int)) return ((IConvertible)this).ToInt32(provider);
            if (conversionType == typeof(uint)) return ((IConvertible)this).ToUInt32(provider);
            if (conversionType == typeof(long)) return ((IConvertible)this).ToInt64(provider);
            if (conversionType == typeof(ulong)) return ((IConvertible)this).ToUInt64(provider);

            if (conversionType == typeof(float)) return ((IConvertible)this).ToSingle(provider);
            if (conversionType == typeof(double)) return ((IConvertible)this).ToDouble(provider);
            if (conversionType == typeof(decimal)) return ((IConvertible)this).ToDecimal(provider);

            if (conversionType == typeof(bool)) return ((IConvertible)this).ToBoolean(provider);
            if (conversionType == typeof(char)) return ((IConvertible)this).ToChar(provider);

            throw new InvalidCastException();
        }
    }
}