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
        private Scanner? _scanner;
        private Evaluator? _evaluator;
        private LexemeContainer? _lexemes;


        public override MainComponentType ComponentType => MainComponentType.Lexer;

        public LexemeContainer Lexemes => _lexemes ?? throw new NullReferenceException("LexemeContainer null");

        public Lexer(IEnumerable<MainComponentType> debuggedTypes) : base(debuggedTypes) { }

        protected override void OnConstructor()
        {
            _scanner = new Scanner();
            _evaluator = new Evaluator();
        }

        /// <summary>Constructor that performs lexical analysis using lines of code in the source language.</summary>
        /// <param _name="linesArray">Lines of code in the source language.</param>
        protected override TokenContainer OnExecute(string[] linesArray)
        {
            if (_scanner == null || _evaluator == null)
                throw new NullReferenceException("Lexer not properly initialized");

            _lexemes = _scanner.CreateLexemes(linesArray);
            var tokens = _evaluator.CreateTokens(_lexemes);
            return tokens;
        }
    }
}
