using System;
using UnityEngine;

namespace IV_Demo
{
    [Serializable] public struct MinMax
    {
        [SerializeField] float _min;
        public float min { get { return _min; } set { _min = value; if (minInferiorToMax && _min > max) _max = min; } }

        [SerializeField] float _max;
        public float max { get { return _max; } set { _max = value; if (minInferiorToMax && _max < min) _min = max; } }

        [SerializeField] public bool minInferiorToMax;

        public MinMax(float min, float max, bool minInferiorToMax = true) : this()
        {
            this.minInferiorToMax = minInferiorToMax;
            this.min = min;
            this.max = max;
        }
    }
}