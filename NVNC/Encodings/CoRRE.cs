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
using System.Drawing;

namespace NVNC.Encodings
{
    public class CoRRE : RreRectangle
    {
        public CoRRE(RfbProtocol rfb, Framebuffer framebuffer, int[] pixels, Rectangle rectangle)
            : base(rfb, framebuffer, pixels, rectangle)
        {
            this.pixels = pixels;
        }

        public override void Encode()
        {
            base.Encode();
        }
        public override void WriteData()
        {
            System.Diagnostics.Stopwatch Watch = System.Diagnostics.Stopwatch.StartNew();
            pwriter.Write(Convert.ToUInt16(rectangle.X));
            pwriter.Write(Convert.ToUInt16(rectangle.Y));
            pwriter.Write(Convert.ToUInt16(rectangle.Width));
            pwriter.Write(Convert.ToUInt16(rectangle.Height));

            pwriter.Write(Convert.ToUInt32(RfbProtocol.Encoding.CORRE_ENCODING));
            pwriter.Write(Convert.ToUInt32(this.subrects.Length));

            pwriter.WritePixel(this.bgpixel);
            for (int i = 0; i < this.subrects.Length; i++)
            {
                pwriter.WritePixel(this.subrects[i].pixel);
                pwriter.Write((byte)this.subrects[i].x);
                pwriter.Write((byte)this.subrects[i].y);
                pwriter.Write((byte)this.subrects[i].w);
                pwriter.Write((byte)this.subrects[i].h);
            }
            Watch.Stop();
            Console.WriteLine("GOTOVO! " + Watch.Elapsed);
        }
    }
}
