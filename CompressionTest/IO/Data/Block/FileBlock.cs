using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompressionTest.IO.Interfaces;
using System.IO;

namespace CompressionTest.IO.Data.Block
{
    /// <summary>
    /// Class implements the block reading/writing mechanics
    /// </summary>
    class FileBlock : Data.AbstractClasses.AbstractDataSource, IBlockProvider
    {
        /// <summary>
        /// Stream to reading file
        /// </summary>
        protected FileStream _inputStream;
        /// <summary>
        /// Stream to writing file
        /// </summary>
        protected FileStream _outputStream;
        /// <summary>
        /// Wrap for stream, to work with binary data
        /// </summary>
        protected BinaryReader binaryReader;
        /// <summary>
        /// Wrap for stream, to work with binary data
        /// </summary>
        protected BinaryWriter binaryWriter;
        /// <summary>
        /// Default chunk size
        /// </summary>
        private int _chunckSize = 4096;
        /// <summary>
        /// Size of the input file
        /// </summary>
        protected long _fileSize;
        /// <summary>
        /// Info about input file
        /// </summary>
        protected FileInfo _fileInfo;

        public FileBlock(string[] payload,Enums.DirectionType directionType) : base(payload)
        {
            //Maybe need to move this to the factory method
            switch(directionType)
            {
                //Init a class for only input
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
                //Init a class for only output
                case Enums.DirectionType.Out:
                    OutputDirectionValidation(payload);
                    _outputStream = CheckOutputFileExist(payload[0]);
                    binaryWriter = new BinaryWriter(_outputStream);
                    break;
                //Init a class for in/out 
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

        /// <summary>
        /// Overriding abstract methods for direction input parameters validation
        /// </summary>
        /// <param name="payload">Input data for initialize</param>
        public override void InputDirectionValidation(string[] payload)
        {
            base.InputDirectionValidation(payload);
            if(payload.Length < 1)
            {
                throw new Exception(String.Format("Initialize parameters is not passed to the constructor - {0}", this.GetType()));
            }
        }
        /// <summary>
        /// Overriding abstract methods for direction output parameters validation
        /// </summary>
        /// <param name="payload">Input data for initialize</param>
        public override void OutputDirectionValidation(string[] payload)
        {
            base.OutputDirectionValidation(payload);
            if(payload.Length < 1)
            {
                throw new Exception(String.Format("Initialize parameters is not passed to the constructor - {0}", this.GetType()));
            }
        }
        /// <summary>
        /// Overriding abstract methods for 2way direction parameters validation
        /// </summary>
        /// <param name="payload">Input data for initialize</param>
        public override void InOutDirectionValidation(string[] payload)
        {
            base.InOutDirectionValidation(payload);
            if(payload.Length < 2)
            {
                throw new Exception(String.Format("Initialize parameters is not passed to the constructor - {0}", this.GetType()));
            }
        }

        /// <summary>
        /// Get rid of nasty things
        /// </summary>
        public override void Dispose()
        {
            if(binaryWriter != null)binaryWriter.Close();
            if(binaryReader != null)binaryReader.Close();
            if(_inputStream != null)_inputStream.Dispose();
            if(_outputStream != null)_outputStream.Dispose();

            _fileInfo = null;
            
        }

        /// <summary>
        /// Get size of the current file
        /// </summary>
        /// <returns>File size</returns>
        public long GetObjectSize()
        {
            return _fileSize;
        }

        /// <summary>
        /// Get the current working chunk size
        /// </summary>
        /// <returns>Chunk size</returns>
        public int GetChunkSize()
        {
            return _chunckSize;
        }

        /// <summary>
        /// String info about the input payload
        /// </summary>
        /// <returns>Input direction payload info</returns>
        public static string[] GetInputInfo()
        {
            return new List<string>
            {
                "[R][InputPath;ChunkSize]",
                "[R][InputPath] - Path to the file in the system",
                "[O][ChunkSize] - Size of the byte block,\n\rIf not set, the program will be using the size of '4096' as default value"
            }.ToArray();
        }

        /// <summary>
        /// String info about the output payload
        /// </summary>
        /// <returns>Output direction payload info</returns>
        public static string[] GetOutputInfo()
        {
            return new List<string>
            {
                "[R][OutputPath]",
                "[R][OutputPath] - Path to the destination file\n\r"
            }.ToArray();
        }

        /// <summary>
        /// Check for the input file exist
        /// </summary>
        /// <param name="inputPath">path to the read file</param>
        /// <returns>Stream to that file</returns>
        private FileStream CheckInputFileExist(string inputPath)
        {
            if(File.Exists(inputPath))
            {
                return File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                throw new Exception("Can't locate input file at this path!");
            }
        }

        /// <summary>
        /// Check for the output file exists
        /// </summary>
        /// <param name="outputPath">path to the write file</param>
        /// <returns>Stream to that file</returns>
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

        /// <summary>
        /// Read all data from the stream
        /// </summary>
        /// <param name="last">flag for the last chunk read</param>
        /// <returns>Slice of byte of all the stream</returns>
        public byte[] ReadAll(out bool last)
        {
            byte[] result = binaryReader.ReadBytes((int)_fileSize);
            last = binaryReader.BaseStream.Position == binaryReader.BaseStream.Length;
            return result;         
        }

        /// <summary>
        /// Read fixed size of chunk from stream
        /// </summary>
        /// <param name="last">flag for the last chunk read</param>
        /// <returns>Slice of byte of last block from stream</returns>
        public byte[] ReadNext(out bool last)
        {
            byte[] result = binaryReader.ReadBytes(_chunckSize);
            last = binaryReader.BaseStream.Position == binaryReader.BaseStream.Length;
            return result;
        }

        /// <summary>
        /// Write all data to the output
        /// </summary>
        /// <param name="binary"></param>
        public void WriteAll(byte[] binary)
        {
            binaryWriter.Write(binary, 0, binary.Length);
            binaryWriter.Flush();
        }

        /// <summary>
        /// Write chunk of data to the output
        /// </summary>
        /// <param name="binary"></param>
        public void WriteNext(byte[] binary)
        {
            binaryWriter.Write(binary, 0, binary.Length);
            binaryWriter.Flush();
        }
        /// <summary>
        /// Read a fixed size of bytes from input stream
        /// </summary>
        /// <param name="count">number of bytes to read</param>
        /// <returns>Readed slice of bytes</returns>
        public byte[] ReadArray(int count)
        {
            return binaryReader.ReadBytes(count);
        }

        /// <summary>
        /// Specific read through open stream
        /// </summary>
        /// <remarks>
        /// Need this method for the Decompression method. Cause then we are using the multi thread compression.
        /// Every peace of chunk will be compressed with it's own header of compression protocol.
        /// Read about the protocols you can in the internet. Something like RFC GZip.
        /// This method reads until the specific header and get the slice of data byte from stream.
        /// </remarks>
        /// <param name="magicNumber">Protocol header to search</param>
        /// <param name="last">flag for the last chunk read</param>
        /// <returns>Slice of compressed data</returns>
        public byte[] SpecificRead(byte[] magicNumber, out bool last)
        {
            //Init result array of bytes
            List<byte> resultByteArray = new List<byte>();
            //Put the magic numbers to the begining of the array
            resultByteArray.AddRange(magicNumber.ToList());

            //algorithm
            int count = 0;
            bool NextValue = false;
            last = false;
            while(count != magicNumber.Length)
            {
                //if this is the end of stream, then exit and send back slice of readed data
                if (binaryReader.BaseStream.Position == binaryReader.BaseStream.Length)
                {
                    last = true;
                    break;
                }

                //read last byte from stream
                byte current = binaryReader.ReadByte();
                //add it to the result array
                resultByteArray.Add(current);

                
                //check for the pattern of header
                //if current byte is equal to the first byte of pattern then look next
                if ((current == magicNumber[0]) && !NextValue) { count++; NextValue = true; }
                //If next byte is what we are looking for, then check the previous bytes for pattern match
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

            //Uncomment this to see the procentage of working, but will increase slightly cpu working
            /*
            Console.CursorLeft = 0;
            Console.Write("Progress: " + (binaryReader.BaseStream.Position / binaryReader.BaseStream.Length)*100 +"%");
            */

            return resultByteArray.ToArray();
        }
    }
}
