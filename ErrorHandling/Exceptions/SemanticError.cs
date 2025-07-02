namespace SkimSkript.ErrorHandling.Exceptions
{
    /// <summary> <see cref="LanguageError"/> specific to the semantic analysis phase in which it will be thrown. </summary>
    public class SemanticError : LanguageError
    {
        private const string ERROR_LABEL = "Semantic Error: ";

        /// <param _name="message"> Message describing why code is semantically invalid. </param>
        public SemanticError(string message) : base($"{ERROR_LABEL}{message}") { }
    }
}
