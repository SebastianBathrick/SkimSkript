using System.Text;

namespace SkimSkript.Helpers.General
{
    internal static class StringHelper
    {
        private const string PASCAL_CASE_SPACING = " ";

        #region Split Pascal Case Methods
        public static string SplitPascalCaseManual(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                result.Append(GetSpaceBeforeUpper(input, i));
                result.Append(GetSpaceBeforeDigit(input, i));
                result.Append(input[i]);
            }

            return result.ToString();
        }
        private static string GetSpaceBeforeDigit(string input, int index)
        {
            if (index <= 0 || !char.IsDigit(input[index]) || !char.IsLetter(input[index - 1]))
                return string.Empty;

            return PASCAL_CASE_SPACING;
        }
    
        private static string GetSpaceBeforeUpper(string input, int index)
        {
            if (index <= 0 || !char.IsUpper(input[index]))
                return string.Empty;

            var previous = input[index - 1];

            if (!char.IsLower(previous) && !char.IsDigit(previous))
                return string.Empty;

            return PASCAL_CASE_SPACING;
        }
        #endregion
    }
}
