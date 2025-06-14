using SkimSkript.TokenManagement.Tokens;
using SkimSkript.ErrorHandling;

namespace SkimSkript.TokenManagement
{
    /// <summary>Class that represents a queue-like token stream that is populated with tokens during lexical analysis
    /// for later use during parsing.</summary>
    public class TokenContainer
    {
        private List<Token> _tokenList = new List<Token>();
        private int _currentTokenIndex = 0;

        #region Properties
        /// <summary>Indicates whether there are currently any tokens.</summary>
        public bool HasTokens => _currentTokenIndex != _tokenList.Count;

        /// <summary>Gets the line number of the frontmost token.</summary>
        public int CurrentLineNumber => _tokenList[_currentTokenIndex].LineNumber;
        #endregion

        /// <summary>Adds token to the rear of the container structure.</summary>
        public void Add(Token token) => _tokenList.Add(token);

        /// <summary>Gets the frontmost token's <see cref="TokenType"/>.</summary>
        public TokenType PeekType()
        {
            if (HasTokens)
                return _tokenList[_currentTokenIndex].Type;

            return ThrowEndOfFileError();
        }

        public bool TryPeek(out TokenType tokenType)
        {
            tokenType = TokenType.Undefined;

            if(HasTokens)
            {
                tokenType = _tokenList[_currentTokenIndex].Type;
                return true;
            }

            return false;
        }

        /// <summary>Peeks at token at a specified forward offset from the current token pointer position.</summary>
        /// <returns>Returns true if the offset is within the bounds of the container.</returns>
        public bool TryPeekAheadType(out TokenType tokenType, int offset = 1)
        {
            tokenType = TokenType.Undefined;
            int checkIndex = _currentTokenIndex + offset;

            if (checkIndex >= _tokenList.Count)
                return false;

            tokenType = _tokenList[checkIndex].Type;
            return true;
        }

        /// <summary>Removes the frontmost token</summary>
        /// <exception cref="SyntaxError"/>
        public void Remove(TokenType expectedType = TokenType.Undefined)
        {
            if (!HasTokens)
                ThrowEndOfFileError(expectedType);

            _currentTokenIndex++;
        }

        /// <summary>Removes the frontmost token and returns the removed token's stored <see cref="TokenType"/>.</summary>
        public TokenType RemoveAndGetType()
        {
            Remove();
            return _tokenList[_currentTokenIndex - 1].Type;
        }

        /// <summary>Removes the frontmost token and returns the lexeme(string) representing the removed token.</summary>
        public string RemoveAndGetLexeme()
        {
            Remove();
            return _tokenList[_currentTokenIndex - 1].Lexeme;
        }

        /// <summary>Checks if the frontmost token is of a given <see cref="TokenType"/>, and if so,
        /// remove said token. Otherwise, if the token is not of that type, throw an exception. </summary>
        /// <exception cref="SyntaxError"></exception>
        public void MatchAndRemove(TokenType tokenType)
        {
            if (_tokenList[_currentTokenIndex].Type != tokenType)
                throw new SyntaxError(tokenType, this, ErrorTokenPosition.InPlace);

            Remove(tokenType);
        }

        /// <summary>Checks if the frontmost token is of a given <see cref="TokenType"/>, and if so, remove said token.</summary>
        /// <remarks>This assumes a token of this type is not required in the frontmost position when called, 
        /// and as a result, does NOT throw an exception if either the token is not of this type or no tokens are present.</remarks> 
        public bool TryMatchAndRemove(TokenType tokenType)
        {
            if (HasTokens && _tokenList[_currentTokenIndex].Type == tokenType)
            {
                Remove(tokenType);
                return true;
            }

            return false;
        }

        public bool TryMatchAndRemove(TokenType[] tokenTypes)
        {
            foreach (var tokenType in tokenTypes)
                if (TryMatchAndRemove(tokenType))
                    return true;

            return false;
        }

        /// <summary>Checks if the frontmost token is of a given <see cref="TokenType"/>, and if so,
        ///  remove said token. Otherwise, if the token is not of that type, throw an exception. </summary>
        /// <returns>The lexeme(string) representing the removed token.</returns>
        /// <exception cref="SyntaxError"></exception>
        public string MatchRemoveAndGetLexeme(TokenType tokenType)
        {
            MatchAndRemove(tokenType);
            return _tokenList[_currentTokenIndex - 1].Lexeme;
        }

        #region Error Handling Methods
        /// <summary>Moves the token pointer one position towards the front of the structure.</summary>
        public void MovePointerTowardsFront() => --_currentTokenIndex;

        /// <summary>Moves the token pointer one position towards the back of the structure.</summary>
        public void MovePointerTowardsRear() => ++_currentTokenIndex;

        /// <summary>Gets every stored <see cref="Token"/> found within a specific line in the source language input file.</summary>
        public List<Token> GetTokensOnLine(out int errorTokenIndex)
        {
            int targetLineNumber = _tokenList[_currentTokenIndex].LineNumber;

            List<Token> tokens = new List<Token>();
            foreach (Token token in _tokenList)
                if (token.LineNumber == targetLineNumber)
                    tokens.Add(token);
                else if (token.LineNumber > targetLineNumber)
                    break;

            errorTokenIndex = GetCurrentTokenLineIndex(tokens);
            return tokens;
        }

        /// <summary>Gets line number associated with the token at the pointer's position.</summary>
        private int GetCurrentTokenLineIndex(List<Token> tokensOnLine)
        {
            for (int i = 0; i < tokensOnLine.Count; i++)
                if (tokensOnLine[i] == _tokenList[_currentTokenIndex])
                    return i;

            return 0;
        }

        private TokenType ThrowEndOfFileError(TokenType tokenType = TokenType.Undefined) => 
            throw new SyntaxError($"Expected {tokenType} token but instead reached the end of file.", this, ErrorTokenPosition.Backward);
        #endregion
    }
}
