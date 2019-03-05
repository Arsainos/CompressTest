using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CompressionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                char[] method = args[0].ToCharArray();
                method[0] = char.ToUpper(method[0]);

                var aggregateString = args.Aggregate("",(current,next) =>
                {
                    return current + " " + next;
                });

                Regex reg = new Regex(@"\[(.*?)\]", RegexOptions.IgnoreCase);
                var matches = reg.Matches(aggregateString);
                string[] arguments = new string[2];

                for(int i=0; i<matches.Count;i++)
                {
                    arguments[i] = matches[i].Groups[1].Value;
                }

                Utils.CLI.Commands.CheckTheInput(String.Format("{0} -a [Gzip] -provider [Block] -i [File] -ip [{1};4096] -o [File] -op [{2}]",
                    new string(method),
                    arguments[0],
                    arguments[1]));
            }
            else
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


}
