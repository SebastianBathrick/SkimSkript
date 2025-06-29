using System.Text;

namespace SkimSkript.Monitoring.ErrorHandling
{
    internal abstract class SkimSkriptException : Exception
    {
        private readonly object[] _properties;
        private readonly Enum _errorKey;
        private readonly int _line;
        private readonly int _column;   

        public object[] Properties
        {
            get
            {

                List<object> allProperties = [_line, _column];
                allProperties.AddRange(_properties);

                if (TryGetAdditionalContext(out _, out var additionalProps))
                    allProperties.AddRange(additionalProps);

                return allProperties.ToArray();
            }
        }

        public override string Message =>
            base.Message + (TryGetAdditionalContext(out var msg, out _) ? $".\n{msg}" : string.Empty);
                        
        public SkimSkriptException(
            Enum errorKey, string[] messages, int line, int column, params object[] properties) 
            : base(CreateMessage(errorKey, messages, line, column))
        {
            _properties = properties;
            _errorKey = errorKey;
            _line = line;
            _column = column;
        }
       
        protected virtual bool TryGetAdditionalContext(out string message, out object[] properties)
        {
            message = string.Empty;
            properties = [];
            return false;
        }

        private static string CreateMessage(Enum errorKey, string[] messages, int line, int column) =>
            "[L{Line}:C{Column}]" + $" {messages[Convert.ToInt32(errorKey)]}";

        public static string SplitPascalCaseManual(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char current = input[i];

                // Add space before uppercase letter if previous char is lowercase or digit
                if (i > 0 && char.IsUpper(current))
                {
                    char previous = input[i - 1];
                    if (char.IsLower(previous) || char.IsDigit(previous))
                    {
                        result.Append(' ');
                    }
                }

                // Add space before digit if previous char is letter
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
