using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Enums;

namespace CompressionTest.Compression.FabricMethods
{
    /// <summary>
    /// Fabric method for compression object generation
    /// </summary>
    /// <remarks>
    /// This factory method made for easier way to get specific class for strategy
    /// </remarks>
    static class CompressionFabricMethod
    {
        /// <summary>
        /// Generate specific compression class
        /// </summary>
        /// <param name="algorithmType">Get specific class by algorithm enum</param>
        /// <returns></returns>
        public static Strategy.Compression Create(CompressionAlgorithms algorithmType)
        {
            //switch for algorithm
            switch (algorithmType)
            {
                //case for getting gzip class
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
