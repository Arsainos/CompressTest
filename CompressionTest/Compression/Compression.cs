using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Interfaces;

namespace CompressionTest.Compression
{
    class Compression : ICompressionStrategy
    {
        public ICompressionStrategy CompressionStrategy { private get; set; }

        public Compression(ICompressionStrategy compressionStrategy)
        {
            CompressionStrategy = compressionStrategy;
        }

        public byte[] Compress(byte[] input)
        {
            return CompressionStrategy.Compress(input);
        }

        public byte Decompress(byte[] input)
        {
            return CompressionStrategy.Decompress(input);
        }
    }
}
