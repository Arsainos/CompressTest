using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Interfaces;

namespace CompressionTest.Compression.Strategy
{
    class Compression : ICompressionStrategy, ICloneable
    {
        public ICompressionStrategy CompressionStrategy { private get; set; }
        public Enums.CompressionAlgorithms algorithms { get; private set; }
        public Data.Structure.CompressingSpecificationStructure CompressingSpecification;

        public Compression(ICompressionStrategy compressionStrategy, 
            Enums.CompressionAlgorithms algorithms, 
            Data.Structure.CompressingSpecificationStructure specificationStructure)

        {
            CompressionStrategy = compressionStrategy;
            this.algorithms = algorithms;
            this.CompressingSpecification = specificationStructure;
        }

        public byte[] Compress(byte[] input)
        {
            return CompressionStrategy.Compress(input);
        }

        public byte[] Decompress(byte[] input)
        {
            return CompressionStrategy.Decompress(input);
        }

        public object Clone()
        {
            return new Compression(this.CompressionStrategy,this.algorithms, this.CompressingSpecification);
        }
    }
}
