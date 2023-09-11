using System.Collections.Generic;
using System;

namespace IV_Demo
{
    public static class OnValidateUtility
    {
        public static arrayT[] EnforceConstantEnumArray<enumT, arrayT>(arrayT[] array, Func<arrayT, string> getEnumStrFunc,
            Func<arrayT, string, arrayT> setElemToEnumStrFunc, int enumCount = -1)
        {
            if (array == null)
                array = new arrayT[0];

            string[] enumNames = Enum.GetNames(typeof(enumT));

            if (enumCount <= 0)
                enumCount = enumNames.Length;

            Dictionary<string, arrayT> enumStrToElemDico = new Dictionary<string, arrayT>();

            foreach (arrayT elem in array)
            {
                string enumStr = getEnumStrFunc.Invoke(elem);

                if (!string.IsNullOrEmpty(enumStr) && !enumStrToElemDico.ContainsKey(enumStr))
                    enumStrToElemDico.Add(enumStr, elem);
            }

            if (array.Length != enumCount)
                Array.Resize(ref array, enumCount);

            for (int i = 0; i < enumCount; i++)
            {
                string enumStr = Enum.GetName(typeof(enumT), (enumT)(object)i);

                if (enumStrToElemDico.ContainsKey(enumStr))
                    array[i] = enumStrToElemDico[enumStr];

                array[i] = setElemToEnumStrFunc(array[i], enumStr);
            }

            return array;
        }
    }
}