// NVNC - .NET VNC Server Library
// Copyright (C) 2012 T!T@N
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

namespace NVNC.Encodings
{
    /// <summary>
    /// A 32-bit PixelWriter.
    /// </summary>
    public sealed class PixelWriter32 : PixelWriter
    {
        public PixelWriter32(BinaryWriter writer, Framebuffer framebuffer)
            : base(writer, framebuffer)
        { }

        public override void WritePixel(int px)
        {
            int pixel;
            int b = 0;
            byte[] bytes = new byte[4];
            pixel = px;

            bytes[b++] = (byte)(pixel & 0xFF);
            bytes[b++] = (byte)((pixel >> 8) & 0xFF);
            bytes[b++] = (byte)((pixel >> 16) & 0xFF);
            bytes[b++] = (byte)((pixel >> 24) & 0xFF);

            writer.Write(bytes);
        }
    }
}
