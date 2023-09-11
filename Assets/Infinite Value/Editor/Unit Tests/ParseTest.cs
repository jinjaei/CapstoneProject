using System.Globalization;

namespace InfiniteValue
{
    class ParseTest : AUnitTest
    {
        public override string description => "This test will create a random value, use it's ToString representation as an argument for a new InfVal, " +
            "and then check if both value correspond.\n" +
            "It will ignore cases where it failed because of a failed cast and not because of the parsing method.";

        public override void DrawParameters()
        {
            D_VarTypeField();
            D_IterationsField();
        }

        public override TestResult Process(ref float threadProgressRatio)
        {
            TestResult res = new TestResult(iterations);

            for (long i = 0; i < iterations; i++)
            {
                dynamic val = P_CreateRandomValue();

                if (!P_TryCastValue(val, out InfVal _, out string valToString))
                    --res.usedIterations;
                else
                    res.SubscribeResult(valToString, P_ConvertInfValToTypeToString(InfVal.ParseOrDefault(valToString, null, CultureInfo.InvariantCulture)));

                threadProgressRatio = ((float)i + 1) / iterations;
            }

            return res;
        }
    }
}
