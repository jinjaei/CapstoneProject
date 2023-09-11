using System.Linq;
using UnityEngine;
using System;

namespace IV_Demo
{
    public static class CustomRandom
    {
        public static T Ponderated<T>(T[] elemsArray, float[] ponderationsArray)
        {
            if (elemsArray.Length != ponderationsArray.Length)
                throw new ArgumentException("Tried to use a PonderateRandom() with two different size arrays");

            float sum = ponderationsArray.Sum();

            if (sum <= 0)
                throw new ArgumentException("Tried to use a PonderateRandom() with no ponderations");

            float rand = UnityEngine.Random.value * sum;

            for (int i = 0; i < elemsArray.Length; i++)
            {
                if (ponderationsArray[i] > 0 && rand <= ponderationsArray[i])
                    return elemsArray[i];
                rand -= ponderationsArray[i];
            }

            throw new Exception("PonderateRandom() didn't find any result");
        }
    }
}