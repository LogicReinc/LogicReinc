using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Templates
{
    public class HandlerBase<HandlerType, IdentifierType, ContextObject> 
        where HandlerType : HandlerBase<HandlerType, IdentifierType, ContextObject>
    {
        static Dictionary<IdentifierType, MethodInfo> _handlers = new Dictionary<IdentifierType, MethodInfo>();
        public static List<MethodInfo> Infos => _handlers.Values.ToList();

        public static Dictionary<IdentifierType, MethodInfo> Handlers => _handlers.ToDictionary(x => x.Key, y => y.Value);

        static HandlerBase()
        {
            MethodInfo[] infos = typeof(HandlerType).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(x => Attribute.IsDefined(x, typeof(HandlerAttribute))).ToArray();

            _handlers = infos.ToDictionary(x => (IdentifierType)((HandlerAttribute)Attribute.GetCustomAttribute(x, typeof(HandlerAttribute))).Value, y => y);
        }

        public static void Handle(IdentifierType key, ContextObject obj)
        {
            if (_handlers.ContainsKey(key))
                _handlers[key].Invoke(null, new object[] { obj });
            else
                throw new Exception("Key does not exist");
        }
    }

    public class HandlerAttribute : Attribute
    {
        public object Value { get; set; }

        public HandlerAttribute(object obj)
        {
            Value = obj;
        }
    }
}
