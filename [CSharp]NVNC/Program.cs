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
            VncServer s = new VncServer("robert", 5900, "VNC-Test");
            s.Start();
            Console.ReadLine();
        }
    }
}
