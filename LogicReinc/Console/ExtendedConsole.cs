using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Console
{
    public class ExtendedConsole<T> : ConsoleHandler<T>
    {
        
        public static void Show()
        {
            User32.ShowWindow(User32.GetConsoleWindow(), User32.SW_SHOW);
        }
        public static void Hide()
        {
            User32.ShowWindow(User32.GetConsoleWindow(), User32.SW_HIDE);
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
