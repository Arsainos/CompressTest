using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Algorithm
{
    static class StrategyChooser
    {
        public static Computation.Enums.ComputationType FindMostRelevantComponent(List<double[]> inputData,
            Func<List<double[]>,Computation.Enums.ComputationType> Algorithm)
        {
            if(inputData.Count>0)
            {
                return Algorithm(inputData);
            }
            else
            {
                throw new Exception("В функцию вычисления подходящего компонента не были переданы данные!");
            }
        }
    }
}
