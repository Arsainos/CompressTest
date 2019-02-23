using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Interfaces;

namespace CompressionTest.Compression.Algorithms
{
    class GZipCompression : ICompressionStrategy
    {
        public byte[] Compress(byte[] input)
        {
            throw new NotImplementedException();
        }

        public byte Decompress(byte[] input)
        {
            throw new NotImplementedException();
        }
    }
}
