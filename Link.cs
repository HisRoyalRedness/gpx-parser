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
            Url = url;
        }

        public string Text { get; private set; }
        public string Type { get; private set; }
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
            var link = new Link(new Uri(element.Attribute(Constants.Link.href).Value));
            element.SetValueFromElement(Constants.Link.text, val => link.Text = val);
            element.SetValueFromElement(Constants.Link.type, val => link.Type = val);
            link.DisplayString = $"{link.Url}. ";
            if (!string.IsNullOrWhiteSpace(link.Text))
                link.DisplayString += $"Text: {link.Text} ";
            if (!string.IsNullOrWhiteSpace(link.Type))
                link.DisplayString += $"Type: {link.Type} ";
            return link;
        }

        protected override void InternalWrite(XmlWriter writer)
        {
            writer.WriteStartElement(Constants.Link.link);
            writer.WriteAttribute(Constants.Link.href, Url);
            if (!string.IsNullOrWhiteSpace(Text))
                writer.WriteElement(Constants.Link.text, Text);
            if (!string.IsNullOrWhiteSpace(Type))
                writer.WriteElement(Constants.Link.type, Type);
            writer.WriteEndElement();
        }

        string DisplayString { get; set; }
    } 
}
