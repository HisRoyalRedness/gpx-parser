using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Link Link { get; set; }
        public DateTime? Time { get; set; }
        public string Keywords { get; set; }
        public Bounds Bounds { get; set; }

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
            element.SetValueFromElement(Constants.Metadata.name, val => meta.Name = val);
            element.SetValueFromElement(Constants.Metadata.desc, val => meta.Description = val);
            element.SetValueFromElement(Constants.Metadata.author, val => meta.Author = Person.Parse(val));
            element.SetValueFromElement(Constants.Metadata.copyright, val => meta.Copyright = Copyright.Parse(val));
            element.SetValueFromElement(Constants.Metadata.link, val => meta.Link = Link.Parse(val));
            element.SetValueFromElement(Constants.Metadata.time, val => meta.Time = DateTime.Parse(val, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal));
            element.SetValueFromElement(Constants.Metadata.keywords, val => meta.Keywords = val);
            element.SetValueFromElement(Constants.Metadata.bounds, val => Bounds.Parse(val));
            return meta;
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteStartElement(Constants.Metadata.metadata);
            writer.WriteElement(Constants.Metadata.name, Name);
            writer.WriteElement(Constants.Metadata.desc, Description);
            writer.WriteElement(Constants.Metadata.author, Author);
            writer.WriteElement(Constants.Metadata.copyright, Copyright);
            writer.WriteElement(Constants.Metadata.link, Link);
            writer.WriteElement(Constants.Metadata.time, Time);
            writer.WriteElement(Constants.Metadata.keywords, Keywords);
            writer.WriteElement(Constants.Metadata.bounds, Bounds);
            writer.WriteEndElement();
        }
    }
}
