using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Interfaces;

namespace CompressionTest.IO.DataProviders
{
    class BlockDataProvider : IBlockProvider
    {
        public IBlockProvider _blockProvider { private get; set; }

        public BlockDataProvider(IBlockProvider blockProvider)
        {
            _blockProvider = blockProvider;
        }

        public byte[] ReadAll()
        {
            return _blockProvider.ReadAll();
        }

        public byte[] ReadNext()
        {
            return _blockProvider.ReadNext();
        }

        public void WriteAll(byte[] binary)
        {
            _blockProvider.WriteAll(binary);
        }

        public void WriteNext(byte[] binary)
        {
            _blockProvider.WriteNext(binary);
        }
    }
}
