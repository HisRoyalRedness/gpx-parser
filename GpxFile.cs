using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    //http://www.topografix.com/gpx/1/1/

    [DebuggerDisplay("{DisplayString}")]
    public class GpxFile : GpxItem<GpxFile>
    {
        static GpxFile()
        {
            var exeName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)?.FileVersion;
            _creatorString = $"{exeName} - {version}";
        }

        public GpxFile()
            : this(null)
        { }

        public GpxFile(string creator)
        {
            Creator = string.IsNullOrWhiteSpace(creator) ? _creatorString : creator;
            DisplayString = $"{Version}, ${Creator}";
        }

        private GpxFile(string fileName, string creator)
            : this(creator)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
        public string Version { get; set; }
        public string Creator { get; private set; }
        public Metadata Metadata { get; set; }
        public IEnumerable<WayPoint> Waypoints => _waypoints;
        public IEnumerable<Route> Routes => _routes;
        public IEnumerable<Track> Tracks => _tracks;

        public static GpxFile Load(string fileName)
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
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                throw new FileNotFoundException();

            var path = Path.GetFullPath(fileName);
            using (var reader = File.OpenRead(path))
            {
                var doc = XDocument.Load(reader);
                var ns = doc.Root.GetDefaultNamespace();

                var element = doc.Root;
                var gpx = new GpxFile(
                    path,
                    element.Attribute(_creator).Value);
                gpx._ns = ns;
                element.SetValueFromElement(ns + _metadata, val => Metadata.Parse(val));
                foreach (var wpt in element.Elements(ns + _wpt))
                    gpx._waypoints.Add(WayPoint.Parse(wpt));
                foreach (var rte in element.Elements(ns + _rte))
                    gpx._routes.Add(Route.Parse(rte));
                foreach (var trk in element.Elements(ns + _trk))
                    gpx._tracks.Add(Track.Parse(trk));
                gpx.DisplayString += $", {Path.GetFileName(gpx.FileName)}";
                return gpx;
            }
        }

        public void Save(string fileName, bool prettyPrint = false) => Save(fileName, _ns, prettyPrint);

        public void Save(string fileName, XNamespace ns, bool prettyPrint = false)
        {
            var settings = prettyPrint
                ? new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    IndentChars = "   "
                }
                : null;

            using (var writer = XmlWriter.Create(Path.GetFullPath(fileName), settings))
                Write(writer, ns);
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(ns + _gpx);
            writer.WriteAttributeString("xmlns", ns.ToString());
            writer.WriteAttribute(_version, GetVersion(ns));
            writer.WriteAttribute(_creator, Creator);
            writer.WriteElement(ns + _metadata, Metadata);
            foreach (var wpt in Waypoints)
                writer.WriteElement(ns + _wpt, wpt);
            foreach (var rte in Routes)
                writer.WriteElement(ns + _rte, rte);
            foreach (var trk in Tracks)
                writer.WriteElement(ns + _trk, trk);
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public void AddWaypoint(WayPoint wpt) => _waypoints.Add(wpt);
        public void Add(Route rte) => _routes.Add(rte);
        public void Add(Track trk) => _tracks.Add(trk);
        public void Add(TrackSegment seg) => GetLastTrack().Add(seg);
        public void AddTrackPoint(WayPoint trkpt) => GetLastTrack().GetLastTrackSegment().Add(trkpt);
        public void AddRoutePoint(WayPoint rtept) => GetLastRoute().Add(rtept);

        internal Track GetLastTrack()
        {
            if (_tracks.Count == 0)
                Add(new Track());
            return _tracks.Last();
        }

        internal Route GetLastRoute()
        {
            if (_routes.Count == 0)
                Add(new Route());
            return _routes.Last();
        }

        static string GetVersion(XNamespace ns)
        {
            if (ns == null)
                throw new ArgumentNullException($"{ns}");

            if (ns == Constants.topoNs1_0)
                return "1.0";
            if (ns == Constants.topoNs1_1)
                return "1.1";

            throw new ArgumentException("Unsupported namespace", $"{ns}");
        }

        string DisplayString { get; set; }
        readonly static string _creatorString;

        List<WayPoint> _waypoints = new List<WayPoint>();
        List<Route> _routes = new List<Route>();
        List<Track> _tracks = new List<Track>();
        XNamespace _ns = Constants.topoNs1_1;

        const string _gpx = "gpx";
        const string _version = "version";
        const string _creator = "creator";
        const string _metadata = "metadata";
        const string _wpt = "wpt";
        const string _rte = "rte";
        const string _trk = "trk";
    }
}
