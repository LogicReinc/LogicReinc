using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.SyntaxSugar
{
    public class Switcher
    {
        Dictionary<object, Action> cases = new Dictionary<object, Action>();
        

        public Switcher Case(object obj, Action result)
        {
            cases.Add(obj, result);
            return this;
        }

        public void Switch(object obj)
        {
            if (cases.ContainsKey(obj))
                cases[obj]();
        }
    }

    public class Switcher<T>
    {
        Dictionary<object, Func<T>> cases = new Dictionary<object, Func<T>>();


        public Switcher<T> Case(object obj, Func<T> result)
        {
            cases.Add(obj, result);
            return this;
        }

        public T Switch(object obj)
        {
            if (cases.ContainsKey(obj))
                return cases[obj]();
            return default(T);
        }
    }
}
