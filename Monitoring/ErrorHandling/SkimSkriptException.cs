using System.Text;

namespace SkimSkript.Monitoring.ErrorHandling
{
    internal abstract class SkimSkriptException : Exception
    {
        protected readonly object[] _properties;      
        protected readonly int _line;
        protected readonly int _column;
        protected readonly Enum _errorKey;

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
            Enum errorKey, 
            string[] messages, 
            int line, 
            int column, 
            params object[] properties
            ) 
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
    }
}
