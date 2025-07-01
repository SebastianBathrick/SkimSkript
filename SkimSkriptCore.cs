using SkimSkript.Interpretation;
using SkimSkript.LexicalAnalysis;
using SkimSkript.Logging;
using SkimSkript.Monitoring.ErrorHandling;
using SkimSkript.Nodes;
using SkimSkript.Parsing;

namespace SkimSkript
{
    /// <summary> Main compiler class for the source language.</summary>
    public class SkimSkriptCore
    {
        private const LogLevel DEFAULT_LOG_LVL = LogLevel.Error;
        private const int ERROR_EXIT_CODE = -1;

        private Lexer? _lexer;
        private Parser? _parser;
        private Interpreter? _interpreter;

        /// <summary>
        /// Creates new instance of <see cref="SkimSkriptCore"/> and initializes its components
        /// (null or otherwise).
        /// </summary>
        /// <param _name="isDebugInfoGathered">A boolean value indicating whether debugging mode is enabled.  
        /// If <see langword="true"/>, additional debugging information may be logged or processed.</param>
        public SkimSkriptCore()
        {        
            _lexer = null;
            _parser = null;
            _interpreter = null;
        }

        /// <summary>
        /// Instantiates and readies main components to interpret new source code.
        /// </summary>
        public void Initialize(MainComponentType[]? debuggedComponents = null)
        {
            debuggedComponents ??= [];
            _lexer = new(debuggedComponents);
            _parser = new(debuggedComponents);
            _interpreter = new(debuggedComponents);

            if(Log.IsLoggerSet)
                Log.Info("SkimSkriptCore initialized");
        }

        /// <summary>
        /// Executes the provided source code by tokenizing, parsing, and interpreting it.
        /// </summary>
        /// <param _name="sourceCode">An array of strings representing the source code to execute. 
        /// Each string corresponds to a line of code.</param>
        public int Execute(string[] sourceCode)
        {
            if(_lexer == null || _parser == null || _interpreter == null)
                throw new NullReferenceException(
                    "SkimScriptCore instance's Initialize() was not called before Execute()");

            if (!Log.IsLoggerSet)
                throw new NullReferenceException(
                    "Logger is not set. Please set a logger before executing the source code");

            try
            {
                var tokens = _lexer.Execute(sourceCode);

                var treeRoot = _parser.Execute(tokens);

                return _interpreter.Execute(treeRoot);
            }
            catch(AssertionException ex)
            {
                Console.Error.WriteLine($"Assertion failed: {ex.Message}");
                return ERROR_EXIT_CODE;
            }
            catch (TokenContainerException ex)
            {
                var logger = new ConsoleLogger();
                logger.Error(ex.Message, ex.Properties);
                return ERROR_EXIT_CODE;
            }

        }

        /// <summary>
        /// Initalizes static logger for interpreter if not already set.
        /// </summary>
        /// <param _name="logger">Logger instance of class that inherits from SkimSkript.Logging.Logger.</param>
        /// <param _name="minLogLevel">Minimum log level that will be logged.</param>
        /// <param _name="isThreadLocking">Whether this logger instance will be accesssed by multiple threads</param>
        public void InitializeLogger(
            Logger logger,
            LogLevel minLogLevel = DEFAULT_LOG_LVL,
            bool isThreadLocking = true
            )
        {
            Log.SetLogger(logger)
               .SetMinimumLogLevel(minLogLevel)
               .SetLockBehavior(isThreadLocking);
        }
    }
}
