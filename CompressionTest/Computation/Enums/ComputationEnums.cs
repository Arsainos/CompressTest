using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Enums
{
    /// <summary>
    /// Computation type enum
    /// </summary>
    public enum ComputationType : int
    {
        Disk = 0,
        CPU = 1
    }

    /// <summary>
    /// Algorithm enum
    /// </summary>
    public enum Algorithms : int
    {
        LowestIdleTime = 0
    }
}
