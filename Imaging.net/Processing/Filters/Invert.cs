using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;

namespace Imaging.net.Processing.Filters
{
    public class Invert : IImageFilter
    {
        public FilterError ProcessImage(
            DirectAccessBitmap bmp,
            params object[] args)
        {
            switch (bmp.Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return ProcessImage24rgb(bmp);
                case PixelFormat.Format32bppRgb:
                    return ProcessImage32rgb(bmp);
                case PixelFormat.Format32bppArgb:
                    return ProcessImage32rgba(bmp);
                case PixelFormat.Format32bppPArgb:
                    return ProcessImage32prgba(bmp);
                default:
                    return FilterError.IncompatiblePixelFormat;
            }
        }

        public FilterError ProcessImage24rgb(DirectAccessBitmap bmp)
        {
            int cx = bmp.Width;
            int cy = bmp.Height;
            int endX = cx + bmp.StartX;
            int endY = cy + bmp.StartY;
            byte[] data = bmp.Bits;
            int stride = bmp.Stride;
            int pos;
            int x, y;

            for (y = bmp.StartY; y < endY; y++)
            {
                pos = stride * y + bmp.StartX * 3;

                for (x = bmp.StartX; x < endX; x++)
                {
                    data[pos] = (byte)(255 - data[pos]);
                    data[pos + 1] = (byte)(255 - data[pos + 1]);
                    data[pos + 2] = (byte)(255 - data[pos + 2]);

                    pos += 3;
                }
            }

            return FilterError.OK;
        }

        public FilterError ProcessImage32rgb(DirectAccessBitmap bmp)
        {
            int cx = bmp.Width;
            int cy = bmp.Height;
            int endX = cx + bmp.StartX;
            int endY = cy + bmp.StartY;
            byte[] data = bmp.Bits;
            int stride = bmp.Stride;
            int pos;
            int x, y;

            for (y = bmp.StartY; y < endY; y++)
            {
                pos = stride * y + bmp.StartX * 4;

                for (x = bmp.StartX; x < endX; x++)
                {
                    data[pos] = (byte)(255 - data[pos]);
                    data[pos + 1] = (byte)(255 - data[pos + 1]);
                    data[pos + 2] = (byte)(255 - data[pos + 2]);

                    pos += 4;
                }
            }

            return FilterError.OK;
        }

        public FilterError ProcessImage32rgba(DirectAccessBitmap bmp)
        {
            return ProcessImage32rgb(bmp);
        }

        public FilterError ProcessImage32prgba(DirectAccessBitmap bmp)
        {
            int cx = bmp.Width;
            int cy = bmp.Height;
            int endX = cx + bmp.StartX;
            int endY = cy + bmp.StartY;
            byte[] data = bmp.Bits;
            int stride = bmp.Stride;
            int pos;
            int x, y;
            float preAlpha;

            for (y = bmp.StartY; y < endY; y++)
            {
                pos = stride * y + bmp.StartX * 4;

                for (x = bmp.StartX; x < endX; x++)
                {
                    preAlpha = (float)data[pos + 3];
                    if (preAlpha > 0) preAlpha = preAlpha / 255f;

                    data[pos] = (byte)((255f - (data[pos] / preAlpha)) * preAlpha);
                    data[pos + 1] = (byte)((255f - (data[pos + 1] / preAlpha)) * preAlpha);
                    data[pos + 2] = (byte)((255f - (data[pos + 2] / preAlpha)) * preAlpha);

                    pos += 4;
                }
            }

            return FilterError.OK;
        }
    }
}
