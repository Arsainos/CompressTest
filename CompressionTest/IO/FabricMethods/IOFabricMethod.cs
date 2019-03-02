using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Data;

namespace CompressionTest.IO.FabricMethods
{
    static class IOFabricMethod<T>
    {
        public static T Create(DataSource source, DirectionType direction, string[] payload)
        {
            if(typeof(T) == typeof(DataProviders.BlockDataProvider))
            {
                
                switch (source)
                {
                    case DataSource.File:
                        object args = new Data.Block.FileBlock(payload, direction);
                        return (T)Activator.CreateInstance(typeof(T), args);
                }
            }

            return default(T);
        }
    }
}
