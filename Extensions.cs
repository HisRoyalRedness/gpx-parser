using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
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
            if (setAction == null)
                throw new ArgumentNullException(nameof(setAction));
            var child = element?.Element(name);
            if (child == null)
                return default(TOut);
            var value = child.Value;
            return setAction(value);
        }

        public static TOut SetValueFromElement<TOut>(this XElement element, XName name, Func<XElement, TOut> setAction)
        {
            if (setAction == null)
                throw new ArgumentNullException(nameof(setAction));
            var child = element?.Element(name);
            if (child == null)
                return default(TOut);
            return setAction(child);
        }


        static bool SetValueFromElement(this XElement element, XName name, ref string value)
        {
            var child = element?.Element(name);
            if (child == null)
                return false;
            value = child.Value;
            return true;
        }
    }

    internal static class XmlWriterExtensions
    {
        public static void WriteAttribute<T>(this XmlWriter writer, XName name, T value)
            where T : class
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            writer.WriteStartAttribute(name.LocalName, name.NamespaceName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        public static void WriteAttribute<T>(this XmlWriter writer, XName name, T? value)
            where T : struct
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (!value.HasValue)
                throw new ArgumentNullException(nameof(value));
            writer.WriteStartAttribute(name.LocalName, name.NamespaceName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        #region WriteElement
        public static void WriteElement<T>(this XmlWriter writer, XName name, T value)
            where T : class
        {
            if (value == null)
                return;
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            writer.WriteStartElement(name.LocalName, name.NamespaceName);
            if (typeof(IGpxItem).IsAssignableFrom(typeof(T)))
                ((IGpxItem)value).Write(writer, name.Namespace);
            else
                writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteElement<T>(this XmlWriter writer, XName name, string value)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            writer.WriteStartElement(name.LocalName, name.NamespaceName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteElement<T>(this XmlWriter writer, XName name, T? value)
            where T : struct
        {
            if (!value.HasValue)
                return;
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            writer.WriteStartElement(name.LocalName, name.NamespaceName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
        #endregion WriteElement

        public static void WriteStartElement(this XmlWriter writer, XName name)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            writer.WriteStartElement(name.LocalName, name.NamespaceName);
        }
    }
}
