using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    public class Route : GpxItem<Route>
    {
        public Route()
        { }

        public string Name { get; private set; }
        public string Comment { get; private set; }
        public string Description { get; private set; }
        public string Source { get; private set; }
        public Link Link { get; private set; }
        public int? Number { get; private set; }
        public string Type { get; private set; }
        public IEnumerable<WayPoint> Points => _points;

        public static Route Parse(XElement element)
        {
            /*
            <xsd:complexType name="rteType">
                <xsd:sequence>
                    <xsd:element name="name" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="cmt" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="desc" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="src" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="link" type="linkType" minOccurs="0" maxOccurs="unbounded"/>
                    <xsd:element name="number" type="xsd:nonNegativeInteger" minOccurs="0"/>
                    <xsd:element name="type" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="extensions" type="extensionsType" minOccurs="0"/>
                    <xsd:element name="rtept" type="wptType" minOccurs="0" maxOccurs="unbounded"/>
                </xsd:sequence>
            </xsd:complexType>
            */
            var rte = new Route();
            element.SetValueFromElement(Constants.Route.name, val => rte.Name = val);
            element.SetValueFromElement(Constants.Route.cmt, val => rte.Comment = val);
            element.SetValueFromElement(Constants.Route.desc, val => rte.Description = val);
            element.SetValueFromElement(Constants.Route.src, val => rte.Source = val);
            element.SetValueFromElement(Constants.Route.link, val => rte.Link = Link.Parse(val));
            element.SetValueFromElement(Constants.Route.number, val => rte.Number = int.Parse(val));
            element.SetValueFromElement(Constants.Route.type, val => rte.Type = val);
            foreach (var rtept in element.Elements(Constants.Route.rtept))
                rte.Add(WayPoint.Parse(rtept));
            return rte;
        }

        protected override void InternalWrite(XmlWriter writer)
        {
            writer.WriteStartElement(Constants.Route.rte);
            if (!string.IsNullOrWhiteSpace(Name))
                writer.WriteElement(Constants.Route.name, Name);
            if (!string.IsNullOrWhiteSpace(Comment))
                writer.WriteElement(Constants.Route.cmt, Comment);
            if (!string.IsNullOrWhiteSpace(Description))
                writer.WriteElement(Constants.Route.desc, Description);
            if (!string.IsNullOrWhiteSpace(Source))
                writer.WriteElement(Constants.Route.src, Source);
            if (Link != null)
                Link.Write(writer);
            if (Number.HasValue)
                writer.WriteElement(Constants.Route.number, Number.Value);
            if (!string.IsNullOrWhiteSpace(Type))
                writer.WriteElement(Constants.Route.type, Type);
            foreach (var rtept in Points)
                rtept.Write(writer);
            writer.WriteEndElement();
        }

        public void Add(WayPoint trkpt) => _points.Add(trkpt);

        List<WayPoint> _points = new List<WayPoint>();

    }
}
