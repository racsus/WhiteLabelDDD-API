using System;

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

        public static void ShowInfoMessage(
            string text1,
            ConsoleColor color1,
            string text2,
            ConsoleColor color2
        )
        {
            Console.ForegroundColor = color1;
            Console.Write(text1 + " ");
            Console.ResetColor();
            Console.ForegroundColor = color2;
            Console.Write(text2);
            Console.ResetColor();
            Console.WriteLine(string.Empty);
        }

        public static void WaitForUser()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
