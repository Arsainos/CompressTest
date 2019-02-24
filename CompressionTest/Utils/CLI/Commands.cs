using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Utils.CLI
{
    static class Commands
    {
        public const string Greeting = "===============================================\n\r" + 
                                       "= Добро пожаловать в программу сжатия данных. =\n\r" +
                                       "===============================================\n\r";

        public const string CMDExecutables = "===============================\n\r"+
                                             " Перечень доступных команд:\n\r" +
                                             " EXIT - Выход\n\r" +
                                             " INFO - Информация\n\r" +
                                             " Compress - сжатие данных\n\r" +
                                             " Decompress - расжатие данных\n\r";

        const string exit = "\n\r Выход из программы.\n\r";
        const string info = "\n\r Информация о программе:\n\r" +
                            " compress -a [Algorithm] -i [InputType] -ip [Parameters] -o [OutputType] -op [Parameters]\n\r" +
                            " decompress -a [Algorithm] -i [InputType] -ip [Parameters] -o [OutputType] -op [Parameters]\n\r" +
                            " Для более подробной информации напишите INFO compress/decompress [InputParameter]\n\r Например: INFO compress -a\n\r";
        const string info_a = " compress/decompress [Algorithm] - список доступных Алгоритмов компресии:\n\r";
        const string info_t = " compress/decompress [Input/Output Types] - список доступных источников:\n\r";
        const string compress = "\n\r Запускаю механизм архивирование документа\n\r";
        const string decompress = "\n\r Запускаю механизм разархивирования документа\n\r";

        private static readonly List<string> CMDs = new List<string>() { "EXIT","INFO","Compress","Decompress","TEST" };
        private static readonly List<string> InfoCMDs = new List<string>() { "-a", "-i", "-ip" ,"-o","-op"};

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
                            else if(splitted.Length>1 && (splitted[1] == "compress" || splitted[1] == "decompress"))
                            {
                                if (splitted.Length > 2)
                                {
                                    if (InfoCMDs.Contains(splitted[2]))
                                    {
                                        switch (splitted[2])
                                        {
                                            case "-a":
                                                Console.WriteLine(StringAppender(info_a,Compression.Utils.API.getCompressionAlgorithms()));
                                                break;

                                            case "-i":
                                            case "-o":
                                                Console.WriteLine(StringAppender(info_t, IO.Utils.API.GetSourceTypes()));
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

                        case "Compress":
                            Console.WriteLine(compress);
                            break;

                        case "Decompress":
                            Console.WriteLine(decompress);
                            break;

                        case "TEST":
                            new Tests.Tests();
                            Console.WriteLine("\n\r");
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
    }
}
