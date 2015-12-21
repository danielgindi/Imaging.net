using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using BitMiracle.LibJpeg.Classic;
using System.IO;
using System.Drawing.Imaging;

namespace Imaging.net.Encoders
{
    public static class JpegEncoder
    {
        // Handles warning silently
        private class LibJpegErrorHandler : jpeg_error_mgr
        {
            public override void emit_message(int msg_level)
            {
            }
            public override void output_message()
            {
            }
        }

        /// <summary>
        /// Encodes the image using libjpeg.
        /// In any case of failure, will fallback to default encoder.
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="filePath">Target file path, will be overwritten</param>
        /// <param name="quality">Quality of output JPEG. 0.0 - 1.0.</param>
        /// <param name="smooth">Smooth input image? 0.0 - 1.0. Default 0.0;</param>
        /// <param name="progressive">Should the image be progressive? (Tends to result in smaller sizes for bigger resolutions)</param>
        public static unsafe void EncodeImageWithLibjpeg(Image image, string filePath, float quality, float smooth = 0f, bool progressive = true)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    var bmp = NormalizedBitmapForImage(image);

                    try
                    {
                        jpeg_error_mgr errorManager = new LibJpegErrorHandler();
                        jpeg_compress_struct cinfo = new jpeg_compress_struct(errorManager);

                        cinfo.Image_width = image.Width;
                        cinfo.Image_height = image.Height;
                        cinfo.Input_components = 3;
                        cinfo.In_color_space = J_COLOR_SPACE.JCS_RGB;

                        cinfo.jpeg_set_defaults();

                        cinfo.jpeg_set_quality(Math.Min(100, Math.Max(0, (int)Math.Round(quality * 100f))), true);

                        cinfo.Smoothing_factor = Math.Min(100, Math.Max(0, (int)Math.Round(smooth * 100f)));

                        if (progressive)
                        {
                            cinfo.jpeg_simple_progression();
                        }

                        var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                        try
                        {

                            // Set destination
                            cinfo.jpeg_stdio_dest(fs);

                            // Start compressor
                            cinfo.jpeg_start_compress(true);

                            // Process data

                            var pt = (byte*)data.Scan0;

                            int lineWidth = data.Width * 3,
                                stride = data.Stride;

                            byte swap;
                            var scanLine = new byte[stride];
                            var scanLines = new byte[][] { scanLine };

                            for (int y = 0, height = bmp.Height, x; y < height; y++)
                            {
                                System.Runtime.InteropServices.Marshal.Copy(data.Scan0 + y * stride, scanLine, 0, lineWidth);

                                for (x = 0; x < lineWidth; x += 3)
                                {
                                    swap = scanLine[x];
                                    scanLine[x] = scanLine[x + 2];
                                    scanLine[x + 2] = swap;
                                }

                                cinfo.jpeg_write_scanlines(scanLines, 1);
                            }

                            // Finish compression and release memory
                            cinfo.jpeg_finish_compress();
                        }
                        finally
                        {
                            bmp.UnlockBits(data);
                        }
                    }
                    finally
                    {
                        if (bmp != image)
                        {
                            bmp.Dispose();
                        }
                    }
                }
            }
            catch
            {
                // Fallback
                EncodeImageWithDefault(image, filePath, quality);
            }
        }

        /// <summary>
        /// Encodes the image using default framework encoder
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="filePath">Target file path, will be overwritten</param>
        /// <param name="quality">Quality of output JPEG. 0.0 - 1.0.</param>
        public static void EncodeImageWithDefault(Image image, string filePath, float quality)
        {
            System.Drawing.Imaging.ImageCodecInfo encoder = null;
            System.Drawing.Imaging.ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            using (EncoderParameters encoderParameters = new EncoderParameters(1))
            {
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Math.Max(Math.Min((long)Math.Round(quality * 100), 100L), 0L));
                foreach (System.Drawing.Imaging.ImageCodecInfo item in encoders)
                {
                    if (item.MimeType == @"image/jpeg")
                    {
                        encoder = item;
                        break;
                    }
                }

                if (encoder != null)
                {
                    image.Save(filePath, encoder, encoderParameters);
                }
                else
                {
                    image.Save(filePath, ImageFormat.Jpeg);
                }
            }
        }

        private static Bitmap NormalizedBitmapForImage(Image image)
        {
            if (image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                return image as Bitmap;
            }

            Bitmap bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                if ((image.PixelFormat & PixelFormat.Alpha) == PixelFormat.Alpha ||
                    (image.PixelFormat & PixelFormat.PAlpha) == PixelFormat.PAlpha ||
                    (image.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
                {
                    // Chance of transparency
                    g.FillRectangle(new SolidBrush(Color.White), 0, 0, image.Width, image.Height);
                }
                g.DrawImage(image, 0, 0);
            }

            return bmp;
        }
    }
}
