using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Enums;

namespace CompressionTest.Compression.Utils
{
    static class API
    {
        public static Dictionary<string,CompressionAlgorithms> getCompressionAlgorithms()
        {
            var names = Enum.GetNames(typeof(CompressionAlgorithms));
            var values = Enum.GetValues(typeof(CompressionAlgorithms));
            var dictionary = new Dictionary<string, CompressionAlgorithms>();
            for (int i = 0; i < names.Length; i++)
            {
                dictionary.Add(names[i], (CompressionAlgorithms)values.GetValue(i));
            }
            return dictionary;
        }

        public static Dictionary<string, CompressionType> getCompressionTypes()
        {
            var names = Enum.GetNames(typeof(CompressionType));
            var values = Enum.GetValues(typeof(CompressionType));
            var dictionary = new Dictionary<string, CompressionType>();
            for(int i=0;i<names.Length;i++)
            {
                dictionary.Add(names[i], (CompressionType)values.GetValue(i));
            }

            return dictionary;
        }
    }
}
