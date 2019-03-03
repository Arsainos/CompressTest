using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Data.Structures
{
    /// <summary>
    /// Structure for algorithm streategy computation realizaion
    /// </summary>
    /// <remarks>
    /// Get the computation type, such as disk, cpu cores or etc. and some specific info for this type
    /// </remarks>
    public struct StrategyResult
    {

        public Enums.ComputationType ComputationType;
        public object AdditionalInfo;

        public StrategyResult(Enums.ComputationType computationType, object additionalInfo)
        {
            this.ComputationType = computationType;
            this.AdditionalInfo = additionalInfo;
        }
    }
}
