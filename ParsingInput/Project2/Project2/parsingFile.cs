using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Project2
{
    class parsingFile
    {
       
        List<int> count = new List<int>();      //for record all the number of action
        List<string> record = new List<string>();//for store the txt input 
        List<dataWithAction> getData = new List<dataWithAction>(); // store the action with the corresponding data
        public parsingFile(string userInput)
        {
            //filePath = userInput;
            open(userInput);
        }

        public bool open(string filePath)
        {
            int counter = 0;
            string line;
            if (File.Exists(filePath))
            {
                System.IO.StreamReader file =
                new System.IO.StreamReader(filePath);
                while ((line = file.ReadLine()) != null)
                {
                    record.Add(line);       //store the whole input line by line into a string list
                   // counter++;
                }

                foreach (string com in record)  // record all the position of the action lines(*insert*select...etc)
                {
                    if ((com.Trim())[0] == '*')   
                    {
                        count.Add(counter);
                    }
                    //if ((com.Trim(new Char[] {' ', '.' } )).ToUpper() == "NEW PROJECT")
                    //{
                    //    count.Add(counter);
                    //}
                    counter++;
                }

                for(int i =0 ; i < count.Count;i++)
                {  
                    List<string> inputData = new List<string>();
                    //int j = count[i];

                    if (i < count.Count-1)
                    {
                        for (int j = count[i] + 1; j < count[i + 1]; j++)
                        {
                            inputData.Add(record[j]);
                        }
                        getData.Add(new dataWithAction(record[count[i]], inputData)); 
                    }
                    else
                    {
                        for (int j = count[i] + 1; j < record.Count; j++)
                        {
                            inputData.Add(record[j]);
                        }
                        getData.Add(new dataWithAction(record[count[i]], inputData)); 
                    }              

                }

                //getData[1].print();

                    return true;
            }
            else
            {
                Console.WriteLine("No Such file,check your path please!");
                return false;
            }
        }

    }
}
