using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Algorithm.Realizations
{
    /// <summary>
    /// Realization for how to find lowest idle component in system
    /// </summary>
    /// <remarks>
    /// We are using info from Utils in this component to get some calculation about CPU core idle time and disk idle time
    /// </remarks>
    static class LowestIdleTime
    {
        /// <summary>
        /// Get strategy result for this algorithm computation
        /// </summary>
        /// <param name="inputData">some payload for this method - just disk name</param>
        /// <returns></returns>
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
