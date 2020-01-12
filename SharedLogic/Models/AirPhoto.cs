using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System.Collections.Generic;
using System.Drawing;

namespace SharedLogic
{
    public class AirPhoto
    {
        public KeyPoint[] keyPoints;
        public KeyPoint[] Keypoints
        {
            get { return keyPoints; }
            set { keyPoints = value; }
        }

        public Bitmap StartFromPB { get; set; }
        public string FileName { get; set; }
        public MatOfFloat Descriptors { get; set; } = new MatOfFloat();
        public List<Point2f> Keypoints_goodRAW { get; set; } = new List<Point2f>();
        public List<Point2f> Current_good_keypoints { get; set; } = new List<Point2f>();
        public Mat RAWwithKP { get; set; }
        public Mat AfterClusterization { get; set; }
        public Mat AfterFilter { get; set; }

        public AirPhoto(Bitmap startFromPB, string fileName)
        {
            StartFromPB = startFromPB;
            FileName = fileName;
        }

        public Bitmap RAWwithKPtoBitmap()
        {
            return RAWwithKP.ToBitmap();
        }

        public Bitmap afterFiltertoBitmap()
        {
            return AfterFilter.ToBitmap();
        }

        public Bitmap AfterClusterizationtoBitmap()
        {
            return AfterClusterization.ToBitmap();
        }
    }
}