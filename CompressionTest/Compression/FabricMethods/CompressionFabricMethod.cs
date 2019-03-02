using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Enums;

namespace CompressionTest.Compression.FabricMethods
{
    static class CompressionFabricMethod
    {
        public static Strategy.Compression Create(CompressionAlgorithms algorithmType)
        {
            switch (algorithmType)
            {
                case CompressionAlgorithms.Gzip:
                    return new Strategy.Compression(
                        new Algorithms.GZipCompression(), 
                        algorithmType, 
                        Utils.MagicNumbersAlgorithmDictionary.AlgorithmMagicNumbers[algorithmType]);
                /*
                case CompressionAlgorithms.Deflate:
                    return new Strategy.Compression(new Algorithms.DeflateCompression(), algorithmType);
                    */

            }

            return null;
        }
    }
}
