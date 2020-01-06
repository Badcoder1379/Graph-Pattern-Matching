﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching
{
    class TestMaker
    {
        public void Main()
        {
            int nodeCount = 3;
            int edgeCount = 2;
            try
            {
                HashSet<Guid> allIDs = new HashSet<Guid>();
                StreamWriter sw = new StreamWriter("../src/nodes.txt");
                sw.WriteLine(nodeCount);
                var rnd = new Random();
                for (int i = 0; i < nodeCount; i++)
                {
                    var newGuid = Guid.NewGuid();
                    allIDs.Add(newGuid);
                    char A = (char)('A' + rnd.Next(4));
                    sw.WriteLine(newGuid.ToString() + ' ' + A);
                }
                sw.Flush();
                sw.Close();


                sw = new StreamWriter("../src/edges.txt");
                sw.WriteLine(edgeCount);
                for (int i = 0; i < edgeCount; i++)
                {
                    var newGuid = Guid.NewGuid();
                    char A = (char)('A' + rnd.Next(4));
                    Guid a, b;
                    do
                    {
                        a = allIDs.ToList()[rnd.Next(nodeCount)];
                        b = allIDs.ToList()[rnd.Next(nodeCount)];
                    } while (a == b);
                    sw.WriteLine(newGuid.ToString() + ' ' + a.ToString() + ' ' + b.ToString() + ' ' + A);
                }
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
