using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Compression.Utils
{
    /// <summary>
    /// Static class to provide speacial info about compression algorithm specification
    /// </summary>
    /// <remarks>
    /// Get info about magic numbers, mask and etc.
    /// </remarks>
    static class MagicNumbersAlgorithmDictionary
    {
        public static readonly Dictionary<Enums.CompressionAlgorithms, Data.Structure.CompressingSpecificationStructure>
            AlgorithmMagicNumbers = new Dictionary<Enums.CompressionAlgorithms, Data.Structure.CompressingSpecificationStructure>()
            {
                {
                    Enums.CompressionAlgorithms.Gzip,
                    new Data.Structure.CompressingSpecificationStructure
                    (
                        new byte[]{0x1f,0x8b},
                        8
                    )
                }
            };
    }
}
