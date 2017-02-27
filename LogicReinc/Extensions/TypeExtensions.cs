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
        private static Dictionary<Type, MethodInfo[]> CachedMethods { get; } = new Dictionary<Type, MethodInfo[]>();

        public static PropertyInfo[] GetPropertiesCached(this Type type)
        {
            if (!CachedProperties.ContainsKey(type))
                CachedProperties.Add(type, type.GetProperties());
            return CachedProperties[type];
        }

        public static MethodInfo[] GetMethodsCached(this Type type)
        {
            if (!CachedMethods.ContainsKey(type))
                CachedMethods.Add(type, type.GetMethods());
            return CachedMethods[type];
        }

        public static MethodInfo GetMethodCached(this Type type, string name)
        {
            if (!CachedMethods.ContainsKey(type))
                CachedMethods.Add(type, type.GetMethods());
            return CachedMethods[type].FirstOrDefault(x => x.Name == name);
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
