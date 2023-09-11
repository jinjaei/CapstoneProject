namespace InfiniteValue
{
    class ToStringTest : AUnitTest
    {
        public override string description => "This test will create a random value, cast it to an InfVal, " +
            "and then check if both value ToString methods give the same result.\n" +
            "It will ignore cases where it failed because of a failed cast and not because of the ToString method.\n" +
            "The InfVal ToString will be converted to a default like representation " +
            "('e' replaced with 'E' and an added '0' at the beggining of the exponent if needed)";

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

                if (!P_TryCastValue(val, out InfVal iv, out string valToString))
                    --res.usedIterations;
                else
                    res.SubscribeResult(valToString, P_InfValToSystemLikeString(iv, P_CountDigits(valToString)));

                threadProgressRatio = ((float)i + 1) / iterations;
            }

            return res;
        }
    }
}
