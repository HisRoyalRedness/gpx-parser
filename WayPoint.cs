using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    [DebuggerDisplay("{DisplayString}")]
    public class WayPoint : GpxItem<WayPoint>, IComparable<WayPoint>, IEquatable<WayPoint>
    {
        public WayPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public double? Elevation { get; private set; }
        public double? MagneticVariation { get; private set; }
        public double? GeoidHeight { get; private set; }
        public DateTime? Time
        {
            get { return _dateTime; }
            set
            {
                if (value.HasValue)
                {
                    _dateTime = value.Value.ToUniversalTime();
                }
                else
                    _dateTime = null;
            }
        }
        DateTime? _dateTime = null;

        public string Name { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public IEnumerable<Link> Links => _links;
        public string SymbolName { get; set; }
        public string Type { get; set; }

        public FixTypes FixType { get; set; }
        public int? Satellites { get; set; }
        public decimal? HorizontalDilutionOfPrecision { get; set; }
        public decimal? VerticalDilutionOfPrecision { get; set; }
        public decimal? PositionDilutionOfPrecision { get; set; }
        public decimal? AgeOfDgpsData { get; set; }
        public int? DgpsId { get; set; }

        public static WayPoint Parse(XElement element, bool requireTime = false)
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
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            var trkPt = new WayPoint(
                double.Parse(element.Attribute(_lat).Value),
                double.Parse(element.Attribute(_lon).Value));

            // Position info
            element.SetValueFromElement(ns + _ele, val => trkPt.Elevation = double.Parse(val));
            element.SetValueFromElement(ns + _time, val => trkPt.Time = DateTime.Parse(val, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal));
            element.SetValueFromElement(ns + _magvar, val => trkPt.MagneticVariation = double.Parse(val));
            element.SetValueFromElement(ns + _geoidheight, val => trkPt.GeoidHeight = double.Parse(val));

            if (requireTime && !trkPt.Time.HasValue)
                throw new ApplicationException($"{nameof(WayPoint)} requires a {nameof(WayPoint.Time)} value.");

            // Description info
            element.SetValueFromElement(ns + _name, val => trkPt.Name = val);
            element.SetValueFromElement(ns + _cmt, val => trkPt.Comment = val);
            element.SetValueFromElement(ns + _desc, val => trkPt.Description = val);
            element.SetValueFromElement(ns + _src, val => trkPt.Source = val);
            foreach (var link in element.Elements(ns + _link))
                trkPt.Add(Link.Parse(link));
            element.SetValueFromElement(ns + _sym, val => trkPt.SymbolName = val);
            element.SetValueFromElement(ns + _type, val => trkPt.Type = val);

            // Accuracy info
            element.SetValueFromElement(ns + _fix, val => trkPt.FixType = StringToFixType(val));
            element.SetValueFromElement(ns + _sat, val => trkPt.Satellites = int.Parse(val));
            element.SetValueFromElement(ns + _hdop, val => trkPt.HorizontalDilutionOfPrecision = decimal.Parse(val));
            element.SetValueFromElement(ns + _vdop, val => trkPt.VerticalDilutionOfPrecision = decimal.Parse(val));
            element.SetValueFromElement(ns + _pdop, val => trkPt.PositionDilutionOfPrecision = decimal.Parse(val));
            element.SetValueFromElement(ns + _ageofdgpsdata, val => trkPt.AgeOfDgpsData = decimal.Parse(val));
            element.SetValueFromElement(ns + _dgpsid, val => trkPt.DgpsId = int.Parse(val));

            trkPt.DisplayString = $"{trkPt.Latitude}, {trkPt.Longitude}";

            return trkPt;
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteAttribute(_lat, Latitude);
            writer.WriteAttribute(_lon, Longitude);
            writer.WriteElement(ns + _ele, Elevation);
            writer.WriteElement(ns + _time, Time);
            writer.WriteElement(ns + _magvar, MagneticVariation);
            writer.WriteElement(ns + _geoidheight, GeoidHeight);
            writer.WriteElement(ns + _name, Name);
            writer.WriteElement(ns + _cmt, Comment);
            writer.WriteElement(ns + _desc, Description);
            writer.WriteElement(ns + _src, Source);
            foreach (var link in _links)
                writer.WriteElement(_link, link);
            writer.WriteElement(ns + _sym, SymbolName);
            writer.WriteElement(ns + _type, Type);
            writer.WriteElement(ns + _fix, FixTypeToString(FixType));
            writer.WriteElement(ns + _sat, Satellites);
            writer.WriteElement(ns + _hdop, HorizontalDilutionOfPrecision);
            writer.WriteElement(ns + _vdop, VerticalDilutionOfPrecision);
            writer.WriteElement(ns + _pdop, PositionDilutionOfPrecision);
            writer.WriteElement(ns + _ageofdgpsdata, AgeOfDgpsData);
            writer.WriteElement(ns + _dgpsid, DgpsId);
        }

        #region IComparable<WayPoint> implementation
        public int CompareTo(WayPoint other)
        {
            if (other == null)
                return 1;

            if (Time.HasValue && other.Time.HasValue)
                return Time.Value.CompareTo(other.Time.Value);

            if (Time.HasValue)
                return 1;

            if (other.Time.HasValue)
                return -1;

            return 0;
        }
        #endregion IComparable<WayPoint> implementation

        #region IEquatable<WayPoint> implementation
        public bool Equals(WayPoint other) => CompareTo(other) == 0;
        #endregion IEquatable<WayPoint> implementation

        public override bool Equals(object obj) => Equals(obj as WayPoint);
        public static bool operator==(WayPoint a, WayPoint b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.Equals(b);
        }
        public static bool operator !=(WayPoint a, WayPoint b) => !(a == b);
        public override int GetHashCode() => Time.HasValue ? Time.Value.GetHashCode() : 0;

        string DisplayString { get; set; }

        public void Add(Link link) => _links.Add(link);

        List<Link> _links = new List<Link>();

        #region FixTypes
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

        static FixTypes StringToFixType(string fixType)
        {
            switch (fixType?.ToLower())
            {
                case _fixNone: return FixTypes.None;
                case _fix2d: return FixTypes.TwoD;
                case _fix3d: return FixTypes.ThreeD;
                case _fixDgps: return FixTypes.DGps;
                case _fixPps: return FixTypes.Pps;
                default: return FixTypes.NotSet;
            }
        }

        static string FixTypeToString(FixTypes fixType)
        {
            switch (fixType)
            {
                case FixTypes.None: return _fixNone;
                case FixTypes.TwoD: return _fix2d;
                case FixTypes.ThreeD: return _fix3d;
                case FixTypes.DGps: return _fixDgps;
                case FixTypes.Pps: return _fixPps;
                default: return null;
            }
        }
        #endregion FixTypes

        const string _lat = "lat";
        const string _lon = "lon";

        const string _ele = "ele";
        const string _time = "time";
        const string _magvar = "magvar";
        const string _geoidheight = "geoidheight";

        const string _name = "name";
        const string _cmt = "cmt";
        const string _desc = "desc";
        const string _src = "src";
        const string _link = "link";
        const string _sym = "sym";
        const string _type = "type";

        const string _fix = "fix";
        const string _sat = "sat";
        const string _hdop = "hdop";
        const string _vdop = "vdop";
        const string _pdop = "pdop";
        const string _ageofdgpsdata = "ageofdgpsdata";
        const string _dgpsid = "dgpsid";

        const string _fixNone = "none";
        const string _fix2d = "2d";
        const string _fix3d = "3d";
        const string _fixDgps = "dgps";
        const string _fixPps = "pps";
    }
}
