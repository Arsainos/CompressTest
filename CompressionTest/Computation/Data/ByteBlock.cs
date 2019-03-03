using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Data
{
    /// <summary>
    /// Data block proccessing
    /// </summary>
    public class ByteBlock
    {
        /// <summary>
        /// Slice of transfering bytes
        /// </summary>
        public byte[] Slice;
        /// <summary>
        /// current block id
        /// </summary>
        public int blockNumber;
        /// <summary>
        /// is this last readed data?
        /// </summary>
        public bool lastSubset;

        public ByteBlock(byte[] slice, int blockNumber,bool lastSubset = false)
        {
            this.Slice = slice;
            this.blockNumber = blockNumber;
            this.lastSubset = lastSubset;
        }
    }
}
