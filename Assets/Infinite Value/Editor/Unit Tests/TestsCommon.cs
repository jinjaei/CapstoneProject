using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InfiniteValue
{
    /// Utility class used by all of the unit tests scripts.
    static class TestsCommon
    {
        // consts
        public const string exceptionPrefix = "Exception: ";

        public const int maxNumberOfFailedResults = 250;

        public static readonly Color failColor = Color.red;
        public static readonly Color extraCharColor = new Color(1f, 0.5f, 0);

        public static readonly IReadOnlyList<string> specialNumberStr = new List<string> { "NaN", "Infinity", "-Infinity", };

        // methods
        public static bool IsValidNumber(string nbStr) => !specialNumberStr.Contains(nbStr) && !nbStr.Contains(exceptionPrefix);
    }
}