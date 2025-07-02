using SkimSkript.Helpers.LexicalAnalysis;
using SkimSkript.LexicalAnalysis.Helpers;
using SkimSkript.Tokens;
using System.Text;

namespace SkimSkript.MainComponents
{
    /// <summary>Coordinates lexeme scanning and evaluation to produce tokens.
    /// Populates a <see cref="TokenManagement.TokenContainer"/> using input lines.</summary>
    internal class Lexer : MainComponent<string[], TokenContainer>
    {
        private Scanner _scanner = new();
        private Evaluator _evaluator = new();

        private LexemeContainer? _lexemes;
        private TokenContainer? _tokens;

        public override MainComponentType ComponentType => MainComponentType.Lexer;

        public LexemeContainer Lexemes => _lexemes!;

        public TokenContainer Tokens => _tokens!;

        public Lexer(IEnumerable<MainComponentType> debuggedTypes) : base(debuggedTypes) { }

        /// <summary>Constructor that performs lexical analysis using lines of code in the source language.</summary>
        /// <param _name="linesArray">Lines of code in the source language.</param>
        protected override TokenContainer OnExecute(string[] linesArray)
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
