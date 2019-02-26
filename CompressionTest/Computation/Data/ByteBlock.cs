using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Data
{
    public class ByteBlock
    {
        public byte[] Slice;
        public int blockNumber;

        public ByteBlock(byte[] slice, int blockNumber)
        {
            this.Slice = slice;
            this.blockNumber = blockNumber;
        }
    }
}
