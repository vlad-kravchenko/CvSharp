using OpenCvSharp;

namespace SharedLogic
{
    public class ComplexObject
    {
        public CvSeq<CvPoint> Cont { get; set; }
        public int Id { get; set; }
        public bool Etalon { get; set; }
        public CvPoint2D32f Center { get; set; }
        public double Area { get; set; }
        public double Distance { get; set; }

        public ComplexObject()
        {
        }

        public ComplexObject(CvSeq<CvPoint> cont, CvPoint2D32f center, bool etalon, int id, double area, double distance)
        {
            Cont = cont;
            Center = center;
            Etalon = etalon;
            Id = id;
            Area = area;
            Distance = distance;
        }
    }
}