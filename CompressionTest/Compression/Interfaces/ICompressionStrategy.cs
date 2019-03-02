using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Compression.Interfaces
{
    /// <summary>
    /// Interface for compression strategy
    /// </summary>
    /// <remarks>
    /// This interface is made for compression strategy. By using it we can implement classes with compress and decompress methods
    /// and then use them as strategy fo higher class - compression
    /// </remarks>
    interface ICompressionStrategy
    {
        /// <summary>
        /// Interface method for compress realization
        /// </summary>
        /// <param name="input">Input slice of bytes</param>
        /// <returns>Compressed slice of bytes</returns>
        byte[] Compress(byte[] input);
        /// <summary>
        /// Interface method for decompress realization
        /// </summary>
        /// <param name="input">Input slice of bytes</param>
        /// <returns>Decompressed slice of bytes</returns>
        byte[] Decompress(byte[] input);
    }
}
