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
    public sealed class PixelWriter8 : PixelWriter
    {
        private RfbProtocol rfb = null;

        public PixelWriter8(BinaryWriter reader, Framebuffer framebuffer, RfbProtocol rfb)
            : base(reader, framebuffer)
        {
            this.rfb = rfb;
        }

        /// <summary>
        /// Writes an 8-bit pixel.
        /// </summary>
        /// <returns>Writes an Integer value representing the pixel in GDI+ format.</returns>
        public override void WritePixel(int px)
        {
            int pixel;
            pixel = px;

            byte bp = (byte)(pixel & 0xFF);
            writer.Write(bp);
        }
    }
}
