using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CompressionTest.Computation.Data.Workers
{
    /// <summary>
    /// Multi Threading Cpu Block realization
    /// </summary>
    /// <remarks>
    /// This class made for computation for CPU cores. So it gets info about specific lowest idle core and start compression in multi thread.
    /// This class implements way of reading from one source in multi thread withoit thread for disk usage. All is working on CPU. 
    /// Each thread get the new chunk of data only after completing the full cycle of reading, compressing/decompressing, writing.
    /// </remarks>
    class BlockCpuWorker : Interfaces.IComputation, IDisposable
    {
        /// <summary>
        /// DataProvider for reading data from source
        /// </summary>
        protected IO.DataProviders.BlockDataProvider _input;
        /// <summary>
        /// DataProvider for writing data to the destination
        /// </summary>
        protected IO.DataProviders.BlockDataProvider _output;
        /// <summary>
        /// Current proccessing block of compression
        /// </summary>
        protected int _currentBlock;
        /// <summary>
        /// Last readed block from the source
        /// </summary>
        protected bool _last;
        /// <summary>
        /// Compression type enum (compression,decompression)
        /// </summary>
        private Compression.Enums.CompressionType _compressionType;
        /// <summary>
        /// High level compression class with specific realization of strategy in it
        /// </summary>
        protected Compression.Strategy.Compression compression;

        /// <summary>
        /// Lock sync for data reading
        /// </summary>
        private readonly object readLock = new object();
        /// <summary>
        /// Lock sync for data writing
        /// </summary>
        private readonly object writeLock = new object();
        /// <summary>
        /// Lock sync for getting current lock
        /// </summary>
        private readonly object currentLock = new object();
        /// <summary>
        /// Lock symc for getting info about last readed block
        /// </summary>
        private readonly object lastLock = new object();
        /// <summary>
        /// Lock sync for
        /// </summary>
        private readonly object comptypeLock = new object();

        /// <summary>
        /// ThreadPool
        /// </summary>
        protected Thread[] _threads;
        /// <summary>
        /// Handler to wait competion of threads
        /// </summary>
        AutoResetEvent waitHandle = new AutoResetEvent(false);
        /// <summary>
        /// Core of lowest idle time
        /// </summary>
        protected int currentCore;

        public BlockCpuWorker(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider output,
            Compression.Strategy.Compression compression,
            Compression.Enums.CompressionType compressionType,
            int currentCore)
        {
            Initialize(input, output, compression, compressionType, currentCore);
        }

        /// <summary>
        /// Starting of computation proccess
        /// </summary>
        public void Start()
        {
            //Send come info about starting to the console
            Console.WriteLine("[output]: Computation proccess is started!");
            //We need to get info about the current proccess
            Process process = Process.GetCurrentProcess();
            //and also offset of how much threads is working in it
            int offset = process.Threads.Count;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //We are starting out threads with parameter of compression, 
            //clone made hear for race for elements
            //cause we don't need to use similar object of high level compression
            //we just give every thread his own compression object to work with
            for (int i=0;i<_threads.Length;i++)
            {
                _threads[i].Start(compression.Clone());
            }
            //refresh info about process
            process.Refresh();
            //we are uisng ProcessorAffinity to change the priority of core for processor
            //as a parameter we use here the lowest idle core
            for(int i=0;i<_threads.Length;i++)
            {
                process.Threads[i + offset].ProcessorAffinity = (IntPtr)(1L << currentCore);
            }
            //wait for threads completion
            waitHandle.WaitOne();
            stopwatch.Stop();
            Console.WriteLine("\n\r[Output]: Computation completed!\n\r");
            Console.WriteLine("[Output]: The computation took about {0}sec\n\r", stopwatch.Elapsed.TotalSeconds);
        }

        /// <summary>
        /// Get rid of nasty things
        /// </summary>
        public void Stop()
        {
            this._input = null;
            this._output = null;
            this.compression = null;
            foreach(var t in this._threads)
            {
                t.Abort();
            }
            this._threads = null;
        }

        /// <summary>
        /// Class initialization
        /// </summary>
        /// <param name="input">DataProvider for reading data from source</param>
        /// <param name="output">DataProvider for writing data to destionation</param>
        /// <param name="compression">High level of compression class</param>
        /// <param name="compressionType">Compress/Decompress</param>
        /// <param name="currentCore">Lowest idle core</param>
        public void Initialize(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider output,
            Compression.Strategy.Compression compression,
            Compression.Enums.CompressionType compressionType,
            int currentCore)
        {
            _threads = new Thread[2];

            this._input = input;
            this._output = output;
            //Create new parameterize threads
            _threads[0] = new Thread(new ParameterizedThreadStart(Processing));
            _threads[1] = new Thread(new ParameterizedThreadStart(Processing));

            _currentBlock = 0;
            _last = false;
            this.compression = compression;
            _compressionType = compressionType;
            this.currentCore = currentCore;
        }

        /// <summary>
        /// Main process
        /// </summary>
        /// <remarks>
        /// Using this method for milti threading file computation
        /// </remarks>
        /// <param name="param">Compression clone</param>
        private void Processing(object param)
        {
            //Each thread get his own compression copy of class
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
                        //Read Next chunk of datam, if this is a compress, so we can read const size of block
                        case Compression.Enums.CompressionType.Compress:
                            byteBlock = _input.ReadNext(out latest);
                            break;
                        //Then we are decompressing we don't know the size of the block, so we need to read from the file by specification of
                        //headers for compression
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

                //Call method to compress or decompress
                switch (type)
                {
                    case Compression.Enums.CompressionType.Compress:
                        block.Slice = compression.Compress(block.Slice);
                        break;

                    case Compression.Enums.CompressionType.Decompress:
                        block.Slice = compression.Decompress(block.Slice);
                        break;
                }

                //Writing next chunk to the destination
                lock (writeLock)
                {
                    _output.WriteNext(block.Slice);
                }               
                block.Slice = null;

                //if the last block - call the handler
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
