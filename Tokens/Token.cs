using SkimSkript.TokenManagement;

namespace SkimSkript.TokenManagement.Tokens
{
    using LineNumber = System.UInt16; // alias for unsigned int
    using ColumnNumber = System.UInt16;

    /// <summary>Class that represents a single token created during lexical analysis utilized for parsing.</summary>
    public class Token
    {        
        private string _lexeme;      
        private TokenType _type;
        private int _lineNumber;

        /// <summary>The lexeme the token was derived from.</summary>
        /// <remarks>Is only guarenteed to be complete if the associated <see cref="TokenType"/>
        /// is an identifier.</remarks>
        public string Lexeme => _lexeme;

        /// <summary>Enum that serves as a label to determine how the token should be treated during parsing.</summary>
        public TokenType Type => _type;

        /// <summary>The line number of where the associated lexeme was found in the source language file.</summary>
        public int LineNumber => _lineNumber;

        /// <param name="type">Enum that serves as a label to determine how the token should be treated during parsing.</param>
        /// <param name="lexeme">The lexeme the token was derived from.</param>
        /// <param name="lineNumber">The line number of where the associated lexeme was found in the source language file.</param>
        public Token(TokenType type, in string lexeme, int lineNumber)
        {
            _type = type;
            _lexeme = lexeme;
            _lineNumber = lineNumber;
        }

        /// <summary>Returns string meant to be utilized for debugging.</summary>
        public override string ToString() => $"{_type} ({_lexeme})";
    }
}
