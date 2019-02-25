﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.FabricMethods
{
    static class AlgorithmFactoryMethod
    {
        public static Func<List<double[]>,Enums.ComputationType> GetFunc(Enums.Algorithms algorithm)
        {
            switch(algorithm)
            {
                case Enums.Algorithms.LowestIdleTime:
                    return Algorithm.Realizations.LowestIdleTime.Algorithm;
            }

            throw new Exception(String.Format("Ошибка возвращения объекта из фабрики - AlgorithmFactoryMethod"));
        }
    }
}
