namespace InfiniteValue
{
    class CastTest : AUnitTest
    {
        public override string description => "This test will try casting a primitive value instance to an InfVal and then back to it's original type.";

        public override void DrawParameters()
        {
            D_VarTypeField();
            D_IgnoreSpecialValues();
            D_IterationsField();
        }

        public override TestResult Process(ref float threadProgressRatio)
        {
            TestResult res = new TestResult(iterations);

            for (long i = 0; i < iterations; i++)
            {
                dynamic val = P_CreateRandomValue();

                res.SubscribeResult(P_ValToString(val), P_ConvertInfValToTypeToString(new InfVal(val)));

                threadProgressRatio = ((float)i + 1) / iterations;
            }

            return res;
        }
    }
}
