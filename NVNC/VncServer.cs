// NVNC - .NET VNC Server Library
// Copyright (C) 2014 T!T@N
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Drawing;
using System.Threading;
using System.IO;

namespace NVNC
{
    public class VncServer
    {
        protected RfbProtocol host;
        protected Framebuffer fb;

        private int _port;

        [System.ComponentModel.DefaultValue(5900)]
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public VncServer()
        {
            Size screenSize = ScreenSize();
            fb = new Framebuffer(screenSize.Width, screenSize.Height);

            fb.BitsPerPixel = 32;
            fb.Depth = 24;
            fb.BigEndian = true;
            fb.TrueColour = true;
            fb.RedShift = 16;
            fb.GreenShift = 8;
            fb.BlueShift = 0;
            fb.RedMax = fb.GreenMax = fb.BlueMax = 0xFF;
        }
        public VncServer(string password, int port, string name)
        {
            Size screenSize = ScreenSize();
            fb = new Framebuffer(screenSize.Width, screenSize.Height);

            fb.BitsPerPixel = 32;
            fb.Depth = 24;
            //fb.BigEndian = true;
            fb.TrueColour = true;
            fb.RedShift = 16;
            fb.GreenShift = 8;
            fb.BlueShift = 0;
            fb.RedMax = fb.GreenMax = fb.BlueMax = 0xFF;

            this.Password = password;
            this.Port = port;
            this.Name = name;
        }

        public void Start()
        {
            start();
            /*Thread t = new Thread(start);
            t.ApartmentState = ApartmentState.STA;
            t.Start();*/
        }
        private void start()
        {
            if (String.IsNullOrEmpty(Name))
                throw new ArgumentNullException("Name", "The VNC Server Name cannot be empty.");
            if (Port == 0)
                throw new ArgumentNullException("Port", "The VNC Server port cannot be zero.");
            Console.WriteLine("Started VNC Server at port: " + Port);

            fb.DesktopName = Name;
            host = new RfbProtocol(Port, Name);

            host.WriteProtocolVersion();
            Console.WriteLine("Wrote Protocol Version");

            host.ReadProtocolVersion();
            Console.WriteLine("Read Protocol Version");

            Console.WriteLine("Awaiting Authentication");
            if (!host.WriteAuthentication(Password))
            {
                Console.WriteLine("Authentication failed !");
                host.Close();
                Start();
            }
            else
            {
                Console.WriteLine("Authentication successfull !");

                bool share = host.ReadClientInit();
                Console.WriteLine("Share: " + share.ToString());

                Console.WriteLine("Server name: " + fb.DesktopName);
                host.WriteServerInit(this.fb);

                //RobotClient r = new RobotClient(host);
                //r.defaultPixel = fb;

                while ((host.isRunning))
                {
                    //new Thread(r.DoShit).Start();
                    //r.DoShit();
                    switch (host.ReadServerMessageType())
                    {
                        case 0:
                            Console.WriteLine("Read SetPixelFormat");
                            //Console.WriteLine("Before SPF:");
                            //fb.Print();
                            Framebuffer f = host.ReadSetPixelFormat(fb.Width, fb.Height);
                            if (f != null)
                                fb = f;
                            //Console.WriteLine("\nAfter SPF:");
                            //fb.Print();
                            break;
                        case 1:
                            Console.WriteLine("Read ReadColourMapEntry");
                            host.ReadColourMapEntry();
                            break;
                        case 2:
                            Console.WriteLine("Read SetEncodings");
                            host.ReadSetEncodings();
                            break;
                        case 3:
                            Console.WriteLine("Read FrameBufferUpdateRequest");
                            host.ReadFrameBufferUpdateRequest(fb);
                            break;
                        case 4:
                            Console.WriteLine("Read KeyEvent");
                            host.ReadKeyEvent();
                            break;
                        case 5:
                            Console.WriteLine("Read PointerEvent");
                            host.ReadPointerEvent();
                            break;
                        case 6:
                            Console.WriteLine("Read CutText");
                            host.ReadClientCutText();
                            break;
                    }
                }
                if (!host.isRunning)
                    Start();
            }
        }
        private Size ScreenSize()
        {
            Size s = new Size();
            s.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            s.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            return s;
        }

    }
}
