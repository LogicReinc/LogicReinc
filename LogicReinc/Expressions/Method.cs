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
    public static class Method
    {

        private static DualDictionary<Type, string, Action<object[]>> _aMethods = new DualDictionary<Type, string, Action<object[]>>();

        public static Action<object, object[]> BuildMethodAction(MethodInfo info)
        {
            List<Expression> paras = new List<Expression>();

            ParameterExpression iPara = Expression.Parameter(typeof(object), "instance");
            ParameterExpression para = Expression.Parameter(typeof(object[]), "paras");
            

            var ps = info.GetParameters();
            for (int i = 0; i < ps.Length; i++)
                paras.Add(Expression.Convert(Expression.ArrayAccess(para, Expression.Constant(i)), ps[i].ParameterType));

            MethodCallExpression call = Expression.Call(Expression.Convert(iPara, info.DeclaringType), info, paras);

            Console.WriteLine(call.ToString());

            Action<object, object[]> lambda = Expression.Lambda<Action<object, object[]>>(call, iPara, para).Compile();


            return lambda;
        }

        public static object CallGeneric(MethodInfo method, object instance, Type[] types, params object[] parameters)
        {
            MethodInfo m = method.MakeGenericMethod(types);
            return m.Invoke(instance, parameters);
        }

        public static T CallGeneric<T>(MethodInfo method, object instance, Type[] types, params object[] parameters)
        {
            return (T)CallGeneric(method, instance, types, parameters);
        }
    }
}
