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

        public static Func<object, object> BuildPropertyGetter(string property, Type type)
        {
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
            return Expression.Lambda<Func<object, object>>(getExpression, arg).Compile();
        }


        public static Action<object, object> BuildPropertySetter(string property, Type type)
        {
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
            return Expression.Lambda<Action<object, object>>(call, argObj, argVal).Compile();
        }
    }
}
