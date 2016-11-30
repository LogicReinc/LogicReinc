using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Collections
{
    public class SDictionary<TK,TV>
    {
        Dictionary<TK, TV> dictionary = new Dictionary<TK, TV>();

        public Dictionary<TK, TV> Dict => dictionary;

        public static implicit operator Dictionary<TK,TV>(SDictionary<TK,TV> sd)
        {
            return sd.dictionary;
        }

        public TV this[TK i]
        {
            get
            {
                if (dictionary.ContainsKey(i))
                    return dictionary[i];
                return default(TV);
            }
            set
            {
                if (dictionary.ContainsKey(i))
                    dictionary[i] = value;
                else
                    dictionary.Add(i, value);
            }
        }

    }
}
