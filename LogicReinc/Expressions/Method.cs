using LogicReinc.Collection;
using LogicReinc.Extensions;
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

        private static TSDualDictionary<Type, string, Action<object, object[]>> _aMethods = new TSDualDictionary<Type, string, Action<object, object[]>>(true);
        private static TSDualDictionary<Type, string, Func<object, object[], object>> _fMethods = new TSDualDictionary<Type, string, Func<object, object[], object>>(true);

        public static Action<object, object[]> BuildMethodAction(MethodInfo info, bool cache = false)
        {
            if (cache && _aMethods.ContainsKey(info.DeclaringType, info.Name))
                return _aMethods[info.DeclaringType, info.Name];

            List<Expression> paras = new List<Expression>();

            ParameterExpression iPara = Expression.Parameter(typeof(object), "instance");
            ParameterExpression para = Expression.Parameter(typeof(object[]), "paras");
            

            var ps = info.GetParameters();
            for (int i = 0; i < ps.Length; i++)
                paras.Add(Expression.Convert(Expression.ArrayAccess(para, Expression.Constant(i)), ps[i].ParameterType));

            MethodCallExpression call = Expression.Call(Expression.Convert(iPara, info.DeclaringType), info, paras);
            

            Action<object, object[]> lambda = Expression.Lambda<Action<object, object[]>>(call, iPara, para).Compile();

            if (cache)
                _aMethods[info.DeclaringType, info.Name] = lambda;

            return lambda;
        }

        public static Func<object, object[], object> BuildMethodFunction(MethodInfo info, bool cache = false)
        {
            if (cache && _fMethods.ContainsKey(info.DeclaringType, info.Name))
                return _fMethods[info.DeclaringType, info.Name];

            List<Expression> paras = new List<Expression>();

            ParameterExpression iPara = Expression.Parameter(typeof(object), "instance");
            ParameterExpression para = Expression.Parameter(typeof(object[]), "paras");


            var ps = info.GetParameters();
            for (int i = 0; i < ps.Length; i++)
                paras.Add(Expression.Convert(Expression.ArrayAccess(para, Expression.Constant(i)), ps[i].ParameterType));

            MethodCallExpression call = Expression.Call(Expression.Convert(iPara, info.DeclaringType), info, paras);
            

            Func<object, object[], object> lambda = Expression.Lambda<Func<object, object[], object>>(call, iPara, para).Compile();

            if (cache)
                _fMethods[info.DeclaringType, info.Name] = lambda;

            return lambda;
        }


        public static object Call(Type type, string name, object instance, params object[] parameters)
        {
            MethodInfo method = type.GetMethodCached(name);

            if (method.ReturnType != typeof(void))
                return BuildMethodFunction(method, true)(instance, parameters);
            else
            {
                BuildMethodAction(method, true)(instance, parameters);
                return null;
            }
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
