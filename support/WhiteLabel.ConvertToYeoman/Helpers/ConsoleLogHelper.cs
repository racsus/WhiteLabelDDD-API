using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteLabel.ConvertToYeoman.Helpers
{
    public class ConsoleLogHelper
    {
        public static void ShowInfoMessage(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WaitForUser()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

    }
}
