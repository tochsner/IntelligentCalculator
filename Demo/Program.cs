using IntelligentCalculator;
using System;
using System.Collections.Generic;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Bitte gib eine Rechnung in RPN ein:");

                    string instructions = Console.ReadLine();

                    List<string> list = new List<string>();

                    foreach (String s in instructions.Split(' '))
                        list.Add(s);

                    Module mainModule = new Module("", 0, list);

                    Console.WriteLine(mainModule.Compute(new List<double>()));

                    Console.WriteLine("Kennst du die richtige Antwort? (y, n)");

                    if (Console.ReadLine() == "y")
                    {
                        Console.WriteLine("Bitte gib die korrekte Antwort ein.");
                        ModuleManager.Train(instructions, Double.Parse(Console.ReadLine()));
                    }
                }
                catch
                {

                }
                        
            }
        }

        static void PrintArray(List<string> array)
        {
            foreach (string s in array)
                Console.Write(s + " ");

            Console.WriteLine();
        }
    }
}
