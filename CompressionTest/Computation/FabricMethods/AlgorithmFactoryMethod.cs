using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.FabricMethods
{
    /// <summary>
    /// Factory to get algorithm
    /// </summary>
    static class AlgorithmFactoryMethod
    {
        public static Func<object[],Data.Structures.StrategyResult> GetFunc(Enums.Algorithms algorithm)
        {
            switch(algorithm)
            {
                case Enums.Algorithms.LowestIdleTime:
                    return Algorithm.Realizations.LowestIdleTime.Algorithm;
            }

            throw new Exception(String.Format("Can't get instance of algorithm class in AlgorithmFactoryMethod"));
        }
    }
}
