using System.Collections.Generic;

namespace InfiniteValue
{
    /// Class providing information about the result of a test.
    class TestResult
    {
        public List<OneFailedResult> failedResultsList = new List<OneFailedResult>();
        public long extraFailedResults;
        public long usedIterations;
        public double perFailCharSuccess;

        public TestResult(long usedIterations)
        {
            this.usedIterations = usedIterations;
        }

        public void Deconstruct(out List<OneFailedResult> failedResultsList, out long extraFailedResults, out long usedIterations, out double perFailCharSuccess)
        {
            failedResultsList = this.failedResultsList;
            extraFailedResults = this.extraFailedResults;
            usedIterations = this.usedIterations;
            perFailCharSuccess = this.perFailCharSuccess;
        }

        public void SubscribeResult(string primitiveResult, string infValResult, string primitiveTooltip = null, string infValTooltip = null)
        {
            if (primitiveResult == infValResult || (primitiveResult.Contains(TestsCommon.exceptionPrefix) && infValResult.Contains(TestsCommon.exceptionPrefix)))
                return;

            if (failedResultsList.Count == 0)
                failedResultsList.Add(OneFailedResult.Header());

            if (failedResultsList.Count > TestsCommon.maxNumberOfFailedResults)
                ++extraFailedResults;
            else
            {
                double successPercent;
                failedResultsList.Add(OneFailedResult.New((primitiveResult, primitiveTooltip), (infValResult, infValTooltip), out successPercent));
                perFailCharSuccess += successPercent;
            }
        }
    }
}