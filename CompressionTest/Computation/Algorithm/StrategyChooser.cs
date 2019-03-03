using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Algorithm
{
    /// <summary>
    /// Strategy for algorithm realization
    /// </summary>
    /// <remarks>
    /// We need to pass func of algorithm for component in system
    /// </remarks>
    static class StrategyChooser
    {
        /// <summary>
        /// Method to call the func of specific algorithm
        /// </summary>
        /// <param name="inputData">payload for algorithm</param>
        /// <param name="Algorithm">func to call</param>
        /// <returns></returns>
        public static Data.Structures.StrategyResult FindMostRelevantComponent(object[] inputData,
            Func<object[], Data.Structures.StrategyResult> Algorithm)
        {
            return Algorithm(inputData);
        }
    }
}
