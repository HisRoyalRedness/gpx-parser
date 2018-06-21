using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var trkSeg = new TrackSegment();
            foreach (var trackPoint in element.Elements(Constants.TrackSegment.trkpt))
                trkSeg.Add(WayPoint.Parse(trackPoint, true));
            return trkSeg;
        }

        protected override void InternalWrite(XmlWriter writer)
        {
            writer.WriteStartElement(Constants.TrackSegment.trkseg);
            foreach (var trkpt in Points)
                trkpt.Write(writer);
            writer.WriteEndElement();
        }

        public void Add(WayPoint trkpt) => _points.Add(trkpt.Time.Value, trkpt);

        SortedList<DateTime, WayPoint> _points = new SortedList<DateTime, WayPoint>();
    }
}
