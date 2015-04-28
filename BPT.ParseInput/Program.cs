using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPT.ParseInput
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input the file path ");
            string filepath;
            //filepath = Console.ReadLine() ;
            filepath = "C:\\localshare\\project 2B+tree\\input.txt";

            parsingFile file1 = new parsingFile(filepath);

        }
    }
}
