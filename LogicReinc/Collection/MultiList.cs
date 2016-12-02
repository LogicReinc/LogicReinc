using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Collections
{
    public class MultiList<KT,VT>
    {
        public Dictionary<KT, List<VT>> Collections { get; set; } = new Dictionary<KT, List<VT>>();

        public List<VT> this[KT i]
        {
            get
            {
                if (!Collections.ContainsKey(i))
                    Collections.Add(i, new List<VT>());
                return Collections[i];
            }
        }

        public void Add(KT key, VT value)
        {
            if (!Collections.ContainsKey(key))
                Collections.Add(key, new List<VT>());
            Collections[key].Add(value);
        }

        public void Remove(KT key, VT value)
        {
            if (Collections.ContainsKey(key))
                Collections[key].Remove(value);
        }
    }
}
