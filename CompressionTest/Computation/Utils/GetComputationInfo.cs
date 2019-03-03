using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Utils
{
    /// <summary>
    /// Special classes for getting some computation info
    /// </summary>
    static class GetComputationInfo
    {
        public static double GetMaximumChunks(double chunkSize)
        {
            double RamFreeSize = Computation.Utils.ResourceCheck.checkRamSize() * 0.9; 
            return Math.Ceiling(RamFreeSize / chunkSize);
        }

        public static double GetNumberOfChunksForFile(double chunkSize, double objectSize)
        {
            return Math.Ceiling(objectSize / chunkSize);
        }
    }
}
