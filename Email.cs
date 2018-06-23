using System;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    public class Email : GpxItem<Email>
    {
        public Email(string id, string domain)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(id)} cannot be empty");
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException($"{nameof(domain)} cannot be empty");
            Id = id;
            Domain = domain;
        }

        public Email(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentException($"{nameof(emailAddress)} cannot be empty");
            var atPos = emailAddress.IndexOf('@');
            if (atPos < 1 || atPos >= (emailAddress.Length - 1))
                throw new ArgumentException("Not a valid email address");
            Id = emailAddress.Substring(0, atPos);
            Domain = emailAddress.Substring(atPos + 1);
        }

        public string Id { get; set; }
        public string Domain { get; set; }
        public string EmailAddress => $"{Id}@{Domain}";

        public static Email Parse(XElement element)
        {
            /*
            <xsd:complexType name="emailType">
                <xsd:attribute name="id" type="xsd:string" use="required"/>
                <xsd:attribute name="domain" type="xsd:string" use="required"/>
            </xsd:complexType>
            */
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var ns = element.GetDefaultNamespace();
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            return new Email(
                element.Attribute(_id).Value,
                element.Attribute(_domain).Value);
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteAttribute(_id, Id);
            writer.WriteAttribute(_domain, Domain);
        }

        const string _id = "id";
        const string _domain = "domain";
    }
}
