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
    [DebuggerDisplay("{DisplayString}")]
    public class GpxFile : GpxItem<GpxFile>
    {
        static GpxFile()
        {
            var exeName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)?.FileVersion;
            _creator = $"{exeName} - {version}";
        }

        public GpxFile()
            : this(null, null)
        { }

        public GpxFile(string creator)
            : this(null, creator)
        { }

        public GpxFile(string version, string creator)
        {
            Version = version ?? "1.1";
            Creator = creator ?? _creator;
            DisplayString = $"{Version}, ${Creator}";
        }

        private GpxFile(string fileName, string version, string creator)
            : this(version, creator)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
        public string Version { get; private set; }
        public string Creator { get; private set; }
        public Metadata Metadata { get; set; }
        public List<WayPoint> Waypoints { get; } = new List<WayPoint>();
        public List<Route> Routes { get; } = new List<Route>();
        public List<Track> Tracks { get; } = new List<Track>();

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
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                throw new FileNotFoundException();

            var path = Path.GetFullPath(fileName);
            using (var reader = File.OpenRead(path))
            {
                var doc = XDocument.Load(reader);
                var element = doc.Root;
                var gpx = new GpxFile(
                    path,
                    element.Attribute(Constants.GpxFile.version).Value,
                    element.Attribute(Constants.GpxFile.creator).Value);
                element.SetValueFromElement(Constants.GpxFile.metadata, val => Metadata.Parse(val));
                foreach (var wpt in element.Elements(Constants.GpxFile.wpt))
                    gpx.Waypoints.Add(WayPoint.Parse(wpt));
                foreach (var rte in element.Elements(Constants.GpxFile.rte))
                    gpx.Routes.Add(Route.Parse(rte));
                foreach (var trk in element.Elements(Constants.GpxFile.trk))
                    gpx.Tracks.Add(Track.Parse(trk));
                gpx.DisplayString += $", {Path.GetFileName(gpx.FileName)}";
                return gpx;
            }
        }

        protected override void InternalWrite(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(Constants.GpxFile.gpx);
            writer.WriteAttribute(Constants.GpxFile.version, Version);
            writer.WriteAttribute(Constants.GpxFile.creator, Creator);
            writer.WriteElement(Constants.GpxFile.metadata, Metadata);
            foreach (var wpt in Waypoints)
                wpt.Write(writer);
            foreach (var rte in Routes)
                rte.Write(writer);
            foreach (var trk in Tracks)
                trk.Write(writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public void AddWaypoint(WayPoint wpt) => Waypoints.Add(wpt);
        public void Add(Route rte) => Routes.Add(rte);
        public void Add(Track trk) => Tracks.Add(trk);
        public void Add(TrackSegment seg) => GetLastTrack().Add(seg);
        public void AddTrackPoint(WayPoint trkpt) => GetLastTrack().GetLastTrackSegment().Add(trkpt);

        internal Track GetLastTrack()
        {
            if (Tracks.Count == 0)
                Add(new Track());
            return Tracks.Last();
        }

        string DisplayString { get; set; }
        readonly static string _creator;
    }
}
