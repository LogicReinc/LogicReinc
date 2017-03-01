using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Expressions.Helpers
{
    public class ExpressionObject
    {
        public Type Type { get; set; }
        public Expression Instance { get; set; }

        public ExpressionObject(Type type)
        {
            Type = type;
        }

        public static ExpressionObject Typed<T>()
        {
            return new ExpressionObject(typeof(T));
        }

        public Expression Create()
        {
            Instance = Expression.New(Type);
            return Instance;
        }
        
        public ParameterExpression Variable(string name = null)
        {
            if (name != null)
                return Expression.Variable(Type, name);
            else
                return Expression.Variable(Type);
        }
        
        public Expression Assign(ParameterExpression var, params Expression[] arguments)
        {
            Expression assign = Expression.Assign(var, Create(arguments));
            Instance = var;
            return assign;
        }
        public Expression Assign(ParameterExpression var, string name, params Expression[] arguments)
        {
            Expression assign = Expression.Assign(var, Create(arguments));
            Instance = var;
            return assign;
        }

        public Expression Create(params Expression[] arguments)
        {
            Type[] types = arguments.Select(x => x.Type).ToArray();

            ConstructorInfo info = Type.GetConstructor(types);

            Instance = Expression.New(info, arguments);
            return Instance;
        }

        public Expression Call(string methodName, params Expression[] arguments)
        {
            Type[] types = arguments.Select(x => x.Type).ToArray();

            MethodInfo method = Type.GetMethod(methodName, types);
            ParameterInfo[] paras = method.GetParameters();

            List<Expression> parameters = new List<Expression>();

            for(int i = 0; i < arguments.Length; i++)
            {
                Type shouldBe = paras[i].ParameterType;
                if (types[i] != shouldBe)
                    parameters.Add(Expression.Convert(arguments[i], shouldBe));
                else
                    parameters.Add(arguments[i]);
            }

            if (method.IsStatic)
            {
                if (arguments.Length == 0)
                    return Expression.Call(method);
                    return Expression.Call(method, parameters);
            }

            if (arguments.Length == 0)
                return Expression.Call(Instance, method);
                return Expression.Call(Instance, method, parameters);
        }

        public Expression Property(string name)
        {
            PropertyInfo prop = Type.GetProperty(name);
            return Expression.Property(Instance, prop);
        }

        public Expression Field(string name)
        {
            FieldInfo field = Type.GetField(name);
            return Expression.Field(Instance, field);
        }
    }
}
