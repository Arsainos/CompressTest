using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Interfaces;

namespace CompressionTest.IO.DataProviders.Block
{
    class FileBlock : IBlockProvider
    {
        protected string _filePath;
        protected int _chunckSize;

        public FileBlock(string filePath, int chunckSize)
        {
            _filePath = filePath;
            _chunckSize = chunckSize;
        }

        public byte[] ReadAll()
        {
            throw new NotImplementedException();
        }

        public byte[] ReadNext()
        {
            throw new NotImplementedException();
        }

        public void WriteAll(byte[] binary)
        {
            throw new NotImplementedException();
        }

        public void WriteNext(byte[] binary)
        {
            throw new NotImplementedException();
        }
    }
}
