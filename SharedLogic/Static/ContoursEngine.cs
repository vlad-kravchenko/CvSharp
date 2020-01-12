using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SharedLogic
{
    public static class ContoursEngine
    {
        public static void GetSingleContour(Bitmap src, Point point, int id, Bitmap scene, out Bitmap result, out double area)
        {
            string adr = "temp.jpg";
            File.Delete(adr);
            src.Save(adr);
            IplImage image = Cv.LoadImage(adr, LoadMode.AnyColor);
            IplImage gray = Cv.CreateImage(Cv.GetSize(image), BitDepth.U8, 1);
            IplImage bin = Cv.CreateImage(Cv.GetSize(image), BitDepth.U8, 1);
            scene.Save(adr);
            IplImage dst = Cv.LoadImage(adr, LoadMode.AnyColor);
            Cv.CvtColor(image, gray, ColorConversion.RgbToGray);
            Cv.InRangeS(gray, 150, 255, bin);
            CvMemStorage storage = Cv.CreateMemStorage(0);
            CvSeq<CvPoint> contours = null;
            int cont = Cv.FindContours(bin, storage, out contours, CvContour.SizeOf, ContourRetrieval.List, ContourChain.ApproxTC89KCOS, Cv.Point(0, 0));
            contours = Cv.ApproxPoly(contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, 0, true);
            double temp = 0;
            for (CvSeq<CvPoint> seq0 = contours; seq0 != null; seq0 = seq0.HNext)
            {
                if (Cv.PointPolygonTest(seq0, new CvPoint2D32f(point.X, point.Y), false) > 0
                    && Cv.ContourArea(seq0) > 1000
                    && Cv.ContourArea(seq0) < (image.Height * image.Width * 0.5))
                {
                    CvMoments moments = new CvMoments();
                    Cv.Moments(seq0, out moments, true);
                    int xc = (int)(moments.M10 / moments.M00);
                    int yc = (int)(moments.M01 / moments.M00);
                    CvConnectedComp comp;
                    if (id == 0)
                        Cv.FloodFill(dst, Cv.Point(point.X, point.Y), Cv.RGB(200, 0, 0), Cv.ScalarAll(10), Cv.ScalarAll(10), out comp, FloodFillFlag.FixedRange, null);
                    else
                        Cv.FloodFill(dst, Cv.Point(point.X, point.Y), Cv.RGB(0, 150, 50), Cv.ScalarAll(10), Cv.ScalarAll(10), out comp, FloodFillFlag.FixedRange, null);
                    dst.PutText(
                        id.ToString(),
                        Cv.Point(xc, yc),
                        new CvFont(FontFace.HersheySimplex, 2, 2, 1, 5, LineType.Link8),
                        CvColor.Black);
                    temp = Cv.ContourArea(seq0);
                }
            }
            result = dst.ToBitmap();
            area = temp;
        }

        public static void GetAllObjects(Bitmap src, Point point, out Bitmap result, out ComplexObject etalon, out List<ComplexObject> allcontours)
        {
            allcontours = new List<ComplexObject>();
            etalon = new ComplexObject();
            string adr = "temp.jpg";
            if (File.Exists(adr)) File.Delete(adr);
            src.Save(adr);            
            IplImage image = Cv.LoadImage(adr, LoadMode.AnyColor);
            if (File.Exists(adr)) File.Delete(adr);
            IplImage gray = Cv.CreateImage(Cv.GetSize(image), BitDepth.U8, 1);
            IplImage bin = Cv.CreateImage(Cv.GetSize(image), BitDepth.U8, 1);
            IplImage dst = Cv.CloneImage(image);
            Cv.CvtColor(image, gray, ColorConversion.RgbToGray);
            Cv.InRangeS(gray, 150, 255, bin);
            CvMemStorage storage = Cv.CreateMemStorage(0);
            CvSeq<CvPoint> contours = null;
            int cont = Cv.FindContours(bin, storage, out contours, CvContour.SizeOf, ContourRetrieval.List, ContourChain.ApproxTC89KCOS, Cv.Point(0, 0));
            contours = Cv.ApproxPoly(contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, 0, true);
            int id = 1;
            for (CvSeq<CvPoint> seq0 = contours; seq0 != null; seq0 = seq0.HNext)
            {
                if (Cv.ContourArea(seq0) > 1000 && Cv.ContourArea(seq0) < (image.Height * image.Width * 0.5))
                {
                    CvMoments moments = new CvMoments();
                    Cv.Moments(seq0, out moments, true);
                    double xc = (moments.M10 / moments.M00);
                    double yc = (moments.M01 / moments.M00);
                    double distance = Math.Sqrt(xc * xc + yc * yc);
                    if (Cv.PointPolygonTest(seq0, new CvPoint2D32f(point.X, point.Y), false) > 0)
                    {
                        etalon = new ComplexObject(seq0, new CvPoint2D32f(xc, yc), true, 0, Cv.ContourArea(seq0), distance);
                        Cv.DrawContours(dst, seq0, Cv.RGB(250, 0, 0), Cv.RGB(50, 250, 0), 0, -1, LineType.Link8);
                        dst.PutText(
                            "0",
                            Cv.Point((int)xc, (int)yc),
                            new CvFont(FontFace.HersheySimplex, 2, 2, 1, 5, LineType.Link8),
                            CvColor.Black);
                    }
                    else
                    {
                        allcontours.Add(new ComplexObject(seq0, new CvPoint2D32f(xc, yc), false, id, Cv.ContourArea(seq0), distance));
                        id++;
                    }
                }
            }
            allcontours.Sort(delegate (ComplexObject ob1, ComplexObject ob2)
            {
                return ob1.Distance.CompareTo(ob2.Distance);
            });
            for (int i = 0; i < allcontours.Count; i++)
            {
                allcontours[i].Id = i + 1;
                Cv.DrawContours(dst, allcontours[i].Cont, Cv.RGB(0, 150, 50), Cv.RGB(50, 250, 0), 0, -1, LineType.Link8);
                dst.PutText(
                    allcontours[i].Id.ToString(),
                    allcontours[i].Center,
                    new CvFont(FontFace.HersheySimplex, 2, 2, 1, 5, LineType.Link8),
                    CvColor.Black);
            }
            allcontours.Sort(delegate (ComplexObject ob1, ComplexObject ob2)
            {
                return ob1.Id.CompareTo(ob2.Id);
            });
            result = dst.ToBitmap();
        }
        
        public static void GetAllObjects(Bitmap src, out Bitmap result, out List<BloodObjects> allObjects, string adrWEB)
        {
            allObjects = new List<BloodObjects>();
            string adr = string.Empty;
            try
            {
                adr = "temp.jpg";
                File.Delete(adr);
                src.Save(adr);
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                adr = adrWEB;
                File.Delete(adr);
                src.Save(adr);
            }
            IplImage image = Cv.LoadImage(adr, LoadMode.AnyColor);
            IplImage gray = Cv.CreateImage(Cv.GetSize(image), BitDepth.U8, 1);
            IplImage bin = Cv.CreateImage(Cv.GetSize(image), BitDepth.U8, 1);
            IplImage dst = Cv.CloneImage(image);
            Cv.CvtColor(image, gray, ColorConversion.RgbToGray);
            Cv.InRangeS(gray, 150, 255, bin);
            CvMemStorage storage = Cv.CreateMemStorage(0);
            CvSeq<CvPoint> contours = null;
            int cont = Cv.FindContours(bin, storage, out contours, CvContour.SizeOf, ContourRetrieval.List, ContourChain.ApproxTC89KCOS, Cv.Point(0, 0));
            contours = Cv.ApproxPoly(contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, 0, true);
            int id = 0;
            for (CvSeq<CvPoint> seq0 = contours; seq0 != null; seq0 = seq0.HNext)
            {
                if (Cv.ContourArea(seq0) > 100 && Cv.ContourArea(seq0) < (image.Height * image.Width * 0.5))
                {
                    CvMoments moments = new CvMoments();
                    Cv.Moments(seq0, out moments, true);
                    double xc = (moments.M10 / moments.M00);
                    double yc = (moments.M01 / moments.M00);
                    double distance = Math.Sqrt(xc * xc + yc * yc);
                    allObjects.Add(new BloodObjects(
                        id, 
                        seq0, 
                        seq0.ToList(),
                        Cv.ContourArea(seq0), 
                        new CvPoint2D32f(xc, yc), 
                        distance, 
                        Math.Pow(Cv.ContourPerimeter(seq0),2)/Cv.ContourArea(seq0),
                        Cv.ContourPerimeter(seq0),
                        Group.Interest));
                    id++;
                }
            }
            allObjects = Classified(allObjects);
            allObjects.Sort(delegate (BloodObjects ob1, BloodObjects ob2)
            {
                return ob1.Distance.CompareTo(ob2.Distance);
            });
            for (int i = 0; i < allObjects.Count; i++)
            {
                allObjects[i].Id = i;
                if (allObjects[i].Group == Group.Interest)
                    Cv.DrawContours(dst, allObjects[i].Contour, Cv.RGB(0, 250, 0), Cv.RGB(50, 250, 0), 0, -1, LineType.Link8);
                else if(allObjects[i].Group == Group.Small)
                    Cv.DrawContours(dst, allObjects[i].Contour, Cv.RGB(0, 0, 250), Cv.RGB(50, 250, 0), 0, -1, LineType.Link8);
                else
                    Cv.DrawContours(dst, allObjects[i].Contour, Cv.RGB(250, 0, 0), Cv.RGB(50, 250, 0), 0, -1, LineType.Link8);
                dst.PutText(
                    allObjects[i].Id.ToString(),
                    allObjects[i].Center,
                    new CvFont(FontFace.HersheySimplex, 0.4, 0.4, 0.5, 1, LineType.Link8),
                    CvColor.Black);
            }
            allObjects.Sort(delegate (BloodObjects ob1, BloodObjects ob2)
            {
                return ob1.Id.CompareTo(ob2.Id);
            });
            result = dst.ToBitmap();
        }

        private static List<BloodObjects> Classified(List<BloodObjects> allObjects)
        {
            allObjects.Sort(delegate (BloodObjects ob1, BloodObjects ob2)
            {
                return ob1.Area.CompareTo(ob2.Area);
            });
            List<int> areas = new List<int>();
            List<int> compacts = new List<int>();
            List<int> perimeters = new List<int>();
            foreach (var obj in allObjects)
            {
                areas.Add((int)obj.Area);
                compacts.Add((int)obj.Compact);
                perimeters.Add((int)obj.Perimeter);
            }
            foreach (var obj in allObjects)
            {
                double dev_area = StdDev(areas, true);
                double median_area = (double)GetMedian(areas);
                double dev_compact = StdDev(compacts, true);
                double median_compact = (double)GetMedian(compacts);
                double dev_perimeter = StdDev(perimeters, true);
                double median_perimeter = (double)GetMedian(perimeters);
                if ((obj.Area > median_area - dev_area && obj.Area < median_area + dev_area) &&
                    (obj.Compact > median_compact - dev_compact && obj.Compact < median_compact + dev_compact) &&
                    (obj.Perimeter > median_perimeter - dev_perimeter && obj.Perimeter < median_perimeter + dev_perimeter))
                    obj.Group = Group.Interest;
                else
                    obj.Group = Group.Small;
            }
            List<BloodObjects> forSmall = allObjects.Where(x => x.Group == Group.Small).ToList();
            for (int i = 1; i < forSmall.Count; i++)
            {
                if (forSmall[i].Area / forSmall[i - 1].Area > 2)
                    for (int j = i; j < forSmall.Count; j++)
                        forSmall[j].Group = Group.Unknown;
            }
            foreach (var obj1 in allObjects)
                foreach (var obj2 in forSmall)
                    if (obj1.Id == obj2.Id)
                        obj1.Group = obj2.Group;
            return allObjects;
        }

        private static decimal GetMedian(List<int> source)
        {
            // Create a copy of the input, and sort the copy
            int[] temp = source.ToArray();
            Array.Sort(temp);
            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                int a = temp[count / 2 - 1];
                int b = temp[count / 2];
                return (a + b) / 2m;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }

        private static double StdDev(List<int> values, bool as_sample)
        {
            // Если второй аргумент равен True, оцените как образец.
            // Если второй аргумент False, оцените как совокупность.
            // Преобразуем в перечислимое число пар.
            IEnumerable<double> doubles = values.Select(value => Convert.ToDouble(value));
            // Получить среднее значение.
            double mean = doubles.Sum() / doubles.Count();
            // Получим сумму квадратов различий
            // между значениями и средним значением.
            var squares_query =
                from double value in doubles
                select (value - mean) * (value - mean);
            double sum_of_squares = squares_query.Sum();
            if (as_sample)
            {
                return Math.Sqrt(sum_of_squares / (doubles.Count() - 1));
            }
            else
            {
                return Math.Sqrt(sum_of_squares / doubles.Count());
            }
        }

        public static Bitmap DrawObjectsOnImage(Bitmap src, List<BloodObjects> list)
        {
            Bitmap result = src.Clone() as Bitmap;
            Graphics g = Graphics.FromImage(result);
            foreach (var item in list)
            {
                List<Point> path = new List<Point>();
                foreach (var p in item.ContourPath)
                    if (p.HasValue)
                        path.Add(new Point(p.Value.X, p.Value.Y));

                if (path.Count > 0)
                    g.FillPolygon(Brushes.Red, path.ToArray());
            }
            return result;
        }
    }
}