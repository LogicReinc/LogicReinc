using LogicReinc.Attributes;
using LogicReinc.SyntaxSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Parsing
{
    public class StringParser
    {

        public static StringParser Static { get; } = new StringParser();

        private Dictionary<Type, Func<string, object>> ParseMethods { get; } = new Dictionary<Type, Func<string, object>>();


        public StringParser()
        {
            Type t = GetType();

            ParseMethods = t.GetMethods().Where(x => TypeAttribute.HasAttribute(x)).ToDictionary(
                (m) => TypeAttribute.GetAttribute(m).Type,
                (m) => new Func<string, object>((str) =>
                    Try.Func<object>(
                        () => m.Invoke(this, new object[] { str }),
                        (ex) => { throw ex.InnerException; }
                    )));
        }


        public T Parse<T>(string input) => (T)Parse(typeof(T), input);
        public object Parse(Type outputType, string input)
        {
            if (!ParseMethods.ContainsKey(outputType))
                throw new ArgumentException($"Type {outputType.Name} is not supported, Inherit StringParser and add your own method");

            return ParseMethods[outputType](input);
        }




        [Type(typeof(string))]
        public virtual object ToString(string str) => str;

        [Type(typeof(bool))]
        public virtual object ToBool(string str) => bool.Parse(str);

        //Integers
        [Type(typeof(short))]
        public virtual object ToInt16(string str) => Convert.ToInt16(str);
        [Type(typeof(int))]
        public virtual object ToInt32(string str) => Convert.ToInt32(str);
        [Type(typeof(long))]
        public virtual object ToInt64(string str) => Convert.ToInt64(str);

        [Type(typeof(double))]
        public virtual object ToDouble(string str) => Convert.ToDouble(str);

        [Type(typeof(byte))]
        public virtual object ToByte(string str) => byte.Parse(str);
        [Type(typeof(char))]
        public virtual object ToChar(string str) => str[0];

        [Type(typeof(string[]))]
        public virtual object ToStringArray(string str) => str.Split(',');

    }
}
