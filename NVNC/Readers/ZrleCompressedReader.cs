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
using System.IO;
using System.Text;
using System.Threading;
using ComponentAce.Compression.Libs.zlib;

namespace NVNC.Readers
{
    /// <summary>
    /// ZRLE compressed binary reader, used by ZrleRectangle.
    /// </summary>
    public sealed class ZRLECompressedReader : BinaryReader
    {
        MemoryStream zlibMemoryStream;
        
        ZOutputStream zlibDecompressedStream;
        BinaryReader uncompressedReader;

        public ZRLECompressedReader(Stream uncompressedStream)
            : base(uncompressedStream)
        {
            zlibMemoryStream = new MemoryStream();
            zlibDecompressedStream = new ZOutputStream(zlibMemoryStream);
            //zlibDecompressedStream.FlushMode = ComponentAce.Compression.Libs.zlib.zlibConst.Z_SYNC_FLUSH;
            uncompressedReader = new BinaryReader(zlibMemoryStream);
            //uncompressedReader = new BinaryReader(uncompressedStream);
        }

        public override byte ReadByte()
        {
            return uncompressedReader.ReadByte();
        }

        public override byte[] ReadBytes(int count)
        {
            return uncompressedReader.ReadBytes(count);
        }

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
            try
            {
                while (pos++ < compressedBufferSize)
                {
                    try
                    {
                        int bData = this.BaseStream.ReadByte();
                        zlibDecompressedStream.WriteByte(bData);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.StackTrace);
            }
            zlibMemoryStream.Position = 0;
            System.Windows.Forms.MessageBox.Show("BufferSize: " + compressedBufferSize);
        }
    }
}
