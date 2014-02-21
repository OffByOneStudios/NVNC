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
using System.IO;
using ComponentAce.Compression.Libs.zlib;

namespace NVNC
{
    public sealed class ZrleCompressedWriter : BinaryWriter
    {
        MemoryStream zMemoryStream;
        ZOutputStream zCompressStream;
        BinaryWriter compressedWriter;
        BigEndianBinaryWriter bigWriter;
        long oldPos;
        public ZrleCompressedWriter(Stream uncompressedStream)
            : base(uncompressedStream)
        {
            zMemoryStream = new MemoryStream();
            zCompressStream = new ZOutputStream(zMemoryStream, 9);
            zCompressStream.FlushMode = zlibConst.Z_SYNC_FLUSH;
            compressedWriter = new BinaryWriter(uncompressedStream);
            bigWriter = new BigEndianBinaryWriter(uncompressedStream);
            oldPos=0;
        }
        public override void Write(byte[] buffer, int index, int count)
        {
            zCompressStream.Write(buffer, index, count);
            long cPos = zMemoryStream.Position;
            int len = (int)(cPos - oldPos);
            long nPos = cPos - len;

            //compressedWriter.Write(len);
            bigWriter.Write(len);

            int pos = 0;
            zMemoryStream.Position = nPos;
            using (MemoryStream tmp = new MemoryStream())
            {
                while (pos++ < len)
                {
                    try
                    {
                        int bData = zMemoryStream.ReadByte();
                        tmp.WriteByte((byte)bData);
                        //compressedWriter.Write((byte)bData);
                        //bigWriter.Write((byte)bData);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.ToString());
                    }
                }
                compressedWriter.Write(tmp.ToArray());
            }
            Console.WriteLine("Compressed data length: " + len);
            oldPos = cPos;
        }
        public override void Write(byte[] buffer)
        {
            this.Write(buffer, 0, buffer.Length);
        }
        public override void Write(byte value)
        {
            zCompressStream.WriteByte(value);
            //compressedWriter.Write(value);
        }
    }
}
