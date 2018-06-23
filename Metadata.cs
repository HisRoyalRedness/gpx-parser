using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    public class Metadata : GpxItem<Metadata>
    {
        public Metadata()
        { }

        public string Name { get; set; }
        public string Description { get; set; }
        public Person Author { get; set; }
        public Copyright Copyright { get; set; }
        public IEnumerable<Link> Links => _links;
        public string Keywords { get; set; }
        public Bounds Bounds { get; set; }

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

        public static Metadata Parse(XElement element)
        {
            /*
            <xsd:complexType name="metadataType">
                <xsd:sequence>
                    <-- elements must appear in this order -->
                    <xsd:element name="name" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="desc" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="author" type="personType" minOccurs="0"/>
                    <xsd:element name="copyright" type="copyrightType" minOccurs="0"/>
                    <xsd:element name="link" type="linkType" minOccurs="0" maxOccurs="unbounded"/>
                    <xsd:element name="time" type="xsd:dateTime" minOccurs="0"/>
                    <xsd:element name="keywords" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="bounds" type="boundsType" minOccurs="0"/>
                    <xsd:element name="extensions" type="extensionsType" minOccurs="0"/>
                </xsd:sequence>
            </xsd:complexType>
            */
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            var meta = new Metadata();
            element.SetValueFromElement(ns + _name, val => meta.Name = val);
            element.SetValueFromElement(ns + _desc, val => meta.Description = val);
            element.SetValueFromElement(ns + _author, val => meta.Author = Person.Parse(val));
            element.SetValueFromElement(ns + _copyright, val => meta.Copyright = Copyright.Parse(val));
            foreach (var link in element.Elements(ns + _link))
                meta._links.Add(Link.Parse(link));
            element.SetValueFromElement(ns + _time, val => meta.Time = DateTime.Parse(val, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal));
            element.SetValueFromElement(ns + _keywords, val => meta.Keywords = val);
            element.SetValueFromElement(ns + _bounds, val => Bounds.Parse(val));
            return meta;
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteElement(ns + _name, Name);
            writer.WriteElement(ns + _desc, Description);
            writer.WriteElement(ns + _author, Author);
            writer.WriteElement(ns + _copyright, Copyright);
            foreach(var link in _links)
                writer.WriteElement(ns + _link, link);
            writer.WriteElement(ns + _time, Time);
            writer.WriteElement(ns + _keywords, Keywords);
            writer.WriteElement(ns + _bounds, Bounds);
        }

        public void Add(Link link) => _links.Add(link);

        List<Link> _links = new List<Link>();

        const string _name = "name";
        const string _desc = "desc";
        const string _author = "author";
        const string _copyright = "copyright";
        const string _link = "link";
        const string _time = "time";
        const string _keywords = "keywords";
        const string _bounds = "bounds";
    }
}
