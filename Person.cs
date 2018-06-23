using System;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    public class Person : GpxItem<Person>
    {
        public Person()
        { }

        public string Name { get; set; }
        public Email Email { get; set; }
        public Link Link { get; set; }

        public static Person Parse(XElement element)
        {
            /*
            <xsd:complexType name="personType">
                <xsd:sequence>
                    <-- elements must appear in this order -->
                    <xsd:element name="name" type="xsd:string" minOccurs="0"/>
                    <xsd:element name="email" type="emailType" minOccurs="0"/>
                    <xsd:element name="link" type="linkType" minOccurs="0"/>
                </xsd:sequence>
            </xsd:complexType>
            */
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            var person = new Person();
            element.SetValueFromElement(ns + _name, val => person.Name = val);
            element.SetValueFromElement(ns + _email, val => person.Email = Email.Parse(val));
            element.SetValueFromElement(ns + _link, val => person.Link = Link.Parse(val));
            return person;
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteElement(ns + _name, Name);
            writer.WriteElement(ns + _email, Email);
            writer.WriteElement(ns + _link, Link);
        }

        const string _name = "name";
        const string _email = "email";
        const string _link = "link";
    }
}





