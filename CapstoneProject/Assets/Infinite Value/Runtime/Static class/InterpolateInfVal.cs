using UnityEngine;

namespace InfiniteValue
{
    /// <summary> 
    /// Provide different way to interpolate between two <see cref="InfVal"/>.
    /// </summary>
    public static class InterpolateInfVal
    {
        // public methods

        /// <summary> Linearly interpolates between <paramref name="min"/> and <paramref name="max"/> by <paramref name="t"/>. </summary> 
        public static InfVal Linear(in InfVal min, in InfVal max, float t, bool clamped = true)
            => BasicInterpolate(min, max, T_Linear(clamped ? Mathf.Clamp01(t) : t));

        /// <summary> Interpolates between <paramref name="min"/> and <paramref name="max"/> by <paramref name="t"/> with smoothing at the limits. </summary> 
        public static InfVal SmoothStep(in InfVal min, in InfVal max, float t, bool clamped = true)
            => BasicInterpolate(min, max, T_SmoothStep(clamped ? Mathf.Clamp01(t) : t));

        /// <summary> Interpolates between <paramref name="min"/> and <paramref name="max"/> by <paramref name="t"/> with a modifiable smoothing at the beggining. </summary> 
        public static InfVal EaseIn(in InfVal min, in InfVal max, float t, float easePower, bool clamped = true)
            => BasicInterpolate(min, max, T_EaseIn(clamped ? Mathf.Clamp01(t) : t, easePower));

        /// <summary> Interpolates between <paramref name="min"/> and <paramref name="max"/> by <paramref name="t"/> with a modifiable smoothing at the end. </summary> 
        public static InfVal EaseOut(in InfVal min, in InfVal max, float t, float easePower, bool clamped = true)
            => BasicInterpolate(min, max, T_EaseOut(clamped ? Mathf.Clamp01(t) : t, easePower));

        /// <summary> Interpolates between <paramref name="min"/> and <paramref name="max"/> by <paramref name="t"/> with a modifiable smoothing at the limits. </summary> 
        public static InfVal EaseInAndOut(in InfVal min, in InfVal max, float t, float easePower, bool clamped = true)
            => BasicInterpolate(min, max, T_EaseInAndOut(clamped ? Mathf.Clamp01(t) : t, easePower));

        /// <summary> Calculates the linear parameter <paramref name="t"/> that produces the interpolant value within the range [<paramref name="min"/>, <paramref name="max"/>]. </summary> 
        public static float InverseLinear(in InfVal min, in InfVal max, in InfVal value) => (float)((value - min) / (max - min));

        /// <summary> Same as <see cref="Linear(in InfVal, in InfVal, float, bool)"/> but makes sure the values interpolate correctly when they wrap around 360 degrees. </summary> 
        public static InfVal LinearAngle(in InfVal minAngle, in InfVal maxAngle, float t, bool clamped = true)
            => minAngle + (MathInfVal.DeltaAngle(minAngle, maxAngle) * T_Linear(clamped ? Mathf.Clamp01(t) : t));

        /// <summary> Same as <see cref="SmoothStep(in InfVal, in InfVal, float, bool)"/> but makes sure the values interpolate correctly when they wrap around 360 degrees. </summary> 
        public static InfVal SmoothStepAngle(in InfVal minAngle, in InfVal maxAngle, float t, bool clamped = true)
            => minAngle + (MathInfVal.DeltaAngle(minAngle, maxAngle) * T_SmoothStep(clamped ? Mathf.Clamp01(t) : t));

        /// <summary> Same as <see cref="EaseIn(in InfVal, in InfVal, float, float, bool)"/> but makes sure the values interpolate correctly when they wrap around 360 degrees. </summary> 
        public static InfVal EaseInAngle(in InfVal minAngle, in InfVal maxAngle, float t, float easePower, bool clamped = true)
            => minAngle + (MathInfVal.DeltaAngle(minAngle, maxAngle) * T_EaseIn(clamped ? Mathf.Clamp01(t) : t, easePower));

        /// <summary> Same as <see cref="EaseOut(in InfVal, in InfVal, float, float, bool)"/> but makes sure the values interpolate correctly when they wrap around 360 degrees. </summary> 
        public static InfVal EaseOutAngle(in InfVal minAngle, in InfVal maxAngle, float t, float easePower, bool clamped = true)
            => minAngle + (MathInfVal.DeltaAngle(minAngle, maxAngle) * T_EaseOut(clamped ? Mathf.Clamp01(t) : t, easePower));

        /// <summary> Same as <see cref="EaseInAndOut(in InfVal, in InfVal, float, float, bool)"/> but makes sure the values interpolate correctly when they wrap around 360 degrees. </summary> 
        public static InfVal EaseInAndOutAngle(in InfVal minAngle, in InfVal maxAngle, float t, float easePower, bool clamped = true)
            => minAngle + (MathInfVal.DeltaAngle(minAngle, maxAngle) * T_EaseInAndOut(clamped ? Mathf.Clamp01(t) : t, easePower));

        // private methods
        static InfVal BasicInterpolate(in InfVal a, in InfVal b, float t) => (((1 - t) * a) + (t * b));

        static float T_Linear(float t) => t;

        static float T_SmoothStep(float t) => t * t * (3f - 2f * t);

        static float T_EaseIn(float t, float factor) => Mathf.Pow(t, factor);

        static float T_EaseOut(float t, float factor) => 1 - Mathf.Pow(1 - t, factor);

        static float T_EaseInAndOut(float t, float factor) => Mathf.Lerp(Mathf.Pow(t, factor), 1 - (Mathf.Pow(1 - t, factor)), t);
    }
}