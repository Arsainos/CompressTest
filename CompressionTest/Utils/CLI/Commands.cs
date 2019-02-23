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

        private static readonly List<string> CMDs = new List<string>() { "EXIT","INFO","Compress","Decompress" };

        public static string CheckTheInput(string input)
        {
            if (CMDs.Contains(input))
            {
                switch (input.Split(' ')[0])
                {
                    case "EXIT":
                        Console.WriteLine("\n\r Выход из программы.\n\r");
                        break;

                    case "INFO":
                        Console.WriteLine("\n\r Информация о программе:\n\r");
                        break;

                    case "Compress":
                        Console.WriteLine("\n\r Запускаю механизм архивирование документа\n\r");
                        break;

                    case "Decompress":
                        Console.WriteLine("\n\r Запускаю механизм разархивирования документа\n\r");
                        break;
                }
                return input;
            }

            Console.WriteLine(" Введена неверная команда. Попытайтесь снова или введите ('EXIT') для выхода.\n\r");
            return input;
        }

    }
}
