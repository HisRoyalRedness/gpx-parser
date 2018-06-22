using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    [DebuggerDisplay("{DisplayString}")]
    public class Link : GpxItem<Link>
    {
        public Link(Uri url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public string Text { get; set; }
        public string Type { get; set; }
        public Uri Url { get; private set; }

        public static Link Parse(XElement element)
        {
            /*
            <xsd:complexType name="linkType">
                <xsd:sequence>
                    <-- elements must appear in this order -->
                    <xsd:element name="text" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="type" type="xsd:string" minOccurs="0"/>
                </xsd:sequence>
                <xsd:attribute name="href" type="xsd:anyURI" use="required"/>
            </xsd:complexType>
            */
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            var link = new Link(new Uri(element.Attribute(_href).Value));
            element.SetValueFromElement(ns + _text, val => link.Text = val);
            element.SetValueFromElement(ns + _type, val => link.Type = val);

            link.DisplayString = $"{link.Url}. ";
            if (!string.IsNullOrWhiteSpace(link.Text))
                link.DisplayString += $"Text: {link.Text} ";
            if (!string.IsNullOrWhiteSpace(link.Type))
                link.DisplayString += $"Type: {link.Type} ";

            return link;
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteAttribute(_href, Url);
            writer.WriteElement(ns + _text, Text);
            writer.WriteElement(ns + _type, Type);
        }

        string DisplayString { get; set; }

        const string _text = "text";
        const string _type = "type";
        const string _href = "href";
    } 
}
