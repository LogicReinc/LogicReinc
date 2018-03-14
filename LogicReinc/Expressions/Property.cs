using LogicReinc.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicReinc.Expressions
{
    public class Property
    {
        private static TSDualDictionary<Type, string, Func<object, object>> _cachedGetters = new TSDualDictionary<Type, string, Func<object, object>>(true);
        private static TSDualDictionary<Type, string, Action<object, object>> _cachedSetters = new TSDualDictionary<Type, string, Action<object, object>>(true);


        static AssemblyBuilder _assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.RunAndCollect);
        static ModuleBuilder _module = _assembly.DefineDynamicModule(Guid.NewGuid().ToString());


        static Property()
        {
        }
        
        public static object Get(object obj, string name)
        {
            Type type = obj.GetType();
            return BuildPropertyGetter(name, type, true)(obj);
        }
        public static T Get<T>(object obj, string name)
        {
            return (T)Get(obj, name);
        }

        public static void Set(object obj, string name, object value)
        {
            Type type = obj.GetType();
            BuildPropertySetter(name, type, true)(obj, value);
        }

        public static Func<object, object> BuildPropertyGetter(string property, PropertyInfo prop, Type type, bool cache = false)
        {
            if (cache && _cachedGetters.ContainsKey(type, property))
                return _cachedGetters[type, property];
            if (prop == null)
                throw new ArgumentException($"Property [{property}] does not exist");

            //Parameters
            ParameterExpression arg = Expression.Parameter(typeof(object), "obj");

            //Conversion
            UnaryExpression convertedArg = Expression.Convert(arg, type);

            //Property
            Expression getExpression = Expression.Property(convertedArg, property);


            //(obj) => obj.[Property];

            Func<object, object> lambda = Expression.Lambda<Func<object, object>>(Expression.Convert(getExpression, typeof(object)), arg).Compile();
            if (cache)
                _cachedGetters[type, property] = lambda;
            return lambda;
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

        public static List<Func<object, object>> BuildILPropertyGetters(params PropertyReference[] refs)
        {
            List<Func<object, object>> objs = new List<Func<object, object>>();
            //AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.RunAndCollect);
            //ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            //TypeBuilder typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString());
            foreach (PropertyReference pref in refs)
            {
                PropertyInfo prop = pref.Type.GetProperty(pref.Name);
                if (prop == null)
                    throw new ArgumentException($"Property [{pref.Name}] does not exist");

                //Parameters
                ParameterExpression arg = Expression.Parameter(typeof(object), "obj");

                //Conversion
                UnaryExpression convertedArg = Expression.Convert(arg, pref.Type);

                //Property
                Expression getExpression = Expression.Property(convertedArg, pref.Name);


                //(obj) => obj.[Property];


                string mName = "m" + Guid.NewGuid().ToString().Replace("-", "");
                //MethodBuilder method = typeBuilder.DefineMethod(mName, MethodAttributes.Static | MethodAttributes.Public);


                objs.Add(Expression.Lambda<Func<object, object>>(Expression.Convert(getExpression, typeof(object)), arg).Compile());//.CompileToMethod(method);
                //pref.Builder = method;
            }
            /*
            Type t = typeBuilder.AsType();//.CreateType();

            foreach(PropertyReference pref in refs)
            {
                Func<object, object> fAction = (Func<object, object>)t.GetMethod(pref.Builder.Name, BindingFlags.Static | BindingFlags.Public)
                    .CreateDelegate(typeof(Func<object, object>));
                objs.Add(fAction);
                _cachedGetters[pref.Type, pref.Name] = fAction;
            }
            */
            return objs;
        }

        public static Func<object, object> BuildEmitGet(string property, Type type, bool cache = false)
        {
            throw new NotImplementedException();
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

        public static List<Action<object, object>> BuildILPropertySetters(params PropertyReference[] refs)
        {

            List<Action<object, object>> objs = new List<Action<object, object>>();
            //AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.RunAndCollect);
            //ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            //TypeBuilder typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString());


            foreach (PropertyReference pref in refs)
            {

                PropertyInfo prop = pref.Type.GetProperty(pref.Name);
                if (prop == null)
                    throw new ArgumentException($"Property [{pref.Name}] does not exist");

                MethodInfo setMethod = prop.GetSetMethod();
                if (setMethod == null)
                    throw new ArgumentException($"Property [{pref.Name}] has no Set Method");

                //Parameter
                //(object obj, object val)
                ParameterExpression argObj = Expression.Parameter(typeof(object), "obj");
                ParameterExpression argVal = Expression.Parameter(typeof(object), "val");

                //Casting
                //([InstanceType])[obj];
                UnaryExpression argObjCasted = Expression.Convert(argObj, pref.Type);
                //([PropertyType])[val];
                UnaryExpression argValCasted = Expression.Convert(argVal, prop.PropertyType);

                //Call
                //obj.[Property] = ([PropertyType])[val];
                MethodCallExpression call = Expression.Call(argObjCasted, setMethod, argValCasted);

                //(obj, val) => obj[Property] = [val]
                

                string mName = "m" + Guid.NewGuid().ToString().Replace("-", "");
                //MethodBuilder method = typeBuilder.DefineMethod(mName, MethodAttributes.Static | MethodAttributes.Public);


                objs.Add(Expression.Lambda<Action<object, object>>(call, argObj, argVal).Compile());//.CompileToMethod(method);
                //pref.Builder = method;
            }
            /*Type t = typeBuilder.AsType();

            foreach (PropertyReference pref in refs)
            {
                Action<object, object> fAction = (Action<object, object>)t.GetMethod(pref.Builder.Name, BindingFlags.Static | BindingFlags.Public)
                    .CreateDelegate(typeof(Action<object, object>));
                objs.Add(fAction);
                _cachedSetters[pref.Type, pref.Name] = fAction;
            }*/

            return objs;
        }


    }


    public class PropertyReference
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        internal MethodBuilder Builder { get; set; }

        public PropertyReference(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public static PropertyReference FromProperty(PropertyInfo info)
        {
            return new PropertyReference(info.DeclaringType, info.Name);
        }

        public static List<PropertyReference> FromClass(Type type, BindingFlags flags)
        {
            return type.GetProperties(flags).Select(x => FromProperty(x)).ToList();
        }
    }
}
