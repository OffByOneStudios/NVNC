using System;
using System.Collections.Generic;
using System.Text;
using NVNC;

namespace LVNC
{
    class Program
    {
        static void Main(string[] args)
        {
            //VncServer s = new VncServer("T1T4N", 5900, "T!T@N-VNC");
            VncServer s = new VncServer("T!T@N", 5900, "T!T@N-VNC");
            try
            {
                s.Start();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            Console.ReadLine();
        }
    }
}
