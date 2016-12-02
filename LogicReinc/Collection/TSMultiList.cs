using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Collections
{
    public class TSMultiList<KT, VT>
    {
        private Dictionary<KT, TSList<VT>> collections = new Dictionary<KT, TSList<VT>>();

        public TSList<VT> this[KT i]
        {
            get
            {
                if (!collections.ContainsKey(i))
                    collections.Add(i, new TSList<VT>());
                return collections[i];
            }
        }

        public void Add(KT key, VT value)
        {
            if (!collections.ContainsKey(key))
                collections.Add(key, new TSList<VT>());
            collections[key].Add(value);
        }

        public void Remove(KT key, VT value)
        {
            if (collections.ContainsKey(key))
                collections[key].Remove(value);
        }
    }
}
