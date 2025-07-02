namespace SkimSkript.Tokens
{
    /// <summary>Class that represents a single token created during lexical analysis utilized for parsing.</summary>
    public class Token
    {
        private TokenType _type;
        private int _lexemeStartIndex, _lexemeEndIndex;

        /// <summary>Enum that serves as a label to determine how the token should be treated during parsing.</summary>
        public TokenType Type => _type;

        public int LexemeStartIndex => _lexemeStartIndex;

        public int LexemeEndIndex => _lexemeEndIndex;

        public Token(TokenType type, int lexemeStartIndex, int lexemeEndIndex)
        {
            _type = type;
            _lexemeStartIndex = lexemeStartIndex;
            _lexemeEndIndex = lexemeEndIndex;
        }

        /// <summary>Returns string meant to be utilized for debugging.</summary>
        public override string ToString() => $"{_type} ({LexemeStartIndex} to {LexemeEndIndex})";
    }
}
