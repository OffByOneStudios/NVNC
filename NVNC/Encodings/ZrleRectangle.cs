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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
//using ICSharpCode.SharpZipLib.Zip.Compression;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
namespace NVNC.Encodings
{
	/// <summary>
	/// Implementation of ZRLE encoding, as well as drawing support. See RFB Protocol document v. 3.8 section 6.6.5.
	/// </summary>
	public sealed class ZrleRectangle : EncodedRectangle
	{
		private const int TILE_WIDTH = 64;
		private const int TILE_HEIGHT = 64;
        private int[] pixels;
        public ZrleRectangle(RfbProtocol rfb, Framebuffer framebuffer, int[] pixels, Rectangle rectangle)
            : base(rfb, framebuffer, rectangle, RfbProtocol.Encoding.ZRLE_ENCODING)
        {
            this.pixels = pixels;
        }

        public override void Encode()
        {
            int x = rectangle.X;
            int y = rectangle.Y;
            int w = rectangle.Width;
            int h = rectangle.Height;

            Console.WriteLine("Landed at ZRLE start!");
            //System.Diagnostics.Stopwatch Watch = System.Diagnostics.Stopwatch.StartNew();
            int rawDataSize = w * h * (this.framebuffer.BitsPerPixel / 8);
            byte[] data = new byte[rawDataSize];
            int currentX, currentY;
            int tileW, tileH;

            //Bitmap bmp = PixelGrabber.GrabImage(rectangle.Width, rectangle.Height, pixels);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (BinaryWriter toPack = new BinaryWriter(ms))
                {
                    for (currentY = y; currentY < y + h; currentY += TILE_HEIGHT)
                    {
                        tileH = TILE_HEIGHT;
                        tileH = Math.Min(tileH, y + h - currentY);
                        for (currentX = x; currentX < x + w; currentX += TILE_WIDTH)
                        {
                            tileW = TILE_WIDTH;
                            tileW = Math.Min(tileW, x + w - currentX);

                            int[] pixelz = CopyPixels(pixels, w, currentX, currentY, tileW, tileH);

                            byte subencoding = 0;
                            ms.WriteByte(subencoding);
                            //toPack.Write(subencoding);
                            //PixelGrabber.GrabPixels(pixels, new Rectangle(currentX, currentY, tileW, tileH), this.framebuffer);

                            for (int i = 0; i < pixelz.Length; ++i)
                            {
                                int bb = 0;
                                byte[] bytes = new byte[3];
                                int pixel = pixelz[i];


                                bytes[bb++] = (byte)(pixel & 0xFF);
                                bytes[bb++] = (byte)((pixel >> 8) & 0xFF);
                                bytes[bb++] = (byte)((pixel >> 16) & 0xFF);
                                //bytes[b++] = (byte)((pixel >> 24) & 0xFF);

                                ms.Write(bytes, 0, bytes.Length);
                                //toPack.Write(bytes);
                            }
                        }
                    }

                    Console.WriteLine("Length1 (uncompressed): " + ms.ToArray().Length);
                    byte[] uncompressed = ms.ToArray();
                    this.bytes = uncompressed;


                    //byte[] compressed = Ionic.Zlib.ZlibStream.CompressBuffer(uncompressed);

                    /*using (MemoryStream compressedStream = new MemoryStream())
                    {
                        compressedStream.Position = 0;
                        //using (Ionic.Zlib.ZlibStream deflater = new Ionic.Zlib.ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed))
                        using(DevelopDotNet.Compression.ZStream deflater = new DevelopDotNet.Compression.ZStream(compressedStream, DevelopDotNet.Compression.CompressionLevel.None))
                        {
                            Console.WriteLine("CLength1: " + deflater.CompressedLength);
                            Console.WriteLine("DataType: " + deflater.DataType);
                            deflater.Write(uncompressed, 0, uncompressed.Length);
                        }
                         compressed = compressedStream.ToArray();
                    }*/
                    /*
                    System.IO.File.WriteAllBytes("outu.bin", uncompressed);

                    //System.IO.File.WriteAllBytes("outc.bin", compressed);
                    Console.WriteLine("Length1 (compressed): " + compressed.Length);

                    this.bytes = compressed;
                    //this.bytes = uncompressed;
                 */
                }

            }
        }
        
        public override void WriteData()
        {
            base.WriteData();
            rfb.Writer.Write(Convert.ToUInt32(RfbProtocol.Encoding.ZRLE_ENCODING));
            rfb.ZrleWriter.Write(this.bytes);
        }
        /*
        public override byte[] WriteStream()
        {
            System.Diagnostics.Stopwatch Watch = System.Diagnostics.Stopwatch.StartNew();

            using (System.IO.MemoryStream finalData = new System.IO.MemoryStream())
            {
                using (BinaryWriter finalWriter = new BigEndianBinaryWriter(finalData))
                {
                    finalWriter.Write(base.WriteStream());
                    finalWriter.Write(Convert.ToUInt32(RfbProtocol.Encoding.ZRLE_ENCODING));

                    finalWriter.Write((uint)this.bytes.Length);
                    finalWriter.Write(this.bytes);
                }
                //System.IO.File.WriteAllBytes("outc2.bin", finalData.ToArray());
                Watch.Stop();
                Console.WriteLine("Final data size: " + finalData.ToArray().Length);
                return finalData.ToArray();
            }
        }
        */
	}
}