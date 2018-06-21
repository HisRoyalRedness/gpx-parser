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

namespace HisRoyalRedness.com
{
    static class Program
    {
        static void Main(string[] args)
        {
            Sort(@"Data", "Cruise.gpx");
        }

        static void Sort(string folder, string targetFile)
        {
            var points = Directory.GetFiles(Path.GetFullPath(folder), "*.gpx")
                .SelectMany(file => GpxFile.Parse(file).Tracks)
                .SelectMany(trk => trk.Segments)
                .SelectMany(seg => seg.Points)
                .OrderBy(pt => pt.Time)
                .ToList();

            var gpx = new GpxFile("1.0", "keith@fletcher.org");
            foreach (var trkpt in points)
                gpx.AddTrackPoint(trkpt);

            using (var writer = new XmlTextWriter(Path.GetFullPath(targetFile), Encoding.UTF8))
                gpx.Write(writer);

            Console.WriteLine();
        }
    }
}
