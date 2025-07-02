using SkimSkript.ErrorHandling;
using SkimSkript.Helpers.General;
using SkimSkript.Helpers.LexicalAnalysis;
using System.Text;

namespace SkimSkript.Tokens
{
    /// <summary>Class that represents a queue-like token stream that is populated with tokens during lexical analysis
    /// for later use during parsing.</summary>
    internal class TokenContainer
    {
        #region Data Members
        private List<Token> _tokenList = new();
        private LexemeContainer _lexemes;
        private int _currentTokenIndex = 0;
        #endregion

        #region Properties
        public bool HasTokens => _currentTokenIndex != _tokenList.Count;

        public TokenContainer(LexemeContainer lexemeContainer) => _lexemes = lexemeContainer;
        #endregion

        #region Append Methods
        /// <summary>Adds token to the rear of the container structure.</summary>
        public void Append(Token token) => _tokenList.Add(token);
        #endregion

        #region Peek Methods
        /// <summary>Gets the frontmost token's <see cref="TokenType"/>.</summary>
        public TokenType PeekType()
        {
            if (HasTokens)
                return _tokenList[_currentTokenIndex].Type;

            throw GetEndOfFileError();
        }

        public bool TryPeek(out TokenType tokenType)
        {
            tokenType = TokenType.Undefined;

            if (HasTokens)
            {
                tokenType = _tokenList[_currentTokenIndex].Type;
                return true;
            }

            return false;
        }

        public bool TryPeekAheadType(out TokenType tokenType, int offset = 1)
        {
            tokenType = TokenType.Undefined;
            int checkIndex = _currentTokenIndex + offset;

            if (checkIndex >= _tokenList.Count)
                return false;

            tokenType = _tokenList[checkIndex].Type;
            return true;
        }
        #endregion

        #region Removal Methods
        /// <summary>Removes the frontmost token</summary>
        public void Remove(TokenType expectedType = TokenType.Undefined)
        {
            if (!HasTokens)
                throw GetEndOfFileError(expectedType);

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
            return GetLexemeFromToken(_currentTokenIndex - 1);
        }
        #endregion

        #region Match & Remove Methods
        /// <summary>Checks if the frontmost token is of a given <see cref="TokenType"/>, and if so,
        /// remove said token. Otherwise, if the token is not of that type, throw an exception. </summary>
        public void MatchAndRemove(TokenType tokenType)
        {
            if (_tokenList[_currentTokenIndex].Type != tokenType)
                throw GetTokenExceptionError(tokenType);

            Remove(tokenType);
        }

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
        #endregion

        #region Get Lexeme Methods
        public int GetLexemeStartIndex(int offset = 0) => 
            _tokenList[_currentTokenIndex - offset].LexemeStartIndex;

        public int GetLexemeEndIndex(int offset = 1) => 
            _tokenList[_currentTokenIndex - offset].LexemeEndIndex;

        public string MatchRemoveAndGetLexeme(TokenType tokenType)
        {
            MatchAndRemove(tokenType);
            return GetLexemeFromToken(_currentTokenIndex - 1);
        }

        private string GetLexemeFromToken(int index) => 
            _lexemes.GetLexemesAsString(
                _tokenList[index].LexemeStartIndex, 
                _tokenList[index].LexemeEndIndex
                );
        #endregion

        #region Helper Methods
        public TokenContainerException GetTokenExceptionError(
            TokenType expectedType,
            int tokenIndexOffset = 0,
            params object[] properties
            ) =>
            throw new TokenContainerException(
                expectedType, 
                _tokenList[_currentTokenIndex - tokenIndexOffset], 
                _lexemes,
                properties
                );

        private TokenContainerException GetEndOfFileError(TokenType tokenType = TokenType.Undefined) =>
            GetTokenExceptionError(TokenType.EndOfFile, tokenIndexOffset: 0, tokenType, TokenType.EndOfFile);

        public override string ToString()
        {
            if (!HasTokens)
                return "TokenContainer is empty.";
            var sb = new StringBuilder();
            sb.AppendLine("TokenContainer:");
            for (int i = _currentTokenIndex; i < _tokenList.Count; i++)
            {
                sb.AppendLine($"[{i}] {_tokenList[i].Type} - {GetLexemeFromToken(i)}");
            }
            return sb.ToString();
        }
        #endregion
    }
}
