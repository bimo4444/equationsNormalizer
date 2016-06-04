using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Logic logic = new Logic();
            string input, output;
            while (true)
            {
                try
                {
                    input = Console.ReadLine();
                    output = logic.Go(input);
                    Console.WriteLine(output);
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                    Console.WriteLine(x.StackTrace);
                }
            }
        }
    }
}
