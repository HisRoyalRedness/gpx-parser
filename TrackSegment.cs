using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    public class TrackSegment : GpxItem<TrackSegment>
    {
        public TrackSegment()
        { }

        public IEnumerable<WayPoint> Points => _points.Values;

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
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            var trkSeg = new TrackSegment();
            foreach (var trackPoint in element.Elements(ns + _trkpt))
                trkSeg.Add(WayPoint.Parse(trackPoint, true));
            return trkSeg;
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            foreach (var trkpt in Points)
                writer.WriteElement(ns + _trkpt, trkpt);
        }

        public void Add(WayPoint trkpt) => _points.Add(trkpt.Time.Value, trkpt);

        SortedList<DateTime, WayPoint> _points = new SortedList<DateTime, WayPoint>();

        const string _trkpt = "trkpt";
    }
}
