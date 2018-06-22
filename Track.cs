using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Link Link { get; private set; }
        public int? Number { get; private set; }
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
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var trk = new Track();

            element.SetValueFromElement(Constants.Track.name, val => trk.Name = val);
            element.SetValueFromElement(Constants.Track.cmt, val => trk.Comment = val);
            element.SetValueFromElement(Constants.Track.desc, val => trk.Description = val);
            element.SetValueFromElement(Constants.Track.src, val => trk.Source = val);
            element.SetValueFromElement(Constants.Track.link, val => trk.Link = Link.Parse(val));
            element.SetValueFromElement(Constants.Track.number, val => trk.Number = int.Parse(val));
            element.SetValueFromElement(Constants.Track.type, val => trk.Type = val);
            foreach (var trackSegment in element.Elements(Constants.Track.trkseg))
                trk.Segments.Add(TrackSegment.Parse(trackSegment));

            return trk;
        }

        protected override void InternalWrite(XmlWriter writer)
        {
            writer.WriteStartElement(Constants.Track.trk);
            writer.WriteElement(Constants.Track.name, Name);
            writer.WriteElement(Constants.Track.cmt, Comment);
            writer.WriteElement(Constants.Track.desc, Description);
            writer.WriteElement(Constants.Track.src, Source);
            writer.WriteElement(Constants.Track.link, Link);
            writer.WriteElement(Constants.Track.number, Number);
            writer.WriteElement(Constants.Track.type, Type);
            foreach (var trkseg in Segments)
                trkseg.Write(writer);
            writer.WriteEndElement();
        }

        public void Add(TrackSegment seg) => Segments.Add(seg);
        public void Add(WayPoint trkpt) => GetLastTrackSegment().Add(trkpt);

        internal TrackSegment GetLastTrackSegment()
        {
            if (Segments.Count == 0)
                Add(new TrackSegment());
            return Segments.Last();
        }

    }
}
