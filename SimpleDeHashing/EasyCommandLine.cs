using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDeHashing
{
    //made static since only one instance is ever nedded
    internal static class EasyCommandLine
    {
        public static string PromtString(string text)
        {

            Console.Write(text + ": ");

            return Console.ReadLine();

        }

        public static void scrollDown(int lines)
        {
            var posision = Console.GetCursorPosition();
            for (int x = 0; x < lines; x++)
            {
                Console.WriteLine();
            }
            Console.SetCursorPosition(posision.Left, posision.Top);
        }

        public static int PromtInt(string text) {

            int returnNumber = 0;

            while (true)
            {
                try
                {
                    Console.Write(text + ": ");
                    string? val;
                    val = Console.ReadLine();
                    returnNumber = Convert.ToInt32(val);
                    break;

                }
                catch
                {
                    ClearLine();
                }
            }

            return returnNumber;

        }

        public static int PromtInt(string text, List<int> values)
        {
            int output = PromtInt(text);
            if (values.Contains(output))
            {
                return output;
            } else
            {
                ClearLine();
                return PromtInt(text, values);
            }
        }

        public static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

    }
}
