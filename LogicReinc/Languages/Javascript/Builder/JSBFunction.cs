using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Languages.Javascript.Builder
{
    public class JSBFunction : IJSBComponent
    {
        public string Name { get; private set; }
        public List<string> Parameters { get; private set; }
        public JSBuilder Code { get; private set; }

        public JSBFunction(string name, JSBuilder code, params string[] paras)
        {
            Name = name;
            Parameters = paras.ToList();
            Code = code;
        }

        public JSBFunction(string name, string code, params string[] paras)
        {
            Name = name;
            Parameters = paras.ToList();
            Code = new JSBuilder().AddCode(code);
        }

        public string BuildCode(int indented = 0)
        {
            StringBuilder b = new StringBuilder();
            b.Append("function ");
            if(Name != null)
                b.Append(Name);
            b.Append("(");
            for (int i = 0; i < Parameters.Count; i++)
            {
                b.Append(Parameters[i]);
                if (i < Parameters.Count - 1)
                    b.Append(",");
            }
            b.Append(")\n" + JSBuilder.GetIndented(indented) + "{\n");

            b.Append(Code.BuildCode(indented + 1));
            b.Append(JSBuilder.GetIndented(indented) + "}");

            return b.ToString();
        }
    }
}
