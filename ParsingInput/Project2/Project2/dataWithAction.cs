using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2
{
    class dataWithAction
    {
        string action;
        List<string> inputData = new List<string>();

        public dataWithAction(string act, List<string> getInput)
        {
            action = act;
            inputData = getInput;
        }

        public void print()             // print function for testing
        {
            Console.WriteLine(action);
           foreach (string s in inputData)
           { Console.WriteLine(s); }
        }
    }
}
