using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Utils
{
    static class API
    {
        public static Dictionary<string,Data.DataSource> GetSourceTypes()
        {
            var names = Enum.GetNames(typeof(IO.Data.DataSource));
            var values = Enum.GetValues(typeof(IO.Data.DataSource));

            var result = new Dictionary<string, Data.DataSource>();
            for(int i=0;i<names.Length;i++)
            {
                result.Add(names[i], (Data.DataSource)values.GetValue(i));
            }

            return result;
        }

        public static Dictionary<string,Data.ProviderType> GetDataProvidersTypes()
        {
            var names = Enum.GetNames(typeof(IO.Data.ProviderType));
            var values = Enum.GetValues(typeof(IO.Data.ProviderType));

            var result = new Dictionary<string, Data.ProviderType>();
            for (int i = 0; i < names.Length; i++)
            {
                result.Add(names[i], (Data.ProviderType)values.GetValue(i));
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
