using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GpxParser
{
    internal static class MiscExtensions
    {
        public static T Tap<T>(this T item, Action<T> action)
        {
            action?.Invoke(item);
            return item;
        }
    }

    internal static class LinqXmlExtensions
    {
        public static string GetValueFromElement(this XElement element, XName name)
            => element?.Element(name)?.Value;

        public static TOut SetValueFromElement<TOut>(this XElement element, XName name, Func<string, TOut> setAction)
        {
            var child = element?.Element(name);
            if (child == null)
                return default(TOut);
            var value = child.Value;
            return setAction(value);
        }

        public static TOut SetValueFromElement<TOut>(this XElement element, XName name, Func<XElement, TOut> setAction)
        {
            var child = element?.Element(name);
            if (child == null)
                return default(TOut);
            return setAction.Invoke(child);
        }


        static bool SetValueFromElement(this XElement element, XName name, ref string value)
        {
            var child = element.Element(name);
            if (child == null)
                return false;
            value = child.Value;
            return true;
        }
    }

    internal static class XmlWriterExtensions
    {
        public static void WriteAttribute<T>(this XmlWriter writer, XName name, T value)
        {
            writer.WriteStartAttribute(name.LocalName, name.NamespaceName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        public static void WriteElement<T>(this XmlWriter writer, XName name, T value)
        {
            writer.WriteStartElement(name.LocalName, name.NamespaceName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteStartElement(this XmlWriter writer, XName name)
        {
            writer.WriteStartElement(name.LocalName, name.NamespaceName);
        }
    }
}
