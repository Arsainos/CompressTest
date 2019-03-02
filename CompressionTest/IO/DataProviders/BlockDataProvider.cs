using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Interfaces;

namespace CompressionTest.IO.DataProviders
{
    class BlockDataProvider : IO.Data.AbstractClasses.AbstractDataProvider, IBlockProvider, IDisposable
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

        public byte[] ReadAll(out bool last)
        {
            return _blockProvider.ReadAll(out last);
        }

        public byte[] ReadNext(out bool last)
        {
            return _blockProvider.ReadNext(out last);
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

        public byte[] SpecificRead(byte[] magicNumber, out bool last)
        {
            return _blockProvider.SpecificRead(magicNumber, out last);
        }

        public byte[] ReadArray(int count)
        {
            return _blockProvider.ReadArray(count);
        }
    }
}
