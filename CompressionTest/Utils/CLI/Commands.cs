using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CompressionTest.Utils.CLI
{
    /// <summary>
    /// Class for CLI processing
    /// </summary>
    static class Commands
    {
        /// <summary>
        /// Greeting string
        /// </summary>
        public const string Greeting = "================================================\n\r" +
                                       "= Welcom to the multi thread compression prog. =\n\r" +
                                       "================================================\n\r";
        /// <summary>
        /// Executable string
        /// </summary>
        public const string CMDExecutables = "===============================\n\r" +
                                             "Avaliable commands:\n\r" +
                                             " EXIT - Exit from the programm\n\r" +
                                             " INFO - Get info\n\r" +
                                             " Compress - Compress file\n\r" +
                                             " Decompress - Decompress file\n\r" +
                                             " Example - Example of using\n\r";
        /// <summary>
        /// Exit string
        /// </summary>
        const string exit = "\n\rExiting the programm\n\r";
        /// <summary>
        /// Info string
        /// </summary>
        const string info = "\n\rInformation:\n\r" +
                            "Compress -a [R][Algorithm] -provider [R][ProviderType] -i [R][InputType] -ip [R][Parameters] -o [R][OutputType] -op [R][Parameters]\n\r" +
                            "Decompress -a [Algorithm] -i [InputType] -ip [Parameters] -o [OutputType] -op [Parameters]\n\r" +
                            "Parameters type [R] - required, [O] - optional\n\r"+
                            "To get more info type - compress/decompress [InputParameter]\n\r xample: INFO compress -a\n\r";
        /// <summary>
        /// Algorithm info string
        /// </summary>
        const string info_a = "Compress/Decompress [Algorithm] - Avaliable compression algorithms:\n\r";
        /// <summary>
        /// Source types info string
        /// </summary>
        const string info_t = "Compress/Decompress [Input/Output Types] - Avaliable source types:\n\r";
        /// <summary>
        /// Source input ouput parameters string
        /// </summary>
        const string info_ipop = "Compress/Decompress [Input/Output Parameters] - Avaliable parameters for source types:\n\r";
        /// <summary>
        /// Provider info string
        /// </summary>
        const string info_provider = "Compress/Decompress [Provider] - Avaliable providers:\n\r";
        const string compress = "\n\rStarting the compression\n\r";
        const string decompress = "\n\rStarting the decompression\n\r";
        //Example string
        const string example = @"Compress -a Gzip -i File -ip C:\1.txt;1024 -o File -op D:\1.txt\n\r";

        private static readonly List<string> CMDs = new List<string>() { "EXIT","INFO","Compress","Decompress","TEST","Example" };
        private static readonly List<string> InfoCMDs = new List<string>() { "-a", "-provider", "-i", "-ip" ,"-o","-op"};

        const string wrong_type = "Incorrect input command. Try again or type ('EXIT') to leave the programm.\n\r";

        /// <summary>
        /// main procesing method
        /// </summary>
        /// <param name="input">Get command from cmd</param>
        /// <returns>return result value after processing command</returns>
        public static string CheckTheInput(string input)
        {
            try
            {
                //Split income string
                //This is just a processing of the command, nothing intresting
                string[] splitted = input.Split(' ');
                if (splitted.Length > 0 && !string.IsNullOrEmpty(splitted[0]))
                {
                    if (CMDs.Contains(splitted[0]))
                    {
                        switch (splitted[0])
                        {
                            case "EXIT":
                                Console.WriteLine(exit);
                                break;

                            case "INFO":
                                if (splitted.Length < 2)
                                {
                                    Console.WriteLine(info);
                                }
                                else if (splitted.Length > 1 && (splitted[1] == "Compress" || splitted[1] == "Decompress"))
                                {
                                    if (splitted.Length > 2)
                                    {
                                        if (InfoCMDs.Contains(splitted[2]))
                                        {
                                            switch (splitted[2])
                                            {
                                                case "-a":
                                                    Console.WriteLine(StringAppender(info_a, Compression.Utils.API.getCompressionAlgorithms().Keys.ToArray()));
                                                    break;

                                                case "-i":
                                                case "-o":
                                                    Console.WriteLine(StringAppender(info_t, IO.Utils.API.GetSourceTypes().Keys.ToArray()));
                                                    break;

                                                case "-ip":
                                                case "-op":
                                                    Console.WriteLine(info_ipop);
                                                    foreach (var t in IO.Utils.API.GetSourceTypes())
                                                    {
                                                        Console.WriteLine("============");
                                                        Console.WriteLine(t);
                                                        if (t.Key == "File")
                                                        {
                                                            Console.WriteLine(StringAppender("Input Parameters\n\r", IO.Utils.API.GetBlockFileInfoInput()));
                                                            Console.WriteLine(StringAppender("OutputParameters\n\r", IO.Utils.API.GetBlockFileInfoOutput()));
                                                        }
                                                        Console.WriteLine("============");
                                                    }
                                                    break;

                                                case "-provider":
                                                    Console.WriteLine(info_provider);
                                                    foreach (var t in IO.Utils.API.GetDataProvidersTypes())
                                                    {
                                                        Console.WriteLine(t.Key);
                                                    }
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine(wrong_type);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(wrong_type);
                                    }
                                }
                                else Console.WriteLine(wrong_type);
                                break;

                            //Compute the compress and decompress strings
                            case "Decompress":
                            case "Compress":
                                //Get the algorithm from the input 
                                var algorithm = Compression.Utils.API.getCompressionAlgorithms()[GetParamsFromCMDString(input, "a")[0]];
                                //Get compression enum type from input string
                                var method = Compression.Utils.API.getCompressionTypes()[splitted[0]];
                                //Get provider enum type from input string
                                var provider = IO.Utils.API.GetDataProvidersTypes()[GetParamsFromCMDString(input, "provider")[0]];
                                //Generate the high level compression class from algorithm input
                                var compression = Compression.FabricMethods.CompressionFabricMethod.Create(algorithm);
                                //Get source enum type for input
                                var sourceInput = IO.Utils.API.GetSourceTypes()[GetParamsFromCMDString(input, "i")[0]];
                                //Get source enum type for output
                                var sourceOutput = IO.Utils.API.GetSourceTypes()[GetParamsFromCMDString(input, "o")[0]];
                                //Get the additional parameters for input source
                                var inputParameters = GetParamsFromCMDString(input, "ip");
                                //Get the additional parameters for output source
                                var outputParameters = GetParamsFromCMDString(input, "op");

                                switch (method)
                                {
                                    case Compression.Enums.CompressionType.Compress:
                                        Console.WriteLine(compress);
                                        break;

                                    case Compression.Enums.CompressionType.Decompress:
                                        Console.WriteLine(decompress);
                                        break;
                                }

                                //Generate worker for processing
                                switch (provider)
                                {
                                    //If user type the block provider
                                    case IO.Enums.ProviderType.Block:
                                        //Generate input provider type, to data from
                                        using (var dataProviderInput = IO.FabricMethods.IOFabricMethod<IO.DataProviders.BlockDataProvider>
                                            .Create(sourceInput, IO.Enums.DirectionType.In, inputParameters))
                                        {
                                            //if the income method decompress, then get info about the specific implementation read from RFC
                                            if(method == Compression.Enums.CompressionType.Decompress)
                                            compression.CompressingSpecification.completeMask =
                                                compression.CompressingSpecification.magicNumber.Concat(
                                                dataProviderInput.ReadArray(
                                                    compression.CompressingSpecification.maskLength +
                                                    compression.CompressingSpecification.magicNumber.Length)
                                                    .Skip(compression.CompressingSpecification.magicNumber.Length)).ToArray();

                                            //Generate ouput provider type, to write data
                                            using (var dataProviderOutput = IO.FabricMethods.IOFabricMethod<IO.DataProviders.BlockDataProvider>
                                                .Create(sourceOutput, IO.Enums.DirectionType.Out, outputParameters))
                                            {
                                                //Get the optimal system component to execute on
                                                var funcResult = Computation.Algorithm.StrategyChooser.FindMostRelevantComponent(
                                                    new object[] { inputParameters[0][0].ToString() },
                                                    Computation.FabricMethods.AlgorithmFactoryMethod
                                                        .GetFunc(Computation.Enums.Algorithms.LowestIdleTime));

                                                Console.WriteLine("[Output]: Set method for computation to - {0}", funcResult.ComputationType);

                                                //Switch for different realizaions
                                                switch (funcResult.ComputationType)
                                                {
                                                    //Block CPU
                                                    case Computation.Enums.ComputationType.CPU:
                                                        using (var CPU = new Computation.Workers.WorkerProvider(
                                                                new Computation.Data.Workers.BlockCpuWorker(
                                                                    dataProviderInput,
                                                                    dataProviderOutput,
                                                                    compression,
                                                                    method,
                                                                    (int)funcResult.AdditionalInfo)))
                                                        {
                                                            CPU.Start();
                                                        }
                                                        break;
                                                    
                                                    //Block Disk
                                                    case Computation.Enums.ComputationType.Disk:
                                                        using (var Disk = new Computation.Workers.WorkerProvider(
                                                                new Computation.Data.Workers.BlockDiskWorker(
                                                                    dataProviderInput,
                                                                    dataProviderOutput,
                                                                    compression,
                                                                    method)))
                                                        {
                                                            Disk.Start();
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                }

                                break;

                            /*
                            case "TEST":
                                new Tests.Tests();
                                Console.WriteLine("\n\r");
                                break;
                            */

                            case "Example":
                                Console.WriteLine(example);
                                break;
                        }
                        return input;
                    }
                    else
                    {
                        Console.WriteLine(wrong_type);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n\r"+ex.Message);
            }
            Console.WriteLine("Waiting for command...\n\r");            
            return input;
        }

        /// <summary>
        /// Method to to static string the string array
        /// </summary>
        /// <param name="append">out const info string</param>
        /// <param name="input">array of string to append</param>
        /// <returns>new string for insert to the console</returns>
        static string StringAppender(string append,string[] input)
        {
            StringBuilder sb = new StringBuilder(append);

            for(int i=0;i<input.Length;i++)
            {
                sb.Append(input[i]).Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Read data from cmd by given parameter
        /// </summary>
        /// <param name="input">cmd string</param>
        /// <param name="parameter">parameter to read</param>
        /// <returns>array of options for given parameter</returns>
        static string[] GetParamsFromCMDString(string input,string parameter)
        {
            Regex reg = new Regex(String.Format(@"(-{0}\s([\w+\d+;:\\\.\-]*))", parameter), RegexOptions.IgnoreCase);
            var matches = reg.Matches(input);

            if(matches.Count > 0)
            {
                return matches[0].Groups[2].Value.Split(';');
            }
            else
            {
                throw new Exception(wrong_type);
            }
        }
    }
}
