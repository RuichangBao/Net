using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //NetManager1 netManager1 = new NetManager1();
            //Test test = new Test();
            NetManager2 netManager2 = new NetManager2();


            while (true)
            {
                if(Console.ReadLine().ToLower()== "clear")
                {
                    //Console.Clear();
                    Console.WriteLine("清屏");
                }
            }
        }

    }
}