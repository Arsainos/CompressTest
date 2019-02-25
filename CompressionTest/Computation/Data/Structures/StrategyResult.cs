using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Data.Structures
{
    
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
