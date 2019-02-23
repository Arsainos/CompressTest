using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Interfaces;
using System.IO.Compression;
using System.IO;

namespace CompressionTest.Compression.Algorithms
{
    class DeflateCompression : ICompressionStrategy
    {
        public byte[] Compress(byte[] input)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    deflateStream.Write(input, 0, input.Length);
                }

                return memoryStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] input)
        {
            using (var zipStream = new DeflateStream(new MemoryStream(input), CompressionMode.Decompress))
            {
                const int block = 4096;
                byte[] buffer = new byte[block];

                using (var memoryStream = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = zipStream.Read(buffer, 0, block);
                        if (count > 0)
                        {
                            memoryStream.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
