using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Compression.Enums
{
    /// <summary>
    /// Compression algorithms enum
    /// </summary>
    public enum CompressionAlgorithms : int
    {
        Gzip = 0,
        Deflate = 1
    }

    /// <summary>
    /// Compression type enum
    /// </summary>
    public enum CompressionType : int
    {
        Compress = 0,
        Decompress = 1
    }
}
