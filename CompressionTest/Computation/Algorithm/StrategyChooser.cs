using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Algorithm
{
    static class StrategyChooser
    {
        public static Data.Structures.StrategyResult FindMostRelevantComponent(object[] inputData,
            Func<object[], Data.Structures.StrategyResult> Algorithm)
        {
            return Algorithm(inputData);
        }
    }
}
