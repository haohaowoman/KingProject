using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.IO;
namespace mcLogic.Data
{
    /// <summary>
    /// 二维坐标样点。
    /// </summary>
    [Serializable]
    public struct SamplePoint:IComparable<SamplePoint>
    {
        public SamplePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public int CompareTo(SamplePoint other)
        {
            return X.CompareTo(other.X);
        }
    }

    [Serializable]
    public class SamplePointCollection : List<SamplePoint>
    {
        //void WriteXML(string path)
        //{
        //    XmlSerializer xs = new XmlSerializer(typeof(SamplePointCollection));
        //    using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
        //    {
        //        xs.Serialize(fs, this);                
        //    }
        //}
    }

}
