using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CompressionTest.IO.DataProviders;
using CompressionTest.IO.Data;
using CompressionTest.IO;

namespace CompressionTest.Tests
{
    class Tests
    {
        private const string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam vel tortor nec dolor mattis bibendum.Pellentesque hendrerit tristique magna ac tempor. Integer at sapien ligula. Mauris ultrices pulvinar auctor. Proin rhoncus sagittis tellus nec fringilla. Phasellus auctor sit amet tellus nec facilisis.Aliquam porttitor, turpis vel porta bibendum, est ante posuere urna, vitae imperdiet sem erat sed ante.Curabitur pellentesque enim vitae elit porttitor posuere.Fusce sed rutrum mi, ut dictum felis. Suspendisse venenatis sed nulla commodo tincidunt. Integer tempus dolor in pulvinar elementum.";

        private readonly IO.DataProviders.BlockDataProvider blockDataProvider;
        private readonly Compression.Compression compression;

        public Tests()
        {
            List<String> IOPayload = new List<string>()
            {
                @"C:\TestFolder\1.txt",
                @"C:\TestFolder\1.txt.gz",
                "4096"
            };

            blockDataProvider = IOFabricMethod<BlockDataProvider>.Create(DataSource.File, DirectionType.InOut, IOPayload.ToArray());
            //CheckFileCreation();
            //TryReadAllFileData();
            //CheckOutputFileTruncate();
            //TryWriteAllData();

            compression = Compression.CompressionFabricMethod.Create(Compression.Data.CompressionAlgorithms.Gzip);
            TryCompressGZipData();
            //TryCompressFile();
            TryDecompressGZipData();
            //TryDecompressFile();

            TestDiskUsage();
            TestRamUsage();
            TestCPUusage();

            TestChooseAlgorithm();

            TestDiskStrategy();
        }

        public void CheckFileCreation()
        {
            if (File.Exists(@"C:\TestFolder\2.txt"))
            {
                Console.WriteLine(String.Format("Tests - [1]CheckFileCreation - true"));
                return;
            }
            Console.WriteLine("Tests - [1]CheckFileCreation - false");
        }

        public void TryReadAllFileData()
        {
            var info = new FileInfo(@"C:\TestFolder\1.txt.").Length;
            var methodLength = blockDataProvider.ReadAll().Length;
            if (methodLength == info)
            {
                Console.WriteLine(String.Format("Tests - [2]TryReadAllFileData - true"));
                return;
            }
            Console.WriteLine(String.Format("Tests - [2]TryReadAllFileData - false (source length={0}, target length={1})", methodLength, info));
        }

        public void CheckOutputFileTruncate()
        {
            var info = new FileInfo(@"C:\TestFolder\2.txt").Length;

            if (info == 0)
            {
                Console.WriteLine(String.Format("Tests - [4]CheckOutputFileTruncate - true"));
                return;
            }
            Console.WriteLine(String.Format("Tests - [4]CheckOutputFileTruncate - false"));
        }

        public void TryWriteAllData()
        {
            string test = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam vel tortor nec dolor mattis bibendum.Pellentesque hendrerit tristique magna ac tempor. Integer at sapien ligula. Mauris ultrices pulvinar auctor. Proin rhoncus sagittis tellus nec fringilla. Phasellus auctor sit amet tellus nec facilisis.Aliquam porttitor, turpis vel porta bibendum, est ante posuere urna, vitae imperdiet sem erat sed ante.Curabitur pellentesque enim vitae elit porttitor posuere.Fusce sed rutrum mi, ut dictum felis. Suspendisse venenatis sed nulla commodo tincidunt. Integer tempus dolor in pulvinar elementum.";
            var reader = new StringReader(test);

            List<byte> azaza = new List<byte>();

            foreach (var t in test)
            {
                azaza.Add((byte)t);
            }

            blockDataProvider.WriteAll(azaza.ToArray());

            var info = new FileInfo(@"C:\TestFolder\2.txt").Length;

            if (info > 0)
            {
                Console.WriteLine(String.Format("Tests - [3]TryWriteAllData - true"));
                return;
            }
            Console.WriteLine(String.Format("Tests - [3]TryWriteAllData - false"));
        }

        public void TryReadNextChunkOfData()
        {

        }

        public void TryWriteNextChunkOfData()
        {

        }

        public void TryCompressGZipData()
        {
            List<byte> defaultArray = new List<byte>();

            foreach (var t in lorem)
            {
                defaultArray.Add((byte)t);
            }

            byte[] resultArray = compression.Compress(defaultArray.ToArray());

            Console.WriteLine(String.Format("Tests - [5]TryCompressGZipData - default:{0}, target:{1}", defaultArray.Count, resultArray.Length));
        }

        public void TryCompressFile()
        {
            byte[] defaults = blockDataProvider.ReadAll();
            byte[] Result = compression.Compress(defaults);

            Console.WriteLine(String.Format("Tests - [5]TryCompressGZipData - default:{0}, target:{1}", defaults.Length, Result.Length));

        }

        public void TryDecompressGZipData()
        {
            List<byte> defaultArray = new List<byte>();

            foreach (var t in lorem)
            {
                defaultArray.Add((byte)t);
            }

            byte[] compressed = compression.Compress(defaultArray.ToArray());

            byte[] resultArray = compression.Decompress(compressed);

            Console.WriteLine(String.Format("Tests - [6]TryCompressGZipData - default:{0}, target:{1}", compressed.Length, resultArray.Length));
        }

        public void TryDecompressFile()
        {
            byte[] defaults = blockDataProvider.ReadAll();
            byte[] compressed = compression.Compress(defaults);
            byte[] resultArray = compression.Decompress(compressed);

            Console.WriteLine(String.Format("Tests - [6]TryCompressGZipData - default:{0}, target:{1}", compressed.Length, resultArray.Length));

        }

        public void TestDiskUsage()
        {
            Console.WriteLine(String.Format("DiskIdle = {0}", Computation.Utils.ResourceCheck.checkDiskUsage("C")));
        }

        public void TestRamUsage()
        {
            Console.WriteLine(String.Format("RamAvaliable = {0}", Computation.Utils.ResourceCheck.checkRamSize()));
        }

        public void TestCPUusage()
        {
            var result = Computation.Utils.ResourceCheck.checkCPUUsage();
            for (int i = 0; i < result.Length; i++)
            {
                Console.WriteLine(String.Format("Core{0}IdleTime = {1}", i.ToString(), result[i]));
            }
        }

        public void TestChooseAlgorithm()
        {
            Console.WriteLine(String.Format("Algorithm = {0}", Computation.Algorithm.StrategyChooser.FindMostRelevantComponent(new object[] { "C:" },
                Computation.FabricMethods.AlgorithmFactoryMethod.GetFunc(Computation.Enums.Algorithms.LowestIdleTime)).ComputationType));
        }

        public void TestDiskStrategy()
        {

            var a = new Computation.Workers.WorkerProvider(new Computation.Data.Workers.BlockDiskWorker(
                blockDataProvider, blockDataProvider, compression));

            a.Start();
        }
    
    }
}