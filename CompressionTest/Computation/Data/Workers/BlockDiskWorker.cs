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
        protected Compression.Compression compression;

        protected Queue<Data.ByteBlock> readQueue;
        protected Queue<Data.ByteBlock> writeQueue;
        protected Thread[] _threads;

        private readonly object readLock = new object();
        private readonly object writeLock = new object();

        private Disk _disk;
        private Cpu _cpu;

        public BlockDiskWorker(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider Output,
            Compression.Compression compression,
            Compression.Enums.CompressionType compressionType)
        {
            initialize(input, Output, compression, compressionType);
        }

        void initialize(IO.DataProviders.BlockDataProvider input,
            IO.DataProviders.BlockDataProvider Output,
            Compression.Compression compression,
            Compression.Enums.CompressionType compressionType)
        {
            Console.WriteLine("\n\rНачинаю обработку\n\r");
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
                queueCapacity, (int)fileChunks,
                ref waitHandler);

            _cpu = new Cpu(this.readLock, this.writeQueue, this.writeQueue, this.readQueue, this.compression, (int)fileChunks,compressionType);

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
            Console.WriteLine("Обработка завершена!");
        }

        public void Stop()
        {
            _disk.CleanResources();
            _cpu.CleanResources();

            input.Dispose();
            Output.Dispose();

            compression = null;
            readQueue = null;
            writeQueue = null;           
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
            private int chunksNumber;
            private AutoResetEvent waitHandle;
            public Disk(object readLock, object writeLock,
                BlockDataProvider readProvider, BlockDataProvider writeProvider, 
                Queue<ByteBlock> readQueue, Queue<ByteBlock> writeQueue, 
                int queueCapacity, int chunksNumber, 
                ref AutoResetEvent resetEvent) : base(readLock, writeLock,writeQueue,readQueue)
            {
                this.queueCapacity = queueCapacity;
                this.chunksNumber = chunksNumber;
                this.readProvider = readProvider;
                this.writeProvider = writeProvider;
                this.waitHandle = resetEvent;
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
                            byte[] NextSlice = readProvider.ReadNext();
                            if (NextSlice.Length > 0)
                            {
                                readQueue.Enqueue(new ByteBlock(NextSlice, chunkNumber));
                                chunkNumber++;
                            }
                            else
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
                Console.WriteLine("[Output]: Чтение данных из файла завершено!");
            }

            public void StartWrite()
            {
                int chunks = chunksNumber;
                while(chunks > 0)
                {
                    lock(writeLock)
                    {
                        if(writeQueue.Count > 0)
                        {
                            writeProvider.WriteNext(writeQueue.Dequeue().Slice);
                            chunks--;
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
            private Compression.Compression compression;
            private Compression.Enums.CompressionType CompressionType;
            private int chunks;

            public Cpu(object readLock, object writeLock,
                Queue<ByteBlock> writeQueue, Queue<ByteBlock> readQueue,
                Compression.Compression compression,
                int maxChunks,
                Compression.Enums.CompressionType compressionType) : base(readLock, writeLock, writeQueue, readQueue)
            {
                this.compression = compression;
                this.chunks = maxChunks;
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
                            if (byteBlock.blockNumber == chunks) canWork = false;
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
