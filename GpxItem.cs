using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HisRoyalRedness.com
{
    public interface IGpxItem
    {
        void Write(XmlWriter writer);
    }

    public abstract class GpxItem<T> : IGpxItem
    {
        public void Write(XmlWriter writer)
            => InternalWrite(writer);

        protected abstract void InternalWrite(XmlWriter writer);
    }
}
