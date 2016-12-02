using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Attributes
{
    public class TypeAttribute : Attribute
    {
        public Type Type { get; private set; }
        public TypeAttribute(Type t)
        {
            Type = t;
        }

        public static bool HasAttribute(MethodInfo method)
        {
            return method.GetCustomAttribute<TypeAttribute>() != null;
        }

        public static TypeAttribute GetAttribute(MethodInfo method)
        {
            return method.GetCustomAttribute<TypeAttribute>();
        }
    }
}
