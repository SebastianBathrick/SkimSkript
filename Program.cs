using SkimSkript;
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

        

        foreach(var filepath in args)
        {
            var core = new SkimSkriptCore();
            string[]? linesOfCode = null;

            try
            {
                linesOfCode = GetLinesOfCode(filepath);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to read file: {FilePath}", filepath);
                return -1;
            }

            core.Execute(linesOfCode!); // Interpret the lines of code
        }

        return 0;
    }

    private static string[] GetLinesOfCode(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found");

        return File.ReadLines(filePath).ToArray();
    }
}
