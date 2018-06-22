using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace HisRoyalRedness.com
{
    static class Program
    {
        static void Main(string[] args)
        {
            //Sort(@"Data", "Cruise.gpx");
            Create();
        }

        static void Create()
        {
            var gpx = new GpxFile()
            {
                Metadata = new Metadata()
                {
                    Name = "meta name",
                    Description = "meta desc",
                    Author = "meta auth",
                    Copyright = "meta copy",
                    Link = new Link(new Uri("http://www.hisroyalredness.com"))
                    {
                        Text = "link text",
                        Type = "link type"
                    },
                    Time = DateTime.Now,
                    Keywords = "meta keywords",
                    Bounds = new Bounds(1.0, 2.0, 3.0, 4.0)
                }
            };

            var fileName = "test.xml";
            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
                gpx.Write(writer);

            Validate(fileName);
        }

        static void Sort(string folder, string targetFile)
        {
            var points = Directory.GetFiles(Path.GetFullPath(folder), "*.gpx")
                .SelectMany(file => GpxFile.Parse(file).Tracks)
                .SelectMany(trk => trk.Segments)
                .SelectMany(seg => seg.Points)
                .Distinct();

            var gpx = new GpxFile();
            foreach (var trkpt in points)
                gpx.AddTrackPoint(trkpt);

            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "   "
            };
            using (var writer = XmlWriter.Create(Path.GetFullPath(targetFile), settings))
                gpx.Write(writer);

            Console.WriteLine();
        }

        static bool Validate(string filename)
        {
            
            var schema = new XmlSchemaSet();            
            using (var schemaReader = XmlReader.Create("gpx.xsd"))
                schema.Add("http://www.topografix.com/GPX/1/1", schemaReader);
            var doc = XDocument.Load(filename);
            doc.Validate(schema, (o, e) => Console.WriteLine($"{(e.Severity.ToString().ToUpper())}: {e.Message}"));
               
            Console.WriteLine();

            //static void Main(string[] args)
            //{
            //    var path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            //    XmlSchemaSet schema = new XmlSchemaSet();
            //    schema.Add("", path + "\\input.xsd");
            //    XmlReader rd = XmlReader.Create(path + "\\input.xml");
            //    XDocument doc = XDocument.Load(rd);
            //    doc.Validate(schema, ValidationEventHandler);
            //}
            //static void ValidationEventHandler(object sender, ValidationEventArgs e)
            //{
            //    XmlSeverityType type = XmlSeverityType.Warning;
            //    if (Enum.TryParse<XmlSeverityType>("Error", out type))
            //    {
            //        if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            //    }
            //}

            return false;
        }

    }

    //class WayPointComparer : IEqualityComparer<WayPoint>
    //{
    //    public bool Equals(WayPoint x, WayPoint y)
    //        => x.CompareTo(y) == 0;

    //    public int GetHashCode(WayPoint obj)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
