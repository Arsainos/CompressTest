using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Compression.Interfaces
{
    interface ICompressionStrategy
    {
        byte[] Compress(byte[] input);
        byte[] Decompress(byte[] input);
    }
}
