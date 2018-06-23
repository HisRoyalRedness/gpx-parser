using System;
using System.Collections.Generic;
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
        public IEnumerable<Link> Links => _links;
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
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            var rte = new Route();
            element.SetValueFromElement(ns + _name, val => rte.Name = val);
            element.SetValueFromElement(ns + _cmt, val => rte.Comment = val);
            element.SetValueFromElement(ns + _desc, val => rte.Description = val);
            element.SetValueFromElement(ns + _src, val => rte.Source = val);
            foreach (var link in element.Elements(ns + _link))
                rte.Add(Link.Parse(link));
            element.SetValueFromElement(ns + _number, val => rte.Number = int.Parse(val));
            element.SetValueFromElement(ns + _type, val => rte.Type = val);
            foreach (var rtept in element.Elements(ns + _rtept))
                rte.Add(WayPoint.Parse(rtept));
            return rte;
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
            foreach (var rtept in _points)
                writer.WriteElement(_rtept, rtept);
        }

        public void Add(WayPoint trkpt) => _points.Add(trkpt);
        public void Add(Link link) => _links.Add(link);

        List<WayPoint> _points = new List<WayPoint>();
        List<Link> _links = new List<Link>();

        const string _name = "name";
        const string _cmt = "cmt";
        const string _desc = "desc";
        const string _src = "src";
        const string _link = "link";
        const string _number = "number";
        const string _type = "type";
        const string _rtept = "rtept";
    }
}
