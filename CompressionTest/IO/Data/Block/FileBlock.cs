using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Interfaces;
using System.IO;

namespace CompressionTest.IO.Data.Block
{
    class FileBlock : Data.AbstractClasses.AbstractDataSource, IBlockProvider
    {
        protected FileStream _inputStream;
        protected FileStream _outputStream;
        protected BinaryReader binaryReader;
        protected BinaryWriter binaryWriter;
        private int _chunckSize = 4096;
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
                    if (payload.Length > 1)
                    {
                        _chunckSize = Convert.ToInt32(payload[1]);
                    }
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
                    if(payload.Length > 2)
                    {
                        _chunckSize = Convert.ToInt32(payload[2]);
                    }                   
                    _fileInfo = new FileInfo(payload[0]);
                    _fileSize = _fileInfo.Length;
                    break;
            }
        }

        public override void InputDirectionValidation(string[] payload)
        {
            base.InputDirectionValidation(payload);
            if(payload.Length < 1)
            {
                throw new Exception(String.Format("Не переданы данные для инициализации коснструктора в класс - {0}", this.GetType()));
            }
        }

        public override void OutputDirectionValidation(string[] payload)
        {
            base.OutputDirectionValidation(payload);
            if(payload.Length < 1)
            {
                throw new Exception(String.Format("Не переданы данные для инициализации коснструктора в класс - {0}", this.GetType()));
            }
        }

        public override void InOutDirectionValidation(string[] payload)
        {
            base.InOutDirectionValidation(payload);
            if(payload.Length < 2)
            {
                throw new Exception(String.Format("Не переданы данные для инициализации коснструктора в класс - {0}", this.GetType()));
            }
        }

        public override void Dispose()
        {
            binaryWriter.Close();
            binaryReader.Close();
            _inputStream.Dispose();
            _outputStream.Dispose();

            _fileInfo = null;
            
        }

        public long GetObjectSize()
        {
            return _fileSize;
        }

        public int GetChunkSize()
        {
            return _chunckSize;
        }

        public static string[] GetInputInfo()
        {
            return new List<string>
            {
                "[R][InputPath;ChunkSize;DataProvider]",
                "[R][InputPath] - Путь до файла из которого будте производится считывание данных",
                "[O][ChunkSize] - Размер блока который необходимо будет считать,\n\rВ случае если не указать в явном виде, то по дефолту будет использован параметр '4096'",
                "[R][DataProvider] - Тип предоставления доступа к данным"
            }.ToArray();
        }

        public static string[] GetOutputInfo()
        {
            return new List<string>
            {
                "[O][OutputPath;DataProvider]",
                "[O][OutputPath] - Путь до файла в который будет производится запись данных\n\rВ случае если не указать в явном виде, файл заархивируется в туже папку где и оригинальный файл",
                "[O][DataProvider] - Тип предоставления доступа к данным, если не указать то будет использован точно такой же метод как в Input"
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
