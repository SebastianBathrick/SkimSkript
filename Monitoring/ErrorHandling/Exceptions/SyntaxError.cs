using SkimSkript.Tokens;

namespace SkimSkript.ErrorHandling
{
    /// <summary> Represents the position of the problematic token relative to the <see cref="TokenContainer"/>
    /// pointer in the event of an error. Intended to help in displaying accurate and relevant syntax errors'
    /// during error handling. </summary>
    public enum ErrorTokenPosition { Forward, Backward, InPlace }

    /// <summary> <see cref="LanguageError"/> specific to the parsing phase, during which it will be thrown. </summary>
    internal class SyntaxError : LanguageError
    {
        private const string ERROR_LABEL = "Syntax Error: ";

        #region Global Variables
        private TokenContainer _tokenContainer;
        private bool _isExpectedTokenType = false;
        private TokenType _expectedTokenType;
        private ErrorTokenPosition _position;
        #endregion

        #region Properties
        /// <summary> See <see cref="TokenManagement.TokenContainer"/> docs. </summary>
        public TokenContainer TokenContainer => _tokenContainer;

        /// <summary> Whether the parser had thrown the exception because the token type was not what was expected. </summary>
        public bool IsExpectedTokenType => _isExpectedTokenType;

        /// <summary> The expected <see cref="TokenType"/> where the parser had thrown the exception. </summary>
        public TokenType ExpectedTokenType => _expectedTokenType;

        /// <summary> Where the problem token is found relative to the <see cref="TokenContainer"/>'s cursor position. </summary>
        public ErrorTokenPosition Position => _position;
        #endregion

        #region Constructors
        /// <param _name="message"> Message describing why syntactically is semantically invalid. </param>
        /// <param _name="tokenContainer"> See <see cref="TokenManagement.TokenContainer"/> docs. </param>
        /// <param _name="position"> See <see cref="SyntaxError.Position"/> doc. </param>
        public SyntaxError(string message, TokenContainer tokenContainer, ErrorTokenPosition position) : base($"{ERROR_LABEL}{message}")
        {
            _tokenContainer = tokenContainer;
            _position = position;
        }

        /// <remarks> The <see cref="ErrorHandler"/> will generate the error message & not an instance of this class object. </remarks>
        /// <param _name="expectedTokenType"> See <see cref="SyntaxError.ExpectedTokenType"/> doc. </param>
        /// <param _name="tokenContainer"> See <see cref="TokenManagement.TokenContainer"/> docs. </param>
        /// <param _name="position"> See <see cref="SyntaxError.Position"/> doc. </param>
        public SyntaxError(TokenType expectedTokenType, TokenContainer tokenContainer, ErrorTokenPosition position) : this(String.Empty, tokenContainer, position)
        {
            _expectedTokenType = expectedTokenType;
            _isExpectedTokenType = true;            
        }
        #endregion
    }
}
