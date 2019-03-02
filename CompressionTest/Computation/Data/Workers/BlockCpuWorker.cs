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
        protected bool _last;
        private Compression.Enums.CompressionType _compressionType;
        protected Compression.Strategy.Compression compression;

        private readonly object readLock = new object();
        private readonly object writeLock = new object();
        private readonly object currentLock = new object();
        private readonly object lastLock = new object();
        private readonly object comptypeLock = new object();

        protected Thread[] _threads;
        AutoResetEvent waitHandle = new AutoResetEvent(false);

        protected int currentCore;

        public BlockCpuWorker(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider output,
            Compression.Strategy.Compression compression,
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
            Compression.Strategy.Compression compression,
            Compression.Enums.CompressionType compressionType,
            int currentCore)
        {
            _threads = new Thread[2];

            this._input = input;
            this._output = output;

            _threads[0] = new Thread(new ParameterizedThreadStart(Processing));
            _threads[1] = new Thread(new ParameterizedThreadStart(Processing));

            _currentBlock = 0;
            _last = false;
            this.compression = compression;
            _compressionType = compressionType;
            this.currentCore = currentCore;
        }

        public void Processing(object param)
        {
            Compression.Strategy.Compression compression = (Compression.Strategy.Compression)param;
            Compression.Enums.CompressionType type;
            bool last = false;

            lock(comptypeLock)
            {
                type = _compressionType;
            }

            while(!last)
            {
                lock(lastLock)
                {
                    last = _last;
                }

                byte[] byteBlock = null;
                int current = GetCurrentBlock();
                bool latest = false;
                lock(readLock)
                {
                    switch(type)
                    {
                        case Compression.Enums.CompressionType.Compress:
                            byteBlock = _input.ReadNext(out latest);
                            break;

                        case Compression.Enums.CompressionType.Decompress:
                            byteBlock = _input.SpecificRead(
                                Compression.Utils.MagicNumbersAlgorithmDictionary
                                .AlgorithmMagicNumbers[compression.algorithms].completeMask, out latest);
                            break;
                    }                    
                }

                ByteBlock block = new ByteBlock(byteBlock, current,latest);
                current++;
                lock(currentLock)
                {
                    _currentBlock++;
                }

                lock(lastLock)
                {
                    _last = latest;
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

                if(latest)
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
