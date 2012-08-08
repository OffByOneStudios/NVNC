// VncSharp - .NET VNC Client Library
// Copyright (C) 2008 David Humphrey
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
using System.IO;
using System.Net;
using System.Drawing;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Security.Cryptography;
using NVNC.Encodings;

namespace NVNC
{
    /// <summary>
    /// Contains methods and properties to handle all aspects of the RFB Protocol versions 3.3 - 3.8.
    /// </summary>
    public class RfbProtocol
    {
        // Encoding Constants
        public enum Encoding : int
        {
            RAW_ENCODING = 0,  //working
            COPYRECT_ENCODING = 1,  //working
            RRE_ENCODING = 2,  //working
            CORRE_ENCODING = 4,  //error
            HEXTILE_ENCODING = 5,  //working
            ZRLE_ENCODING = 16, //not implemented
        }

        // Server to Client Message-Type constants
        protected const int FRAMEBUFFER_UPDATE = 0;
        protected const int SET_COLOUR_MAP_ENTRIES = 1;
        protected const int BELL = 2;
        protected const int SERVER_CUT_TEXT = 3;

        // Client to Server Message-Type constants
        protected const byte SET_PIXEL_FORMAT = 0;
        protected const byte SET_ENCODINGS = 2;
        protected const byte FRAMEBUFFER_UPDATE_REQUEST = 3;
        protected const byte KEY_EVENT = 4;
        protected const byte POINTER_EVENT = 5;
        protected const byte CLIENT_CUT_TEXT = 6;

        //Version numbers
        protected int verMajor = 3;	// Major version of Protocol--probably 3
        protected int verMinor = 8;//8; // Minor version of Protocol--probably 3, 7, or 8

        //Shared flag
        private bool _shared;
        public bool Shared
        {
            get
            {
                return _shared;
            }
            set
            {
                _shared = value;
            }
        }

        public string CutText;

        protected Socket localClient;		// Network object used to communicate with host
        protected TcpListener serverSocket;

        protected NetworkStream stream;	// Stream object used to send/receive data
        protected BinaryReader reader;	// Integral rather than Byte values are typically
        protected BinaryWriter writer;	// sent and received, so these handle this.

        public bool isRunning;
        public bool isConnected
        {
            get
            {
                return localClient.Connected;
            }
        }

        //Port property
        private int _port;
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

        public string DisplayName;

        private List<Socket> clients = new List<Socket>();
        public List<Socket> Clients
        {
            get
            {
                return clients;
            }
        }

        //Supported encodings
        private uint[] _encodings;
        public uint[] Encodings
        {
            get
            {
                return _encodings;
            }
        }

        /// <summary>
        /// Gets the best encoding from the ones the client sent.
        /// </summary>
        /// <returns>Returns a Int32 representation of the encoding.</returns>
        public Encoding GetPreferredEncoding()
        {
            Encoding prefEnc = Encoding.RAW_ENCODING;
            try
            {
                for (int i = 0; i < Encodings.Length; i++)
                    if (((Encoding)Encodings[i]) == prefEnc)
                        return prefEnc;
            }
            catch
            {
                prefEnc = Encoding.RAW_ENCODING;
            }
            return prefEnc;
        }


        public RfbProtocol(int port, string displayname)
        {
            Port = port;
            DisplayName = displayname;
            Start();

        }

        /// <summary>
        /// Gets the Protocol Version of the remote VNC Host--probably 3.3, 3.7, or 3.8.
        /// </summary>
        public float ServerVersion
        {
            get
            {
                return (float)verMajor + (verMinor * 0.1f);
            }
        }

        public BinaryReader Reader
        {
            get
            {
                return reader;
            }
        }
        public BinaryWriter Writer
        {
            get
            {
                return writer;
            }
        }

        /*
        public ZRLECompressedReader ZrleReader
        {
            get
            {
                return zrleReader;
            }
        }
        public ZRLECompressedWriter ZrleWriter
        {
            get
            {
                return zrleWriter;
            }
        }

       */

        //Main server loop
        public void Start()
        {
            isRunning = true;
            try
            {
                serverSocket = new TcpListener(IPAddress.Any, Port);
                serverSocket.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //Close();
                return;
            }
            try
            {
                localClient = serverSocket.AcceptSocket();
                IPAddress localIP = IPAddress.Parse(((IPEndPoint)localClient.RemoteEndPoint).Address.ToString());
                Console.WriteLine(localIP);
                stream = new NetworkStream(localClient);
                reader = new BigEndianBinaryReader(stream);
                writer = new BigEndianBinaryWriter(stream);
                clients.Add(localClient);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

        }
        /*public void DoShit()
        {
            this.dw = this.device.getDefaultConfiguration().getBounds().width;
            this.dh = this.device.getDefaultConfiguration().getBounds().height;
            int i = this.dw / 4;
            int j = this.dh / 4;
            BufferedImage localBufferedImage = this.robot.createScreenCapture(new Rectangle(0, 0, this.dw, this.dh));
            for (int k = 0; k < 4; k++)
              for (m = 0; m < 4; m++)
              {
                localObject1 = new Rectangle();
                ((Rectangle)localObject1).x = (i * k);
                ((Rectangle)localObject1).y = (j * m);
                ((Rectangle)localObject1).width = i;
                ((Rectangle)localObject1).height = j;
                localObject1 = alignRectangle((Rectangle)localObject1);
                this.rects[(k * 4 + m)] = localObject1;
                this.oldImages[(k * 4 + m)] = localBufferedImage.getSubimage(((Rectangle)localObject1).x, ((Rectangle)localObject1).y, ((Rectangle)localObject1).width, ((Rectangle)localObject1).height);
              }
            ArrayList localArrayList = new ArrayList();
            int m = 0;
            Object localObject1 = null;
            try
            {
              Thread.sleep(300L);
            }
            catch (Exception localException1)
            {
            }
            long l1 = System.currentTimeMillis();
            while (!this.stop)
            {
              if (this.defaultPixel == null)
              {
                try
                {
                  Thread.sleep(1000L);
                }
                catch (Exception localException2)
                {
                }
                continue;
              }
              if (this.times++ > 0)
                try
                {
                  if (this.clients.size() > 0)
                  {
                    int n = 0;
                    Rectangle localRectangle1 = null;
                    do
                    {
                      if (this.rectRatos[m] >= 0)
                      {
                        Rectangle localRectangle2 = this.rects[m];
                        long l2 = System.currentTimeMillis();
                        localObject1 = this.robot.createScreenCapture(localRectangle2);
                        l2 = System.currentTimeMillis();
                        localRectangle1 = getChangeArea(this.oldImages[m], (BufferedImage)localObject1, localRectangle2);
                        if (localRectangle1 != null)
                        {
                          Iterator localIterator = this.clients.keySet().iterator();
                          Object localObject2;
                          while (localIterator.hasNext())
                          {
                            RFBClient localRFBClient = (RFBClient)localIterator.next();
                            localObject2 = (RobotClient)this.clients.get(localRFBClient);
                            ((RobotClient)localObject2).pe = localRFBClient.getPreferredEncoding();
                            ((RobotClient)localObject2).pf = localRFBClient.getPixelFormat();
                            if (((RobotClient)localObject2).pf == null)
                              ((RobotClient)localObject2).pf = this.defaultPixel;
                            Rect localRect = Rect.encode(((RobotClient)localObject2).pe, ((RobotClient)localObject2).pf, ((BufferedImage)localObject1).getSubimage(localRectangle1.x, localRectangle1.y, localRectangle1.width, localRectangle1.height), localRectangle1.x + localRectangle2.x, localRectangle1.y + localRectangle2.y);
                            Rect[] arrayOfRect = { localRect };
                            if (arrayOfRect == null)
                              continue;
                            try
                            {
                              localRFBClient.writeFrameBufferUpdate(arrayOfRect);
                            }
                            catch (SocketException localSocketException)
                            {
                              localArrayList.add(localRFBClient);
                            }
                            catch (Exception localException9)
                            {
                              localException9.printStackTrace();
                            }
                          }
                          for (int i1 = 0; i1 < localArrayList.size(); i1++)
                          {
                            localObject2 = (RFBClient)localArrayList.get(i1);
                            removeClient((RFBClient)localObject2);
                          }
                          localArrayList.clear();
                          try
                          {
                            Thread.sleep(5L);
                          }
                          catch (Exception localException8)
                          {
                          }
                          n += 5;
                          this.oldImages[m] = localObject1;
                          this.rectRatos[m] += 1;
                        }
                        else
                        {
                          try
                          {
                            Thread.sleep(10L);
                          }
                          catch (Exception localException7)
                          {
                          }
                          this.rectRatos[m] -= 1;
                        }
                      }
                      m = (m + 1) % 16;
                      if (m == 0)
                      {
                        if (System.currentTimeMillis() - l1 > 3000L)
                        {
                          l1 = System.currentTimeMillis();
                          Arrays.fill(this.rectRatos, 0);
                        }
                        if (n == 0)
                          try
                          {
                            Thread.sleep(500L);
                          }
                          catch (Exception localException6)
                          {
                          }
                      }
                      if (localRectangle1 != null)
                        break;
                    }
                    while (m != 0);
                  }
                  else
                  {
                    try
                    {
                      Thread.sleep(1000L);
                    }
                    catch (Exception localException3)
                    {
                    }
                  }
                  continue;
                }
                catch (Exception localException4)
                {
                  localException4.printStackTrace();
                  continue;
                }
                finally
                {
                }
              try
              {
                Thread.sleep(2000L);
              }
              catch (Exception localException5)
              {
              }
            }
        }*/
        /// <summary>
        /// Reads VNC Protocol Version message (see RFB Doc v. 3.8 section 6.1.1)
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the version of the is not known or supported.</exception>
        public void ReadProtocolVersion()
        {
            try
            {
                byte[] b = reader.ReadBytes(12);

                // As of the time of writing, the only supported versions are 3.3, 3.7, and 3.8.
                if (b[0] == 0x52 &&					// R
                        b[1] == 0x46 &&					// F
                        b[2] == 0x42 &&					// B
                        b[3] == 0x20 &&					// (space)
                        b[4] == 0x30 &&					// 0
                        b[5] == 0x30 &&					// 0
                        b[6] == 0x33 &&					// 3
                        b[7] == 0x2e &&					// .
                       (b[8] == 0x30 ||                    // 0
                        b[8] == 0x38) &&					// BUG FIX: Apple reports 8 
                       (b[9] == 0x30 ||                     // 0
                        b[9] == 0x38) &&					// BUG FIX: Apple reports 8 
                       (b[10] == 0x33 ||					// 3, 7, OR 8 are all valid and possible
                        b[10] == 0x36 ||					// BUG FIX: UltraVNC reports protocol version 3.6!
                        b[10] == 0x37 ||
                        b[10] == 0x38 ||
                        b[10] == 0x39) &&                   // BUG FIX: Apple reports 9					
                        b[11] == 0x0a)						// \n
                {
                    // Since we only currently support the 3.x protocols, this can be assumed here.
                    // If and when 4.x comes out, this will need to be fixed--however, the entire 
                    // protocol will need to be updated then anyway :)
                    verMajor = 3;

                    // Figure out which version of the protocol this is:
                    switch (b[10])
                    {
                        case 0x33:
                        case 0x36:	// BUG FIX: pass 3.3 for 3.6 to allow UltraVNC to work, thanks to Steve Bostedor.
                            verMinor = 3;
                            break;
                        case 0x37:
                            verMinor = 7;
                            break;
                        case 0x38:
                            verMinor = 8;
                            break;
                        case 0x39:  // BUG FIX: Apple reports 3.889
                            // According to the RealVNC mailing list, Apple is really using 3.3 
                            // (see http://www.mail-archive.com/vnc-list@realvnc.com/msg23615.html).  I've tested with
                            // both 3.3 and 3.8, and they both seem to work (I obviously haven't hit the issues others have).
                            // Because 3.8 seems to work, I'm leaving that, but it might be necessary to use 3.3 in future.
                            verMinor = 8;
                            break;
                    }
                }
                else
                {
                    throw new NotSupportedException("Only versions 3.3, 3.7, and 3.8 of the RFB Protocol are supported.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Send the Protocol Version supported by the server.  Will be highest supported by client (see RFB Doc v. 3.8 section 6.1.1).
        /// </summary>
        public void WriteProtocolVersion()
        {
            try
            {
                // We will use which ever version the server understands, be it 3.3, 3.7, or 3.8.
                System.Diagnostics.Debug.Assert(verMinor == 3 || verMinor == 7 || verMinor == 8, "Wrong Protocol Version!",
                             string.Format("Protocol Version should be 3.3, 3.7, or 3.8 but is {0}.{1}", verMajor.ToString(), verMinor.ToString()));

                writer.Write(GetBytes(string.Format("RFB 003.00{0}\n", verMinor.ToString())));
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Closes the active connection and stops the VNC Server.
        /// </summary>
        public void Close()
        {
            isRunning = false;
            try
            {
                serverSocket.Stop();
                localClient.Disconnect(true);
            }
            catch { }
        }

        /// <summary>
        /// Converts the VNC password to a byte array.
        /// </summary>
        /// <param name="password">The VNC password as a String, to be converted to bytes.</param>
        /// <returns>Returns a byte array of the password.</returns>
        private byte[] PasswordToKey(string password)
        {
            byte[] key = new byte[8];
            // Key limited to 8 bytes max.
            if (password.Length >= 8)
            {
                System.Text.Encoding.ASCII.GetBytes(password, 0, 8, key, 0);
            }
            else
            {
                System.Text.Encoding.ASCII.GetBytes(password, 0, password.Length, key, 0);
            }
            // VNC uses reverse byte order in key
            for (int i = 0; i < 8; i++)
                key[i] = (byte)(((key[i] & 0x01) << 7) |
                                 ((key[i] & 0x02) << 5) |
                                 ((key[i] & 0x04) << 3) |
                                 ((key[i] & 0x08) << 1) |
                                 ((key[i] & 0x10) >> 1) |
                                 ((key[i] & 0x20) >> 3) |
                                 ((key[i] & 0x40) >> 5) |
                                 ((key[i] & 0x80) >> 7));
            return key;
        }

        /// <summary>
        /// If the password is not empty, perform VNC Authentication with it.
        /// </summary>
        /// <param name="password">The current VNC Password</param>
        public bool WriteAuthentication(string password)
        {
            // Indicate to the client which type of authentication will be used.
            //The type of Authentication to be used, 1 (None) or 2 (VNC Authentication).
            if (String.IsNullOrEmpty(password))
            {
                // Protocol Version 3.7 onward supports multiple security types, while 3.3 only 1
                if (verMinor == 3)
                {
                    WriteUint32(1);
                }
                else
                {
                    byte[] types = new byte[] { 1 };
                    writer.Write((byte)types.Length);

                    for (int i = 0; i < types.Length; i++ )
                        writer.Write(types[i]);
                }
                if (verMinor >= 7)
                    reader.ReadByte();
                if(verMinor == 8)
                    WriteSecurityResult(0);
                return true;
            }
            else
            {
                if (verMinor == 3)
                {
                    WriteUint32(2);
                }
                else
                {
                    byte[] types = new byte[] { 2 };
                    writer.Write((byte)types.Length);

                    for (int i = 0; i < types.Length; i++)
                        writer.Write(types[i]);
                }
                if (verMinor >= 7)
                    reader.ReadByte();

                byte[] bChallenge = new byte[16];
                Random rand = new Random(System.DateTime.Now.Millisecond);
                rand.NextBytes(bChallenge);

                // send the bytes to the client and wait for the response
                writer.Write(bChallenge);
                writer.Flush();

                byte[] receivedBytes = reader.ReadBytes(16);
                byte[] key = PasswordToKey(password);

                DES des = new DESCryptoServiceProvider();
                des.Padding = PaddingMode.None;
                des.Mode = CipherMode.ECB;
                ICryptoTransform enc = des.CreateEncryptor(key, null);
                byte[] ourBytes = new byte[16];
                enc.TransformBlock(bChallenge, 0, bChallenge.Length, ourBytes, 0);
                /*
                Console.WriteLine("Us: " + System.Text.Encoding.ASCII.GetString(ourBytes));
                Console.WriteLine("Client sent us: " + System.Text.Encoding.ASCII.GetString(receivedBytes));
                */
                bool authOK = true;
                for (int i = 0; i < ourBytes.Length; i++)
                    if (receivedBytes[i] != ourBytes[i])
                        authOK = false;

                if (authOK)
                {
                    WriteSecurityResult(0);
                    return true;
                }
                else
                {
                    WriteSecurityResult(1);
                    if (verMinor == 8)
                    {
                        string ErrorMsg = "Wrong password, sorry";
                        WriteUint32((uint)ErrorMsg.Length);
                        writer.Write(GetBytes(ErrorMsg));
                    }
                    return false;
                }

            }
        }

        /// <summary>
        /// Sends the encrypted Response back to the server.
        /// </summary>
        /// <param name="response">The DES password encrypted challege sent by the server.</param>
        public void WriteSecurityResponse(byte[] response)
        {
            writer.Write(response, 0, response.Length);
            writer.Flush();
        }

        /// <summary>
        /// When the client uses VNC Authentication, after the Challege/Response,
        /// a status code is sent to indicate whether authentication worked.
        /// </summary>
        /// <param name="sr">An unsigned integer indicating the status of authentication: 0 = OK; 1 = Failed; 2 = Too Many (deprecated).</return
        public void WriteSecurityResult(uint sr)
        {
            writer.Write(sr);
        }

        /// <summary>
        /// Receives an Initialisation message from the client.
        /// </summary>
        /// <returns>True if the server allows other clients to connect, otherwise False.</returns>
        public bool ReadClientInit()
        {
            bool sh = false;
            try
            {
                this.Shared = (reader.ReadByte() == 1);
                sh = this.Shared;
                return Shared;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
            }
            return sh;
        }

        /// <summary>
        /// Writes the server's Initialization message, specifically the Framebuffer's properties.
        /// </summary>
        /// <param name="fb">The framebuffer which properties are sent.</param>
        public void WriteServerInit(Framebuffer fb)
        {
            try
            {
                writer.Write(Convert.ToUInt16(fb.Width));
                writer.Write(Convert.ToUInt16(fb.Height));
                writer.Write(fb.ToPixelFormat());

                writer.Write(Convert.ToUInt32(fb.DesktopName.Length));
                writer.Write(GetBytes(fb.DesktopName));
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Receives the format to be used when sending Framebuffer Updates.
        /// </summary>
        /// <returns>A Framebuffer telling the server how to encode pixel data. Typically this will be the same one sent by the server during initialization.</returns>
        public Framebuffer ReadSetPixelFormat()
        {
            Framebuffer ret = null;
            try
            {
                ReadPadding(3);
                byte[] pf = ReadBytes(16);
                ret = Framebuffer.FromPixelFormat(pf, 0, 0);
                return ret;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
            }
            return ret;
        }

        /// <summary>
        /// Reads the supported encodings from the client.
        /// </summary>
        public void ReadSetEncodings()
        {
            try
            {
                ReadPadding(1);
                ushort len = reader.ReadUInt16();
                uint[] enc = new uint[(Int32)len];

                for (int i = 0; i < (Int32)len; i++)
                    enc[i] = reader.ReadUInt32();
                _encodings = enc;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Reads a request for an update of the area specified by (x, y, w, h).
        /// </summary>
        public void ReadFrameBufferUpdateRequest(Framebuffer fb)
        {
            try
            {
                bool incremental = Convert.ToBoolean((int)(reader.ReadByte()));
                ushort x = reader.ReadUInt16();
                ushort y = reader.ReadUInt16();
                ushort width = reader.ReadUInt16();
                ushort height = reader.ReadUInt16();

                /*new Thread(delegate() { */DoFrameBufferUpdate(fb, incremental, x, y, width, height); /*}).Start();*/
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Creates the encoded pixel data in a form of an EncodedRectangle with the preferred encoding.
        /// </summary>
        private void DoFrameBufferUpdate(Framebuffer fb, bool incremental, int x, int y, int width, int height)
        {
            int w = fb.Width;
            int h = fb.Height;
            if ((x < 0) || (y < 0) || (width <= 0) || (height <= 0))
            {
                Console.WriteLine("Neg:" + x + ":" + y + ":" + width + ":" + height);
                return;
            }
            if (x + width > w)
            {
                Console.WriteLine("Too wide");
                return;
            }
            if (y + height > h)
            {
                Console.WriteLine("Too high");
                return;
            }

            /*
            Rectangle[] rects = new Rectangle[16];
            Bitmap[] oldImages = new Bitmap[16];
            EncodedRectangle[] arrEnc = null;

            int i = width / 4;
            int j = height / 4;
            int m = 0;
            Rectangle lRect = Rectangle.Empty;

            Bitmap localBitmap = PixelGrabber.CreateScreenCapture(new Rectangle(0, 0, width, height));
            for (int k = 0; k < 4; k++)
                for (m = 0; m < 4; m++)
                {
                    lRect = new Rectangle();
                    lRect.X = (i * k);
                    lRect.Y = (j * m);
                    lRect.Width = i;
                    lRect.Height = j;
                    lRect = PixelGrabber.AlignRectangle(lRect, width, height);
                    rects[(k * 4 + m)] = lRect;
                    oldImages[(k * 4 + m)] = PixelGrabber.GetSubImage(localBitmap, new Rectangle(lRect.X, lRect.Y, lRect.Width, lRect.Height));
                }
            List<EncodedRectangle> rCol = new List<EncodedRectangle>();
            foreach (Rectangle r in rects)
            {
                try
                {
                    
                    EncodedRectangleFactory factory = new EncodedRectangleFactory(this, fb);
                    EncodedRectangle localRect = factory.Build(r, GetPreferredEncoding()); //factory.Build(PixelGrabber.GetSubImage(lBitmap, new Rectangle(localRectangle1.X, localRectangle1.Y, localRectangle1.Width, localRectangle1.Height)), localRectangle1.X + localRectangle2.X, localRectangle1.Y + localRectangle2.Y, rp.GetPreferredEncoding());
                    localRect.Encode();
                    rCol.Add(localRect);
                }
                catch (Exception localException)
                {
                    Console.WriteLine(localException.StackTrace.ToString());
                    if (localException is IOException)
                    { this.Close(); return; }
                }
            }
            arrEnc = rCol.ToArray();
            if (arrEnc != null)
                WriteFrameBufferUpdate(arrEnc);
            */
            
            EncodedRectangle[] arrayOfRect = null;
            try
            {
                EncodedRectangleFactory factory = new EncodedRectangleFactory(this, fb);
                EncodedRectangle localRect = factory.Build(new Rectangle(x, y, width, height), GetPreferredEncoding());
                localRect.Encode();
                arrayOfRect = new EncodedRectangle[] { localRect };
            }
            catch (Exception localException)
            {
                Console.WriteLine(localException.StackTrace.ToString());
                if (localException is IOException)
                { this.Close(); return; }
            }
            if (arrayOfRect != null)
                WriteFrameBufferUpdate(arrayOfRect);
            
        }

        /// <summary>
        /// Writes the number of update rectangles being sent to the client.
        /// After that, for each rectangle, the encoded data is written.
        /// </summary>
        public void WriteFrameBufferUpdate(EncodedRectangle[] arrRectangles)
        {
            try
            {
                WriteServerMessageType(RfbProtocol.FRAMEBUFFER_UPDATE);
                WritePadding(1);
                writer.Write(Convert.ToUInt16(arrRectangles.Length));

                foreach (EncodedRectangle e in arrRectangles)
                    e.WriteData();

                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Receives a key press or release event from the client.
        /// </summary>
        public void ReadKeyEvent()
        {
            try
            {
                bool pressed = (reader.ReadByte() == 1);
                ReadPadding(2);
                uint keysym = reader.ReadUInt32();

                //Do KeyEvent
                //new Thread(delegate() { 
                Robot.KeyEvent(pressed, (int)keysym);
                //}).Start();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Receives a mouse movement or button press/release from the client.
        /// </summary>
        public void ReadPointerEvent()
        {
            try
            {
                byte buttonMask = reader.ReadByte();
                ushort X = reader.ReadUInt16();
                ushort Y = reader.ReadUInt16();
                new Thread(delegate() { Robot.PointerEvent(buttonMask, X, Y); }).Start();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Receives the clipboard data from the client.
        /// </summary>
        public void ReadClientCutText()
        {
            try
            {
                ReadPadding(3);
                int len = (int)reader.ReadUInt32();
                string text = GetString(reader.ReadBytes(len));
                CutText = text;
                System.Windows.Forms.Clipboard.SetDataObject(text.Replace("\n", Environment.NewLine), true);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Reads the type of message being sent by the client--all messages are prefixed with a message type.
        /// </summary>
        /// <returns>Returns the message type as an integer.</returns>
        public int ReadServerMessageType()
        {
            int x = 0;
            try
            {
                x = (int)reader.ReadByte();
                return x;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
            }
            return x;
        }

        /// <summary>
        /// Writes the type of message being sent to the client--all messages are prefixed with a message type.
        /// </summary>
        public void WriteServerMessageType(int paramInt)
        {
            try { writer.Write((byte)paramInt); }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        public void WriteServerMessageType(byte paramInt)
        {
            try { writer.Write(paramInt); }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        // TODO: this colour map code should probably go in Framebuffer.cs
        private ushort[,] mapEntries = new ushort[256, 3];
        public ushort[,] MapEntries
        {
            get
            {
                return mapEntries;
            }
        }

        /// <summary>
        /// Reads 8-bit RGB colour values (or updated values) into the colour map.
        /// </summary>
        public void ReadColourMapEntry()
        {
            ReadPadding(1);
            ushort firstColor = ReadUInt16();
            ushort nbColors = ReadUInt16();

            for (int i = 0; i < nbColors; i++, firstColor++)
            {
                mapEntries[firstColor, 0] = (byte)(ReadUInt16() * byte.MaxValue / ushort.MaxValue);    // R
                mapEntries[firstColor, 1] = (byte)(ReadUInt16() * byte.MaxValue / ushort.MaxValue);    // G
                mapEntries[firstColor, 2] = (byte)(ReadUInt16() * byte.MaxValue / ushort.MaxValue);    // B
            }
        }

        /// <summary>
        /// Writes 8-bit RGB colour values (or updated values) from the colour map.
        /// </summary>
        public void WriteColourMapEntry(ushort firstColor, System.Drawing.Color[] colors)
        {
            try
            {
                WriteServerMessageType(1);

                WritePadding(1);
                writer.Write(firstColor);
                writer.Write((ushort)colors.Length);

                for (int i = 0; i < colors.Length; i++)
                {
                    writer.Write((ushort)colors[i].R);
                    writer.Write((ushort)colors[i].G);
                    writer.Write((ushort)colors[i].B);
                }
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Writes the text from the Cut Buffer on the server.
        /// </summary>
        public void WriteServerCutText(String text)
        {
            try
            {
                WriteServerMessageType(3);
                WritePadding(3);
                writer.Write((uint)text.Length);
                writer.Write(GetBytes(text));
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();
                return;
            }
        }

        // ---------------------------------------------------------------------------------------
        // Here's all the "low-level" protocol stuff so user objects can access the data directly

        /// <summary>
        /// Reads a single UInt32 value from the server, taking care of Big- to Little-Endian conversion.
        /// </summary>
        /// <returns>Returns a UInt32 value.</returns>
        public uint ReadUint32()
        {
            return reader.ReadUInt32();
        }

        /// <summary>
        /// Reads a single UInt16 value from the server, taking care of Big- to Little-Endian conversion.
        /// </summary>
        /// <returns>Returns a UInt16 value.</returns>
        public ushort ReadUInt16()
        {
            return reader.ReadUInt16();
        }

        /// <summary>
        /// Reads a single Byte value from the server.
        /// </summary>
        /// <returns>Returns a Byte value.</returns>
        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        /// <summary>
        /// Reads the specified number of bytes from the server, taking care of Big- to Little-Endian conversion.
        /// </summary>
        /// <param name="count">The number of bytes to be read.</param>
        /// <returns>Returns a Byte Array containing the values read.</returns>
        public byte[] ReadBytes(int count)
        {
            return reader.ReadBytes(count);
        }

        /// <summary>
        /// Writes a single UInt32 value to the server, taking care of Little- to Big-Endian conversion.
        /// </summary>
        /// <param name="value">The UInt32 value to be written.</param>
        public void WriteUint32(uint value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a single UInt16 value to the server, taking care of Little- to Big-Endian conversion.
        /// </summary>
        /// <param name="value">The UInt16 value to be written.</param>
        public void WriteUInt16(ushort value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a single Byte value to the server.
        /// </summary>
        /// <param name="value">The UInt32 value to be written.</param>
        public void WriteByte(byte value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Reads the specified number of bytes of padding (i.e., garbage bytes) from the server.
        /// </summary>
        /// <param name="length">The number of bytes of padding to read.</param>
        public void ReadPadding(int length)
        {
            ReadBytes(length);
        }

        /// <summary>
        /// Writes the specified number of bytes of padding (i.e., garbage bytes) to the server.
        /// </summary>
        /// <param name="length">The number of bytes of padding to write.</param>
        public void WritePadding(int length)
        {
            byte[] padding = new byte[length];
            writer.Write(padding, 0, padding.Length);
        }

        /// <summary>
        /// Converts a string to bytes for transfer to the server.
        /// </summary>
        /// <param name="text">The text to be converted to bytes.</param>
        /// <returns>Returns a Byte Array containing the text as bytes.</returns>
        protected static byte[] GetBytes(string text)
        {
            return System.Text.Encoding.ASCII.GetBytes(text);
        }

        /// <summary>
        /// Converts a series of bytes to a string.
        /// </summary>
        /// <param name="bytes">The Array of Bytes to be converted to a string.</param>
        /// <returns>Returns a String representation of bytes.</returns>
        protected static string GetString(byte[] bytes)
        {
            return System.Text.ASCIIEncoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}