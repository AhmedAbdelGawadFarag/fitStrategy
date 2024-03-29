﻿using System;
using System.Collections.Generic;
using System.IO;


namespace FitStrategies
{
    class record
    {
        public int startpos;
        public int size;
        public string name;
        public record(string name)
        {
            this.name = name;

        }

    }
    class comparebysize : IComparer<record>
    {
        public int Compare(record left, record right)
        {

            return Math.Sign(left.size - right.size);
        }

    }
    interface ifitAlgorithms
    {

        bool reclaimSpace(record r, List<record> ls);
        void addtoavail_list(record r);

    }
    class fitAlgorithms
    {
        public int fragmentationSize = 0;

    }
    class firstfit : fitAlgorithms, ifitAlgorithms //list
    {
        public List<record> availList;

        public firstfit()
        {
            availList = new List<record>();
        }

        public bool reclaimSpace(record r, List<record> ls)
        {
            for (int i = availList.Count - 1; i >= 0; i--)
            {
                if (r.size <= availList[i].size)
                {
                    for (int j = 0; j < ls.Count; j++)
                    {
                        if (availList[i].name == ls[j].name)//remove from the list
                        {
                            ls[j] = r;
                            break;
                        }
                    }
                    availList.RemoveAt(i);
                    fragmentationSize -= r.size;
                    return true;
                }
            }

            return false;
        }
        public void addtoavail_list(record r)
        {
            fragmentationSize += r.size;
            availList.Add(r);
        }

    }
    class bestfit : fitAlgorithms, ifitAlgorithms
    {
        SortedSet<record> availList;
        public bestfit()
        {
            availList = new SortedSet<record>(new comparebysize());

        }
        public bool reclaimSpace(record r, List<record> ls)
        {
            foreach (var availRecord in availList)
            {
                if (r.size <= availRecord.size)
                {
                    for (int j = 0; j < ls.Count; j++)
                    {
                        if (availRecord.name == ls[j].name)//remove from the list
                        {
                            ls[j] = r;
                            break;
                        }
                    }
                    availList.Remove(availRecord);
                    fragmentationSize -= r.size;
                    return true;
                }
            }

            return false;
        }

        public void addtoavail_list(record r)
        {
            fragmentationSize += r.size;
            availList.Add(r);
        }


    }
    class readrecords
    {

        List<record> ls = new List<record>();
        ifitAlgorithms algo;

        public readrecords(FileStream fs, ifitAlgorithms algo)
        {
            StreamReader sr = new StreamReader(fs);
            this.algo = algo;
            while (sr.Peek() != -1)
            {
                string line = sr.ReadLine();
                if (line == "") continue;

                record r = Get_recordFromFile(line);

                if (line[0] == 'A' || line[0] == 'a')
                {
                    if (!algo.reclaimSpace(r, ls))
                    {
                        ls.Add(r);
                    }


                }
                else
                {
                    deleteRecordFromList(r);
                }


            }

            sr.Close();

        }


        public record Get_recordFromFile(String s)
        {
            int cnt = 0;
            string name = "";
            string size = "";

            int ptr1 = s.Length, ptr2 = s.Length;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ',' && cnt == 0)
                {
                    ptr1 = i;
                    cnt++;
                }
                else if (s[i] == ',' && cnt != 0)
                {
                    ptr2 = i;
                }

            }

            for (int i = ptr1 + 1; i < s.Length && s[i] != ','; i++)
            {
                if (s[i] != ' ')
                    name += s[i];
            }

            for (int i = ptr2 + 1; i < s.Length && s[i] != ','; i++)
            {
                if (s[i] != ' ')
                    size += s[i];
            }

            if (size == "") size = "0";

            record r = new record(name);
            r.size = int.Parse(size);

            return r;
        }
        public void deleteRecordFromList(record r)
        {
            for (int i = 0; i < ls.Count; i++)//search for record in the list
            {
                if (r.name.Equals(ls[i].name))
                {
                    algo.addtoavail_list(ls[i]);

                    string s = '*' + ls[i].name;
                    ls[i].name = s;

                    break;
                }
            }

        }
        public int getfragm()
        {
            fitAlgorithms m = (fitAlgorithms)algo;
            return m.fragmentationSize;
        }
        public void printFInalList()
        {
            Console.Write("the recrod after doing the fit algorithm: ");
            for (int i = 0; i < ls.Count; i++)
            {
                Console.Write(ls[i].name);
                Console.Write(" ");
            }
            Console.WriteLine();
        }

    }

    class Program
    {
        static bool checkstring(string s)
        {
            foreach (var d in s)
            {
                if (!char.IsDigit(d))
                    return false;
            }

            return ((s.Length == 1));

        }
        static char strtochar(string s)
        {
            return s[0];

        }
        static void Main(string[] args)
        {
            Console.WriteLine("ther are 5 files in this project each one of them have test cases");
            Console.WriteLine("1- testcase1 (dr hashem testcases)");
            Console.WriteLine("2- testcase2 ");
            Console.WriteLine("3- testcase3 ");
            Console.WriteLine("4- testcase4 ");
            Console.WriteLine("5- testcase5 ");

            while (true)
            {

                Console.ResetColor();
                Console.WriteLine("enter the number (1 or 2 or 3 or 4 or 5) to  choose the file with testcases");

                string filename = Console.ReadLine();
                if (!checkstring(filename) || (filename[0] > '5' || filename[0] < '1')) continue;

                string file = "testcase" + strtochar(filename) + ".txt";
                FileStream fs = new FileStream(file, FileMode.Open);

                Console.WriteLine();
                Console.WriteLine();
                readrecords r = new readrecords(fs, new firstfit());

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("first fit fragmantation is : " + r.getfragm());
                r.printFInalList();
                Console.WriteLine("\t");
                fs.Close();

                fs = new FileStream(file, FileMode.Open);
                readrecords r2 = new readrecords(fs, new bestfit());
                Console.WriteLine("best fit fragmantation is : " + r2.getfragm());
                r2.printFInalList();

                Console.WriteLine();
                Console.WriteLine();


                fs.Close();
            }
        }
    }
}
