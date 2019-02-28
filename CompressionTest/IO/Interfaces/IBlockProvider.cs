using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Interfaces
{
    interface IBlockProvider
    {
        byte[] ReadAll();
        byte[] ReadNext();
        void WriteAll(byte[] binary);
        void WriteNext(byte[] binary);
        long GetObjectSize();
        int GetChunkSize();
    }
}
