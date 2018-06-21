using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    internal static class Constants
    {
        internal readonly static XNamespace topoNs = XNamespace.Get("http://www.topografix.com/GPX/1/0");

        #region Link
        internal static class Link
        {
            // Atributes
            internal static readonly XName href = "href";

            //Elements
            internal static readonly XName link = Constants.topoNs + "link";
            internal static readonly XName text = Constants.topoNs + "text";
            internal static readonly XName type = Constants.topoNs + "type";
        }
        #endregion Link

        #region WayPoint
        internal static class WayPoint
        {
            // Atributes
            internal static readonly XName lat = "lat";
            internal static readonly XName lon = "lon";

            // Elements
            internal static readonly XName trkpt = Constants.topoNs + "trkpt";
            internal static readonly XName ele = Constants.topoNs + "ele";
            internal static readonly XName time = Constants.topoNs + "time";
            internal static readonly XName magvar = Constants.topoNs + "magvar";
            internal static readonly XName geoidheight = Constants.topoNs + "geoidheight";

            internal static readonly XName name = Constants.topoNs + "name";
            internal static readonly XName cmt = Constants.topoNs + "cmt";
            internal static readonly XName desc = Constants.topoNs + "desc";
            internal static readonly XName src = Constants.topoNs + "src";
            internal static readonly XName link = Constants.Link.link;
            internal static readonly XName sym = Constants.topoNs + "sym";
            internal static readonly XName type = Constants.topoNs + "type";

            internal static readonly XName fix = Constants.topoNs + "fix";
            internal static readonly XName sat = Constants.topoNs + "sat";
            internal static readonly XName hdop = Constants.topoNs + "hdop";
            internal static readonly XName vdop = Constants.topoNs + "vdop";
            internal static readonly XName pdop = Constants.topoNs + "pdop";
            internal static readonly XName ageofdgpsdata = Constants.topoNs + "ageofdgpsdata";
            internal static readonly XName dgpsid = Constants.topoNs + "dgpsid";
        }
        #endregion WayPoint

        #region TrackSegment
        internal static class TrackSegment
        {
            //Elements
            internal static readonly XName trkseg = Constants.topoNs + "trkseg";
            internal static readonly XName trkpt = WayPoint.trkpt;

        }
        #endregion TrackSegment

        #region Track
        internal static class Track
        {
            //Elements
            internal static readonly XName trk = Constants.topoNs + "trk";
            internal static readonly XName trkseg = TrackSegment.trkseg;
            internal static readonly XName name = Constants.topoNs + "name";
            internal static readonly XName cmt = Constants.topoNs + "cmt";
            internal static readonly XName desc = Constants.topoNs + "desc";
            internal static readonly XName src = Constants.topoNs + "src";
            internal static readonly XName link = Constants.topoNs + "link";
            internal static readonly XName number = Constants.topoNs + "number";
            internal static readonly XName type = Constants.topoNs + "type";
        }
        #endregion Track

        #region Route
        internal static class Route
        {
            //Elements
            internal static readonly XName rte = Constants.topoNs + "rte";
            internal static readonly XName name = Constants.topoNs + "name";
            internal static readonly XName cmt = Constants.topoNs + "cmt";
            internal static readonly XName desc = Constants.topoNs + "desc";
            internal static readonly XName src = Constants.topoNs + "src";
            internal static readonly XName link = Constants.topoNs + "link";
            internal static readonly XName number = Constants.topoNs + "number";
            internal static readonly XName type = Constants.topoNs + "type";
            internal static readonly XName rtept = Constants.topoNs + "rtept";
        }
        #endregion Route

        #region GpxFile
        internal static class GpxFile
        {
            // Atributes
            internal static readonly XName version = "version";
            internal static readonly XName creator = "creator";

            //Elements
            internal static readonly XName gpx = Constants.topoNs + "gpx";
            internal static readonly XName wpt = Constants.topoNs + "wpt";
            internal static readonly XName rte = Constants.topoNs + "rte";
            internal static readonly XName trk = Track.trk;
        }
        #endregion GpxFile
    }
}
