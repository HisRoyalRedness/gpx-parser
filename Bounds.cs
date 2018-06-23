using System;
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

        public double? MinimumLatitude { get; private set; }
        public double? MaximumLatitude { get; private set; }
        public double? MinimumLongitude { get; private set; }
        public double? MaximumLongitude { get; private set; }

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
                double.Parse(element.Attribute(_minlat).Value),
                double.Parse(element.Attribute(_minlon).Value),
                double.Parse(element.Attribute(_maxlat).Value),
                double.Parse(element.Attribute(_maxlon).Value));
        }

        protected override void InternalWrite(XmlWriter writer, XNamespace ns)
        {
            writer.WriteAttribute(_minlat, MinimumLatitude);
            writer.WriteAttribute(_minlon, MinimumLongitude);
            writer.WriteAttribute(_maxlat, MaximumLatitude);
            writer.WriteAttribute(_maxlon, MaximumLongitude);
        }

        const string _minlat = "minlat";
        const string _minlon = "minlon";
        const string _maxlat = "maxlat";
        const string _maxlon = "maxlon";
    }
}
