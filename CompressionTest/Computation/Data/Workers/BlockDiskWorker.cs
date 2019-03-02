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
    class BlockDiskWorker : Interfaces.IComputation, IDisposable
    {
        AutoResetEvent waitHandler = new AutoResetEvent(false);

        protected IO.DataProviders.BlockDataProvider input;
        protected IO.DataProviders.BlockDataProvider Output;
        protected Compression.Strategy.Compression compression;

        protected Queue<Data.ByteBlock> readQueue;
        protected Queue<Data.ByteBlock> writeQueue;
        protected Thread[] _threads;

        private readonly object readLock = new object();
        private readonly object writeLock = new object();

        private Disk _disk;
        private Cpu _cpu;

        public BlockDiskWorker(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider Output,
            Compression.Strategy.Compression compression,
            Compression.Enums.CompressionType compressionType)
        {
            initialize(input, Output, compression, compressionType);
        }

        void initialize(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider Output,
            Compression.Strategy.Compression compression,
            Compression.Enums.CompressionType compressionType)
        {
            Console.WriteLine("\n\rComputation started!\n\r");
            var chunkSize = input.GetChunkSize();
            var fileChunks = Computation.Utils.GetComputationInfo.GetNumberOfChunksForFile(chunkSize, input.GetObjectSize());           
            var numberOfChunks = Computation.Utils.GetComputationInfo.GetMaximumChunks(chunkSize);
            int queueCapacity = (int)(numberOfChunks / 2);

            this.input = input;
            this.Output = Output;
            this.readQueue = new Queue<Data.ByteBlock>(queueCapacity);
            this.writeQueue = new Queue<Data.ByteBlock>(queueCapacity);
            this.compression = compression;

            _disk = new Disk(this.readLock, this.writeLock,
                this.input, this.Output,
                this.readQueue, this.writeQueue,
                queueCapacity,
                ref waitHandler,
                this.compression.algorithms,
                compressionType);

            _cpu = new Cpu(this.readLock, this.writeQueue, this.writeQueue, this.readQueue, this.compression,compressionType);

            _threads = new Thread[3];
            _threads[0] = new Thread(new ThreadStart(_disk.StartRead));
            _threads[1] = new Thread(new ThreadStart(_disk.StartWrite));
            _threads[2] = new Thread(new ThreadStart(_cpu.StartWork));
        }

        public void Start()
        {
            for(int i=0; i< _threads.Length;i++)
            {
                _threads[i].Start();
            }
            waitHandler.WaitOne();
            Console.WriteLine("\n\rComputation completed!\n\r");
        }

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

        public void Dispose()
        {
            this.Stop();
        }

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

            public virtual void StartWork()
            {

            }

            public virtual void CleanResources()
            {
                this.readLock = null;
                this.writeLock = null;
            }
        }
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
                                case Compression.Enums.CompressionType.Compress:
                                    NextSlice = readProvider.ReadNext(out last);
                                    break;

                                case Compression.Enums.CompressionType.Decompress:
                                    NextSlice = readProvider
                                                    .SpecificRead(Compression.Utils.MagicNumbersAlgorithmDictionary
                                                    .AlgorithmMagicNumbers[algorithms].completeMask,out last);
                                    break;
                            }

                            readQueue.Enqueue(new ByteBlock(NextSlice, chunkNumber, last));
                            chunkNumber++;

                            if (last) canRead = false;
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
                Console.WriteLine("[Output]: Чтение данных из файла завершено!");
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
                Console.WriteLine("[Output]: Запись данных в файл завершена!");
                waitHandle.Set();
            }

            public override void CleanResources()
            {
                base.CleanResources();
                this.readProvider = null;
                this.writeProvider = null;
            }
        }

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
