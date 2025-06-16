namespace SkimSkript.ErrorHandling
{
    /// <summary> <see cref="LanguageError"/> thrown runtime if an identifier can't be found/accessed or in the current scope. </summary>
    public class UnknownIdentifierError : LanguageError
    {
        private const string ERROR_MSG_LEADING = "Unknown IdentfierNode Error: An unknown identifier called '";
        private const string ERROR_MSG_TRAILING = "' was used.";

        /// <param name="identifier"> IdentfierNode that couldn't be found/accessed </param>
        public UnknownIdentifierError(string identifier) : base($"{ERROR_MSG_LEADING}{identifier}{ERROR_MSG_TRAILING}") { }
    }
}
