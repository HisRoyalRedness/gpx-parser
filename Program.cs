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
            //ChangeToRoute();
            ChangeToWaypoints();
            //Create();
            //Load(@"Data\20180618.gpx");
        }

        static void Load(string fileName)
        {
            var gpx = GpxFile.Load(fileName);
            gpx.Save("test2.xml");
            Validate("test2.xml");
        }

        static void Create()
        {
            var gpx = new GpxFile()
            {
                Metadata = new Metadata()
                {
                    Name = "meta name",
                    Description = "meta desc",
                    Author = new Person()
                    {
                        Name = "person name",
                        Email = new Email("me@email.com"),
                        Link = new Link(new Uri("http://personlink.com"))
                        {
                            Text = "person link text",
                            Type = "person link type"
                        }
                    },
                    Copyright = new Copyright("copyright author")
                    {
                        License = new Uri("http://copyrightlicense.com"),
                        Year = 2018
                    },
                    Time = DateTime.Now,
                    Keywords = "meta keywords",
                    Bounds = new Bounds(1.0, 2.0, 3.0, 4.0)
                }
                .Tap(meta => meta.Add(new Link(new Uri("http://metadatalink1.com"))
                {
                    Text = "meta link text 1",
                    Type = "meta link type 1"
                }))
                .Tap(meta => meta.Add(new Link(new Uri("http://metadatalink2.com"))
                {
                    Text = "meta link text 2",
                    Type = "meta link type 2"
                }))
            };

            var fileName = "test.xml";
            gpx.Save(fileName, true);
            Validate(fileName);
        }

        static void Sort(string folder, string targetFile)
        {
            var points = Directory.GetFiles(Path.GetFullPath(folder), "*.gpx")
                .SelectMany(file => GpxFile.Load(file).Tracks)
                .SelectMany(trk => trk.Segments)
                .SelectMany(seg => seg.Points)
                .Distinct();

            var gpx = new GpxFile();
            foreach (var trkpt in points)
                gpx.AddTrackPoint(trkpt);

            gpx.Metadata = new Metadata()
            {
                Name = "Food & wine cruise, 2018",
                Author = new Person()
                {
                    Name = "Keith Fletcher",
                    Email = new Email("keith@fletcher.com"),
                },
                Copyright = new Copyright("Keith Fletcher")
                {
                    Year = 2018
                },
                Time = DateTime.Now
            };

            gpx.Save(targetFile, true);
            Console.WriteLine();
        }

        static void ChangeToRoute()
        {
            var points = GpxFile.Load("Track.gpx")
                .Tracks
                .SelectMany(t => t.Segments)
                .SelectMany(s => s.Points);

            var gpx = new GpxFile();
            foreach (var pt in points)
                gpx.AddRoutePoint(pt);
            gpx.Save("Route.gpx", true);
        }

        static void ChangeToWaypoints()
        {
            var points = GpxFile.Load("Track.gpx")
                .Tracks
                .SelectMany(t => t.Segments)
                .SelectMany(s => s.Points);

            var gpx = new GpxFile();
            var i = 1;
            foreach (var pt in points)
            {
                pt.Name = $"My waypoint - {i++}";
                pt.Description = $"Desc My waypoint - {i++}";
                gpx.AddWaypoint(pt);
            }
            gpx.Save("Waypoints.gpx", true);
        }

        static bool Validate(string filename)
        {
            var schema = new XmlSchemaSet();
            using (var schemaReader = XmlReader.Create("gpx.xsd"))
                schema.Add("http://www.topografix.com/GPX/1/1", schemaReader);
            var doc = XDocument.Load(filename);
            var valid = true;
            doc.Validate(schema, (o, e) => {
                valid = false;
                Console.WriteLine($"{(e.Severity.ToString().ToUpper())}: {e.Message}");
            });
               
            Console.WriteLine();
            return valid;
        }

    }
}
