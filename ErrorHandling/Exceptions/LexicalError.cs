namespace SkimSkript.ErrorHandling
{
    /// <summary> <see cref="LanguageError"/> specific to the lexical analysis phase, during which it will be thrown. </summary>
    public class LexicalError : LanguageError
    {
        private const string ERROR_LABEL = "Invalid Token Error: ";
        private const string LINE_LABEL = "Line ";

        /// <summary> Constructor for a token or lexeme error. </summary>
        /// <param name="message"> Message describing why a token/lexeme is is invalid. </param>
        /// <param name="lineNumber"> Line number the token/lexeme was found on in the source code. </param>
        /// <param name="line"> Entire line of source code where the token/lexeme was found. </param>
        public LexicalError(string message, int lineNumber, string line) :
            // Even lexemes are referred to as tokens from the user's perspective, so we use "Token" in the error message.
            base($"{ERROR_LABEL}{message}\n{LINE_LABEL}{lineNumber}: {line}") { }
    }

}
