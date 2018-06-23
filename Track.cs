using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    public class Track : GpxItem<Track>
    {
        public Track()
        { }

        public string Name { get; private set; }
        public string Comment { get; private set; }
        public string Description { get; private set; }
        public string Source { get; private set; }
        public IEnumerable<Link> Links => _links;
        public int? Number { get; private set; }
        public string Type { get; private set; }
        public IEnumerable<TrackSegment> Segments => _trackSegments;

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
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            var trk = new Track();
            element.SetValueFromElement(ns + _name, val => trk.Name = val);
            element.SetValueFromElement(ns + _cmt, val => trk.Comment = val);
            element.SetValueFromElement(ns + _desc, val => trk.Description = val);
            element.SetValueFromElement(ns + _src, val => trk.Source = val);
            foreach (var link in element.Elements(ns + _link))
                trk.Add(Link.Parse(link));
            element.SetValueFromElement(ns + _number, val => trk.Number = int.Parse(val));
            element.SetValueFromElement(ns + _type, val => trk.Type = val);
            foreach (var trackSegment in element.Elements(ns + _trkseg))
                trk.Add(TrackSegment.Parse(trackSegment));
            return trk;
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteElement(ns + _name, Name);
            writer.WriteElement(ns + _cmt, Comment);
            writer.WriteElement(ns + _desc, Description);
            writer.WriteElement(ns + _src, Source);
            foreach (var link in _links)
                writer.WriteElement(_link, link);
            writer.WriteElement(ns + _number, Number);
            writer.WriteElement(ns + _type, Type);
            foreach (var trkseg in Segments)
                writer.WriteElement(ns + _trkseg, trkseg);
        }

        public void Add(TrackSegment seg) => _trackSegments.Add(seg);
        public void Add(WayPoint trkpt) => GetLastTrackSegment().Add(trkpt);
        public void Add(Link link) => _links.Add(link);

        internal TrackSegment GetLastTrackSegment()
        {
            if (_trackSegments.Count == 0)
                Add(new TrackSegment());
            return _trackSegments.Last();
        }

        List<Link> _links = new List<Link>();
        List<TrackSegment> _trackSegments = new List<TrackSegment>();

        const string _name = "name";
        const string _cmt = "cmt";
        const string _desc = "desc";
        const string _src = "src";
        const string _link = "link";
        const string _number = "number";
        const string _type = "type";
        const string _trkseg = "trkseg";

    }
}
