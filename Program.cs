using SkimSkript;
using SkimSkript.Logging;

class Program
{
    #region Constants
    private const int ERROR_EXIT_CODE = -1;
    private const string EXPECTED_FILE_EXTENSION = ".skim"; // Expected file extension for SkimSkript source files
    private const LogLevel ENTRY_POINT_LOG_LVL = LogLevel.Information;
    #endregion

    private static readonly Logger _log = new ConsoleLogger().SetMinimumLogLevel(ENTRY_POINT_LOG_LVL);

    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            _log.Error("Requires at least one file path argument");
            return ERROR_EXIT_CODE;
        }
            
        var core = new SkimSkriptCore();
        core.InitializeLogger(new ConsoleLogger());

        int returnCode = ERROR_EXIT_CODE; // Source code will modify this after its execution

        for (int i = 0; i < args.Length && TryGetSourceCode(args[i], out var sourceCode); i++)
        {
            core.Initialize(); // Initialize core for each file to ensure fresh state
            returnCode = core.Execute(sourceCode);
        }

        return returnCode;
    }

    private static bool TryGetSourceCode(string filePath, out string[] sourceCode)
    {
        string fileExtension = Path.GetExtension(filePath);

        if (!string.Equals(fileExtension, EXPECTED_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
        {
            Log.Error(
                "Wrong file extension. Expected: {ExpectedExtension}, " +
                "Actual: {ActualExtension} for file: {FilePath}", 
                EXPECTED_FILE_EXTENSION, fileExtension, filePath
                );
            sourceCode = [];
            return false;
        }

        try
        {
            sourceCode = File.ReadAllLines(filePath);
            return true;
        }
        catch (FileNotFoundException)
        {
            Log.Error("File not found: {FilePath}", filePath);
        }
        catch (DirectoryNotFoundException)
        {
            Log.Error("Directory not found: {FilePath}", filePath);
        }
        catch (UnauthorizedAccessException)
        {
            Log.Error("Access denied: {FilePath}", filePath);
        }
        catch (PathTooLongException)
        {
            Log.Error("Path too long: {FilePath}", filePath);
        }
        catch (NotSupportedException)
        {
            Log.Error("File format not supported: {FilePath}", filePath);
        }
        catch (IOException ex)
        {
            Log.Error("IO error reading file {FilePath}: {Message}", filePath, ex.Message);
        }
        catch (OutOfMemoryException)
        {
            Log.Error("Out of memory reading file: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            Log.Error("Unexpected error reading file {FilePath}: {Message}", filePath, ex.Message);
        }

        sourceCode = [];
        return false;
    }
}
