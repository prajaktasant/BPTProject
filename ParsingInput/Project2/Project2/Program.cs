using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input the file path ");
            string filepath = Console.ReadLine() ;

            parsingFile file1 = new parsingFile(filepath);

        }
    }
}
