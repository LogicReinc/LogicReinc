using LogicReinc.Data.Unified.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified
{
    public class IUnifiedIMObject
    {
        virtual public string ObjectID { get; set; }
        virtual internal IList DatabaseBase { get; }
        virtual internal Dictionary<string, UIMPropertyState> PropertyStates { get; } = new Dictionary<string, UIMPropertyState>();
        virtual internal List<KeyValuePair<UnifiedIMReference, IUnifiedIMObject>> RefTo { get; } = new List<KeyValuePair<UnifiedIMReference, IUnifiedIMObject>>();
        virtual internal Type DataType { get; }
    }
}
