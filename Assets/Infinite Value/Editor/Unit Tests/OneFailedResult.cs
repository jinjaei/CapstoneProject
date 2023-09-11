using System;
using System.Text;
using UnityEngine;

namespace InfiniteValue
{
    /// Class providing information about a single failed result.
    class OneFailedResult
    {
        static readonly string failColorStr = $"<color=#{ColorUtility.ToHtmlStringRGB(TestsCommon.failColor)}>";
        static readonly string extraColorStr = $"<color=#{ColorUtility.ToHtmlStringRGB(TestsCommon.extraCharColor)}>";
        static readonly string endColorStr = "</color>";

        static readonly StringBuilder strBuilder1 = new StringBuilder();
        static readonly StringBuilder strBuilder2 = new StringBuilder();

        public GUIContent primitiveResult { get; private set; }
        public GUIContent infValResult { get; private set; }

        OneFailedResult(string primitiveResult, string infValResult)
        {
            this.primitiveResult = new GUIContent(primitiveResult);
            this.infValResult = new GUIContent(infValResult);
        }

        public static OneFailedResult Header() => new OneFailedResult("<b>Primitive results</b>", "<b>InfVal results</b>");

        public static OneFailedResult New((string result, string tooltip) primitive, (string result, string tooltip) infVal, out double successRatio)
        {
            if (!TestsCommon.IsValidNumber(primitive.result) || !TestsCommon.IsValidNumber(infVal.result))
            {
                successRatio = 0;
                return new OneFailedResult($"{failColorStr}{primitive.result}{endColorStr}", $"{failColorStr}{infVal.result}{endColorStr}");
            }

            double charSuccess = 0;
            strBuilder1.Clear();
            strBuilder1.Append(primitive.result);
            strBuilder2.Clear();
            strBuilder2.Append(infVal.result);

            int c1 = 0;
            int c2 = 0;
            // check each char, color to fail if they differ otherwise add to charSuccess
            while (c1 < strBuilder1.Length && c2 < strBuilder2.Length)
            {
                if (char.IsDigit(strBuilder1[c1]) && !char.IsDigit(strBuilder2[c2]))
                {
                    strBuilder1.Insert(c1, extraColorStr);
                    strBuilder1.Insert(c1 + extraColorStr.Length + 1, endColorStr);

                    c1 += ($"{extraColorStr}{endColorStr}").Length + 1;
                }
                else if (!char.IsDigit(strBuilder1[c1]) && char.IsDigit(strBuilder2[c2]))
                {
                    strBuilder2.Insert(c2, extraColorStr);
                    strBuilder2.Insert(c2 + extraColorStr.Length + 1, endColorStr);

                    c2 += ($"{extraColorStr}{endColorStr}").Length + 1;
                }
                else
                {
                    if (strBuilder1[c1] != strBuilder2[c2])
                    {
                        strBuilder1.Insert(c1, failColorStr);
                        strBuilder1.Insert(c1 + failColorStr.Length + 1, endColorStr);
                        strBuilder2.Insert(c2, failColorStr);
                        strBuilder2.Insert(c2 + failColorStr.Length + 1, endColorStr);

                        c1 += ($"{failColorStr}{endColorStr}").Length + 1;
                        c2 += ($"{failColorStr}{endColorStr}").Length + 1;
                    }
                    else
                    {
                        charSuccess += 1;
                        ++c1;
                        ++c2;
                    }
                }
            }

            // color extra chars
            if (c1 < strBuilder1.Length)
            {
                strBuilder1.Insert(c1, extraColorStr);
                strBuilder1.Append(endColorStr);
            }
            if (c2 < strBuilder2.Length)
            {
                strBuilder2.Insert(c2, extraColorStr);
                strBuilder2.Append(endColorStr);
            }

            successRatio = charSuccess / Math.Max(primitive.result.Length, infVal.result.Length);

            OneFailedResult ret = new OneFailedResult(strBuilder1.ToString(), strBuilder2.ToString());
            ret.primitiveResult.tooltip = primitive.tooltip;
            ret.infValResult.tooltip = infVal.tooltip;

            return ret;
        }
    }
}
