using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Enums
{
    /// <summary>
    /// Data source enum
    /// </summary>
    public enum DataSource : int
    {
        File = 0
    }
    /// <summary>
    /// Provider type enum
    /// </summary>
    public enum ProviderType : int
    {
        Block = 0
    }
    /// <summary>
    /// Direction Type enum
    /// </summary>
    public enum DirectionType : int
    {
        In = 0,
        Out = 1,
        InOut = 2
    }
}
