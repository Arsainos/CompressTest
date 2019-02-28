using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Utils
{
    static class GetComputationInfo
    {
        public static double GetMaximumChunks(double chunkSize)
        {
            double RamFreeSize = Computation.Utils.ResourceCheck.checkRamSize() * 0.9; // Чтобы не всю оперативную память занять,
                                                                                       // оставим 10% для жизни на Марсе
            return Math.Ceiling(RamFreeSize / chunkSize);
        }

        public static double GetNumberOfChunksForFile(double chunkSize, double objectSize)
        {
            return Math.Ceiling(objectSize / chunkSize);
        }
    }
}
