using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Interfaces;
using System.IO;

namespace CompressionTest.IO.DataProviders.Block
{
    class FileBlock : Data.AbstractDataSource, IBlockProvider
    {
        protected FileStream _inputStream;
        protected FileStream _outputStream;
        protected BinaryReader binaryReader;
        protected BinaryWriter binaryWriter;
        protected int _chunckSize;
        protected long _fileSize;
        protected FileInfo _fileInfo;

        public FileBlock(string[] payload,Data.DirectionType directionType) : base(payload)
        {
            switch(directionType)
            {
                case Data.DirectionType.In:
                    InputDirectionValidation(payload);
                    _inputStream = CheckInputFileExist(payload[0]);
                    binaryReader = new BinaryReader(_inputStream);
                    _chunckSize = Convert.ToInt32(payload[1]);
                    _fileInfo = new FileInfo(payload[0]);
                    _fileSize = _fileInfo.Length;
                    break;

                case Data.DirectionType.Out:
                    OutputDirectionValidation(payload);
                    _outputStream = CheckOutputFileExist(payload[0]);
                    binaryWriter = new BinaryWriter(_outputStream);
                    break;

                case Data.DirectionType.InOut:
                    InOutDirectionValidation(payload);
                    _inputStream = CheckInputFileExist(payload[0]);
                    _outputStream = CheckOutputFileExist(payload[1]);
                    binaryReader = new BinaryReader(_inputStream);
                    binaryWriter = new BinaryWriter(_outputStream);
                    _chunckSize = Convert.ToInt32(payload[2]);
                    _fileInfo = new FileInfo(payload[0]);
                    _fileSize = _fileInfo.Length;
                    break;
            }
        }

        public override void InputDirectionValidation(string[] payload)
        {
            base.InputDirectionValidation(payload);
        }

        public override void OutputDirectionValidation(string[] payload)
        {
            base.OutputDirectionValidation(payload);
        }

        public override void InOutDirectionValidation(string[] payload)
        {
            base.InOutDirectionValidation(payload);
        }

        public override void Dispose()
        {
            //
        }

        public long GetObjectSize()
        {
            return _fileSize;
        }

        public static string[] GetInputInfo()
        {
            return new List<string>
            {
                "[InputPath] - Путь до файла из которого будте производится считывание данных",
                "[ChunkSize] - Размер блока который необходимо будет считать" 
            }.ToArray();
        }

        public static string[] GetOutputInfo()
        {
            return new List<string>
            {
                "[OutputPath] - Путь до файла в который будет производится запись данных"
            }.ToArray();
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
            return binaryReader.ReadBytes((int)_fileSize);         
        }

        public byte[] ReadNext()
        {
            return binaryReader.ReadBytes(_chunckSize);
        }

        public void WriteAll(byte[] binary)
        {
            binaryWriter.Write(binary, 0, binary.Length);
            binaryWriter.Flush();
        }

        public void WriteNext(byte[] binary)
        {
            binaryWriter.Write(binary, 0, binary.Length);
            binaryWriter.Flush();
        }
    }
}
