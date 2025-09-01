using JustLogger;
using JustLogger.ConsoleLogging;
using SkimSkript.ErrorHandling;
using SkimSkript.MainComponents;
using SkimSkript.Nodes;

namespace SkimSkript
{
    /// <summary>
    /// Main class for the SkimSkript source language.
    /// </summary>
    /// <remarks>
    /// Handles the complete pipeline: lexical analysis, parsing, and interpretation.
    /// Must be initialized before execution.
    /// </remarks>
    public class SkimSkriptCore
    {
        #region Constants
        private const LogLevel DEFAULT_LOG_LVL = LogLevel.Error;
        private const int ERROR_EXIT_CODE = -1;
        #endregion

        #region Fields
        private Lexer? _lexer;
        private Parser? _parser;
        private Interpreter? _interpreter;
        private Logger _monitoringLogger;

        private bool _isDebugging;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates new instance of SkimSkriptCore with uninitialized components.
        /// </summary>
        /// <remarks>
        /// Components are set to null and must be initialized via <see cref="Initialize"/> before use.
        /// </remarks>
        public SkimSkriptCore(Logger errorLogger)
        {
            _lexer = null;
            _parser = null;
            _interpreter = null;
            _monitoringLogger = errorLogger;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Instantiates and readies main components to interpret new source code.
        /// </summary>
        /// <param name="debuggedComponents">Optional array of components to enable debugging for.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <remarks>
        /// Creates new instances of Lexer, Parser, and Interpreter with optional debugging support.
        /// Must be called before <see cref="Execute"/>.
        /// </remarks>
        public SkimSkriptCore Initialize(MainComponentType[]? debuggedComponents = null, MainComponentType[]? verboseComponents = null)
        {
            // Use empty array if no debug components specified
            debuggedComponents ??= [];
            verboseComponents ??= [];

            _isDebugging = debuggedComponents.Length > 0;

            if(_isDebugging)
                _monitoringLogger.SetMinimumLogLevel(LogLevel.Debug);

            // Initialize all core components with optional debugging
            _lexer = new(debuggedComponents, verboseComponents);
            _parser = new(debuggedComponents, verboseComponents);
            _interpreter = new(debuggedComponents, verboseComponents);

            return this;
        }
        #endregion

        #region Execution
        /// <summary> Executes the provided source code by tokenizing, parsing, and interpreting it. </summary>
        /// <param name="sourceCode">Array of strings representing the source code to execute. Each string corresponds to a line of code.</param>
        /// <returns>Exit code from the interpreted program execution.</returns>
        /// <remarks> Returns SOURCE_CODE_ERROR (-1) on any lexer, parser, or runtime errors. </remarks>
        public int Execute(string[] sourceCode)
        {
            // Validate that all components have been properly initialized
            if (_lexer == null || _parser == null || _interpreter == null)
                throw new NullReferenceException(
                    "SkimScriptCore instance's Initialize() was not called before Execute()");

            try
            {
                // Step 1: Lexical Analysis - Convert source code to tokens
                var tokens = _lexer.Execute(sourceCode);

                // Step 2: Parsing - Convert tokens to abstract syntax tree
                var treeRoot = _parser.Execute(tokens);

                // Step 3: Interpretation - Execute the abstract syntax tree
                var exitCode = _interpreter.Execute(treeRoot);

                if (_isDebugging)
                    _monitoringLogger.Debug("Total Measured Time: {Amount} ms", GlobalClock.GlobalElapsedTime);

                return exitCode;
            }
            catch (RuntimeException ex)
            {
                // Handle runtime errors with detailed error reporting
                var message = ex.Message;

                // Add statement context if available for better debugging
                if (ex.StatementNode != null)
                    message += $"\n{StatementNode.ToString(ex.StatementNode, _lexer.Lexemes)}";

                _monitoringLogger.Error(message, ex.Properties);
                return ERROR_EXIT_CODE;
            }
            catch (SkimSkriptException ex)
            {
                // Handle token processing errors with dedicated logger
                _monitoringLogger.Error(ex.Message, ex.Properties);
                return ERROR_EXIT_CODE;
            }
        }
        #endregion
    }
}
