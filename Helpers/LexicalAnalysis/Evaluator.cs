using SkimSkript.Syntax;
using SkimSkript.Tokens;
using SkimSkript.Helpers.LexicalAnalysis;

namespace SkimSkript.LexicalAnalysis.Helpers
{
    /// <summary> Processes lexemes and generates tokens for use in the <see cref="TokenManagement.TokenContainer"/>. It includes 
    ///  functionality to build a reserved keyword trie, evaluate lexemes, and handle multi-word tokens. </summary>
    internal class Evaluator
    {
        
        private CharacterNode _keywordTrieRoot = new();
        private LexemeContainer? _lexemes;

        protected LexemeContainer Lexemes => _lexemes!;

        /// <summary> Builds reserved words trie. </summary>
        public Evaluator() => BuildReservedWordsTrie();


        public TokenContainer CreateTokens(LexemeContainer lexemes)
        {
            _lexemes = lexemes;
            return EvaluateLexemes();
        }

        /// <summary> Iterates through each lexeme and uses their data to create <see cref="Token"/>s 
        /// to populate an instance of <see cref="TokenManagement.TokenContainer"/>. </summary>
        private TokenContainer EvaluateLexemes()
        {
            var tokens = new TokenContainer(Lexemes);

            do
            {
                TokenType tokenType;
                var lexemeStartIndex = Lexemes.CurrentLexemeIndex;

                switch (Lexemes.GetLexemeType())
                {
                    case LexemeType.Alphanumeric: 
                        tokenType = TokenType.Identifier; 
                        break;
                    case LexemeType.NumericDecimal: 
                        tokenType = TokenType.Float; 
                        break;
                    case LexemeType.Numeric: 
                        tokenType = TokenType.Integer; 
                        break;
                    case LexemeType.Textual: 
                        tokenType = TokenType.String; 
                        break;
                    case LexemeType.Operator: 
                        tokenType = SyntaxSpec.operatorDict[Lexemes.GetLexemeSpan().ToString()]; 
                        break;
                    case LexemeType.Delimeter:
                        Lexemes.TryGetLexemeChar(out var lexemeChar);
                        tokenType = SyntaxSpec.GetDelimeterType(lexemeChar);
                        break;
                    default: 
                        tokenType = EvaluateAlphabetic(); 
                        break;
                }
                
                tokens.Append(new Token(tokenType, lexemeStartIndex, Lexemes.CurrentLexemeIndex));
            }
            while (Lexemes.TryMoveToNextLexeme());

            return tokens;
        }

        /// <summary> Determines if a lexeme or a continuous group of lexems form a reserved keyword/multi-word-token. </summary>
        private TokenType EvaluateAlphabetic()
        {           
            var navNode = _keywordTrieRoot;

            var tokenType = TokenType.Identifier;
            var isEvaluationComplete = false;
            var undefinedLexemesVisited = 0;

            do
            {
                // Continue feeding characters until reaching the end of the lexeme or the search failed
                while (!isEvaluationComplete && Lexemes.TryGetLexemeChar(out var lexemeChar))
                {
                    navNode = navNode!.GetChild(lexemeChar);

                    // If null that means there was no node child associated with the char.
                    isEvaluationComplete = navNode == null;
                }

                if(isEvaluationComplete)
                    continue;

                // If we've arrived at a node with a non-identifier TokenType that is different from the last
                if (navNode!.TokenType != TokenType.Identifier && navNode.TokenType != tokenType)
                {
                    tokenType = navNode.TokenType;
                    undefinedLexemesVisited = 0;
                }

                // If this is part of a larger multi-word token there would be a space child node.
                navNode = navNode!.GetSpaceChild();
                var isPartialMatch = navNode != null;

                // Can the search continue and is the next lexeme alphabetic?
                if (isPartialMatch && Lexemes.TryPeekNextLexeme(out var nextType) && nextType == LexemeType.Alphabetic)
                {
                    Lexemes.MoveToNextLexeme();
                    undefinedLexemesVisited++;
                }
                else
                    isEvaluationComplete = true;
            }
            while (!isEvaluationComplete);

            // Backtrack to lexemes that were visited a failed search
            Lexemes.BackTrackLexemes(undefinedLexemesVisited);

            // If the serach was successful return reserved word(s) TokenType, otherwise return Identifier.
            return tokenType; 
        }

        /// <summary> Constructs trie that stores reserved words and their associated <see cref="TokenType"/>s. </summary>
        private void BuildReservedWordsTrie()
        {
            var reservedWords = SyntaxSpec.reservedWords;

            for (int i = 0; i < reservedWords.Length; i++)
            {
                CharacterNode navNode = _keywordTrieRoot;

                //For each char in a keyword add a node. Adding a special node if the char's a space.
                for (int j = 0; j < reservedWords[i].text.Length; j++)
                    navNode = reservedWords[i].text[j] != ' ' ? navNode.AddChild(reservedWords[i].text[j]) : navNode.AddSpaceChild();

                //Assign the TokenType to the leaf node containing the last character of the keyword.
                navNode.AssignTokenType(reservedWords[i].key);
            }
        }
    }
}
