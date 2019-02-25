using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Algorithm.Realizations
{
    static class LowestIdleTime
    {
        public static Data.Structures.StrategyResult Algorithm(object[] inputData)
        {
            var CPUs = Utils.ResourceCheck.checkCPUUsage();
            var Disk = Utils.ResourceCheck.checkDiskUsage((string)inputData[0]);

            double lowest = CPUs[0];
            int objectType = 0;
            object info = null;
            for(int i=1; i<CPUs.Length;i++)
            {
                if (CPUs[i] < lowest)
                {
                    lowest = CPUs[i];
                    info = i;
                }
            }

            if(Disk < lowest)
            {
                lowest = Disk;
                objectType = 1;
            }

            return new Data.Structures.StrategyResult((Enums.ComputationType)objectType, info);
        }
    }
}
