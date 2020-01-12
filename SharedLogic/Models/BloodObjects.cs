using OpenCvSharp;
using System.Collections.Generic;

namespace SharedLogic
{
    public class BloodObjects
    {
        public int Id { get; set; }
        public CvSeq<CvPoint> Contour { get; set; }
        public List<CvPoint?> ContourPath { get; set; } = new List<CvPoint?>();
        public double Area { get; set; }
        public CvPoint2D32f Center { get; set; }
        public double Distance { get; set; }
        public double Compact { get; set; }
        public double Perimeter { get; set; }
        public Group Group { get; set; }

        public BloodObjects(int Id, 
            CvSeq<CvPoint> Contour, 
            List<CvPoint?> Path,
            double Area, 
            CvPoint2D32f Center, 
            double Distance, 
            double Compact, 
            double Perimeter,
            Group group)
        {
            this.Id = Id;
            this.Contour = Contour;
            this.ContourPath = Path;
            this.Area = Area;
            this.Center = Center;
            this.Distance = Distance;
            this.Compact = Compact;
            this.Perimeter = Perimeter;
            Group = group;
        }
    }
}