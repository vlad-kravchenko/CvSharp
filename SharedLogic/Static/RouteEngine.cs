using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SharedLogic
{
    public static class RouteEngine
    {
        public static Result Run(AirPhoto mainMap, AirPhoto image)
        {
            SIFT sift = new SIFT();
            BFMatcher matcher = new BFMatcher();
            Result res = new Result(mainMap, image);
            Mat map = new Mat(res.MainMap.FileName);
            Mat singleImage = new Mat(res.SingleImage.FileName);

            sift.Run(map, null, out res.MainMap.keyPoints, res.MainMap.Descriptors);
            sift.Run(singleImage, null, out res.SingleImage.keyPoints, res.SingleImage.Descriptors);
            res.Matches = matcher.Match(res.MainMap.Descriptors, res.SingleImage.Descriptors);

            for (int i = 0; i < res.MainMap.Descriptors.Rows; i++)
            {
                double dist = res.Matches[i].Distance;
                if (dist < res.Min_dist) res.Min_dist = dist;
                if (dist > res.Max_dist) res.Max_dist = dist;
            }
            for (int i = 0; i < res.MainMap.Descriptors.Rows; i++)
            {
                if (res.Matches[i].Distance <= Math.Max(2 * res.Min_dist, 0.25))
                {
                    res.Good_matchesRAW.Add(res.Matches[i]);
                    res.Current_good_matches.Add(res.Matches[i]);
                }
            }
            for (int i = 0; i < res.Good_matchesRAW.Count; i++)
            {
                res.MainMap.Keypoints_goodRAW.Add(res.MainMap.Keypoints[res.Good_matchesRAW[i].QueryIdx].Pt);
                res.SingleImage.Keypoints_goodRAW.Add(res.SingleImage.Keypoints[res.Good_matchesRAW[i].TrainIdx].Pt);
                res.MainMap.Current_good_keypoints.Add(res.MainMap.Keypoints[res.Good_matchesRAW[i].QueryIdx].Pt);
                res.SingleImage.Current_good_keypoints.Add(res.SingleImage.Keypoints[res.Good_matchesRAW[i].TrainIdx].Pt);
            }
            Mat view = new Mat();
            Mat map_draw = new Mat(res.MainMap.FileName);
            Mat singleImage_draw = new Mat(res.SingleImage.FileName);
            Cv2.DrawMatches(map, res.MainMap.Keypoints, singleImage, res.SingleImage.Keypoints, res.Current_good_matches, view);
            for (int i = 0; i < res.MainMap.Current_good_keypoints.Count; i++)
            {
                Cv.DrawCircle((IplImage)map_draw,
                    new CvPoint(Convert.ToInt32(res.MainMap.Current_good_keypoints[i].X),
                    Convert.ToInt32(res.MainMap.Current_good_keypoints[i].Y)),
                    map_draw.Width / 500,
                    CvColor.Red, 2);
            }
            for (int i = 0; i < res.SingleImage.Current_good_keypoints.Count; i++)
            {
                Cv.DrawCircle((IplImage)singleImage_draw,
                    new CvPoint(Convert.ToInt32(res.SingleImage.Current_good_keypoints[i].X),
                    Convert.ToInt32(res.SingleImage.Current_good_keypoints[i].Y)),
                    singleImage_draw.Width / 100,
                    CvColor.Red, 2);
            }
            res.MainMap.RAWwithKP = map_draw;
            res.SingleImage.RAWwithKP = singleImage_draw;
            res.RAWmatches = view;
            return res;
        }

        public static Bitmap DrawKPAtMap(Bitmap src, List<Result> results)
        {
            List<Point2f> allKeypoints = new List<Point2f>();
            foreach (Result result in results)
            {
                foreach (var kp in result.MainMap.Current_good_keypoints)
                {
                    allKeypoints.Add(kp);
                }
            }
            Mat res = src.ToMat();
            Bitmap bitRes = res.ToBitmap();
            Pen blackPen = new Pen(Color.Red, 1);
            Graphics g = Graphics.FromImage(bitRes);
            for (int i = 0; i < allKeypoints.Count; i++)
            {
                g.DrawEllipse(blackPen, allKeypoints[i].X, allKeypoints[i].Y, 5, 5);
            }
            blackPen.Dispose();
            g.Dispose();
            allKeypoints.Clear();
            return bitRes;
        }

        public static Result FilterPoints(Result res)
        {
            List<Squares> list_squares = new List<Squares>();
            int indexGoodKP = 0;
            //пробегаем по всему главному изображению
            for (int x = 0; x < res.MainMap.RAWwithKP.Width; x += 10)
            {
                for (int y = 0; y < res.MainMap.RAWwithKP.Height; y += 10)
                {
                    //создаём новый объект класса прямоугольников
                    list_squares.Add(new Squares()
                    {
                        X1 = x,
                        Y1 = y,
                        X2 = x + 100,
                        Y2 = y + 100,
                        Count_points = 0,
                        List_points = new List<Point2f>()
                    });
                    //пробегаемся по всем текущим хорошим ключевым точкам для отдельного выбранного изображения и ищем, сколько из них внутри прямоугольника
                    foreach (var a in res.MainMap.Current_good_keypoints)
                    {
                        if (a.X > list_squares[indexGoodKP].X1 &&
                            a.Y > list_squares[indexGoodKP].Y1 &&
                            a.X < list_squares[indexGoodKP].X2 &&
                            a.Y < list_squares[indexGoodKP].Y2)
                        {
                            list_squares[indexGoodKP].Count_points++;
                            list_squares[indexGoodKP].List_points.Add(a);
                        }
                    }
                    indexGoodKP++;
                }
            }
            //находим объект из списка с наибольшим количеством точек, это объект с индесом num
            int count = 0;
            int num = 0;
            for (int i = 0; i < list_squares.Count; i++)
            {
                if (list_squares[i].Count_points > count)
                {
                    count = list_squares[i].Count_points;
                    num = i;
                }
            }
            //перезагружаем текущее (во вкладке результаты) и главное изображения (главное - в ИСХОДНОМ виде)
            Mat src11 = new Mat(res.MainMap.FileName);
            Mat src22 = new Mat(res.SingleImage.FileName);
            //на всякий обнуляем текущие хорошие ключевые точки
            res.MainMap.Current_good_keypoints.Clear();
            res.SingleImage.Current_good_keypoints.Clear();
            //возвращаем отсеянные точки обратно в объект и отрисовываем их на главном изображении
            for (int i = 0; i < list_squares[num].List_points.Count; i++)
            {
                res.MainMap.Current_good_keypoints.Add(list_squares[num].List_points[i]);
                Cv.DrawCircle(
                    (IplImage)src11,
                    new CvPoint(Convert.ToInt32(list_squares[num].List_points[i].X), Convert.ToInt32(list_squares[num].List_points[i].Y)),
                    src11.Width / 500,
                    CvColor.Red,
                    2);
            }
            //отбираем эти же ключевые точки для локального изображения
            for (int j = 0; j < res.MainMap.Current_good_keypoints.Count; j++)
            {
                for (int i = 0; i < res.Current_good_matches.Count; i++)
                {
                    if (res.MainMap.Keypoints[res.Current_good_matches[i].QueryIdx].Pt == res.MainMap.Current_good_keypoints[j])
                    {
                        res.SingleImage.Current_good_keypoints.Add(res.SingleImage.Keypoints[res.Current_good_matches[i].TrainIdx].Pt);
                    }
                }
            }
            //и отрисовываем и их
            foreach (var a in res.SingleImage.Current_good_keypoints)
            {
                Cv.DrawCircle(
                    (IplImage)src22,
                    new CvPoint(Convert.ToInt32(a.X), Convert.ToInt32(a.Y)),
                    src22.Width / 100,
                    CvColor.Red,
                    2);
            }
            res.MainMap.AfterClusterization = src11;
            res.SingleImage.AfterClusterization = src22;
            list_squares.Clear();
            return res;
        }

        public static Bitmap DrawBounds(Bitmap bitmap, List<Result> list)
        {
            Graphics g = Graphics.FromImage(bitmap);
            Pen pen = new Pen(Color.Black, 5);
            foreach (var item in list)
            {
                if (item.SingleImage.Current_good_keypoints.Count < 2) continue;

                var p1s = item.SingleImage.Current_good_keypoints[0];
                var p2s = item.SingleImage.Current_good_keypoints[1];
                var distanceSingle = Math.Sqrt(Math.Pow(p1s.X - p2s.X, 2) + Math.Pow(p1s.Y - p2s.Y, 2));
                var p1m = item.MainMap.Current_good_keypoints[0];
                var p2m = item.MainMap.Current_good_keypoints[1];
                var distanceMap = Math.Sqrt(Math.Pow(p1m.X - p2m.X, 2) + Math.Pow(p1m.Y - p2m.Y, 2));
                var koef = distanceSingle / distanceMap;

                var top = p1m.Y - p1s.Y / koef;
                var left = p1m.X - p1s.X / koef;

                var point = new System.Drawing.Point((int)left, (int)top);
                var size = new System.Drawing.Size((int)(item.SingleImage.StartFromPB.Width / koef), (int)(item.SingleImage.StartFromPB.Height / koef));

                g.DrawRectangle(pen, new Rectangle(point, size));
            }
            g.Dispose();
            pen.Dispose();
            return bitmap;
        }

        public static Bitmap BuildRoute(Bitmap bitmap, List<Result> list)
        {
            Graphics g = Graphics.FromImage(bitmap);
            Pen pen = new Pen(Color.Red, 5);
            List<System.Drawing.Point> route = new List<System.Drawing.Point>();
            foreach (var item in list)
            {
                if (item.SingleImage.Current_good_keypoints.Count < 2) continue;
                route.Add(item.CenterPoint);
            }

            for (int i = 0; i < route.Count - 1; i++)
            {
                g.DrawLine(pen, route[i], route[i + 1]);
            }

            g.Dispose();
            pen.Dispose();
            return bitmap;
        }
    }
}