using LogicReinc.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Expressions
{
    public class Property
    {
        private static DualDictionary<Type, string, Func<object, object>> _cachedGetters = new DualDictionary<Type, string, Func<object, object>>(true);
        private static DualDictionary<Type, string, Action<object, object>> _cachedSetters = new DualDictionary<Type, string, Action<object, object>>(true);

       
        public static object Get(object obj, string name)
        {
            Type type = obj.GetType();
            return BuildPropertyGetter(name, type, true)(obj);
        }
        public static T Get<T>(object obj, string name)
        {
            Type type = obj.GetType();
            return (T)BuildPropertyGetter(name, type, true)(obj);
        }

        public static void Set(object obj, string name, object value)
        {
            Type type = obj.GetType();
            BuildPropertySetter(name, type, true)(obj, value);
        }

        public static Func<object, object> BuildPropertyGetter(string property, Type type, bool cache = false)
        {
            if (cache && _cachedGetters.ContainsKey(type, property))
                return _cachedGetters[type, property];

            PropertyInfo prop = type.GetProperty(property);
            if (prop == null)
                throw new ArgumentException($"Property [{property}] does not exist");

            //Parameters
            ParameterExpression arg = Expression.Parameter(typeof(object), "obj");

            //Conversion
            UnaryExpression convertedArg = Expression.Convert(arg, type);

            //Property
            Expression getExpression = Expression.Property(convertedArg, property);
            

            //(obj) => obj.[Property];
            Func<object,object> lambda = Expression.Lambda<Func<object, object>>(Expression.Convert(getExpression, typeof(object)), arg).Compile();
            if (cache)
                _cachedGetters[type,property] = lambda;
            return lambda;
        }


        public static Action<object, object> BuildPropertySetter(string property, Type type, bool cache = false)
        {
            if (cache && _cachedSetters.ContainsKey(type, property))
                return _cachedSetters[type, property];

            PropertyInfo prop = type.GetProperty(property);
            if (prop == null)
                throw new ArgumentException($"Property [{property}] does not exist");

            MethodInfo setMethod = prop.GetSetMethod();
            if (setMethod == null)
                throw new ArgumentException($"Property [{property}] has no Set Method");

            //Parameter
            //(object obj, object val)
            ParameterExpression argObj = Expression.Parameter(typeof(object), "obj");
            ParameterExpression argVal = Expression.Parameter(typeof(object), "val");

            //Casting
            //([InstanceType])[obj];
            UnaryExpression argObjCasted = Expression.Convert(argObj, type);
            //([PropertyType])[val];
            UnaryExpression argValCasted = Expression.Convert(argVal, prop.PropertyType);

            //Call
            //obj.[Property] = ([PropertyType])[val];
            MethodCallExpression call = Expression.Call(argObjCasted, setMethod, argValCasted);

            //(obj, val) => obj[Property] = [val]
            Action<object,object> lambda = Expression.Lambda<Action<object, object>>(call, argObj, argVal).Compile();
            if (cache)
                _cachedSetters[type, property] = lambda;
            return lambda;
        }
    }
}
