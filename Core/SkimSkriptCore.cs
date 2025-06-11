using SkimSkript.LexicalAnalysis;
using SkimSkript.Interpretation;
using SkimSkript.ErrorHandling;
using SkimSkript.Semantics;
using SkimSkript.Parsing;
using System.Diagnostics;

namespace SkimSkript
{
    /// <summary> Main compiler class for the source language.</summary>
    public class SkimSkriptCore
    {
        private bool _wasExecutionSuccessful;

        public bool WasExecutionSuccessful => _wasExecutionSuccessful;

        ///<summary>Performs lexical analysis, parses, and executes the provided code.</summary>
        public void Execute(string[] linesOfCode, bool isDebugging = false)
        {
            try
            {
                var lexer = new Lexer(linesOfCode);
                var tokens = lexer.TokenContainer;

                var parser = new Parser(tokens);
                var astRoot = parser.AbstractSyntaxTreeRoot;

                var semanticAnalyzer = new SemanticAnalyzer(astRoot);

                var interpreter = new Interpreter(astRoot);
                _wasExecutionSuccessful = true;
            }
            catch (Exception ex)
            {
                new ErrorHandler(ex);
                _wasExecutionSuccessful = false;
            }
        }
    }
}
