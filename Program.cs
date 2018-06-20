using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GpxParser
{
    static class Program
    {
        static void Main(string[] args)
        {
            var folder = @"C:\Users\KeithF\Downloads\Cruise\20180616081240";

            var points = string.Join(
                Environment.NewLine,
                Directory.GetFiles(folder, "*.gpx")
                    .Select(file => GpxFile.Parse(file).Track)
                    .SelectMany(trk => trk.Segments)
                    .SelectMany(seg => seg.Points)
                    .OrderBy(pt => pt.Time)
                    .Select(pt => pt.ToXml()));
                

            "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><gpx version=\"1.0\" creator="GPSLogger 95 - http://gpslogger.mendhak.com/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://www.topografix.com/GPX/1/0" xsi:schemaLocation="http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd"><trk><trkseg>

            Console.WriteLine();
        }

        public enum FixTypes
        {
            NotSet,
            None,
            [Description("2D")]
            TwoD,
            [Description("3D")]
            ThreeD,
            DGps,
            Pps
        }

        #region GpxFile
        [DebuggerDisplay("{DisplayString}")]
        public class GpxFile
        {
            private GpxFile(string fileName, string version, string creator)
            {
                FileName = fileName;
                Version = version;
                Creator = creator;
            }

            public string FileName { get; private set; }
            public string Version { get; private set; }
            public string Creator { get; private set; }
            //public Waypoint Waypoint { get; private set; }
            //public Route Route { get; private set; }
            public Track Track { get; private set; }

            public static GpxFile Parse(string fileName)
            {
                /*
                <gpx
                    version="1.1 [1] ?"
                    creator="xsd:string [1] ?">
                    <metadata> metadataType </metadata> [0..1] ?
                    <wpt> wptType </wpt> [0..*] ?
                    <rte> rteType </rte> [0..*] ?
                    <trk> trkType </trk> [0..*] ?
                    <extensions> extensionsType </extensions> [0..1] ?
                </gpx>
                */

                var path = Path.GetFullPath(fileName);
                using (var reader = File.OpenRead(path))
                {
                    var doc = XDocument.Load(reader);
                    var element = doc.Root;

                    ////var tracks = new List<Track>();
                    //foreach (var track in doc.Root.Elements(_topoNs + "trk"))
                    //{
                    //    var trk = Track.Parse(track);
                    //    points.AddRange(trk.Segments.SelectMany(s => s.Points));
                    //}

                    var gpx = new GpxFile(
                        path,
                        element.Attribute(_version).Value,
                        element.Attribute(_creator).Value);

                    //element.SetValueFromElement(_wpt, val => gpx.Waypoint = Waypoint.Parse(val));
                    //element.SetValueFromElement(_rte, val => gpx.Route = Route.Parse(val));
                    element.SetValueFromElement(_trk, val => gpx.Track = Track.Parse(val));

                    gpx._displayString = Path.GetFileName(gpx.FileName);

                    return gpx;
                }
            }

            string DisplayString => Path.GetFileName(FileName);
            string _displayString;

            static readonly XName _gpx = _topoNs + "gpx";
            static readonly XName _version = "version";
            static readonly XName _creator = "creator";
            static readonly XName _wpt = _topoNs + "wpt";
            static readonly XName _rte = _topoNs + "rte";
            static readonly XName _trk = _topoNs + "trk";
        }
        #endregion GpxFile

        #region Track
        public class Track
        {
            private Track()
            { }

            public string Name { get; private set; }
            public string Comment { get; private set; }
            public string Description { get; private set; }
            public string Source { get; private set; }
            public Link Link { get; private set; }
            public int Number { get; private set; }
            public string Type { get; private set; }
            public List<TrackSegment> Segments { get; } = new List<TrackSegment>();

            public static Track Parse(XElement element)
            {
                /*
                <xsd:complexType name="trkType">
                    <xsd:sequence>
                    <xsd:element name="name" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="cmt" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="desc" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="src" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="link" type="linkType" minOccurs="0" maxOccurs="unbounded"/>
                    <xsd:element name="number" type="xsd:nonNegativeInteger" minOccurs="0"/>
                    <xsd:element name="type" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="extensions" type="extensionsType" minOccurs="0"/>
                    <xsd:element name="trkseg" type="trksegType" minOccurs="0" maxOccurs="unbounded"/>
                    </xsd:sequence>
                </xsd:complexType>
                */
                var trk = new Track();

                element.SetValueFromElement(_name, val => trk.Name = val);
                element.SetValueFromElement(_cmt, val => trk.Comment = val);
                element.SetValueFromElement(_desc, val => trk.Description = val);
                element.SetValueFromElement(_src, val => trk.Source = val);
                element.SetValueFromElement(_link, val => trk.Link = Link.Parse(val));
                element.SetValueFromElement(_number, val => trk.Number = int.Parse(val));
                element.SetValueFromElement(_type, val => trk.Type = val);

                foreach (var trackSegment in element.Elements(_trkseg))
                    trk.Segments.Add(TrackSegment.Parse(trackSegment));
                return trk;
            }

            static readonly XName _trkseg = _topoNs + "trkseg";
            static readonly XName _name = _topoNs + "name";
            static readonly XName _cmt = _topoNs + "cmt";
            static readonly XName _desc = _topoNs + "desc";
            static readonly XName _src = _topoNs + "src";
            static readonly XName _link = _topoNs + "link";
            static readonly XName _number = _topoNs + "number";
            static readonly XName _type = _topoNs + "type";
        }
        #endregion Track

        #region TrackSegment
        public class TrackSegment
        {
            private TrackSegment()
            { }

            public List<TrackPoint> Points { get; } = new List<TrackPoint>();

            public static TrackSegment Parse(XElement element)
            {
                /*
                <xsd:complexType name="trksegType">
                    <xsd:sequence>
                        <-- elements must appear in this order -->
                        <xsd:element name="trkpt" type="wptType" minOccurs="0" maxOccurs="unbounded"/>
                        <xsd:element name="extensions" type="extensionsType" minOccurs="0"/>
                    </xsd:sequence>
                </xsd:complexType>
                */
                var trkSeg = new TrackSegment();
                foreach (var trackPoint in element.Elements(_trkpt))
                    trkSeg.Points.Add(TrackPoint.Parse(trackPoint));
                return trkSeg;
            }

            static readonly XName _trkpt = _topoNs + "trkpt";
        }
        #endregion TrackSegment

        #region TrackPoint
        [DebuggerDisplay("{DisplayString}")]
        public class TrackPoint
        {
            private TrackPoint(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }

            public double Latitude { get; private set; }
            public double Longitude { get; private set; }
            public double? Elevation { get; private set; }
            public DateTime? Time { get; private set; }
            public double? MagneticVariation { get; private set; }
            public double? GeoidHeight { get; private set; }

            public string Name { get; private set; }
            public string Comment { get; private set; }
            public string Description { get; private set; }
            public string Source { get; private set; }
            public Link Link { get; private set; }
            public string SymbolName { get; private set; }
            public string Type { get; private set; }

            public FixTypes FixType { get; private set; }
            public int? Satellites { get; private set; }
            public decimal? HorizontalDilutionOfPrecision { get; private set; }
            public decimal? VerticalDilutionOfPrecision { get; private set; }
            public decimal? PositionDilutionOfPrecision { get; private set; }
            public decimal? AgeOfDgpsData { get; private set; }
            public int? DgpsId { get; private set; }

            public static TrackPoint Parse(XElement element)
            {

                /*
                <xsd:complexType name="wptType">
                    <xsd:sequence>
                        <-- elements must appear in this order -->
                        <-- Position info -->
                        <xsd:element name="ele" type="xsd:decimal" minOccurs="0"/>
                        <xsd:element name="time" type="xsd:dateTime" minOccurs="0"/>
                        <xsd:element name="magvar" type="degreesType" minOccurs="0"/>
                        <xsd:element name="geoidheight" type="xsd:decimal" minOccurs="0"/>
                        <-- Description info -->
                        <xsd:element name="name" type="xsd:string" minOccurs="0"/>
                        <xsd:element name="cmt" type="xsd:string" minOccurs="0"/>
                        <xsd:element name="desc" type="xsd:string" minOccurs="0"/>
                        <xsd:element name="src" type="xsd:string" minOccurs="0"/>
                        <xsd:element name="link" type="linkType" minOccurs="0" maxOccurs="unbounded"/>
                        <xsd:element name="sym" type="xsd:string" minOccurs="0"/>
                        <xsd:element name="type" type="xsd:string" minOccurs="0"/>
                        <-- Accuracy info -->
                        <xsd:element name="fix" type="fixType" minOccurs="0"/>
                        <xsd:element name="sat" type="xsd:nonNegativeInteger" minOccurs="0"/>
                        <xsd:element name="hdop" type="xsd:decimal" minOccurs="0"/>
                        <xsd:element name="vdop" type="xsd:decimal" minOccurs="0"/>
                        <xsd:element name="pdop" type="xsd:decimal" minOccurs="0"/>
                        <xsd:element name="ageofdgpsdata" type="xsd:decimal" minOccurs="0"/>
                        <xsd:element name="dgpsid" type="dgpsStationType" minOccurs="0"/>
                        <xsd:element name="extensions" type="extensionsType" minOccurs="0"/>
                    </xsd:sequence>
                    <xsd:attribute name="lat" type="latitudeType" use="required"/>
                    <xsd:attribute name="lon" type="longitudeType" use="required"/>
                </xsd:complexType>
                */
                var trkPt = new TrackPoint(
                    double.Parse(element.Attribute(_lat).Value),
                    double.Parse(element.Attribute(_lon).Value));

                // Position info
                element.SetValueFromElement(_ele, val => trkPt.Elevation = double.Parse(val));
                element.SetValueFromElement(_time, val => trkPt.Time = DateTime.SpecifyKind(DateTime.Parse(val), DateTimeKind.Utc));
                element.SetValueFromElement(_magvar, val => trkPt.MagneticVariation = double.Parse(val));
                element.SetValueFromElement(_geoidheight, val => trkPt.GeoidHeight = double.Parse(val));

                // Description info
                element.SetValueFromElement(_name, val => trkPt.Name = val);
                element.SetValueFromElement(_cmt, val => trkPt.Comment  = val);
                element.SetValueFromElement(_desc, val => trkPt.Description = val);
                element.SetValueFromElement(_src, val => trkPt.Source  = val);
                element.SetValueFromElement(_link, val => trkPt.Link = Link.Parse(val));
                element.SetValueFromElement(_sym, val => trkPt.SymbolName  = val);
                element.SetValueFromElement(_type, val => trkPt.Type = val);

                // Accuracy info
                var fixType = "";
                element.SetValueFromElement(_fix, val => fixType = val);
                switch(fixType?.ToLower())
                {
                    case _fixNone: trkPt.FixType = FixTypes.None; break;
                    case _fix2d: trkPt.FixType = FixTypes.TwoD; break;
                    case _fix3d: trkPt.FixType = FixTypes.ThreeD; break;
                    case _fixDgps: trkPt.FixType = FixTypes.DGps; break;
                    case _fixPps: trkPt.FixType = FixTypes.Pps; break;
                    default: trkPt.FixType = FixTypes.NotSet; break;
                }
                element.SetValueFromElement(_sat, val => trkPt.Satellites = int.Parse(val));
                element.SetValueFromElement(_hdop, val => trkPt.HorizontalDilutionOfPrecision = decimal.Parse(val));
                element.SetValueFromElement(_vdop, val => trkPt.VerticalDilutionOfPrecision = decimal.Parse(val));
                element.SetValueFromElement(_pdop, val => trkPt.PositionDilutionOfPrecision = decimal.Parse(val));
                element.SetValueFromElement(_ageofdgpsdata, val => trkPt.AgeOfDgpsData = decimal.Parse(val));
                element.SetValueFromElement(_dgpsid, val => trkPt.DgpsId = int.Parse(val));

                trkPt._displayString = $"{trkPt.Latitude}, {trkPt.Longitude}";

                return trkPt;
            }

            public string ToXml()
            {
                var sb = new StringBuilder();
                sb.Append($"<trkpt lat=\"{Latitude}\" lon=\"{Latitude}\">");
                if (Elevation.HasValue)
                    sb.Append($"<ele>{Elevation.Value}</ele>");
                if (Time.HasValue)
                    sb.Append($"<time>{Time.Value.ToString("yyyy-MM-dd")}T{Time.Value.ToString("HH:mm:ss.fff")}Z</time>");
                if (MagneticVariation.HasValue)
                    sb.Append($"<magvar>{MagneticVariation.Value}</magvar>");
                if (GeoidHeight.HasValue)
                    sb.Append($"<geoidheight>{GeoidHeight.Value}</geoidheight>");
                if (!string.IsNullOrWhiteSpace(Name))
                    sb.Append($"<name>{Name}</name>");
                if (!string.IsNullOrWhiteSpace(Comment))
                    sb.Append($"<cmt>{Comment}</cmt>");
                if (!string.IsNullOrWhiteSpace(Description))
                    sb.Append($"<desc>{Description}</desc>");
                if (!string.IsNullOrWhiteSpace(Source))
                    sb.Append($"<src>{Source}</src>");
                if (Link != null)
                    sb.Append(Link.ToXml());
                if (!string.IsNullOrWhiteSpace(SymbolName))
                    sb.Append($"<sym>{SymbolName}</sym>");
                if (!string.IsNullOrWhiteSpace(Type))
                    sb.Append($"<type>{Type}</type>");
                sb.Append($"</trkpt>");
                return sb.ToString();
            }

            //<trkpt lat = "-36.15281608" lon="176.38007741"><ele>49.0</ele><time>2018-06-16T15:43:20.000Z</time><course>352.7</course><speed>1.24</speed><src>gps</src></trkpt>

            string DisplayString => _displayString;
            string _displayString;

            static readonly XName _lat = "lat";
            static readonly XName _lon = "lon";
            static readonly XName _ele = _topoNs + "ele";
            static readonly XName _time = _topoNs + "time";
            static readonly XName _magvar = _topoNs + "magvar";
            static readonly XName _geoidheight = _topoNs + "geoidheight";

            static readonly XName _name = _topoNs + "name";
            static readonly XName _cmt = _topoNs + "cmt";
            static readonly XName _desc = _topoNs + "desc";
            static readonly XName _src = _topoNs + "src";
            static readonly XName _link = _topoNs + "link";
            static readonly XName _sym = _topoNs + "sym";
            static readonly XName _type = _topoNs + "type";

            static readonly XName _fix = _topoNs + "fix";
            static readonly XName _sat = _topoNs + "sat";
            static readonly XName _hdop = _topoNs + "hdop";
            static readonly XName _vdop = _topoNs + "vdop";
            static readonly XName _pdop = _topoNs + "pdop";
            static readonly XName _ageofdgpsdata = _topoNs + "ageofdgpsdata";
            static readonly XName _dgpsid = _topoNs + "dgpsid";

            const string _fixNone = "none";
            const string _fix2d = "2d";
            const string _fix3d = "3d";
            const string _fixDgps = "dgps";
            const string _fixPps = "pps";
        }
        #endregion TrackPoint

        #region Link
        [DebuggerDisplay("{DisplayString}")]
        public class Link
        {
            private Link(Uri url)
            {
                Url = url;
            }

            public string Text { get; private set; }
            public string Type { get; private set; }
            public Uri Url { get; private set; }

            public static Link Parse(XElement element)
            {
                /*
                <xsd:complexType name="linkType">
                    <xsd:sequence>
                        <-- elements must appear in this order -->
                        <xsd:element name="text" type="xsd:string" minOccurs="0"/>
                        <xsd:element name="type" type="xsd:string" minOccurs="0"/>
                    </xsd:sequence>
                    <xsd:attribute name="href" type="xsd:anyURI" use="required"/>
                </xsd:complexType>
                */
                var link = new Link(new Uri(element.Attribute(_href).Value));
                element.SetValueFromElement(_text, val => link.Text = val);
                element.SetValueFromElement(_type, val => link.Type = val);

                link._displayString = $"{link.Url}. ";
                if (!string.IsNullOrWhiteSpace(link.Text))
                    link._displayString += $"Text: {link.Text} ";
                if (!string.IsNullOrWhiteSpace(link.Type))
                    link._displayString += $"Type: {link.Type} ";

                return link;
            }

            public string ToXml()
            {
                var sb = new StringBuilder();
                sb.Append($"<link href=\"{Url}\">");
                if (!string.IsNullOrWhiteSpace(Text))
                    sb.Append($"<text>{Text}</text>");
                if (!string.IsNullOrWhiteSpace(Type))
                    sb.Append($"<type>{Type}</type>");
                sb.Append($"</link>");
                return sb.ToString();
            }

            string DisplayString => _displayString;
            string _displayString;

            static readonly XName _href = "href";
            static readonly XName _text = _topoNs + "text";
            static readonly XName _type = _topoNs + "type";
        }
        #endregion Link

        readonly static XNamespace _topoNs = XNamespace.Get("http://www.topografix.com/GPX/1/0");
    }



    /*
    <trkpt lat="-36.02694253" lon="176.40905897">
        <ele>3.0</ele>
        <time>2018-06-16T20:16:14.358Z</time>
        <course>36.3</course>
        <speed>0.74</speed>
        <geoidheight>40.0</geoidheight>
        <src>gps</src>
        <hdop>1.7</hdop>
        <vdop>1.0</vdop>
        <pdop>1.9</pdop>
    </trkpt>




    */

}
