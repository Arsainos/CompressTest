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

        public FileBlock(string[] payload,Enums.DirectionType directionType) : base(payload)
        {
            switch(directionType)
            {
                case Enums.DirectionType.In:
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

                case Enums.DirectionType.Out:
                    OutputDirectionValidation(payload);
                    _outputStream = CheckOutputFileExist(payload[0]);
                    binaryWriter = new BinaryWriter(_outputStream);
                    break;

                case Enums.DirectionType.InOut:
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
            if(binaryWriter != null)binaryWriter.Close();
            if(binaryReader != null)binaryReader.Close();
            if(_inputStream != null)_inputStream.Dispose();
            if(_outputStream != null)_outputStream.Dispose();

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
                "[R][InputPath;ChunkSize]",
                "[R][InputPath] - Путь до файла из которого будте производится считывание данных",
                "[O][ChunkSize] - Размер блока который необходимо будет считать,\n\rВ случае если не указать в явном виде, то по дефолту будет использован параметр '4096'"
            }.ToArray();
        }

        public static string[] GetOutputInfo()
        {
            return new List<string>
            {
                "[R][OutputPath]",
                "[R][OutputPath] - Путь до файла в который будет производится запись данных\n\r"
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

        public byte[] ReadAll(out bool last)
        {
            byte[] result = binaryReader.ReadBytes((int)_fileSize);
            last = binaryReader.BaseStream.Position == binaryReader.BaseStream.Length;
            return result;         
        }

        public byte[] ReadNext(out bool last)
        {
            byte[] result = binaryReader.ReadBytes(_chunckSize);
            last = binaryReader.BaseStream.Position == binaryReader.BaseStream.Length;
            return result;
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

        public byte[] ReadArray(int count)
        {
            return binaryReader.ReadBytes(count);
        }

        public byte[] SpecificRead(byte[] magicNumber, out bool last)
        {
            List<byte> resultByteArray = new List<byte>();
            resultByteArray.AddRange(magicNumber.ToList());

            int count = 0;
            bool NextValue = false;
            last = false;
            while(count != magicNumber.Length)
            {
                if (binaryReader.BaseStream.Position == binaryReader.BaseStream.Length)
                {
                    last = true;
                    break;
                }

                byte current = binaryReader.ReadByte();
                resultByteArray.Add(current);

                

                if ((current == magicNumber[0]) && !NextValue) { count++; NextValue = true; }
                else if ((current == magicNumber[count])
                    && NextValue)
                {
                    bool canGo = false;
                    var a = resultByteArray[resultByteArray.Count - 2];
                    for (int i=0;i<count;i++)
                    {
                        if(resultByteArray[resultByteArray.Count - 2 - i]==magicNumber[count-1-i])
                        {
                            canGo = true;
                        }
                        else
                        {
                            canGo = false;
                            break;
                        }
                    }

                    if(canGo)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0; NextValue = false;
                    }
                }             
                else { count = 0; NextValue = false; }
            }

            if(!last)resultByteArray.RemoveRange(resultByteArray.Count - magicNumber.Length, magicNumber.Length);

            /*
            Console.CursorLeft = 0;
            Console.Write("Progress: " + (binaryReader.BaseStream.Position / binaryReader.BaseStream.Length)*100 +"%");
            */

            return resultByteArray.ToArray();
        }
    }
}
