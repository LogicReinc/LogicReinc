using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified.Attributes
{
    public class UnifiedIMDerivesAttribute : Attribute
    {
        public List<Type> Derived { get; set; }
        public UnifiedIMDerivesAttribute(params Type[] types)
        {
            Derived = types.ToList();
        }

        public static UnifiedIMDerivesAttribute GetAttribute<T>()
        {
            return (UnifiedIMDerivesAttribute)typeof(T).GetCustomAttributes(typeof(UnifiedIMDerivesAttribute), true).FirstOrDefault();
        }
    }
}
