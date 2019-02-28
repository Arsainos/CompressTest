using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CompressionTest.Computation.Data.Workers
{
    class BlockCpuWorker : Interfaces.IComputation, IDisposable
    {
        protected IO.DataProviders.BlockDataProvider _input;
        protected IO.DataProviders.BlockDataProvider _output;
        protected int _currentBlock;
        protected int _fileChunks;
        private Compression.Enums.CompressionType _compressionType;
        protected Compression.Compression compression;

        private readonly object readLock = new object();
        private readonly object writeLock = new object();
        private readonly object currentLock = new object();
        private readonly object chunksLock = new object();
        private readonly object comptypeLock = new object();

        protected Thread[] _threads;
        AutoResetEvent waitHandle = new AutoResetEvent(false);

        protected int currentCore;

        public BlockCpuWorker(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider output,
            Compression.Compression compression,
            Compression.Enums.CompressionType compressionType,
            int currentCore)
        {
            Initialize(input, output, compression, compressionType, currentCore);
        }

        public void Start()
        {
            Console.WriteLine("[output]: Запуск процесса обработки данных");
            Process process = Process.GetCurrentProcess();
            int offset = process.Threads.Count;

            for(int i=0;i<_threads.Length;i++)
            {
                _threads[i].Start(compression.Clone());
            }

            process.Refresh();

            for(int i=0;i<_threads.Length;i++)
            {
                process.Threads[i + offset].ProcessorAffinity = (IntPtr)(1L << currentCore);
            }

            waitHandle.WaitOne();
            Console.WriteLine("[Output]: Обработка данных закончена");
        }

        public void Stop()
        {
            this._input.Dispose();
            this._output.Dispose();

            this._input = null;
            this._output = null;
        }

        public void Initialize(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider output,
            Compression.Compression compression,
            Compression.Enums.CompressionType compressionType,
            int currentCore)
        {
            _threads = new Thread[2];

            this._input = input;
            this._output = output;

            _threads[0] = new Thread(new ParameterizedThreadStart(Processing));
            _threads[1] = new Thread(new ParameterizedThreadStart(Processing));

            _currentBlock = 0;
            this.compression = compression;
            _compressionType = compressionType;
            _fileChunks = (int)Utils.GetComputationInfo.GetNumberOfChunksForFile(_input.GetChunkSize(), _input.GetObjectSize());
            this.currentCore = currentCore;
        }

        public void Processing(object param)
        {
            Compression.Compression compression = (Compression.Compression)param;
            Compression.Enums.CompressionType type;
            int maxSize = 0;

            lock(chunksLock)
            {
                maxSize = _fileChunks;
            }
            lock(comptypeLock)
            {
                type = _compressionType;
            }

            while(GetCurrentBlock() != maxSize)
            {
                byte[] byteBlock = null;
                int current = GetCurrentBlock();

                lock(readLock)
                {
                    byteBlock = _input.ReadNext();
                }

                ByteBlock block = new ByteBlock(byteBlock, current);
                current++;
                lock(currentLock)
                {
                    _currentBlock++;
                }

                switch (type)
                {
                    case Compression.Enums.CompressionType.Compress:
                        block.Slice = compression.Compress(block.Slice);
                        break;

                    case Compression.Enums.CompressionType.Decompress:
                        block.Slice = compression.Decompress(block.Slice);
                        break;
                }

                lock (writeLock)
                {
                    _output.WriteNext(block.Slice);
                }               
                block.Slice = null;

                if(current == maxSize)
                {
                    waitHandle.Set();
                }
            }
        }

        int GetCurrentBlock()
        {
            lock(currentLock)
            {
                return _currentBlock;
            }
        }

       
        public void Dispose()
        {
            this.Stop();
        }
    }
}
