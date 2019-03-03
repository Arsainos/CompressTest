using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CompressionTest.Computation.Data.Structures;
using CompressionTest.IO.DataProviders;

namespace CompressionTest.Computation.Data.Workers
{
    /// <summary>
    /// Multi Threading Disk Block realization
    /// </summary>
    /// <remarks>
    /// This class made for computation for disk. So it gets info about specific lowest idle disk and start compression in multi thread.
    /// This class made as consumer/producer so here we are using three threads. two for reading/writing data and one for data processing.
    /// </remarks>
    class BlockDiskWorker : Interfaces.IComputation, IDisposable
    {
        /// <summary>
        /// Wait handler for thread complete
        /// </summary>
        AutoResetEvent waitHandler = new AutoResetEvent(false);

        /// <summary>
        /// DataProvider for data reading
        /// </summary>
        protected IO.DataProviders.BlockDataProvider input;
        /// <summary>
        /// DataProvider for data writing
        /// </summary>
        protected IO.DataProviders.BlockDataProvider Output;
        /// <summary>
        /// High level class of compression implementation
        /// </summary>
        protected Compression.Strategy.Compression compression;

        /// <summary>
        /// Queue for readed data
        /// </summary>
        protected Queue<Data.ByteBlock> readQueue;
        /// <summary>
        /// Queue for writing data
        /// </summary>
        protected Queue<Data.ByteBlock> writeQueue;
        /// <summary>
        /// ThreadPool
        /// </summary>
        protected Thread[] _threads;

        /// <summary>
        /// Lock sync for reading/writing from the read queue
        /// </summary>
        private readonly object readLock = new object();
        /// <summary>
        /// Lock sync for reading/writing from the writing queue
        /// </summary>
        private readonly object writeLock = new object();

        /// <summary>
        /// Link for Disk usage class
        /// </summary>
        private Disk _disk;
        /// <summary>
        /// Link for Cpu usage class
        /// </summary>
        private Cpu _cpu;

        public BlockDiskWorker(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider Output,
            Compression.Strategy.Compression compression,
            Compression.Enums.CompressionType compressionType)
        {
            initialize(input, Output, compression, compressionType);
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="input">DataProvider for reading data</param>
        /// <param name="Output">DataProvider for writing data</param>
        /// <param name="compression">High level class for compression</param>
        /// <param name="compressionType">Compress/Decompress</param>
        void initialize(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider Output,
            Compression.Strategy.Compression compression,
            Compression.Enums.CompressionType compressionType)
        {
            Console.WriteLine("\n\r[Output]: Computation started!\n\r");
            //get info about chunks
            var chunkSize = input.GetChunkSize();
            var fileChunks = Computation.Utils.GetComputationInfo.GetNumberOfChunksForFile(chunkSize, input.GetObjectSize());           
            var numberOfChunks = Computation.Utils.GetComputationInfo.GetMaximumChunks(chunkSize);
            //we will be using half of the system free ram for queue, so we won't have any problem with memory leaks
            int queueCapacity = (int)(numberOfChunks / 2);

            this.input = input;
            this.Output = Output;
            this.readQueue = new Queue<Data.ByteBlock>(queueCapacity);
            this.writeQueue = new Queue<Data.ByteBlock>(queueCapacity);
            this.compression = compression;

            //Inner classes
            _disk = new Disk(this.readLock, this.writeLock,
                this.input, this.Output,
                this.readQueue, this.writeQueue,
                queueCapacity,
                ref waitHandler,
                this.compression.algorithms,
                compressionType);

            _cpu = new Cpu(this.readLock, this.writeQueue, this.writeQueue, this.readQueue, this.compression,compressionType);

            //Creating threads with innder classes realizations
            _threads = new Thread[3];
            _threads[0] = new Thread(new ThreadStart(_disk.StartRead));
            _threads[1] = new Thread(new ThreadStart(_disk.StartWrite));
            _threads[2] = new Thread(new ThreadStart(_cpu.StartWork));
        }

        /// <summary>
        /// Start method
        /// </summary>
        public void Start()
        {
            for(int i=0; i< _threads.Length;i++)
            {
                _threads[i].Start();
            }
            waitHandler.WaitOne();
            Console.WriteLine("\n\r[Output]: Computation completed!\n\r");
        }

        /// <summary>
        /// Get rid of nasty things
        /// </summary>
        public void Stop()
        {
            _disk.CleanResources();
            _cpu.CleanResources();

            this.input = null;
            this.Output = null;
            this.compression = null;
            this.readQueue = null;
            this.writeQueue = null;           
        }

        //Just redirect to the Stop, make this just for the using calls
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// Abstract class of diskworker
        /// </summary>
        /// so eahc class here inherited from this
        abstract class DiskWorker
        {
            public object readLock {get; private set; }
            public object writeLock { get; private set; }
            public Queue<Data.ByteBlock> writeQueue { get; private set; }
            public Queue<Data.ByteBlock> readQueue { get; private set; }


            public DiskWorker(object readLock, object writeLock,
                Queue<Data.ByteBlock> writeQueue,
                Queue<Data.ByteBlock> readQueue)
            {
                this.readLock = readLock;
                this.writeLock = writeLock;
                this.writeQueue = writeQueue;
                this.readQueue = readQueue;
            }

            /// <summary>
            /// Start inner class
            /// </summary>
            public virtual void StartWork()
            {

            }

            /// <summary>
            /// Get rid of inner nasty things
            /// </summary>
            public virtual void CleanResources()
            {
                this.readLock = null;
                this.writeLock = null;
            }
        }
        /// <summary>
        /// Class implements works for reading/writing data and sends/receive it from compressor/decompressor
        /// </summary>
        class Disk : DiskWorker
        {
            public bool complete { get; set; }
            private IO.DataProviders.BlockDataProvider readProvider;
            private IO.DataProviders.BlockDataProvider writeProvider;
            private int queueCapacity;
            private AutoResetEvent waitHandle;
            private Compression.Enums.CompressionAlgorithms algorithms;
            private Compression.Enums.CompressionType CompressionType;
            public Disk(object readLock, object writeLock,
                BlockDataProvider readProvider, BlockDataProvider writeProvider, 
                Queue<ByteBlock> readQueue, Queue<ByteBlock> writeQueue, 
                int queueCapacity,
                ref AutoResetEvent resetEvent,
                Compression.Enums.CompressionAlgorithms algorithms,
                Compression.Enums.CompressionType CompressionType) : base(readLock, writeLock,writeQueue,readQueue)
            {
                this.queueCapacity = queueCapacity;
                this.readProvider = readProvider;
                this.writeProvider = writeProvider;
                this.waitHandle = resetEvent;
                this.algorithms = algorithms;
                this.CompressionType = CompressionType;
            }

            public void StartRead()
            {
                bool canRead = true;
                int chunkNumber = 1;
               
                while (canRead)
                {
                    lock (readLock)
                    {
                        if (readQueue.Count < queueCapacity)
                        {
                            byte[] NextSlice = null;
                            bool last = false;
                            switch (CompressionType)
                            {
                                //For compression we can read const size of chunks
                                case Compression.Enums.CompressionType.Compress:
                                    NextSlice = readProvider.ReadNext(out last);
                                    break;
                                //For decompression we need to read specific chunks of byte,
                                //so we detect how to read them by header specification
                                case Compression.Enums.CompressionType.Decompress:
                                    NextSlice = readProvider
                                                    .SpecificRead(Compression.Utils.MagicNumbersAlgorithmDictionary
                                                    .AlgorithmMagicNumbers[algorithms].completeMask,out last);
                                    break;
                            }

                            //send data
                            readQueue.Enqueue(new ByteBlock(NextSlice, chunkNumber, last));
                            chunkNumber++;

                            if (last)
                            {
                                canRead = false;
                            }
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
                Console.WriteLine("[Output]: Data read completed!");
            }

            public void StartWrite()
            {
                bool last = false;
                while(!last)
                {
                    lock(writeLock)
                    {
                        if(writeQueue.Count > 0)
                        {
                            //Get data and write it the destination
                            ByteBlock block = writeQueue.Dequeue();
                            last = block.lastSubset;
                            writeProvider.WriteNext(block.Slice);
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
                Console.WriteLine("[Output]: Data write completed!");
                //Call handler about completion
                waitHandle.Set();
            }

            //Get rid of inner nasty things
            public override void CleanResources()
            {
                base.CleanResources();
                this.readProvider = null;
                this.writeProvider = null;
            }
        }

        /// <summary>
        /// Class implements data computation and also receive/sends from/to DiskWorker
        /// </summary>
        class Cpu : DiskWorker
        {
            private Compression.Strategy.Compression compression;
            private Compression.Enums.CompressionType CompressionType;

            public Cpu(object readLock, object writeLock,
                Queue<ByteBlock> writeQueue, Queue<ByteBlock> readQueue,
                Compression.Strategy.Compression compression,
                Compression.Enums.CompressionType compressionType) : base(readLock, writeLock, writeQueue, readQueue)
            {
                this.compression = compression;
                this.CompressionType = compressionType;
            }

            public override void CleanResources()
            {
                base.CleanResources();
                this.compression = null;
            }

            public override void StartWork()
            {
                bool canWork = true;
                
                while(canWork)
                {
                    bool canSend = false;
                    ByteBlock byteBlock = null;
                    lock(readLock)
                    {
                        if(readQueue.Count > 0)
                        {
                            //Read data from queue
                            byteBlock = readQueue.Dequeue();
                            canSend = true;
                            canWork = !byteBlock.lastSubset;
                        }
                    }
                    if(canSend)
                    {
                        switch(CompressionType)
                        {
                            case Compression.Enums.CompressionType.Compress:
                                byteBlock.Slice = compression.Compress(byteBlock.Slice);
                                break;

                            case Compression.Enums.CompressionType.Decompress:
                                byteBlock.Slice = compression.Decompress(byteBlock.Slice);
                                break;
                        }
                        
                        lock(writeLock)
                        {
                            //write data to queue
                            writeQueue.Enqueue(byteBlock);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }
    }
}
