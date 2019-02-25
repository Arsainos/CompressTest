using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Data
{
    public enum DataSource : int
    {
        File = 0
    }

    public enum ProviderType : int
    {
        Block = 0
    }

    public enum DirectionType : int
    {
        In = 0,
        Out = 1,
        InOut = 2
    }
}
