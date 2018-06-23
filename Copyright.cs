using System;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    public class Copyright : GpxItem<Copyright>
    {
        public Copyright(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException($"{nameof(author)} cannot be empty");
            Author = author;
        }

        public int? Year { get; set; }
        public Uri License { get; set; }
        public string Author { get; set; }

        public static Copyright Parse(XElement element)
        {
            /*
            <xsd:complexType name="copyrightType">
                <xsd:sequence>
                    <-- elements must appear in this order -->
                    <xsd:element name="year" type="xsd:gYear" minOccurs="0"/>
                    <xsd:element name="license" type="xsd:anyURI" minOccurs="0"/>
                </xsd:sequence>
                <xsd:attribute name="author" type="xsd:string" use="required"/>
            </xsd:complexType>
            */
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            var copyright = new Copyright(
                element.Attribute(_author).Value);
            element.SetValueFromElement(ns + _year, val => copyright.Year = int.Parse(val));
            element.SetValueFromElement(ns + _license, val => copyright.License = new Uri(val));
            return copyright;
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteAttribute(_author, Author);
            writer.WriteElement(ns + _year, Year);
            writer.WriteElement(ns + _license, License);
        }

        const string _year = "year";
        const string _license = "license";
        const string _author = "author";
    }
}
