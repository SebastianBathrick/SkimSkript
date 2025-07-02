using SkimSkript.Interpretation;
using SkimSkript.LexicalAnalysis;
using SkimSkript.Monitoring.ErrorHandling;
using SkimSkript.Nodes;
using SkimSkript.Parsing;
using JustLogger.ConsoleLogging;
using JustLogger;

namespace SkimSkript
{
    /// <summary>
    /// Main compiler class for the SkimSkript source language.
    /// </summary>
    /// <remarks>
    /// Handles the complete compilation pipeline: lexical analysis, parsing, and interpretation.
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
        private bool _isLoggerInitialized = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates new instance of SkimSkriptCore with uninitialized components.
        /// </summary>
        /// <remarks>
        /// Components are set to null and must be initialized via <see cref="Initialize"/> before use.
        /// </remarks>
        public SkimSkriptCore()
        {        
            _lexer = null;
            _parser = null;
            _interpreter = null;
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
        public SkimSkriptCore Initialize(MainComponentType[]? debuggedComponents = null)
        {
            // Use empty array if no debug components specified
            debuggedComponents ??= [];
            
            // Initialize all core components with optional debugging
            _lexer = new(debuggedComponents);
            _parser = new(debuggedComponents);
            _interpreter = new(debuggedComponents);

            // Log successful initialization if logger is available
            if(Log.IsLoggerSet)
                Log.Info("SkimSkriptCore initialized");

            return this;
        }

        /// <summary>
        /// Initializes static logger for interpreter if not already set.
        /// </summary>
        /// <param name="logger">Logger instance that inherits from SkimSkript.Logging.Logger.</param>
        /// <param name="minLogLevel">Minimum log level that will be logged.</param>
        /// <param name="isThreadLocking">Whether this logger instance will be accessed by multiple threads.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <remarks>
        /// Configures the global logging system used throughout the compilation pipeline.
        /// </remarks>
        public SkimSkriptCore InitializeLogger(
            Logger logger,
            LogLevel minLogLevel = DEFAULT_LOG_LVL,
            bool isThreadLocking = true
            )
        {
            Log.SetLogger(logger)
               .SetMinimumLogLevel(minLogLevel)
               .SetLockBehavior(isThreadLocking);

            _isLoggerInitialized = true;
            return this;
        }
        #endregion

        #region Execution
        /// <summary>
        /// Executes the provided source code by tokenizing, parsing, and interpreting it.
        /// </summary>
        /// <param name="sourceCode">Array of strings representing the source code to execute. Each string corresponds to a line of code.</param>
        /// <returns>Exit code from the interpreted program execution.</returns>
        /// <exception cref="NullReferenceException">Thrown when Initialize() was not called or logger is not set.</exception>
        /// <exception cref="RuntimeException">Thrown when runtime errors occur during interpretation.</exception>
        /// <exception cref="TokenContainerException">Thrown when token processing errors occur.</exception>
        /// <remarks>
        /// Performs the complete compilation pipeline: lexical analysis → parsing → interpretation.
        /// Returns ERROR_EXIT_CODE (-1) on any compilation or runtime errors.
        /// </remarks>
        public int Execute(string[] sourceCode)
        {
            // Validate that all components have been properly initialized
            if(_lexer == null || _parser == null || _interpreter == null)
                throw new NullReferenceException(
                    "SkimScriptCore instance's Initialize() was not called before Execute()");

            // Ensure logger is configured for error reporting
            if (!Log.IsLoggerSet)
                throw new NullReferenceException(
                    "Logger is not set. Please set a logger before executing the source code");

            try
            {
                // Step 1: Lexical Analysis - Convert source code to tokens
                var tokens = _lexer.Execute(sourceCode);

                // Step 2: Parsing - Convert tokens to abstract syntax tree
                var treeRoot = _parser.Execute(tokens);

                // Step 3: Interpretation - Execute the abstract syntax tree
                return _interpreter.Execute(treeRoot);
            }
            catch(RuntimeException ex)
            {
                // Handle runtime errors with detailed error reporting
                var message = ex.Message;

                // Add statement context if available for better debugging
                if(ex.StatementNode != null)
                    message += $"\n{StatementNode.ToString(ex.StatementNode, _lexer.Lexemes)}";

                Log.Error(message, ex.Properties);
                return ERROR_EXIT_CODE;
            }
            catch (TokenContainerException ex)
            {
                // Handle token processing errors with dedicated logger
                var logger = new ConsoleLogger();
                logger.Error(ex.Message, ex.Properties);
                return ERROR_EXIT_CODE;
            }
        }
        #endregion
    }
}
