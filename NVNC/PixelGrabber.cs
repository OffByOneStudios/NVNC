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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NVNC
{
    public static unsafe class PixelGrabber
    {
        /*
        public static byte[] CreateScreenCapture2(Rectangle r)
        {
            try
            {
                int width = r.Width;
                int height = r.Height;
                Bitmap bitmap = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(r.X, r.Y, 0, 0, new Size(width, height));
                ImageCodecInfo imageEncoder = GetEncoder(ImageFormat.Bmp);
                // Create an Encoder object based on the GUID
                // for the Quality parameter category.
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                // Create an EncoderParameters object.
                // An EncoderParameters object has an array of EncoderParameter
                // objects. In this case, there is only one
                // EncoderParameter object in the array.
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100);
                myEncoderParameters.Param[0] = myEncoderParameter;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bitmap.Save(ms, imageEncoder, myEncoderParameters);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }
        */
        public static Bitmap CreateScreenCapture(Rectangle r)
        {
            try
            {
                int width = r.Width;
                int height = r.Height;
                Bitmap bitmap = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(r.X, r.Y, 0, 0, new Size(width, height));
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                System.Threading.Thread.Sleep(500);
                try
                {
                    int width = r.Width;
                    int height = r.Height;
                    Bitmap bitmap = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(bitmap);
                    g.CopyFromScreen(r.X, r.Y, 0, 0, new Size(width, height));
                }
                catch (Exception)
                {
                    return null;
                }
                return null;
            }
        }
        public static byte[] BitmapToPng(Bitmap bmp)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public static Image BitmapToPng(Image bmp)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                return new Bitmap(stream);
            }
        }
        public static Bitmap CreateScreenCapture(int x, int y, int w, int h)
        {
            Bitmap bitmap = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(x, y, 0, 0, new Size(w, h));
            return bitmap;
        }
        public static byte[] GrabPixels(Bitmap bmp, PixelFormat pf)
        {
            BitmapData bData = bmp.LockBits(new Rectangle(new Point(0,0), bmp.Size),
                ImageLockMode.ReadOnly,
                pf);
            // number of bytes in the bitmap
            int byteCount = bData.Stride * bmp.Height;
            byte[] bmpBytes = new byte[byteCount];

            // Copy the locked bytes from memory
            Marshal.Copy(bData.Scan0, bmpBytes, 0, byteCount);

            // don't forget to unlock the bitmap!!
            bmp.UnlockBits(bData);

            return bmpBytes;
        }
        public static byte[] GrabPixels(int[] pixels, Rectangle rectangle, PixelFormat pf)
        {
            // Encode as bytes
            int x = rectangle.X;
            int y = rectangle.Y;
            int w = rectangle.Width;
            int h = rectangle.Height;

            byte[] bytes = null;

            int b = 0;
            int i = 0;
            int s = 0;
            int pixel;
            int size = rectangle.Width * rectangle.Height;
            int scanline = rectangle.Width;
            int jump = scanline - w;
            int offsetX = rectangle.X;
            int offsetY = rectangle.Y;

            int p = (y - offsetY) * w + x - offsetX;

            switch (pf)
            {
                case (PixelFormat.Format32bppArgb | PixelFormat.Format32bppRgb | PixelFormat.Format32bppPArgb):
                    bytes = new byte[size << 2];
                    for (; i < size; i++, s++, p++)
                    {
                        if (s == w)
                        {
                            s = 0;
                            p += jump;
                        }
                        //pixel = framebuffer.TranslatePixel(pixels[p]);
                        pixel = pixels[p];
                        bytes[b++] = (byte)(pixel & 0xFF); //B
                        bytes[b++] = (byte)((pixel >> 8) & 0xFF); //G
                        bytes[b++] = (byte)((pixel >> 16) & 0xFF); //R
                        bytes[b++] = (byte)((pixel >> 24) & 0xFF); //A
                    }
                    break;
                case (PixelFormat.Format16bppRgb565 | PixelFormat.Format16bppRgb555 | PixelFormat.Format16bppGrayScale | PixelFormat.Format16bppArgb1555):
                    bytes = new byte[size << 1];
                    for (; i < size; i++, s++, p++)
                    {
                        if (s == w)
                        {
                            s = 0;
                            p += jump;
                        }
                        pixel = pixels[p];
                        bytes[b++] = (byte)(pixel & 0xFF); //B
                        bytes[b++] = (byte)((pixel >> 8) & 0xFF); //G
                    }
                    break;
                case (PixelFormat.Format8bppIndexed):
                    bytes = new byte[size];
                    for (; i < size; i++, s++, p++)
                    {
                        if (s == w)
                        {
                            s = 0;
                            p += jump;
                        }
                        bytes[i] = (byte)pixels[p]; //B
                    }
                    break;
            }
            return bytes;

        }
        public static byte[] GrabCompressedPixels(int[] pixels, int scanline, Rectangle rectangle, Framebuffer fb)
        {
            // Encode as bytes
            int x = rectangle.X;
            int y = rectangle.Y;
            int w = rectangle.Width;
            int h = rectangle.Height;

            byte[] bytes = null;

            int b = 0;
            int i = 0;
            int s = 0;
            int pixel;
            int size = rectangle.Width * rectangle.Height;
            int offsetX = rectangle.X;
            int offsetY = rectangle.Y;
            int jump = scanline - w;
            int p = (y - offsetY) * w + x - offsetX;


            bytes = new byte[size << 2];
            for (; i < size; i++, s++, p++)
            {
                if (s == w)
                {
                    s = 0;
                    p += jump;
                }
                int tmp = pixels[p];
                pixel = fb.TranslatePixel(tmp);
                //pixel = pixels[p];

                bytes[b++] = (byte)(pixel & 0xFF); //B
                bytes[b++] = (byte)((pixel >> 8) & 0xFF); //G
                bytes[b++] = (byte)((pixel >> 16) & 0xFF); //R
                // bytes[b++] = (byte)((pixel >> 24) & 0xFF); //A
            }
            return bytes;
        }
        public static byte[] GrabPixels(int[] pixels, Rectangle rectangle, Framebuffer fb)
        {
            // Encode as bytes
            int x = rectangle.X;
            int y = rectangle.Y;
            int w = rectangle.Width;
            int h = rectangle.Height;

            byte[] bytes = null;

            int b = 0;
            int i = 0;
            int s = 0;
            int pixel;
            int size = rectangle.Width * rectangle.Height;
            int scanline = rectangle.Width;
            int offsetX = rectangle.X;
            int offsetY = rectangle.Y;
            int jump = scanline - w;
            int p = (y - offsetY) * w + x - offsetX;

            switch (fb.BitsPerPixel)
            {
                case 32:
                    bytes = new byte[size << 2];
                    for (; i < size; i++, s++, p++)
                    {
                        if (s == w)
                        {
                            s = 0;
                            p += jump;
                        }
                        int tmp = pixels[p];
                        pixel = fb.TranslatePixel(tmp);
                        //pixel = pixels[p];

                        bytes[b++] = (byte)(pixel & 0xFF); //B
                        bytes[b++] = (byte)((pixel >> 8) & 0xFF); //G
                        bytes[b++] = (byte)((pixel >> 16) & 0xFF); //R
                        bytes[b++] = (byte)((pixel >> 24) & 0xFF); //A
                    }
                    break;
                case 24:
                    bytes = new byte[size << 2];
                    for (; i < size; i++, s++, p++)
                    {
                        if (s == w)
                        {
                            s = 0;
                            p += jump;
                        }
                        pixel = fb.TranslatePixel(pixels[p]);
                        bytes[b++] = (byte)(pixel & 0xFF); //B
                        bytes[b++] = (byte)((pixel >> 8) & 0xFF); //G
                        bytes[b++] = (byte)((pixel >> 16) & 0xFF); //R
                    }
                    break;
                case 16:
                    bytes = new byte[size << 1];
                    for (; i < size; i++, s++, p++)
                    {
                        if (s == w)
                        {
                            s = 0;
                            p += jump;
                        }
                        pixel = fb.TranslatePixel(pixels[p]);
                        bytes[b++] = (byte)(pixel & 0xFF); //B
                        bytes[b++] = (byte)((pixel >> 8) & 0xFF); //G
                    }
                    break;
                case 8:
                    bytes = new byte[size];
                    for (; i < size; i++, s++, p++)
                    {
                        if (s == w)
                        {
                            s = 0;
                            p += jump;
                        }
                        bytes[i] = (byte)fb.TranslatePixel(pixels[p]); //B
                    }
                    break;
            }
            return bytes;

        }
        public static byte[] GrabBytes(int pixel, Framebuffer fb)
        {
            int b = 0;
            byte[] bytes = null;
            switch (fb.BitsPerPixel)
            {
                case 32:
                    bytes = new byte[4];
                    bytes[b++] = (byte)(pixel & 0xFF);
                    bytes[b++] = (byte)((pixel >> 8) & 0xFF);
                    bytes[b++] = (byte)((pixel >> 16) & 0xFF);
                    bytes[b++] = (byte)((pixel >> 24) & 0xFF);
                    break;
                case 16:
                    bytes = new byte[2];
                    bytes[b++] = (byte)(pixel & 0xFF);
                    bytes[b++] = (byte)((pixel >> 8) & 0xFF);
                    break;
                case 8:
                    bytes = new byte[1];
                    bytes[b++] = (byte)(pixel & 0xFF);
                    break;
            }
            return bytes;
        }

        public static int[] GrabPixels(Bitmap img)
        {
            int[] array = new int[img.Width * img.Height];
            BitmapData bmp = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, img.PixelFormat);
            unsafe
            {
                int PixelSize = 4;

                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* row = (byte*)bmp.Scan0 + (y * bmp.Stride);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        int a = (int)row[(x * PixelSize) + 3]; //A
                        int r = (int)row[(x * PixelSize) + 2]; //R
                        int g = (int)row[(x * PixelSize) + 1]; //G
                        int b = (int)row[(x * PixelSize)]; //B

                        Color c = Color.FromArgb(a, r, g, b);
                        int val = c.ToArgb();
                        array[y * bmp.Width + x] = val;
                    }
                }

            }
            img.UnlockBits(bmp);
            return array;
        }
        public static int[]  GrabPixels(Bitmap img, int x, int y, int w, int h, PixelFormat pf)
        {
            int[] array = new int[w * h];
            BitmapData bmp = img.LockBits(new Rectangle(x, y, w, h), ImageLockMode.ReadOnly, pf);
            unsafe
            {
                int PixelSize = 4;

                for (int j = 0; j < h; j++)
                {
                    byte* row = (byte*)bmp.Scan0 + (j * bmp.Stride);
                    for (int i = 0; i < w; i++)
                    {
                        int a = (int)row[(i * PixelSize) + 3];
                        int r = (int)row[(i * PixelSize) + 2];
                        int g = (int)row[(i * PixelSize) + 1];
                        int b = (int)row[(i * PixelSize)];

                        Color c = Color.FromArgb(a, r, g, b);
                        int val = c.ToArgb();
                        array[j * w + i] = val;
                    }
                }

            }
            img.UnlockBits(bmp);
            return array;
        }

        public static Bitmap GrabImage(byte[] data, int w, int h, PixelFormat pf)
        {
            int m = 0;
            switch (pf)
            {
                case (PixelFormat.Format32bppRgb | PixelFormat.Format32bppPArgb | PixelFormat.Format32bppArgb):
                    m = 4;
                    break;
                case PixelFormat.Format24bppRgb:
                    m = 3;
                    break;
                case (PixelFormat.Format16bppRgb565 | PixelFormat.Format16bppRgb555 | PixelFormat.Format16bppArgb1555 | PixelFormat.Format16bppGrayScale):
                    m = 2;
                    break;
            }
            fixed (byte* ptr = data)
            {
                Bitmap image = new Bitmap(w, h, m * w, pf, new IntPtr(ptr));
                return image;
            }
        }
        public static Bitmap GrabImage(int w, int h, int[] data)
        {
            /*Color[,] r = new Color[w, h];
            for (int i = 0; i < data.Length; i++)
                r[i % w, i / w] = Color.FromArgb(data[i]); // Copy over into a Color structure
            */

            Bitmap ret = new Bitmap(w, h);
            BitmapData bmd = ret.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, ret.PixelFormat);
            int PixelSize = 4;
            for (int y = 0; y < bmd.Height; y++)
            {
                byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                for (int x = 0; x < bmd.Width; x++)
                {
                    Color c = Color.FromArgb(data[(y * bmd.Width) + x]);
                    row[(x * PixelSize) + 3] = c.A;
                    row[(x * PixelSize) + 2] = c.R;
                    row[(x * PixelSize) + 1] = c.G;
                    row[(x * PixelSize)] = c.B;
                }
            }
            ret.UnlockBits(bmd);
            return ret;
        }

        public static int GetRGB(Bitmap bmp, int x, int y)
        {
            return bmp.GetPixel(x, y).ToArgb();
        }
        public static Bitmap GetSubImage(Bitmap bmp, Rectangle rect)
        {
            try
            {
                int[] data = GrabPixels(bmp, rect.X, rect.Y, rect.Width, rect.Height, bmp.PixelFormat);
                return GrabImage(rect.Width, rect.Height, data);
            }
            catch (Exception)
            {
                return bmp;
            }
        }
        public static Bitmap GetSubImage2(Bitmap bmp, Rectangle rect)
        {
            Bitmap cropped = bmp.Clone(rect, bmp.PixelFormat);
            return cropped;
        }
        public static Rectangle AlignRectangle(Rectangle paramRectangle, int dw, int dh)
        {
            int i = paramRectangle.X % 16;
            if (i != 0)
                paramRectangle.X -= i;
            i = paramRectangle.Y % 16;
            if (i != 0)
                paramRectangle.Y -= i;
            i = paramRectangle.Width % 16;
            if (i != 0)
            {
                paramRectangle.Width = (paramRectangle.Width - i + 16);
                if (paramRectangle.X + paramRectangle.Width > dw)
                    paramRectangle.Width = (dw - paramRectangle.X);
            }
            i = paramRectangle.Height % 16;
            if (i != 0)
            {
                paramRectangle.Height = (paramRectangle.Height - i + 16);
                if (paramRectangle.Y + paramRectangle.Height > dh)
                    paramRectangle.Height = (dh - paramRectangle.Y);
            }
            return paramRectangle;
        }
        public static bool IsChangeArea(Bitmap paramBitmap1, Bitmap paramBitmap2, Rectangle paramRectangle)
        {
            int[] data1 = GrabPixels(paramBitmap1, paramRectangle.X, paramRectangle.Y, paramRectangle.Width, paramRectangle.Height, paramBitmap1.PixelFormat);
            int[] data2 = GrabPixels(paramBitmap2, paramRectangle.X, paramRectangle.Y, paramRectangle.Width, paramRectangle.Height, paramBitmap2.PixelFormat);

            for (int i = 0; i < data1.Length; i++)
            {
                if (data1[i] != data2[i])
                    return true;
            }
            return false;
        }
        public static Rectangle GetChangeArea(Bitmap bmp1, Bitmap bmp2, Rectangle rect)
        {
            bool first_x = false;
            int minx1 = -1, maxx2 = -1, miny1 = -1, maxy2 = -1;
            int w = bmp1.Width;
            int h = bmp1.Height;
            int[] pixels1 = GrabPixels(bmp1);
            int[] pixels2 = GrabPixels(bmp2);

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (pixels2[i + j * w] != pixels1[i + j * w])
                    {
                        if (!first_x) 
                            minx1 = i; miny1 = j; maxx2 = i; maxy2 = j; first_x = true; 

                        if (minx1 > i) minx1 = i;
                        if (miny1 > j) miny1 = j;
                        if (maxx2 < i) maxx2 = i;
                        if (maxy2 < j) maxy2 = j;
                        // System.out.println(i +"x"+ j);
                    }
                }
            }
            //if (minx1 == maxx2 && maxy2 == miny1)
            //Console.WriteLine("Single pixel modified");
            //else
            //{
            //Console.WriteLine("Modified part (rectangle): " + minx1 + "," + miny1 + "<->" + maxx2 + "," + maxy2);
            return new Rectangle(minx1, miny1, maxx2, maxy2);
            //}
            //return Rectangle.Empty;
        }
    }
}
