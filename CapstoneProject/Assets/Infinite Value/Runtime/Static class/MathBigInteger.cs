using System;
using System.Numerics;

namespace InfiniteValue
{
    /// Utility class that provide math methods for BigInteger that doesn't exists in the standard library.
    static class MathBigInteger
    {
        // found on https://stackoverflow.com/questions/17357760/how-can-i-generate-a-random-biginteger-within-a-certain-range/48855115
        public static BigInteger RandomInRange(BigInteger min, BigInteger max)
        {
            if (min > max)
            {
                var buff = min;
                min = max;
                max = buff;
            }

            // offset to set min = 0
            BigInteger offset = -min;
            min = 0;
            max += offset;

            BigInteger value;
            var bytes = max.ToByteArray();

            // count how many bits of the most significant byte are 0
            // NOTE: sign bit is always 0 because max must always be positive
            byte zeroBitsMask = 0b00000000;

            var mostSignificantByte = bytes[bytes.Length - 1];

            // we try to set to 0 as many bits as there are in the most significant byte, starting from the left (most significant bits first)
            // NOTE: i starts from 7 because the sign bit is always 0
            for (var i = 7; i >= 0; i--)
            {
                // we keep iterating until we find the most significant non-0 bit
                if ((mostSignificantByte & (0b1 << i)) != 0)
                {
                    var zeroBits = 7 - i;
                    zeroBitsMask = (byte)(0b11111111 >> zeroBits);
                    break;
                }
            }

            Random rng = new Random();
            do
            {
                rng.NextBytes(bytes);

                // set most significant bits to 0 (because value > max if any of these bits is 1)
                bytes[bytes.Length - 1] &= zeroBitsMask;

                value = new BigInteger(bytes);

                // value > max 50% of the times, in which case the fastest way to keep the distribution uniform is to try again
            } while (value > max);

            return value;
        }

        // found on https://stackoverflow.com/questions/3432412/calculate-square-root-of-a-biginteger-system-numerics-biginteger
        public static BigInteger Sqrt(BigInteger value)
        {
            int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(value, 2)));
            BigInteger root = BigInteger.One << (bitLength >> 1);

            while (!isSqrt(value, root))
            {
                root += value / root;
                root >>= 1;
            }

            return root;

            bool isSqrt(BigInteger bi, BigInteger r)
            {
                BigInteger lowerBound = r * r;
                BigInteger upperBound = (r + 1) * (r + 1);

                return (bi >= lowerBound && bi < upperBound);
            }
        }

        public static BigInteger NthRoot(BigInteger value, int n)
        {
            if (n == 1)
                return value;

            BigInteger high = 1;
            while (BigInteger.Pow(high, n) < value)
                high <<= 1;
            BigInteger low = high >> 1;

            while (high - low > 1)
            {
                BigInteger mid = (low + high) / 2;
                BigInteger midToValue = BigInteger.Pow(mid, n);

                if (midToValue < value)
                    low = mid;
                else if (midToValue > value)
                    high = mid;
                else
                    return mid;
            }

            if (BigInteger.Pow(high, n) == value)
                return high;
            return low;
        }

        public static BigInteger MultiplyByPowerOf10(in BigInteger value, int power)
        {
            if (power == 0)
                return value;
            if (power > 0)
                return value * BigInteger.Pow(10, power);
            // (pow < 0)
            return value / BigInteger.Pow(10, -power);
        }
    }
}