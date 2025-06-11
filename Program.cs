using SkimSkript;
using SkimSkript.EntryPoint;
using SkimSkript.Logging;

class Program
{
    private const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Warning;

    private static int Main(string[] args)
    {
        var log = new ConsoleLogger()
            .SetMinimumLogLevel(DEFAULT_LOG_LEVEL); // Set the minimum log level for logging

        if (args.Length == 0)
        {
            log.Error("Requires at least one file path argument");
            return -1;
        }

        var core = new SkimSkriptCore();
        var fileReader = new FileReader();

        foreach(var filepath in args)
        {
            string[]? linesOfCode = null;

            try
            {
                linesOfCode = fileReader.GetLinesOfCode(filepath);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to read file: {FilePath}", filepath);
                return -1;
            }

            core.Execute(linesOfCode!); // Interpret the lines of code
        }

        return core.WasExecutionSuccessful ? 0 : -1;
    }
}
