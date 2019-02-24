using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Utils
{
    static class API
    {
        public static string[] GetSourceTypes()
        {
            return Enum.GetNames(typeof(IO.Data.DataSource));
        }

        public static string[] GetDataProvidersTypes()
        {
            return Enum.GetNames(typeof(IO.Data.ProviderType));
        }
    }
}
