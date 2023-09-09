using UnityEngine;

namespace IV_Demo
{
    public static class MinMaxExtension
    {
        /// <summary>Return a random float number between min [inclusive] and max [inclusive].</summary>
        public static float RandomRange(this MinMax minMax) => Random.Range(minMax.min, minMax.max);

        /// <summary>Clamp the given value between min [inclusive] and max [inclusive].</summary>
        public static float Clamp(this MinMax minMax, float val) => Mathf.Clamp(val, minMax.min, minMax.max);

        /// <summary>Clamp the given int value between min [inclusive] and max [inclusive].</summary>
        public static int Clamp(this MinMax minMax, int val) => Mathf.Clamp(val, (int)minMax.min, (int)minMax.max);

        /// <summary>Linearly interpolate between min and max.</summary>
        public static float Lerp(this MinMax minMax, float t) => Mathf.Lerp(minMax.min, minMax.max, t);

        /// <summary>Calculates the linear parameter t that produce the interpolant value within the range [min, max].</summary>
        public static float InverseLerp(this MinMax minMax, float value) => Mathf.InverseLerp(minMax.min, minMax.max, value);

        /// <summary>Return the minMax struct with a modified Max value.</summary>
        public static MinMax Max(this MinMax minMax, float value) => new MinMax(minMax.min, value, minMax.minInferiorToMax);

        /// <summary>Return the minMax struct with a modified Min value.</summary>
        public static MinMax Min(this MinMax minMax, float value) => new MinMax(value, minMax.max, minMax.minInferiorToMax);
    }
}