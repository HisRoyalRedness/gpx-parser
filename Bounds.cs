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
    public class Bounds : GpxItem<Bounds>
    {
        public Bounds(double minLat, double minLon, double maxLat, double maxLon)
        {
            MinimumLatitude = minLat;
            MinimumLongitude = minLon;
            MaximumLatitude = maxLat;
            MaximumLongitude = maxLon;
        }

        public double MinimumLatitude { get; private set; }
        public double MaximumLatitude { get; private set; }
        public double MinimumLongitude { get; private set; }
        public double MaximumLongitude { get; private set; }

        public static Bounds Parse(XElement element)
        {
            /*
            <xsd:complexType name="boundsType">
                <xsd:attribute name="minlat" type="latitudeType" use="required"/>
                <xsd:attribute name="minlon" type="longitudeType" use="required"/>
                <xsd:attribute name="maxlat" type="latitudeType" use="required"/>
                <xsd:attribute name="maxlon" type="longitudeType" use="required"/>
            </xsd:complexType>
            */
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return new Bounds(
                double.Parse(element.Attribute(Constants.Bounds.minlat).Value),
                double.Parse(element.Attribute(Constants.Bounds.minlon).Value),
                double.Parse(element.Attribute(Constants.Bounds.maxlat).Value),
                double.Parse(element.Attribute(Constants.Bounds.maxlon).Value));
        }

        protected override void InternalWrite(XmlWriter writer)
        {
            writer.WriteStartElement(Constants.Bounds.bounds);
            writer.WriteAttribute(Constants.Bounds.minlat, MinimumLatitude);
            writer.WriteAttribute(Constants.Bounds.minlon, MinimumLongitude);
            writer.WriteAttribute(Constants.Bounds.maxlat, MaximumLatitude);
            writer.WriteAttribute(Constants.Bounds.maxlon, MaximumLongitude);
            writer.WriteEndElement();
        }
    }
}
