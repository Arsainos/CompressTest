using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Interfaces;

namespace CompressionTest.IO.DataProviders
{
    class BlockDataProvider : IO.Data.AbstractDataProvider, IBlockProvider
    {
        public IBlockProvider _blockProvider { private get; set; }

        public BlockDataProvider(object underlayingStructure) : base(underlayingStructure)
        {
            objectValidation<IBlockProvider>(underlayingStructure);
            _blockProvider = (IBlockProvider)underlayingStructure;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void objectValidation<T>(object uderlayingStructure)
        {
            base.objectValidation<T>(uderlayingStructure);
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

        public long GetObjectSize()
        {
            return _blockProvider.GetObjectSize();
        }

        public int GetChunkSize()
        {
            return _blockProvider.GetChunkSize();
        }
    }
}
