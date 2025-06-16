using SkimSkript.CoreHelpers.LexicalAnalysis;
using SkimSkript.LexicalAnalysis.Helpers;
using SkimSkript.TokenManagement;
using System.Text;

namespace SkimSkript.LexicalAnalysis
{
    /// <summary>Coordinates lexeme scanning and evaluation to produce tokens.
    /// Populates a <see cref="TokenManagement.TokenContainer"/> using input lines.</summary>
    internal class Lexer
    {
        private Scanner _scanner = new();
        private Evaluator _evaluator = new();

        private LexemeContainer? _lexemes;
        private TokenContainer? _tokens;

        public LexemeContainer Lexemes => _lexemes!;

        public TokenContainer Tokens => _tokens!;

        /// <summary>Constructor that performs lexical analysis using lines of code in the source language.</summary>
        /// <param name="linesArray">Lines of code in the source language.</param>
        public TokenContainer Tokenize(string[] linesArray)
        {
            _lexemes = _scanner.CreateLexemes(linesArray);
            _tokens = _evaluator.CreateTokens(_lexemes);
            return _tokens;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("Lexemes:");
            sb.AppendLine(Lexemes.ToString());
            sb.AppendLine("\nTokens:");
            sb.AppendLine(Tokens.ToString());
            return sb.ToString();
        }
    }
}
