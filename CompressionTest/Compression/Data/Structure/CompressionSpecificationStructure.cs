using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Compression.Data.Structure
{
    class CompressingSpecificationStructure
    {
        public byte[] magicNumber { get; private set; }
        public int maskLength { get; private set; }
        public byte[] completeMask {get;set;}

        public CompressingSpecificationStructure(byte[] magicNumber, int mask)
        {
            this.magicNumber = magicNumber;
            this.maskLength = mask;
        }
    }
}
