using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.SQL.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class JoinAttribute : Attribute
    {

        public string Join { get; private set; }

        public JoinAttribute(string join)
        {
            this.Join = join;
        }

        public static JoinAttribute[] GetJoins(Type type)
        {
            return (JoinAttribute[])type.GetCustomAttributes(typeof(JoinAttribute), true);
        }
    }
}
