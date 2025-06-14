namespace SkimSkript.ErrorHandling.Exceptions
{
    internal class InvalidFunctionCallException : Exception
    {
        private const string PREFIX = "Invalid function call to ";

        public InvalidFunctionCallException(string message, string identifier)
            : base($"{PREFIX}{identifier}. {message}") { }
    }
}
