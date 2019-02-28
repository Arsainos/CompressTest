using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.Compression.Enums;

namespace CompressionTest.Compression.Utils
{
    static class API
    {
        public static string[] getCompressionAlgorithms()
        {
            return Enum.GetNames(typeof(CompressionAlgorithms));
        }

        public static Dictionary<string,int> getCompressionTypes()
        {
            var names = Enum.GetNames(typeof(CompressionType));
            var values = Enum.GetValues(typeof(CompressionType));
            var dictionary = new Dictionary<string, int>();
            for(int i=0;i<names.Length;i++)
            {
                dictionary.Add(names[i], (int)values.GetValue(i));
            }

            return dictionary;
        }
    }
}
