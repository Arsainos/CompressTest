using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Data;

namespace CompressionTest.Compression.Utils
{
    static class API
    {
        public static string[] getCompressionAlgorithms()
        {
            return Enum.GetNames(typeof(CompressionAlgorithms));
        }
    }
}
