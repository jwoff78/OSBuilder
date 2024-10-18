using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCompiler
{
    public static class StaticUtil
    {
        public static string ConcatenateWithoutExtraSpace(string[] objects)
        {
            if (objects == null || objects.Length == 0)
                return string.Empty;

            string result = string.Empty;

            for (int i = 0; i < objects.Length; i++)
            {
                result += objects[i];
                if (i < objects.Length - 1)
                {
                    result += " "; // Add space if it's not the last element
                }
            }

            return result;
        }

        public static string GenerateLineWithText(string text)
        {
            const int lineLength = 62; // Change this value to adjust line length

            if (text != null && text.Length > lineLength - 4)
            {
                // Truncate the text if it's longer than the available space
                text = text.Substring(0, lineLength - 4);
            }

            int remainingDashes = lineLength;
            if (text != null)
            {
                remainingDashes -= text.Length + 4; // 4 accounts for spaces and brackets around the text
            }

            int dashCountOnEachSide = remainingDashes / 2;

            string line = new string('-', dashCountOnEachSide);

            if (!string.IsNullOrEmpty(text))
            {
                line += $" [{text}] ";
                int remainingDashesAfterText = lineLength - line.Length;
                line += new string('-', remainingDashesAfterText);
            }
            else
            {
                int remainingDashesAfterText = lineLength - line.Length;
                line += new string('-', remainingDashesAfterText);
            }

            return line;
        }

        public static void PrintColoredText(string text, ConsoleColor firstPartColor = ConsoleColor.Green, ConsoleColor secondPartColor = ConsoleColor.Blue, string seperator = "->")
        {
            //Username remover
            text = text.Replace("djlw7", "*******");
            // Split the text by the '->' separator
            string[] parts = text.Split(seperator);

            // Print the first part in the specified color
            Console.ForegroundColor = firstPartColor;
            Console.Write(parts[0]);

            // Reset color
            Console.ResetColor();

            // Print the separator
            Console.Write(seperator);

            // Print the second part in the specified color
            Console.ForegroundColor = secondPartColor;
            Console.WriteLine(parts[1]);

            // Reset color
            Console.ResetColor();
        }

        public static void SafeWriteLine(string text)
        {
            text = text.Replace("djlw7", "*******");
            Console.WriteLine(text);
        }
    }
}
