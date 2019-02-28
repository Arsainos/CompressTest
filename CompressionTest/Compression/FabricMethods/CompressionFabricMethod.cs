using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Enums;

namespace CompressionTest.Compression
{
    static class CompressionFabricMethod
    {
        public static Compression Create(CompressionAlgorithms algorithmType)
        {
            switch (algorithmType)
            {
                case CompressionAlgorithms.Gzip:
                    return new Compression(new Algorithms.GZipCompression());

                case CompressionAlgorithms.Deflate:
                    return new Compression(new Algorithms.DeflateCompression());

            }

            return null;
        }
    }
}
