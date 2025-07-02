using JustLogger.ConsoleLogging;
using JustLogger;
using SkimSkript;
using Flags;

class Program
{
    #region Constants
    private const int ERROR_EXIT_CODE = -1;
    private const int DEFAULT_EXIT_CODE = 0;
    private const LogLevel ENTRY_POINT_LOG_LVL = LogLevel.Information;
    #endregion

    #region Data Members
    private static readonly Logger _entryPointLogger = new ConsoleLogger().SetMinimumLogLevel(ENTRY_POINT_LOG_LVL);
    private static List<string>? _sourceCodePaths; // Lazy-initialized list of file paths to process
    #endregion

    #region Properties
    /// <summary>
    /// Gets the list of source code file paths, initializing if null.
    /// </summary>
    public static List<string> SourceCodePaths => _sourceCodePaths ?? (_sourceCodePaths =[]); 
    #endregion

    private static int Main(string[] args)
    {
        // Validate that at least one argument was provided
        if (args.Length == 0)
        {
            _entryPointLogger.Error("Requires at least one file path argument");
            return ERROR_EXIT_CODE;
        }

        int exitCode = DEFAULT_EXIT_CODE;

        // Process command-line flags first, add remaining args as file paths if no flags processed
        if(!InterpreterFlags.TryEvaluateArguments(args, _entryPointLogger))
            SourceCodePaths.AddRange(args);

        // Early exit if no files to process
        if (SourceCodePaths.Count == 0)
            return exitCode;
            
        // Initialize the SkimSkript interpreter core
        var core = new SkimSkriptCore().InitializeLogger(new ConsoleLogger());

        // Process each source code file sequentially
        foreach (var filePath in SourceCodePaths)
        {
            // Attempt to read and validate the source code file
            if (!SourceCodeReader.TryGetSourceCode(filePath, out var sourceCode))
                continue; // Skip to next file if current one fails to load

            // Reset core state for each file to ensure clean execution environment
            core.Initialize();
            
            // Execute the source code and capture the exit code
            exitCode = core.Execute(sourceCode);
        }

        return exitCode; // Last interpreted program's exit code
    }
}
