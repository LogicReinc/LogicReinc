using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicReinc.Extensions;

namespace LogicReinc.Languages.Javascript.Builder
{
    public enum JSBOperations
    {
        Asignment,
        Definition,
        Function,
        Custom
    }

    public class JSBOperation
    {
        public JSBOperations Type { get; set; }

        public virtual string BuildCode(int indented = 0)
        {
            return "";
        }
    }

    public class JSBIf : JSBOperation
    {
        public string Condition { get; private set; }
        public JSBuilder Body { get; private set; }

        public JSBIf(string condition, JSBuilder body)
        {
            Condition = condition;
            Body = body;
        }

        public JSBIf(string condition, string body)
        {
            Condition = condition;
            Body = new JSBuilder().AddCode(body);
        }

        public override string BuildCode(int indented = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("if(" + Condition + ") {\n");
            builder.Append(Body.BuildCode(indented + 1));
            builder.Append(JSBuilder.GetIndented(indented) + "}");
            return builder.ToString();
        }
    }

    public class JSBAssignOperation : JSBOperation
    {
        public string VariableName { get; private set; }
        public object VariableValue { get; private set; }
        public bool IsValueType { get; private set; }

        public JSBAssignOperation(string name, object obj)
        {
            Type = JSBOperations.Asignment;

            VariableName = name;

            Type t = obj.GetType();
            if (!t.IsObject())
            {
                VariableValue = obj;
                IsValueType = true;
            }
            else if (obj.GetType() != typeof(JSBObject))
                VariableValue = JSBObject.FromAnonymous(obj);
            else
                VariableValue = (JSBObject)obj;
        }
        public override string BuildCode(int indented = 0)
        {
            StringBuilder b = new StringBuilder();
            b.Append(JSBuilder.GetIndented(indented));
            b.Append(VariableName);
            b.Append(" = ");
            if (!IsValueType)
                b.Append(((JSBObject)VariableValue).BuildCode(indented));
            else
                b.Append(JSBObject.BuildValue(VariableValue, indented));
            b.Append(";");
            return b.ToString();
        }

    }

    public class JSBDefineOperation : JSBOperation, IJSBComponent
    {
        public string VariableName { get; private set; }
        public object VariableValue { get; private set; }
        public bool IsValueType { get; private set; }

        public JSBDefineOperation(string name, object obj)
        {
            Type = JSBOperations.Definition;

            VariableName = name;

            Type t = obj.GetType();
            if (!t.IsObject())
            {
                VariableValue = obj;
                IsValueType = true;
            }
            else if (obj.GetType() != typeof(JSBObject))
                VariableValue = JSBObject.FromAnonymous(obj);
            else
                VariableValue = (JSBObject)obj;
        }

        public override string BuildCode(int indented = 0)
        {
            StringBuilder b = new StringBuilder();
            b.Append(JSBuilder.GetIndented(indented) + "var ");
            b.Append(VariableName);
            b.Append(" = ");
            if (!IsValueType)
                b.Append(((JSBObject)VariableValue).BuildCode(indented));
            else
                b.Append(JSBObject.BuildValue(VariableValue, indented));
            b.Append(";");
            return b.ToString();
        }
    }

    public class JSBFunctionOperation : JSBOperation, IJSBComponent
    {
        public string FunctionName { get; private set; }
        public List<string> Parameters { get; private set; }
        public JSBuilder Code { get; private set; }

        public JSBFunctionOperation(string name, JSBuilder code, params string[] parameters)
        {
            Type = JSBOperations.Function;

            FunctionName = name;
            Parameters = parameters.ToList();

            Code = code;
        }

        public override string BuildCode(int indented = 0)
        {
            return new JSBFunction(FunctionName, Code, Parameters.ToArray()).BuildCode(indented);
        }
    }

    public class JSBCustomOperation : JSBOperation, IJSBComponent
    {
        public string Code { get; private set; }
        public JSBCustomOperation(string code)
        {
            Type = JSBOperations.Custom;
            Code = code;
        }

        public override string BuildCode(int indented = 0)
        {
            if(indented > 0)
            {
               return Code.Replace("\n", JSBuilder.GetIndented(indented)); 
            }
            return Code;
        }
    }
}
