using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CompressionTest.Utils.CLI
{
    static class Commands
    {
        public const string Greeting = "===============================================\n\r" +
                                       "= Добро пожаловать в программу сжатия данных. =\n\r" +
                                       "===============================================\n\r";

        public const string CMDExecutables = "===============================\n\r" +
                                             " Перечень доступных команд:\n\r" +
                                             " EXIT - Выход\n\r" +
                                             " INFO - Информация\n\r" +
                                             " Compress - сжатие данных\n\r" +
                                             " Decompress - расжатие данных\n\r" +
                                             " Example - пример использования\n\r";

        const string exit = "\n\r Выход из программы.\n\r";
        const string info = "\n\r Информация о программе:\n\r" +
                            " Coompress -a [R][Algorithm] -provider [R][ProviderType] -i [R][InputType] -ip [R][Parameters] -o [R][OutputType] -op [R][Parameters]\n\r" +
                            " Decompress -a [Algorithm] -i [InputType] -ip [Parameters] -o [OutputType] -op [Parameters]\n\r" +
                            " Типы параметров [R] - Обязательный, [O] - Не обязательный\n\r"+
                            " Для более подробной информации напишите INFO compress/decompress [InputParameter]\n\r Например: INFO compress -a\n\r";
        const string info_a = " compress/decompress [Algorithm] - список доступных Алгоритмов компресии:\n\r";
        const string info_t = " compress/decompress [Input/Output Types] - список доступных источников:\n\r";
        const string info_ipop = " compress/decompress [Input/Output Parameters] - Для типов данных доступны следующие параметры:\n\r";
        const string info_provider = " compress/decompress [Provider] - список доступных провайдеров:\n\r";
        const string compress = "\n\r Запускаю механизм архивирование документа\n\r";
        const string decompress = "\n\r Запускаю механизм разархивирования документа\n\r";

        const string example = @"Compress -a Gzip -i File -ip C:\1.txt;1024 -o File -op D:\1.txt\n\r";

        private static readonly List<string> CMDs = new List<string>() { "EXIT","INFO","Compress","Decompress","TEST","Example" };
        private static readonly List<string> InfoCMDs = new List<string>() { "-a", "-provider", "-i", "-ip" ,"-o","-op"};

        public static string CheckTheInput(string input)
        {

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
                            if(splitted.Length<2)
                            {
                                Console.WriteLine(info);
                            }
                            else if(splitted.Length>1 && (splitted[1] == "Compress" || splitted[1] == "Decompress"))
                            {
                                if (splitted.Length >2)
                                {
                                    if (InfoCMDs.Contains(splitted[2]))
                                    {
                                        switch (splitted[2])
                                        {
                                            case "-a":
                                                Console.WriteLine(StringAppender(info_a,Compression.Utils.API.getCompressionAlgorithms().Keys.ToArray()));
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
                                                foreach(var t in IO.Utils.API.GetDataProvidersTypes())
                                                {
                                                    Console.WriteLine(t.Key);
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(" Введена неверная команда. Попытайтесь снова или введите ('EXIT') для выхода.\n\r");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(" Введена неверная команда. Попытайтесь снова или введите ('EXIT') для выхода.\n\r");
                                }
                            }
                            else Console.WriteLine(" Введена неверная команда. Попытайтесь снова или введите ('EXIT') для выхода.\n\r");                           
                            break;

                        case "Decompress":
                        case "Compress":

                            var algorithm = Compression.Utils.API.getCompressionAlgorithms()[GetParamsFromCMDString(input, "a")[0]];
                            var method = Compression.Utils.API.getCompressionTypes()[splitted[0]];
                            var provider = IO.Utils.API.GetDataProvidersTypes()[GetParamsFromCMDString(input, "provider")[0]];
                            var compression = Compression.FabricMethods.CompressionFabricMethod.Create(algorithm);
                            var sourceInput = IO.Utils.API.GetSourceTypes()[GetParamsFromCMDString(input, "i")[0]];
                            var sourceOutput = IO.Utils.API.GetSourceTypes()[GetParamsFromCMDString(input, "o")[0]];
                            var inputParameters = GetParamsFromCMDString(input, "ip");
                            var outputParameters = GetParamsFromCMDString(input, "op");
                            
                            switch(method)
                            {
                                case Compression.Enums.CompressionType.Compress:
                                    Console.WriteLine(compress);
                                    break;

                                case Compression.Enums.CompressionType.Decompress:
                                    Console.WriteLine(decompress);
                                    break;
                            }
                            

                            switch (provider)
                            {
                                case IO.Enums.ProviderType.Block:
                                    using (var dataProviderInput = IO.FabricMethods.IOFabricMethod<IO.DataProviders.BlockDataProvider>
                                        .Create(sourceInput, IO.Enums.DirectionType.In, inputParameters))
                                    {
                                        compression.CompressingSpecification.completeMask =
                                            compression.CompressingSpecification.magicNumber.Concat(
                                            dataProviderInput.ReadArray(
                                                compression.CompressingSpecification.maskLength +
                                                compression.CompressingSpecification.magicNumber.Length)
                                                .Skip(compression.CompressingSpecification.magicNumber.Length)).ToArray();

                                        using (var dataProviderOutput = IO.FabricMethods.IOFabricMethod<IO.DataProviders.BlockDataProvider>
                                            .Create(sourceOutput, IO.Enums.DirectionType.Out, outputParameters))
                                        {
                                            var funcResult = Computation.Algorithm.StrategyChooser.FindMostRelevantComponent(
                                                new object[] { inputParameters[0][0].ToString() }, 
                                                Computation.FabricMethods.AlgorithmFactoryMethod
                                                    .GetFunc(Computation.Enums.Algorithms.LowestIdleTime));

                                            Console.WriteLine("[Output]: Set method for computation to - {0}",funcResult.ComputationType);
                                            
                                            switch(funcResult.ComputationType)
                                            {
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

                        case "TEST":
                            new Tests.Tests();
                            Console.WriteLine("\n\r");
                            break;

                        case "Example":
                            Console.WriteLine(example);
                            break;
                    }
                    return input;
                }
                else
                {
                    Console.WriteLine(" Введена неверная команда. Попытайтесь снова или введите ('EXIT') для выхода.\n\r");
                }
            }

            Console.WriteLine("Введите команду...\n\r");            
            return input;
        }

        static string StringAppender(string append,string[] input)
        {
            StringBuilder sb = new StringBuilder(append);

            for(int i=0;i<input.Length;i++)
            {
                sb.Append(input[i]).Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        static string[] GetParamsFromCMDString(string input,string parameter)
        {
            Regex reg = new Regex(String.Format(@"(-{0}\s([\w+\d+;:\\\.]*))", parameter), RegexOptions.IgnoreCase);
            var matches = reg.Matches(input);

            if(matches.Count > 0)
            {
                return matches[0].Groups[2].Value.Split(';');
            }
            else
            {
                throw new Exception(" Введена неверная команда. Попытайтесь снова или введите ('EXIT') для выхода.\n\r");
            }
        }
    }
}
