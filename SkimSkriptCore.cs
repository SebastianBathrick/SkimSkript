using SkimSkript.LexicalAnalysis;
using SkimSkript.Interpretation;
using SkimSkript.Parsing;
using SkimSkript.Logging;

namespace SkimSkript
{
    /// <summary> Main compiler class for the source language.</summary>
    public class SkimSkriptCore
    {
        private bool _wasExecutionSuccessful;
        Lexer _lexer = new();
        Parser _parser = new(new ConsoleLogger());
        Interpreter _interpreter = new();

        public bool WasExecutionSuccessful => _wasExecutionSuccessful;

        ///<summary> Performs lexical analysis, parses, and executes the provided code. </summary>
        public void Execute(string[] sourceCode, bool isDebugging = false)
        {
            var tokens = _lexer.Tokenize(sourceCode);

            var treeRoot = _parser.BuildAbstractSyntaxTree(tokens);

            _interpreter.Execute(treeRoot);

            _wasExecutionSuccessful = true;
        }
    }
}
