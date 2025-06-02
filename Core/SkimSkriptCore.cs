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

        /// <summary>Initializes compiler components and executes compilation for the specified file.</summary>
        /// <param name="filePath">Path to the file to compile.</param>
        public SkimSkriptCore(string filePath)
        {
            try
            {
                Lexer lexer = new Lexer(GetFileContents(filePath));
                Parser parser = new Parser(lexer.TokenContainer);
                SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(parser.AbstractSyntaxTreeRoot);
                Interpreter interpreter = new Interpreter(parser.AbstractSyntaxTreeRoot);
                _wasExecutionSuccessful = true;
            }
            catch(Exception ex)
            {
                new ErrorHandler(ex);
                _wasExecutionSuccessful = false;
            }          
        }

        public string[] GetFileContents(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file '{filePath}' does not exist.");

            return File.ReadLines(filePath).ToArray(); // Return lazy-loaded lines directly
        }
    }
}
