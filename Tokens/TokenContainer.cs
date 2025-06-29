using SkimSkript.TokenManagement.Tokens;
using SkimSkript.ErrorHandling;
using SkimSkript.CoreHelpers.LexicalAnalysis;
using SkimSkript.Monitoring.ErrorHandling;
using System.Text;

namespace SkimSkript.TokenManagement
{
    /// <summary>Class that represents a queue-like token stream that is populated with tokens during lexical analysis
    /// for later use during parsing.</summary>
    internal class TokenContainer
    {
        private List<Token> _tokenList = new();
        private LexemeContainer _lexemes;
        private int _currentTokenIndex = 0;

        /// <summary>Indicates whether there are currently any tokens.</summary>
        public bool HasTokens => _currentTokenIndex != _tokenList.Count;

        public TokenContainer(LexemeContainer lexemeContainer) => _lexemes = lexemeContainer;

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

            if (HasTokens)
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
            return GetLexemeFromToken(_currentTokenIndex - 1);
        }

        /// <summary>Checks if the frontmost token is of a given <see cref="TokenType"/>, and if so,
        /// remove said token. Otherwise, if the token is not of that type, throw an exception. </summary>
        /// <exception cref="SyntaxError"></exception>
        public void MatchAndRemove(TokenType tokenType)
        {
            if (_tokenList[_currentTokenIndex].Type != tokenType)
            {
                var expected = SkimSkriptException.SplitPascalCaseManual(
                       tokenType.ToString()).ToLower();

                var received = SkimSkriptException.SplitPascalCaseManual(
                        _tokenList[_currentTokenIndex].Type.ToString()).ToLower();

                throw new TokenContainerException(
                    TokenContainerError.TokenMismatch,
                    _tokenList[_currentTokenIndex],
                    _lexemes,
                    expected,
                    received
                    );
            }


            Remove(tokenType);
        }

        public void MatchAndRemove(TokenType[] tokenTypes)
        {
            foreach (var tokenType in tokenTypes)
            {
                if (_tokenList[_currentTokenIndex].Type == tokenType)
                {
                    Remove(tokenType);
                    return;
                }
            }
            var received = SkimSkriptException.SplitPascalCaseManual(
                    _tokenList[_currentTokenIndex].Type.ToString());

            throw new TokenContainerException(
                TokenContainerError.TokenMismatch,
                _tokenList[_currentTokenIndex],
                _lexemes,
                string.Join(", ", tokenTypes),
                received
                );
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
            return GetLexemeFromToken(_currentTokenIndex - 1);
        }

        private string GetLexemeFromToken(int index) =>
            _lexemes.GetLexemesAsString(_tokenList[index].LexemeStartIndex, _tokenList[index].LexemeEndIndex);

        private TokenType ThrowEndOfFileError(TokenType tokenType = TokenType.Undefined) =>
            throw new SyntaxError($"Expected {tokenType} token but instead reached the end of file.", this, ErrorTokenPosition.Backward);

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
        public string GetLabeledFileContents()
        {
            var sb = new StringBuilder();

            foreach (var token in _tokenList)
                sb.Append(token.ToString() + ": " + _lexemes.GetLexemesAsStringLines(token.LexemeStartIndex, token.LexemeEndIndex));

            return sb.ToString();
        }
    }
}
