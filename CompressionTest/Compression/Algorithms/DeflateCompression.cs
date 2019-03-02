using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Interfaces;
using System.IO.Compression;
using System.IO;

namespace CompressionTest.Compression.Algorithms
{
    /// <summary>
    /// Deflate algorithm realization class
    /// </summary>
    /// <remarks>
    /// This class is used as a strategy for higher level class - Compression.
    /// </remarks>
    class DeflateCompression : ICompressionStrategy
    {
        /// <summary>
        /// Deflate compress method
        /// </summary>
        /// <param name="input">Slice of bytes</param>
        /// <returns>Compressed slice of bytes</returns>
        public byte[] Compress(byte[] input)
        {
            //Create a memory stream to fix some data in memory
            using (var memoryStream = new MemoryStream())
            {
                //wrap memory stream with deflate stream
                using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    //write to memory stream
                    deflateStream.Write(input, 0, input.Length);
                }
                //return compressed slice of bytes
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Deflate decompress method
        /// </summary>
        /// <param name="input">Slice of bytes</param>
        /// <returns>Decompressed slice of bytes</returns>
        public byte[] Decompress(byte[] input)
        {
            //Create a deflate stream with memory stream underlaying, our input slice of bytes is pinned to that memory stream
            using (var zipStream = new DeflateStream(new MemoryStream(input), CompressionMode.Decompress))
            {
                //making the anonymous block, cause we don't know the original block size
                const int block = 4096;
                byte[] buffer = new byte[block];

                //create a stream to store a decompressed data
                using (var memoryStream = new MemoryStream())
                {
                    int count = 0;
                    //let's try to read until the end of buffer
                    do
                    {
                        count = zipStream.Read(buffer, 0, block);
                        if (count > 0)
                        {
                            memoryStream.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    // return decompressed slice of bytes
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
