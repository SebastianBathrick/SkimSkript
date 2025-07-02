namespace SkimSkript.ErrorHandling
{
    internal abstract class SkimSkriptException : Exception
    {
        protected readonly object[] _properties;

        public object[] Properties
        {
            get
            {
                List<object> allProperties = [];
                allProperties.AddRange(_properties);

                if (TryGetAdditionalContext(out _, out var additionalProps))
                    allProperties.AddRange(additionalProps);

                return allProperties.ToArray();
            }
        }

        public override string Message =>
            base.Message + (TryGetAdditionalContext(out var msg, out _) ? $".\n{msg}" : string.Empty);

        public SkimSkriptException(string message, params object[] properties)
            : base(message)
        {
            _properties = properties;
        }

        protected virtual bool TryGetAdditionalContext(out string message, out object[] properties)
        {
            message = string.Empty;
            properties = [];
            return false;
        }
    }
}
