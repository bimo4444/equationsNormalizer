using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IOService io = new IOService();
            Logic logic = new Logic();
            string input, output;
            Console.CancelKeyPress += ((s, e) =>
            {
                e.Cancel = true;
                if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                    return;                                         //to exit with ctrl-c
            });
            while (true)
            {
                try
                {
                    Console.WriteLine("ввудите уравнение или имя файла");
                    input = Console.ReadLine();
                    if(File.Exists(input))
                    {
                        io.Proceed(input);
                        Console.WriteLine("сохранено");
                    }
                    else
                    {
                        output = logic.Go(input);
                        Console.WriteLine(output);
                    }
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
