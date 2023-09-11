using System.Numerics;
using System.Globalization;
using System;

namespace InfiniteValue
{
    /// <summary> 
    /// An <see cref="InfVal"/> is a value type capable of representing pretty much any value, no matter how big or how small.
    /// </summary>
    public partial struct InfVal
    {
        // factory method

        /// <summary> 
        /// Return a new <see cref="InfVal"/> by providing its fields directly. This is NOT the same as using: 
        /// <code> new InfVal(BigInteger, int) </code>
        /// </summary>
        public static InfVal ManualFactory(in BigInteger digits, int exponent)
        {
            InfVal ret = new InfVal();

            ret.digits = digits;
            ret.s_exponent = exponent;

            return ret;
        }

        // constructors

        /// <summary> 
        /// Initializes a new instance of the <see cref="InfVal"/> structure using another one.
        /// </summary>
        /// <param name="precision"> Optionnaly set the precision of the <see cref="InfVal"/> instance. </param>
        public InfVal(in InfVal value, int? precision = null) : this()
        {
            digits = value.digits;
            s_exponent = value.exponent;

            if (precision.HasValue)
            {
                if (precision.Value <= 0)
                    throw new ArgumentException(cannotBeNegOrZeroStr, nameof(precision));

                this.exponent += this.precision - precision.Value;
            }
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="InfVal"/> structure using a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(in BigInteger value, int? precision = null) : this()
        {
            digits = value;
            s_exponent = 0;

            if (precision.HasValue)
            {
                if (precision.Value <= 0)
                    throw new ArgumentException(cannotBeNegOrZeroStr, nameof(precision));

                this.exponent += this.precision - precision.Value;
            }
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="InfVal"/> structure using a number <see langword="string"/> representation.
        /// This is the same as using <see cref="ParseOrDefault(string)"/>.
        /// </summary>
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(string valStr, int? precision = null) : this()
        {
            SetValueFromString(valStr, Configuration.unitsList, (Configuration.decimalPoints, Configuration.separations,
                Configuration.exponents), false);

            if (precision.HasValue)
            {
                if (precision.Value <= 0)
                    throw new ArgumentException(cannotBeNegOrZeroStr, nameof(precision));

                this.exponent += this.precision - precision.Value;
            }
        }

        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="sbyte"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(sbyte value, int? precision = null) : this((BigInteger)value, precision) { }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="byte"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(byte value, int? precision = null) : this((BigInteger)value, precision) { }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="short"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(short value, int? precision = null) : this((BigInteger)value, precision) { }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="ushort"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(ushort value, int? precision = null) : this((BigInteger)value, precision) { }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="int"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(int value, int? precision = null) : this((BigInteger)value, precision) { }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="uint"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(uint value, int? precision = null) : this((BigInteger)value, precision) { }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="long"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(long value, int? precision = null) : this((BigInteger)value, precision) { }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="ulong"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(ulong value, int? precision = null) : this((BigInteger)value, precision) { }

        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="float"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(float value, int? precision = null) : this()
        {
            SetValueFromString(value.ToString("G9", CultureInfo.InvariantCulture), null,
                (new string[] { CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator },
                 new string[] { CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator },
                 Configuration.cultureExponents), false);

            if (precision.HasValue)
            {
                if (precision.Value <= 0)
                    throw new ArgumentException(cannotBeNegOrZeroStr, nameof(precision));

                this.exponent += this.precision - precision.Value;
            }
            else
                this.exponent += this.precision - 9;
        }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="double"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(double value, int? precision = null) : this()
        {
            SetValueFromString(value.ToString("G17", CultureInfo.InvariantCulture), null,
                (new string[] { CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator },
                 new string[] { CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator },
                 Configuration.cultureExponents), false);

            if (precision.HasValue)
            {
                if (precision.Value <= 0)
                    throw new ArgumentException(cannotBeNegOrZeroStr, nameof(precision));

                this.exponent += this.precision - precision.Value;
            }
            else
                this.exponent += this.precision - 17;
        }
        /// <summary> Initializes a new instance of the <see cref="InfVal"/> structure using a <see langword="decimal"/>. </summary> 
        /// <param name="precision"> <inheritdoc cref="InfVal(in InfVal, int?)" path="/param[@name='precision']"/> </param>
        public InfVal(decimal value, int? precision = null) : this()
        {
            SetValueFromString(value.ToString(CultureInfo.InvariantCulture), null,
                (new string[] { CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator },
                 new string[] { CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator },
                 Configuration.cultureExponents), false);

            if (precision.HasValue)
            {
                if (precision.Value <= 0)
                    throw new ArgumentException(cannotBeNegOrZeroStr, nameof(precision));

                this.exponent += this.precision - precision.Value;
            }
            else
                this.exponent += this.precision - 29;
        }

        // implicit/explicit cast to InfVal
        public static implicit operator InfVal((InfVal value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);

        public static implicit operator InfVal(in BigInteger value) => new InfVal(value);
        public static implicit operator InfVal((BigInteger value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);

        public static explicit operator InfVal(string valStr) => new InfVal(valStr);
        public static explicit operator InfVal((string valStr, int precision) tuple) => new InfVal(tuple.valStr, tuple.precision);

        public static implicit operator InfVal(sbyte value) => new InfVal(value);
        public static implicit operator InfVal(byte value) => new InfVal(value);
        public static implicit operator InfVal(short value) => new InfVal(value);
        public static implicit operator InfVal(ushort value) => new InfVal(value);
        public static implicit operator InfVal(int value) => new InfVal(value);
        public static implicit operator InfVal(uint value) => new InfVal(value);
        public static implicit operator InfVal(long value) => new InfVal(value);
        public static implicit operator InfVal(ulong value) => new InfVal(value);

        public static implicit operator InfVal((sbyte value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((byte value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((short value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((ushort value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((int value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((uint value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((long value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((ulong value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);

        public static implicit operator InfVal(float value) => new InfVal(value);
        public static implicit operator InfVal(double value) => new InfVal(value);
        public static implicit operator InfVal(decimal value) => new InfVal(value);

        public static implicit operator InfVal((float value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((double value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);
        public static implicit operator InfVal((decimal value, int precision) tuple) => new InfVal(tuple.value, tuple.precision);

        // explicit cast from InfVal
        public static explicit operator BigInteger(in InfVal iv) => (iv.ToExponent(0).digits);

        public static explicit operator sbyte(in InfVal iv) => (sbyte)(BigInteger)iv;
        public static explicit operator byte(in InfVal iv) => (byte)(BigInteger)iv;
        public static explicit operator short(in InfVal iv) => (short)(BigInteger)iv;
        public static explicit operator ushort(in InfVal iv) => (ushort)(BigInteger)iv;
        public static explicit operator int(in InfVal iv) => (int)(BigInteger)iv;
        public static explicit operator uint(in InfVal iv) => (uint)(BigInteger)iv;
        public static explicit operator long(in InfVal iv) => (long)(BigInteger)iv;
        public static explicit operator ulong(in InfVal iv) => (ulong)(BigInteger)iv;

        public static explicit operator float(in InfVal iv) =>
            float.TryParse(iv.ToString(0, null, DisplayOption.None, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out float res)
                ? res
                : 0f;
        public static explicit operator double(in InfVal iv) =>
            double.TryParse(iv.ToString(0, null, DisplayOption.None, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out double res)
                ? res
                : 0;
        public static explicit operator decimal(in InfVal iv) =>
            decimal.TryParse(iv.ToString(0, null, DisplayOption.None, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal res)
                ? res
                : 0m;
    }
}
