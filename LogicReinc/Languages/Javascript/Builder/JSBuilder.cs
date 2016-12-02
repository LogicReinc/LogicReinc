using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Languages.Javascript.Builder
{
    public class JSBuilder : IJSBComponent
    {
        public List<JSBOperation> Operations { get; private set; }
        public JSBuilder()
        {
            Operations = new List<JSBOperation>();
        }

        public JSBuilder AssignVariable(string name, object value)
        {
            Operations.Add(new JSBAssignOperation(name, value));
            return this;
        }

        public JSBuilder DefineVariable(string name, object value)
        {
            Operations.Add(new JSBDefineOperation(name, value));
            return this;
        }

        public JSBuilder DefineFunction(string name, JSBuilder code, params string[] parameters)
        {
            Operations.Add(new JSBFunctionOperation(name, code, parameters));
            return this;
        }

        public JSBuilder DefineFunction(string name, string code, params string[] parameters)
        {
            Operations.Add(new JSBFunctionOperation(name, new JSBuilder().AddCode(code), parameters));
            return this;
        }

        public JSBuilder If(string condition, string then)
        {
            Operations.Add(new JSBIf(condition, then));
            return this;
        }

        public JSBuilder If(string condition, JSBuilder then)
        {
            Operations.Add(new JSBIf(condition, then));
            return this;
        }

        public JSBuilder AddCode(JSBuilder code)
        {
            Operations.Add(new JSBCustomOperation(code.BuildCode()));
            return this;
        }

        public JSBuilder AddCode(string code)
        {
            Operations.Add(new JSBCustomOperation(code));
            return this;
        }

        public string BuildCode(int indented = 0)
        {
            StringBuilder b = new StringBuilder();
            foreach(JSBOperation op in Operations)
                b.AppendLine(JSBuilder.GetIndented(indented) + op.BuildCode(indented));
            return b.ToString();
        }
        
        public static string GetIndented(int count)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < count; i++)
                b.Append("\t");
            return b.ToString();
        }
    }
}
