using System.Text;

namespace SkimSkript.Helpers.General
{
    internal static class StringHelper
    {
        public static string SplitPascalCaseManual(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char current = input[i];

                // Append space before uppercase letter if previous char is lowercase or digit
                if (i > 0 && char.IsUpper(current))
                {
                    char previous = input[i - 1];
                    if (char.IsLower(previous) || char.IsDigit(previous))
                    {
                        result.Append(' ');
                    }
                }

                // Append space before digit if previous char is letter
                if (i > 0 && char.IsDigit(current))
                {
                    char previous = input[i - 1];
                    if (char.IsLetter(previous))
                    {
                        result.Append(' ');
                    }
                }

                result.Append(current);
            }

            return result.ToString();
        }
    }
}
