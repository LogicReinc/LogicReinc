using LogicReinc.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogicReinc.Console
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

        public static void HandleCommand(string str)
        {
            List<string> val = Regex.Matches(str, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            HandleCommand(val.ToArray());
        }
        public static void HandleCommand(string[] arr)
        {
            HandleCommand(arr[0], arr.Skip(1).ToArray());
        }
        public static bool HandleCommand(string str, string[] arr)
        {
            if(cmds.ContainsKey(str))
            {
                MethodInfo info = cmds[str];
                List<object> paras = new List<object>();

                foreach (string para in arr)
                    foreach (ParameterInfo p in info.GetParameters())
                        paras.Add(parser.Parse(p.ParameterType, para.Trim('\'', '"')));

                info.Invoke(null, paras.ToArray());
                return true;
            }
            return false;
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
