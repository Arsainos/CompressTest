using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Compression.Data.Structure
{
    /// <summary>
    /// Special class for store additional data about compression algorithms specifications
    /// </summary>
    class CompressingSpecificationStructure
    {
        /// <summary>
        /// Magic Number to detect specific algorithm in binary header
        /// </summary>
        public byte[] magicNumber { get; private set; }
        /// <summary>
        /// Length of mask after magic number and before the data
        /// </summary>
        public int maskLength { get; private set; }
        /// <summary>
        /// Complete mask for lookup
        /// </summary>
        public byte[] completeMask {get;set;}

        public CompressingSpecificationStructure(byte[] magicNumber, int mask)
        {
            this.magicNumber = magicNumber;
            this.maskLength = mask;
        }
    }
}
