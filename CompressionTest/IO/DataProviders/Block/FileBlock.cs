﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Interfaces;
using System.IO;

namespace CompressionTest.IO.DataProviders.Block
{
    class FileBlock : IBlockProvider
    {
        protected FileStream _inputStream;
        protected FileStream _outputStream;
        protected int _chunckSize;
        protected long _fileSize;
        

        public FileBlock(string inputPath, string outputPath, int chunckSize)
        {
            _inputStream = CheckInputFileExist(inputPath);
            _outputStream = CheckOutputFileExist(outputPath);
            _chunckSize = chunckSize;
            var fileInfo = new FileInfo(inputPath);
            _fileSize = fileInfo.Length;
        }

        private FileStream CheckInputFileExist(string inputPath)
        {
            if(File.Exists(inputPath))
            {
                return File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                throw new Exception("Не удалось найти файл для чтения по заданному пути!");
            }
        }

        private FileStream CheckOutputFileExist(string outputPath)
        {
            if(File.Exists(outputPath))
            {
                return File.Open(outputPath, FileMode.Truncate, FileAccess.Write, FileShare.Write);
            }
            else
            {
                return File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.Write);
            }
        }

        public byte[] ReadAll()
        {
            if (Utils.Utils.CheckSpaceForDataLoading(_fileSize))
            {
                using (var reader = new BinaryReader(_inputStream))
                {
                    return reader.ReadBytes((int)_fileSize);
                }
            }
            else
            {
                throw new Exception("Не достаточно места в памяти для полной выгрузки файла в память!");
            }
        }

        public byte[] ReadNext()
        {
            using (var reader = new BinaryReader(_inputStream))
            {
                return reader.ReadBytes(_chunckSize);
            }
        }

        public void WriteAll(byte[] binary)
        {
            using (var writer = new BinaryWriter(_outputStream))
            {
                writer.Write(binary, 0, binary.Length);
                writer.Flush();
            }
        }

        public void WriteNext(byte[] binary)
        {
            using(var writer = new BinaryWriter(_outputStream))
            {
                writer.Write(binary, 0, binary.Length);
                writer.Flush();
            }
        }
    }
}