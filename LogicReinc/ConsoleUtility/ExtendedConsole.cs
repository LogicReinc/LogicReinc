using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.ConsoleUtility
{
    public class ExtendedConsole<T> : ConsoleHandler<T>
    {
        
        public static void WriteLine(string str, ConsoleColor color)
        {
            ConsoleColor current = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(str);
            System.Console.ForegroundColor = current;
        }
        public static void Write(string str, ConsoleColor color)
        {
            ConsoleColor current = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.Write(str);
            System.Console.ForegroundColor = current;
        }

        public static void Show()
        {
            User32.ShowWindow(User32.GetConsoleWindow(), User32.SW_SHOW);
        }
        public static void Hide()
        {
            User32.ShowWindow(User32.GetConsoleWindow(), User32.SW_HIDE);
        }

        public static void ConsoleLoop(Func<bool> isActive, Action<Exception> handleEx = null, Action<string> handleNotFound = null)
        {
            string line = null;
            while (isActive())
            {
                try
                {
                    line = Console.ReadLine();

                    if (!HandleCommand(line))
                        handleNotFound?.Invoke(line);
                }
                catch(Exception ex)
                {
                    handleEx?.Invoke(ex);
                }
            }
        }
    }

    internal class User32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
    }
}
