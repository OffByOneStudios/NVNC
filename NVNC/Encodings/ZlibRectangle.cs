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
using System.IO;
//using ICSharpCode.SharpZipLib.Zip.Compression;

namespace NVNC.Encodings
{
    /// <summary>
    /// Implementation of Raw encoding. 
    /// </summary>
    public sealed class ZlibRectangle : EncodedRectangle
    {
        private int[] pixels;
        public ZlibRectangle(RfbProtocol rfb, Framebuffer framebuffer, int[] pixels, Rectangle rectangle)
            : base(rfb, framebuffer, rectangle, RfbProtocol.Encoding.ZLIB_ENCODING)
        {
            this.pixels = pixels;
        }

        public override void Encode()
        {
            /*
            bytes = PixelGrabber.GrabPixels(bmp, PixelFormat.Format32bppArgb);
            for (int i = 0; i < pixels.Length; i++)
                framebuffer[i] = pixels[i];
             */
            if (bytes == null)
                bytes = PixelGrabber.GrabPixels(pixels, rectangle, framebuffer);

        }
        public override void WriteData()
        {
            base.WriteData();
            rfb.Writer.Write(Convert.ToUInt32(RfbProtocol.Encoding.ZLIB_ENCODING));
            Console.WriteLine("ZLib uncompressed bytes size: " + bytes.Length);
            rfb.ZrleWriter.Write(bytes); //writer for Zlib is a ZlibCompressedWriter
            rfb.ZrleWriter.Flush();

            /*  Very slow, not practically usable
            for (int i = 0; i < framebuffer.pixels.Length; i++)
                pwriter.WritePixel(framebuffer[i]);
            */
        }

        /*
        public override byte[] WriteStream()
        {
            byte[] compressed = null;
            //ZrleCompressedWriter zw = new ZrleCompressedWriter(new System.IO.MemoryStream(bytes));
            //zw.Compress();
            MemoryStream compressedStream = new MemoryStream();
            //Ionic.Zlib.ZlibStream deflater = new Ionic.Zlib.ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.Default);
            //deflater.FlushMode = Ionic.Zlib.FlushType.Sync;
            //DevelopDotNet.Compression.ZStream deflater = new DevelopDotNet.Compression.ZStream(compressedStream, DevelopDotNet.Compression.CompressionLevel.None);
            /*
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream deflater = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(compressedStream);
            int pos = 0;
            MemoryStream ustr = new MemoryStream(bytes);
            while (pos++ < ustr.Length)
            {
                deflater.WriteByte((byte)ustr.ReadByte());
            }
            //deflater.Write(bytes, 0, bytes.Length);
            

            //deflater.Flush();
            //deflater.Close();
            //deflater.Dispose();
            //compressed = compressedStream.ToArray();



            MemoryStream mz = new MemoryStream();
            Org.BouncyCastle.Utilities.Zlib.ZOutputStream oz = new Org.BouncyCastle.Utilities.Zlib.ZOutputStream(mz, 1, false);
            oz.FlushMode = Org.BouncyCastle.Utilities.Zlib.JZlib.Z_SYNC_FLUSH;
            //Org.BouncyCastle.Utilities.Zlib.ZDeflaterOutputStream os = new Org.BouncyCastle.Utilities.Zlib.ZDeflaterOutputStream(mz, 1, false);
            oz.Write(bytes, 0, bytes.Length);
            oz.Finish();
            oz.Flush();
            oz.Close();

            MemoryStream mx = new MemoryStream();
            Deflater d = new Deflater(1, false);
            d.SetStrategy(DeflateStrategy.Default);
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream ds = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(mx, d);
            ds.Write(bytes, 0, bytes.Length);
            ds.Close();

            //compressed = Ionic.Zlib.ZlibStream.CompressBuffer(bytes);     
            File.WriteAllBytes("uc.bin", bytes);
            File.WriteAllBytes("uc3.bin", mx.ToArray());
            File.WriteAllBytes("uc4.bin", mz.ToArray());

            javaDeflate();
            //Console.ReadLine();
            byte[] c = File.ReadAllBytes("uc4.bin");

            MemoryStream data = new MemoryStream();
            data.WriteByte(0x58);
            data.WriteByte(0x85);
            using (var compressor = new System.IO.Compression.DeflateStream(data, System.IO.Compression.CompressionMode.Compress, true))
                compressor.Write(bytes, 0, bytes.Length);
            data.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(Adler32(bytes))), 0, sizeof(uint));

            byte[] desp = File.ReadAllBytes("uc2.bin");//SharpCompressZlib(new MemoryStream(bytes));//ZlibCodecCompress(bytes);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.BinaryWriter bw = new BigEndianBinaryWriter(ms))
                {
                    bw.Write(base.WriteStream());
                    bw.Write(Convert.ToUInt32(RfbProtocol.Encoding.ZLIB_ENCODING));
                    bw.Write(desp.Length);
                    bw.Write(desp);

                    Console.WriteLine("LENGTH: " + desp.Length);
                    //bw.Write((uint)compressed.Length);
                    //bw.Write(compressed, 0, compressed.Length);
                }
                return ms.ToArray();
           }
        }
        */
        private void javaDeflate()
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            startInfo.FileName = "C:\\Program Files\\Java\\jre7\\bin\\java.exe";
            startInfo.Arguments = "-jar zlibencoder.jar uc.bin";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (System.Diagnostics.Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
        }

        /*
        private byte[] ZlibCodecCompress(byte[] data)
        {
            int outputSize = data.Length;
            byte[] output = new Byte[outputSize];
            byte[] uncompressed = data;
            int lengthToCompress = uncompressed.Length;

            // If you want a ZLIB stream, set this to true.  If you want
            // a bare DEFLATE stream, set this to false.
            bool wantRfc1950Header = true;

            using (MemoryStream ms = new MemoryStream())
            {
                Ionic.Zlib.ZlibCodec compressor = new Ionic.Zlib.ZlibCodec();
                compressor.InitializeDeflate(Ionic.Zlib.CompressionLevel.Level1, wantRfc1950Header);

                compressor.InputBuffer = uncompressed;
                compressor.AvailableBytesIn = lengthToCompress;
                compressor.NextIn = 0;
                compressor.OutputBuffer = output;

                foreach (var f in new Ionic.Zlib.FlushType[] { Ionic.Zlib.FlushType.Sync })
                {
                    int bytesToWrite = 0;
                    do
                    {
                        compressor.AvailableBytesOut = outputSize;
                        compressor.NextOut = 0;
                        compressor.Deflate(f);

                        bytesToWrite = outputSize - compressor.AvailableBytesOut;
                        if (bytesToWrite > 0)
                            ms.Write(output, 0, bytesToWrite);
                    }
                    while ((f == Ionic.Zlib.FlushType.None && (compressor.AvailableBytesIn != 0 || compressor.AvailableBytesOut == 0)) ||
                           (f == Ionic.Zlib.FlushType.Finish && bytesToWrite != 0));
                }

                compressor.EndDeflate();

                ms.Flush();
                return ms.ToArray();
            }
        }
        public static byte[] SharpCompressZlib(Stream source)
        {
            byte[] result = null;
            using (MemoryStream outStream = new MemoryStream())
            {
                using (ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream def = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(outStream))
                {
                    int read;
                    while ((read = source.ReadByte()) != -1)
                        def.WriteByte((byte)read);
                }
                result = outStream.ToArray();
            }
            return result;
        }
        */
        // naive implementation of adler-32 checksum
        int Adler32(byte[] bytes)
        {
            const uint a32mod = 65521;
            uint s1 = 1, s2 = 0;
            foreach (byte b in bytes)
            {
                s1 = (s1 + b) % a32mod;
                s2 = (s2 + s1) % a32mod;
            }
            return unchecked((int)((s2 << 16) + s1));
        }

    }
}
