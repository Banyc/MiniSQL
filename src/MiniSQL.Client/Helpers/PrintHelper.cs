using System;

namespace MiniSQL.Client.Helpers
{
    public static class PrintHelper
    {
        public static void Print(string toPrint, ConsoleColor color)
        {
            // change color
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            // print
            Console.Write(toPrint);
            // restore the previous color
            Console.ForegroundColor = defaultColor;
        }
    }
}
