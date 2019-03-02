using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Interfaces;

namespace CompressionTest.Compression.Strategy
{
    /// <summary>
    /// High level class for compression
    /// </summary>
    /// <remarks>
    /// In program realization we use this high level class and set the class with specific realization of interface methods.
    /// This gives us opportunity for high speed prototyping and writing high level architecture without realization.
    /// We use this as a strategy place holder.
    /// </remarks>
    class Compression : ICompressionStrategy, ICloneable
    {
        /// <summary>
        /// Our strategy interface to place
        /// </summary>
        public ICompressionStrategy CompressionStrategy { private get; set; }
        /// <summary>
        /// Algorithm enum
        /// </summary>
        public Enums.CompressionAlgorithms algorithms { get; private set; }
        /// <summary>
        /// Specific info about the algorithm
        /// </summary>
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
