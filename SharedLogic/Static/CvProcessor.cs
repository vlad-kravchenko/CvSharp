using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;

namespace SharedLogic
{
    public static class CvProcessor
    {
        public static Bitmap ChangeBrighness(Bitmap src, float[] kernel)
        {
            using (IplImage dst = Cv.CloneImage(BitmapConverter.ToIplImage(src)))
            {
                CvMat kernel_matrix = Cv.Mat(3, 3, MatrixType.F32C1, kernel);
                Cv.Filter2D(BitmapConverter.ToIplImage(src), dst, kernel_matrix, Cv.Point(-1, -1));
                using (Mat res = new Mat(dst, true))
                    return res.ToBitmap();
            }
        }

        public static Bitmap ChangeSmooth(Bitmap src, int size)
        {
            if (Convert.ToDouble(size) % 2 == 0)
                size += 1;
            using (IplImage res = Cv.CloneImage(src.ToIplImage()))
            {
                Cv.Smooth(src.ToIplImage(), res, SmoothType.Gaussian, size, size);
                using (Mat resMat = new Mat(res, true))
                    return resMat.ToBitmap();
            }
        }

        public static Bitmap ToGray(Bitmap src)
        {
            using (IplImage dst = BitmapConverter.ToIplImage(src))
            {
                using (IplImage result = Cv.CreateImage(Cv.GetSize(dst), BitDepth.U8, 1))
                {
                    if (dst.NChannels > 1)
                    {
                        Cv.CvtColor(dst, result, ColorConversion.RgbToGray);
                        using (Mat res = new Mat(result, true))
                            return res.ToBitmap();
                    }
                    else
                    {
                        return src;
                    }
                }
            }
        }

        public static Bitmap ChangeBin(Bitmap src, int left, int right)
        {
            using (IplImage res = Cv.CreateImage(Cv.GetSize(src.ToIplImage()), BitDepth.U8, 1))
            {
                Cv.InRangeS(src.ToIplImage(), Cv.ScalarAll(left), Cv.ScalarAll(right), res);
                using (Mat res1 = new Mat(res, true))
                    return res1.ToBitmap();
            }
        }

        public static Bitmap Canny(Bitmap src, int left, int right, int size)
        {
            ApertureSize apperture = ApertureSize.Size3;
            switch (size)
            {
                case 0:
                    apperture = ApertureSize.Size3; break;
                case 1:
                    apperture = ApertureSize.Size3; break;
                case 2:
                    apperture = ApertureSize.Size5; break;
                case 3:
                    apperture = ApertureSize.Size7; break;
            }
            using (IplImage temp = src.ToIplImage())
            {
                using (IplImage gray = Cv.CreateImage(Cv.GetSize(temp), BitDepth.U8, 1))
                {
                    using (IplImage dst = Cv.CreateImage(Cv.GetSize(temp), BitDepth.U8, 1))
                    {
                        Cv.CvtColor(temp, gray, ColorConversion.RgbToGray);
                        Cv.Canny(gray, dst, left, right, apperture);
                        return dst.ToBitmap();
                    }
                }
            }
        }

        public static Bitmap AdaptiveThreshold(Bitmap src, int maxValue, int blockSize)
        {
            if (Convert.ToDouble(blockSize) % 2 == 0)
                blockSize += 1;
            using (IplImage temp = src.ToIplImage())
            {
                using (IplImage temp1 = Cv.CreateImage(Cv.GetSize(temp), BitDepth.U8, 1))
                {
                    Cv.CvtColor(temp, temp1, ColorConversion.RgbToGray);
                    using (IplImage dst = Cv.CreateImage(Cv.Size(temp.Width, temp.Height), BitDepth.U8, 1))
                    {
                        Cv.AdaptiveThreshold(temp1, dst, maxValue, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, blockSize, 1);
                        return dst.ToBitmap();
                    }
                }
            }
        }
    }
}