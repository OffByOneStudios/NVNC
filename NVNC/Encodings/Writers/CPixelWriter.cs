using System;
using System.IO;


namespace NVNC.Encodings
{
    // <summary>
    /// A compressed PixelWriter.
    /// </summary>
    public sealed class CPixelWriter : PixelWriter
    {
        public CPixelWriter(BinaryWriter writer, Framebuffer framebuffer)
            : base(writer, framebuffer)
        {
        }

        /*
        public override int ReadPixel()
        {
            byte[] b = reader.ReadBytes(3);
            return ToGdiPlusOrder(b[2], b[1], b[0]);
        }
        */
    }
}
