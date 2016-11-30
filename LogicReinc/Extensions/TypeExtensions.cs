using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Extensions
{
    public static class TypeExtensions
    {
        private static Dictionary<Type, PropertyInfo[]> CachedProperties { get; } = new Dictionary<Type, PropertyInfo[]>();

        public static PropertyInfo[] GetPropertiesCached(this Type type)
        {
            if (!CachedProperties.ContainsKey(type))
                CachedProperties.Add(type, type.GetProperties());
            return CachedProperties[type];
        }


        public static bool IsObject(this Type type)
        {
            if (!type.IsPrimitive && type != typeof(string))
                return true;
            return false;
        }

        public static bool IsArray(this Type type)
        {
            return type.IsArray;
        }

        public static bool IsList(this Type type)
        {
            return type.IsSubclassOf(typeof(IList));
        }
    }
}
