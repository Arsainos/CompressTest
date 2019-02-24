using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Utils.CLI.Commands.Greeting);

            string inputText = "";
            while (inputText != "EXIT")
            {
                Console.WriteLine(Utils.CLI.Commands.CMDExecutables);

                inputText = Console.ReadLine();

                Utils.CLI.Commands.CheckTheInput(inputText);
            }
        }
    }


}
