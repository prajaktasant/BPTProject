using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BPT.Implementation;



namespace BPT.ParseInput
{
    class parsingFile
    {
        BPlusTree tree = new BPlusTree();
        public parsingFile(string userInput)
        {

            open(userInput);

        }
        public void open(string filePath)
        {
            string line;
            if (File.Exists(filePath))
            {
                System.IO.StreamReader file =
                new System.IO.StreamReader(filePath);
                line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    if (line == "")
                    {
                        line = file.ReadLine();
                        continue;
                    }
                    if (line == "*INSERT")
                    {
                        Console.WriteLine("--------------------------------------------------------------");
                        Console.WriteLine("*INSERT");
                        Console.WriteLine("--------------------------------------------------------------");
                        line = file.ReadLine();
                        while (!file.EndOfStream)
                        {
                            if (line != null)
                            {
                                if (line.StartsWith("*")) break;

                                if (line != "" && !line.StartsWith("*"))
                                {
                                    String name = line;
                                    try
                                    {
                                        tree.Insert(name, new char[224]);
                                        Console.WriteLine(name + " successfully Inserted");
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }
                            }

                            line = file.ReadLine();
                        }
                        continue;
                    }
                    if (line == "*SNAPSHOT")
                    {
                        Console.WriteLine("--------------------------------------------------------------");
                        Console.WriteLine("*SNAPSHOT");
                        Console.WriteLine("--------------------------------------------------------------");
                        try
                        {
                            List<string> snapshotList = tree.Snapshot();
                            Console.WriteLine("\nNumber of Records: " + snapshotList.ToArray()[0] + "\n");
                            Console.WriteLine("Number of Blocks: " + snapshotList.ToArray()[1] + "\n");
                            Console.WriteLine("Depth: " + snapshotList.ToArray()[2] + "\n");
                            Console.WriteLine("First and Last keys of all internal B + Tree Nodes in BFS traversal order: ");
                            List<string> firstAndLast = tree.BFSTreeTraversal();
                            foreach (string key in firstAndLast)
                            {
                                Console.WriteLine(key);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        line = file.ReadLine();
                        continue;
                    }
                    if (line == "*SEARCH")
                    {
                        Console.WriteLine("--------------------------------------------------------------");
                        Console.WriteLine("*SEARCH");
                        Console.WriteLine("--------------------------------------------------------------");
                        line = file.ReadLine();
                        while (!file.EndOfStream)
                        {
                            if (line != null)
                            {
                                if (line.StartsWith("*")) break;

                                if (line != "" && !line.StartsWith("*"))
                                {
                                    String name = line;
                                    try
                                    {
                                        String result = tree.Search(name);
                                        Console.WriteLine("Name: " + name + "\tConfidential Information: " + result);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }
                            }

                            line = file.ReadLine();
                        }
                        continue;
                    }
                    if (line == "*LIST")
                    {
                        Console.WriteLine("--------------------------------------------------------------");
                        Console.WriteLine("*LIST");
                        Console.WriteLine("--------------------------------------------------------------");
                        try
                        {
                            List<String> keyList = tree.List();
                            foreach (string key in keyList)
                            {
                                Console.WriteLine(key);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        line = file.ReadLine();
                        continue;
                    }
                    if (line == "*DELETE")
                    {
                        line = file.ReadLine();
                        Console.WriteLine("--------------------------------------------------------------");
                        Console.WriteLine("*DELETE");
                        Console.WriteLine("--------------------------------------------------------------");
                        while (!file.EndOfStream)
                        {
                            if (line != null)
                            {
                                if (line.StartsWith("*")) break;

                                if (line != "" && !line.StartsWith("*"))
                                {
                                    String name = line;
                                    try
                                    {
                                        tree.Delete(name);
                                        Console.WriteLine(name + " Deleted");
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }

                                }
                            }

                            line = file.ReadLine();
                        }
                        continue;
                    }
                    if (line == "*UPDATE")
                    {
                        line = file.ReadLine();
                        //while (!line.StartsWith("*") && !file.EndOfStream)
                        Console.WriteLine("--------------------------------------------------------------");
                        Console.WriteLine("*UPDATE");
                        Console.WriteLine("--------------------------------------------------------------");
                        while (!file.EndOfStream)
                        {
                            if (line != null)
                            {
                                if (line.StartsWith("*")) break;

                                if (line != "" && !line.StartsWith("*"))
                                {
                                    String name = line;
                                    String updateValue = file.ReadLine();
                                    while (updateValue == "")
                                    {
                                        updateValue = file.ReadLine();
                                    }
                                    try
                                    {
                                        tree.Update(name, updateValue);
                                        
                                        Console.WriteLine(name + " Updated with value: " + updateValue);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }
                            }
                            line = file.ReadLine();
                        }
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Operation in the file. \nPress Enter to exit");
                        Console.ReadLine();
                    }
                }
                Console.ReadLine();

            }
            else
            {
                Console.WriteLine("No Such file,check your path please!. \nPress Enter to Exit");
                Console.ReadLine();

            }
        }

    }
}

