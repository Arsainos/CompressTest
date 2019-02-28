using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Compression.Enums
{
    public enum CompressionAlgorithms : int
    {
        Gzip = 0,
        Deflate = 1
    }

    public enum CompressionType : int
    {
        Compress = 0,
        Decompress = 1
    }
}
