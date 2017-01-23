using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified
{
    public interface IUnifiedIMObject
    {
        string ObjectID { get; }
        IList DatabaseBase { get; }
        Dictionary<string, UIMPropertyState> PropertyStates { get; }
        List<IUnifiedIMObject> ReferencedTo { get; }
        Type DataType { get; }
    }
}
