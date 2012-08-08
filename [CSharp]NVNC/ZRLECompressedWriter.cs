using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace LVNC
{
    public partial class RfbProtocol
    {
        /// <summary>
        /// ZRLE compressed binary writer, used by ZrleRectangle.
        /// </summary>
        public sealed class ZRLECompressedWriter : BinaryWriter
        {
            MemoryStream zlibMemoryStream;
            zlib.ZOutputStream zlibCompressedStream;
            BinaryReader uncompressedReader;
            BinaryWriter compressedWriter;

            public ZRLECompressedWriter(Stream compressedStream)
                : base(compressedStream)
            {
                /*zlibMemoryStream = new MemoryStream();
                zlibDecompressedStream = new zlib.ZOutputStream(zlibMemoryStream);
                uncompressedReader = new BinaryReader(zlibMemoryStream);*/
                zlibMemoryStream = new MemoryStream();
                zlibCompressedStream = new zlib.ZOutputStream(zlibMemoryStream, zlib.zlibConst.Z_DEFAULT_COMPRESSION);
                compressedWriter = new BinaryWriter(zlibMemoryStream);
            }

            /*
            public override byte ReadByte()
            {
                return uncompressedReader.ReadByte();
            }
            */

            public void WriteByte(byte b)
            {
                compressedWriter.Write(b);
            }

            /*
            public override byte[] ReadBytes(int count)
            {
                return uncompressedReader.ReadBytes(count);
            }
            */
            public void WriteBytes(byte[] b)
            {
                compressedWriter.Write(b);
            }

            /*
            public void DecodeStream()
            {
                // Reset position to use same buffer
                zlibMemoryStream.Position = 0;

                // Get compressed stream length to read
                byte[] buff = new byte[4];
                if (this.BaseStream.Read(buff, 0, 4) != 4)
                    throw new Exception("ZRLE decoder: Invalid compressed stream size");

                // BigEndian to LittleEndian conversion
                int compressedBufferSize = (int)(buff[3] | buff[2] << 8 | buff[1] << 16 | buff[0] << 24);
                if (compressedBufferSize > 64 * 1024 * 1024)
                    throw new Exception("ZRLE decoder: Invalid compressed data size");

                // Decode stream
                int pos = 0;
                while (pos++ < compressedBufferSize)
                    zlibDecompressedStream.WriteByte(this.BaseStream.ReadByte());

                zlibMemoryStream.Position = 0;
            }
            */

            public void EncodeStream()
            {
                using (zlibMemoryStream)
                using (zlibCompressedStream)
                {
                    byte[] buffer = new byte[2000];
                    int len;
                    while ((len = this.BaseStream.Read(buffer, 0, 2000)) > 0)
                    {
                        zlibCompressedStream.Write(buffer, 0, len);
                    }
                    zlibCompressedStream.Flush();
                    zlibCompressedStream.finish();
                    //outData = outMemoryStream.ToArray();
                }
            }
        }
    }
}
