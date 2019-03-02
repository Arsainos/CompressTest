using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Interfaces
{
    interface IBlockProvider
    {
        byte[] ReadAll(out bool last);
        byte[] ReadNext(out bool last);
        byte[] SpecificRead(byte[] magicNumber, out bool last);
        byte[] ReadArray(int count);
        void WriteAll(byte[] binary);
        void WriteNext(byte[] binary);
        long GetObjectSize();
        int GetChunkSize();
    }
}
