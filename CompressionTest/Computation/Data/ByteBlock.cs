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
        public bool lastSubset;

        public ByteBlock(byte[] slice, int blockNumber,bool lastSubset = false)
        {
            this.Slice = slice;
            this.blockNumber = blockNumber;
            this.lastSubset = lastSubset;
        }
    }
}
