using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SharedLogic
{
    public class Result
    {
        public int Id { get; set; }
        public AirPhoto MainMap { get; set; }
        public AirPhoto SingleImage { get; set; }
        public DMatch[] Matches { get; set; }
        public double Min_dist { get; set; } = 10000;
        public double Max_dist { get; set; } = 0;
        public List<DMatch> Good_matchesRAW { get; set; } = new List<DMatch>();
        public List<DMatch> Current_good_matches { get; set; } = new List<DMatch>();
        public Mat RAWmatches { get; set; }
        public Mat AfterFilter { get; set; }

        public Result(AirPhoto mainMap, AirPhoto singleImage)
        {
            MainMap = mainMap;
            SingleImage = singleImage;
        }

        public Bitmap RAWmatchestoBitmap()
        {
            return RAWmatches.ToBitmap();
        }

        public Bitmap AfterFiltertoBitmap()
        {
            return AfterFilter.ToBitmap();
        }

        public System.Drawing.Point CenterPoint
        {
            get
            {
                var p1s = SingleImage.Current_good_keypoints[0];
                var p2s = SingleImage.Current_good_keypoints[1];
                var distanceSingle = Math.Sqrt(Math.Pow(p1s.X - p2s.X, 2) + Math.Pow(p1s.Y - p2s.Y, 2));
                var p1m = MainMap.Current_good_keypoints[0];
                var p2m = MainMap.Current_good_keypoints[1];
                var distanceMap = Math.Sqrt(Math.Pow(p1m.X - p2m.X, 2) + Math.Pow(p1m.Y - p2m.Y, 2));
                var koef = distanceSingle / distanceMap;

                var top = p1m.Y - p1s.Y / koef;
                var left = p1m.X - p1s.X / koef;

                var point = new System.Drawing.Point((int)left, (int)top);
                var size = new System.Drawing.Size((int)(SingleImage.StartFromPB.Width / koef), (int)(SingleImage.StartFromPB.Height / koef));

                var routePoint = new System.Drawing.Point(point.X + size.Width / 2, point.Y + size.Height / 2);

                return routePoint;
            }
        }
    }
}