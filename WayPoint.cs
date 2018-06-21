using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    [DebuggerDisplay("{DisplayString}")]
    public class WayPoint : GpxItem<WayPoint>, IComparable<WayPoint>
    {
        public WayPoint(double latitude, double longitude)
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
            var trkPt = new WayPoint(
                double.Parse(element.Attribute(Constants.WayPoint.lat).Value),
                double.Parse(element.Attribute(Constants.WayPoint.lon).Value));

            // Position info
            element.SetValueFromElement(Constants.WayPoint.ele, val => trkPt.Elevation = double.Parse(val));
            element.SetValueFromElement(Constants.WayPoint.time, val => trkPt.Time = DateTime.Parse(val, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal));
            element.SetValueFromElement(Constants.WayPoint.magvar, val => trkPt.MagneticVariation = double.Parse(val));
            element.SetValueFromElement(Constants.WayPoint.geoidheight, val => trkPt.GeoidHeight = double.Parse(val));

            if (requireTime && !trkPt.Time.HasValue)
                throw new ApplicationException($"{nameof(WayPoint)} requires a {nameof(WayPoint.Time)} value.");

            // Description info
            element.SetValueFromElement(Constants.WayPoint.name, val => trkPt.Name = val);
            element.SetValueFromElement(Constants.WayPoint.cmt, val => trkPt.Comment = val);
            element.SetValueFromElement(Constants.WayPoint.desc, val => trkPt.Description = val);
            element.SetValueFromElement(Constants.WayPoint.src, val => trkPt.Source = val);
            element.SetValueFromElement(Constants.WayPoint.link, val => trkPt.Link = Link.Parse(val));
            element.SetValueFromElement(Constants.WayPoint.sym, val => trkPt.SymbolName = val);
            element.SetValueFromElement(Constants.WayPoint.type, val => trkPt.Type = val);

            // Accuracy info
            var fixType = "";
            element.SetValueFromElement(Constants.WayPoint.fix, val => fixType = val);
            switch (fixType?.ToLower())
            {
                case _fixNone: trkPt.FixType = FixTypes.None; break;
                case _fix2d: trkPt.FixType = FixTypes.TwoD; break;
                case _fix3d: trkPt.FixType = FixTypes.ThreeD; break;
                case _fixDgps: trkPt.FixType = FixTypes.DGps; break;
                case _fixPps: trkPt.FixType = FixTypes.Pps; break;
                default: trkPt.FixType = FixTypes.NotSet; break;
            }
            element.SetValueFromElement(Constants.WayPoint.sat, val => trkPt.Satellites = int.Parse(val));
            element.SetValueFromElement(Constants.WayPoint.hdop, val => trkPt.HorizontalDilutionOfPrecision = decimal.Parse(val));
            element.SetValueFromElement(Constants.WayPoint.vdop, val => trkPt.VerticalDilutionOfPrecision = decimal.Parse(val));
            element.SetValueFromElement(Constants.WayPoint.pdop, val => trkPt.PositionDilutionOfPrecision = decimal.Parse(val));
            element.SetValueFromElement(Constants.WayPoint.ageofdgpsdata, val => trkPt.AgeOfDgpsData = decimal.Parse(val));
            element.SetValueFromElement(Constants.WayPoint.dgpsid, val => trkPt.DgpsId = int.Parse(val));

            trkPt.DisplayString = $"{trkPt.Latitude}, {trkPt.Longitude}";

            return trkPt;
        }

        protected override void InternalWrite(XmlWriter writer)
        {
            writer.WriteStartElement(Constants.WayPoint.trkpt);
            writer.WriteAttribute(Constants.WayPoint.lat, Latitude);
            writer.WriteAttribute(Constants.WayPoint.lon, Longitude);
            if (Elevation.HasValue)
                writer.WriteElement(Constants.WayPoint.ele, Elevation);
            if (Time.HasValue)
                writer.WriteElement(Constants.WayPoint.time, Time);
            if (MagneticVariation.HasValue)
                writer.WriteElement(Constants.WayPoint.magvar, MagneticVariation);
            if (GeoidHeight.HasValue)
                writer.WriteElement(Constants.WayPoint.geoidheight, GeoidHeight);
            if (!string.IsNullOrWhiteSpace(Name))
                writer.WriteElement(Constants.WayPoint.name, Name);
            if (!string.IsNullOrWhiteSpace(Comment))
                writer.WriteElement(Constants.WayPoint.cmt, Comment);
            if (!string.IsNullOrWhiteSpace(Description))
                writer.WriteElement(Constants.WayPoint.desc, Description);
            if (!string.IsNullOrWhiteSpace(Source))
                writer.WriteElement(Constants.WayPoint.src, Source);
            if (Link != null)
                Link.Write(writer);
            if (!string.IsNullOrWhiteSpace(SymbolName))
                writer.WriteElement(Constants.WayPoint.sym, SymbolName);
            if (!string.IsNullOrWhiteSpace(Type))
                writer.WriteElement(Constants.WayPoint.type, Type);
            writer.WriteEndElement();
        }

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

        string DisplayString { get; set; }

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

        const string _fixNone = "none";
        const string _fix2d = "2d";
        const string _fix3d = "3d";
        const string _fixDgps = "dgps";
        const string _fixPps = "pps";
    }
}
