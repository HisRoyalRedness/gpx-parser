using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HisRoyalRedness.com
{
    public interface IGpxItem
    {
        void Write(XmlWriter writer, XNamespace ns);
    }

    public abstract class GpxItem<T> : IGpxItem
    {
        public void Write(XmlWriter writer) => Write(writer, Constants.topoNs1_1);

        public void Write(XmlWriter writer, XNamespace ns)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            InternalWrite(writer, ns);
        }

        protected abstract void InternalWrite(XmlWriter writer, XNamespace ns);
    }
}
