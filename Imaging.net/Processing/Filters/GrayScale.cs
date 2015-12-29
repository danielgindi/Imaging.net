using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;

namespace Imaging.net.Processing.Filters
{
    public class GrayScale : IImageFilter
    {
        public FilterError ProcessImage(
            DirectAccessBitmap bmp,
            params object[] args)
        {
            FilterGrayScaleWeight mode = FilterGrayScaleWeight.Natural;
            foreach (object arg in args)
            {
                if (arg is FilterGrayScaleWeight)
                {
                    mode = (FilterGrayScaleWeight)arg;
                }
            }

            switch (bmp.Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return ProcessImageRgba(bmp, 3, mode);
                case PixelFormat.Format32bppRgb:
                    return ProcessImageRgba(bmp, 4, mode);
                case PixelFormat.Format32bppArgb:
                    return ProcessImageRgba(bmp, 4, mode);
                case PixelFormat.Format32bppPArgb:
                    return ProcessImage32prgba(bmp, mode);
                default:
                    return FilterError.IncompatiblePixelFormat;
            }
        }

        public FilterError ProcessImageRgba(DirectAccessBitmap bmp, int pixelLength, FilterGrayScaleWeight mode)
        {
            int cx = bmp.Width;
            int cy = bmp.Height;
            int endX = cx + bmp.StartX;
            int endY = cy + bmp.StartY;
            byte[] data = bmp.Bits;
            int stride = bmp.Stride;
            int pos;
            int x, y;

            if (mode == FilterGrayScaleWeight.Accurate)
            {
                for (y = bmp.StartY; y < endY; y++)
                {
                    pos = stride * y + bmp.StartX * pixelLength;

                    for (x = bmp.StartX; x < endX; x++)
                    {
                        data[pos] = data[pos + 1] = data[pos + 2] =
                            (byte)((data[pos] + data[pos + 1] + data[pos + 2]) / 3f);

                        pos += pixelLength;
                    }
                }
            }
            else
            {
                float rL = 0, gL = 0, bL = 0;

                if (mode == FilterGrayScaleWeight.NaturalNTSC)
                {
                    rL = GrayScaleMultiplier.NtscRed;
                    gL = GrayScaleMultiplier.NtscGreen;
                    bL = GrayScaleMultiplier.NtscBlue;
                }
                else if (mode == FilterGrayScaleWeight.Natural)
                {
                    rL = GrayScaleMultiplier.NaturalRed;
                    gL = GrayScaleMultiplier.NaturalGreen;
                    bL = GrayScaleMultiplier.NaturalBlue;
                }
                else if (mode == FilterGrayScaleWeight.Css)
                {
                    rL = GrayScaleMultiplier.CssRed;
                    gL = GrayScaleMultiplier.CssGreen;
                    bL = GrayScaleMultiplier.CssBlue;
                }
                else
                {
                    rL = GrayScaleMultiplier.SimpleRed;
                    gL = GrayScaleMultiplier.SimpleGreen;
                    bL = GrayScaleMultiplier.SimpleBlue;
                }

                for (y = bmp.StartY; y < endY; y++)
                {
                    pos = stride * y + bmp.StartX * pixelLength;

                    for (x = bmp.StartX; x < endX; x++)
                    {
                        data[pos] = data[pos + 1] = data[pos + 2] =
                            (byte)((data[pos] * bL +
                            data[pos + 1] * gL +
                            data[pos + 2] * rL));

                        pos += pixelLength;
                    }
                }
            }

            return FilterError.OK;
        }

        public FilterError ProcessImage32prgba(DirectAccessBitmap bmp, FilterGrayScaleWeight mode)
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

            if (mode == FilterGrayScaleWeight.Accurate)
            {
                for (y = bmp.StartY; y < endY; y++)
                {
                    pos = stride * y + bmp.StartX * 4;

                    for (x = bmp.StartX; x < endX; x++)
                    {
                        preAlpha = (float)data[pos + 3];
                        if (preAlpha > 0) preAlpha = preAlpha / 255f;
                        data[pos] = data[pos + 1] = data[pos + 2] =
                            (byte)(((
                            (data[pos] / preAlpha) +
                            (data[pos + 1] / preAlpha) +
                            (data[pos + 2] / preAlpha)
                            ) / 3f) * preAlpha);

                        pos += 4;
                    }
                }
            }
            else
            {
                float rL = 0, gL = 0, bL = 0;

                if (mode == FilterGrayScaleWeight.NaturalNTSC)
                {
                    rL = GrayScaleMultiplier.NtscRed;
                    gL = GrayScaleMultiplier.NtscGreen;
                    bL = GrayScaleMultiplier.NtscBlue;
                }
                else if (mode == FilterGrayScaleWeight.Natural)
                {
                    rL = GrayScaleMultiplier.NaturalRed;
                    gL = GrayScaleMultiplier.NaturalGreen;
                    bL = GrayScaleMultiplier.NaturalBlue;
                }
                else if (mode == FilterGrayScaleWeight.Css)
                {
                    rL = GrayScaleMultiplier.CssRed;
                    gL = GrayScaleMultiplier.CssGreen;
                    bL = GrayScaleMultiplier.CssBlue;
                }
                else
                {
                    rL = GrayScaleMultiplier.SimpleRed;
                    gL = GrayScaleMultiplier.SimpleGreen;
                    bL = GrayScaleMultiplier.SimpleBlue;
                }

                for (y = bmp.StartY; y < endY; y++)
                {
                    pos = stride * y + bmp.StartX * 4;

                    for (x = bmp.StartX; x < endX; x++)
                    {
                        preAlpha = (float)data[pos + 3];
                        if (preAlpha > 0) preAlpha = preAlpha / 255f;
                        data[pos] = data[pos + 1] = data[pos + 2] =
                            (byte)(((
                            (data[pos] / preAlpha) * bL +
                            (data[pos + 1] / preAlpha) * gL +
                            (data[pos + 2] / preAlpha) * rL
                            )) * preAlpha);

                        pos += 4;

                    }
                }
            }

            return FilterError.OK;
        }
    }
}
