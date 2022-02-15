using LogicReinc.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogicReinc.ConsoleUtility
{
    public class ConsoleHandler<T>
    {
        static Dictionary<string, MethodInfo> cmds;
        static Parser parser = new Parser();

        static ConsoleHandler()
        {
            MethodInfo[] infos = typeof(T).GetMethods(BindingFlags.Static | BindingFlags.Public);

            cmds = infos
                .Where(x => Attribute.IsDefined(x, typeof(HandlerAttribute), false))
                .ToDictionary(x => ((HandlerAttribute)Attribute.GetCustomAttribute(x, typeof(HandlerAttribute), false)).Command.ToLower(), y => y);
        }

        public static bool HandleCommand(string str)
        {
            List<string> val = Regex.Matches(str, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            return HandleCommand(val.ToArray());
        }
        public static bool HandleCommand(string[] arr)
        {
            if (arr.Length == 0)
                return false;
            return HandleCommand(arr[0], arr.Skip(1).ToArray());
        }
        public static bool HandleCommand(string str, string[] arr)
        {
            if(cmds.ContainsKey(str.ToLower()))
            {
                MethodInfo info = cmds[str.ToLower()];
                List<object> paras = new List<object>();

                ParameterInfo[] pars = info.GetParameters();

                for (int i = 0; i < pars.Length; i++)
                    paras.Add(parser.Parse(pars[i].ParameterType, arr[i].Trim('\'', '"')));

                info.Invoke(null, paras.ToArray());
                return true;
            }
            return false;
        }

        public static void PrintFunctionality()
        {
            foreach(KeyValuePair<string, MethodInfo> method in cmds)
            {
                ParameterInfo[] paras = method.Value.GetParameters();

                System.Console.WriteLine($"{method.Key}({string.Join(", ", paras.Select(x => $"[{x.ParameterType.Name}]{x.Name}"))})");
            }
        }


        class Parser : StringParser
        {
            
        }
    }



    public class HandlerAttribute : Attribute
    {
        public string Command { get; set; }

        public HandlerAttribute(string command)
        {
            Command = command.ToLower();
        }
    }
}
