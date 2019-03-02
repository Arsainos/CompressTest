using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Enums;

namespace CompressionTest.IO.Utils
{
    static class API
    {
        public static Dictionary<string,DataSource> GetSourceTypes()
        {
            var names = Enum.GetNames(typeof(DataSource));
            var values = Enum.GetValues(typeof(DataSource));

            var result = new Dictionary<string,DataSource>();
            for(int i=0;i<names.Length;i++)
            {
                result.Add(names[i], (DataSource)values.GetValue(i));
            }

            return result;
        }

        public static Dictionary<string,ProviderType> GetDataProvidersTypes()
        {
            var names = Enum.GetNames(typeof(ProviderType));
            var values = Enum.GetValues(typeof(ProviderType));

            var result = new Dictionary<string, ProviderType>();
            for (int i = 0; i < names.Length; i++)
            {
                result.Add(names[i], (ProviderType)values.GetValue(i));
            }

            return result;
        }

        public static string[] GetBlockFileInfoInput()
        {
            return Data.Block.FileBlock.GetInputInfo();
        }

        public static string[] GetBlockFileInfoOutput()
        {
            return Data.Block.FileBlock.GetOutputInfo();
        }
    }
}
